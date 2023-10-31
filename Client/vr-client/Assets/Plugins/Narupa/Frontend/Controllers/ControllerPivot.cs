﻿// Copyright (c) Intangible Realities Lab. All rights reserved.
// Licensed under the GPL. See License.txt in the project root for license information.

using System;
using Narupa.Core.Math;
using Narupa.Frontend.Input;
using UnityEngine;

namespace Narupa.Frontend.Controllers
{
    /// <summary>
    /// Component to indicate a part of the controller with a position and radius.
    /// </summary>
    public class ControllerPivot : MonoBehaviour, IPosedObject
    {
#pragma warning disable 0649
        [SerializeField]
        private float radius;
#pragma warning restore 0649

        public float Radius => radius;
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, radius);
            Gizmos.DrawSphere(transform.position, 0.2f * radius);
        }

        private void Update()
        {
            if (transform.hasChanged)
            {
                PoseChanged?.Invoke();
                transform.hasChanged = false;
            }
        }

        /// <inheritdoc cref="IPosedObject.Pose" />
        public Transformation? Pose
        {
            get
            {
                var transformation = Transformation.FromTransformRelativeToWorld(transform);
                transformation.Scale = Vector3.one * radius;
                return transformation;
            }
        }

        /// <inheritdoc cref="IPosedObject.PoseChanged" />
        public event Action PoseChanged;
    }
}