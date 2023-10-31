// Copyright (c) 2019 Intangible Realities Lab. All rights reserved.
// Licensed under the GPL. See License.txt in the project root for license information.

using Narupa.Frontend.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Narupa.Core.Async;
using Narupa.Core.Math;
using UnityEngine;
using UnityEngine.XR;

namespace Narupa.Frontend.XR
{
    /// <summary>
    /// Extensions for Unity's XR system, in which you make queries about
    /// XRNode types (e.g LeftHand, TrackingReference, etc) and receive
    /// XRNodeState objects containing identifier and tracking information
    /// for that XR node.
    /// </summary>
    public static partial class UnityXRExtensions
    {
        /// <summary>
        /// Get all XRNodeState for a given XRNode type.
        /// </summary>
        public static IEnumerable<XRNodeState> GetNodeStates(this XRNode nodeType)
        {
            return NodeStates.Where(state => state.nodeType == nodeType);
        }

        /// <summary>
        /// Get the XRNodeState for a given XRNode type, if available.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown when there
        /// are multiple nodes of this type.
        /// </exception>
        public static XRNodeState? GetSingleNodeState(this XRNode nodeType)
        {
            var nodes = NodeStates.Where(state => state.nodeType == nodeType)
                                  .ToList();

            if (nodes.Count == 0)
            {
                return null;
            }
            else if (nodes.Count > 1)
            {
                throw new InvalidOperationException(
                    $"Cannot decide between multiple XRNodes of type {nodeType}.");
            }

            return nodes[0];
        }

        /// <summary>
        /// Return the node state's pose matrix, if available.
        /// </summary>
        public static Transformation? GetPose(this XRNodeState node)
        {
            if (node.TryGetPosition(out var position)
             && node.TryGetRotation(out var rotation))
            {
                return new Transformation(position, rotation, Vector3.one);
            }

            return null;
        }

        /// <summary>
        /// Return the pose matrix for a given XRNode type, if available.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown when there are
        /// multiple nodes of this type
        /// </exception>
        public static Transformation? GetSinglePose(this XRNode nodeType)
        {
            return nodeType.GetSingleNodeState()?.GetPose();
        }
        
        public static IPosedObject WrapAsPosedObject(this XRNode nodeType)
        {
            var wrapper = new DirectPosedObject();

            UpdatePoseInBackground().AwaitInBackground();

            async Task UpdatePoseInBackground()
            {
                while (true)
                {
                    wrapper.SetPose(nodeType.GetSinglePose());
                    await Task.Delay(1);
                }
            }

            return wrapper;
        }

        private static readonly List<XRNodeState> nodeStates = new List<XRNodeState>();

        /// <summary>
        /// Get all the states for tracked XR objects from Unity's XR system.
        /// </summary>
        public static IReadOnlyList<XRNodeState> NodeStates
        {
            get
            {
                InputTracking.GetNodeStates(nodeStates);

                return nodeStates;
            }
        }
    }
}