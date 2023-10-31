﻿// Copyright (c) Intangible Realities Lab. All rights reserved.
// Licensed under the GPL. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Narupa.Core;
using Narupa.Core.Async;
using Narupa.Core.Math;
using Narupa.Grpc.Stream;
using Narupa.Grpc.Trajectory;
using Narupa.Protocol.State;
using UnityEngine;

namespace Narupa.Grpc.Multiplayer
{
    /// <summary>
    /// Manages the state of a single user engaging in multiplayer. Tracks
    /// local and remote avatars and maintains a copy of the latest values in
    /// the shared key/value store.
    /// </summary>
    public sealed class MultiplayerSession : IDisposable
    {
        public const string SimulationPoseKey = "scene";

        public string AccessToken { get; set; }

        public MultiplayerAvatars Avatars { get; }

        public MultiplayerSession()
        {
            Avatars = new MultiplayerAvatars(this);
            
            SimulationPose =
                new MultiplayerResource<Transformation>(this, SimulationPoseKey, PoseFromObject,
                                                        PoseToObject);
        }

        /// <summary>
        /// The transformation of the simulation box.
        /// </summary>
        public readonly MultiplayerResource<Transformation> SimulationPose;

        /// <summary>
        /// Dictionary of the currently known shared state.
        /// </summary>
        public Dictionary<string, object> SharedStateDictionary { get; } =
            new Dictionary<string, object>();

        /// <summary>
        /// Is there an open client on this session?
        /// </summary>
        public bool IsOpen => client != null;

        /// <summary>
        /// How many milliseconds to put between sending our requested value
        /// changes.
        /// </summary>
        public int ValuePublishInterval { get; set; } = 1000 / 30;

        private MultiplayerClient client;

        private IncomingStream<StateUpdate> IncomingValueUpdates { get; set; }

        private Dictionary<string, object> pendingValues
            = new Dictionary<string, object>();

        private List<string> pendingRemovals
            = new List<string>();

        private Task valueFlushingTask;

        public event Action<string, object> SharedStateDictionaryKeyUpdated;

        public event Action<string> SharedStateDictionaryKeyRemoved;

        public event Action ReceiveUpdate;

        public event Action MultiplayerJoined;

        /// <summary>
        /// The index of the next update that we will send to the server. A key
        /// `update.index.{player_id}` will be inserted with this value. By getting this value
        /// when you've scheduled something to be done to the dictionary, you can then determine
        /// when a returned update has incorporated your change.
        /// </summary>
        public int NextUpdateIndex => nextUpdateIndex;

        /// <summary>
        /// The index of the latest changes we sent to the server which have been received by us.
        /// </summary>
        public int LastReceivedIndex => lastReceivedIndex;

        private int nextUpdateIndex = 0;

        private int lastReceivedIndex = -1;

        private string UpdateIndexKey => $"update.index.{AccessToken}";

        /// <summary>
        /// Connect to a Multiplayer service over the given connection. 
        /// Closes any existing client.
        /// </summary>
        public void OpenClient(GrpcConnection connection)
        {
            CloseClient();

            client = new MultiplayerClient(connection);
            AccessToken = Guid.NewGuid().ToString();

            if (valueFlushingTask == null)
            {
                valueFlushingTask = CallbackInterval(FlushValues, ValuePublishInterval);
                valueFlushingTask.AwaitInBackground();
            }
            
            IncomingValueUpdates = client.SubscribeStateUpdates();
            BackgroundIncomingStreamReceiver<StateUpdate>.Start(IncomingValueUpdates,
                                                                OnResourceValuesUpdateReceived,
                                                                MergeResourceUpdates);

            void MergeResourceUpdates(StateUpdate dest, StateUpdate src)
            {
                foreach (var (key, value) in src.ChangedKeys.Fields)
                    dest.ChangedKeys.Fields[key] = value;
            }

            MultiplayerJoined?.Invoke();
        }

        /// <summary>
        /// Close the current Multiplayer client and dispose all streams.
        /// </summary>
        public void CloseClient()
        {
            Avatars.CloseClient();
            FlushValues();
            
            client?.CloseAndCancelAllSubscriptions();
            client?.Dispose();
            client = null;
            
            AccessToken = null;

            ClearSharedState();
            pendingValues.Clear();
            pendingRemovals.Clear();
        }

        /// <summary>
        /// Set the given key in the shared state dictionary, which will be
        /// sent to the server according in the future according to the publish 
        /// interval.
        /// </summary>
        public void SetSharedState(string key, object value)
        {
            pendingValues[key] = value.ToProtobufValue();
            pendingRemovals.Remove(key);
        }
        
        /// <summary>
        /// Remove the given key from the shared state dictionary, which will be
        /// sent to the server according in the future according to the publish 
        /// interval.
        /// </summary>
        public void RemoveSharedStateKey(string key)
        {
            pendingValues.Remove(key);
            pendingRemovals.Add(key);
        }


        /// <summary>
        /// Get a key in the shared state dictionary.
        /// </summary>
        public object GetSharedState(string key)
        {
            return SharedStateDictionary.TryGetValue(key, out var value) ? value : null;
        }

        /// <summary>
        /// Attempt to gain exclusive write access to the shared value of the given key.
        /// </summary>
        public async Task<bool> LockResource(string id)
        {
            return await client.UpdateLocks(AccessToken, new Dictionary<string, float>
                                            {
                                                [id] = 1f
                                            },
                                            new string[0]);
        }

        /// <summary>
        /// Release the lock on the given object of a given key.
        /// </summary>
        public async Task<bool> ReleaseResource(string id)
        {
            return await client.UpdateLocks(AccessToken, new Dictionary<string, float>(), new string[]
            {
                id
            });
        }

        /// <inheritdoc cref="IDisposable.Dispose" />
        public void Dispose()
        {
            FlushValues();

            CloseClient();
        }

        private void ClearSharedState()
        {
            var keys = SharedStateDictionary.Keys.ToList();
            SharedStateDictionary.Clear();

            foreach (var key in keys)
            {
                try
                {
                    SharedStateDictionaryKeyRemoved?.Invoke(key);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }

        private void OnResourceValuesUpdateReceived(StateUpdate update)
        {
            ReceiveUpdate?.Invoke();
            
            if (update.ChangedKeys.Fields.ContainsKey(UpdateIndexKey))
            {
                lastReceivedIndex = (int) update.ChangedKeys
                                                .Fields[UpdateIndexKey]
                                                .NumberValue;
            }

            foreach (var (key, value1) in update.ChangedKeys.Fields)
            {
                var value = value1.ToObject();
                if (value == null)
                {
                    SharedStateDictionary.Remove(key);
                    SharedStateDictionaryKeyRemoved?.Invoke(key);
                }
                else
                {
                    SharedStateDictionary[key] = value;
                    SharedStateDictionaryKeyUpdated?.Invoke(key, value);
                }
            }
        }

        private void FlushValues()
        {
            if (!IsOpen)
                return;

            if (pendingValues.Any() || pendingRemovals.Any())
            {
                pendingValues[UpdateIndexKey] = nextUpdateIndex;

                client.UpdateState(AccessToken, pendingValues, pendingRemovals)
                      .AwaitInBackgroundIgnoreCancellation();

                pendingValues.Clear();
                pendingRemovals.Clear();

                nextUpdateIndex++;
            }
        }

        private static async Task CallbackInterval(Action callback, int interval)
        {
            while (true)
            {
                callback();
                await Task.Delay(interval);
            }
        }

        private static object PoseToObject(Transformation pose)
        {
            var data = new object[]
            {
                pose.Position.x, pose.Position.y, pose.Position.z, pose.Rotation.x, pose.Rotation.y,
                pose.Rotation.z, pose.Rotation.w, pose.Scale.x, pose.Scale.y, pose.Scale.z,
            };

            return data;
        }

        private static Transformation PoseFromObject(object @object)
        {
            if (@object is List<object> list)
            {
                var values = list.Select(value => Convert.ToSingle(value)).ToList();
                var position = list.GetVector3(0);
                var rotation = list.GetQuaternion(3);
                var scale = list.GetVector3(7);

                return new Transformation(position, rotation, scale);
            }

            throw new ArgumentOutOfRangeException();
        }

        public MultiplayerResource<object> GetSharedResource(string key)
        {
            return new MultiplayerResource<object>(this, key);
        }
    }
}