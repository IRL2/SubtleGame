// Copyright (c) 2019 Intangible Realities Lab. All rights reserved.
// Licensed under the GPL. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Narupa.Core.Math;
using NarupaImd.Interaction;
using UnityEngine;

namespace Narupa.Frontend.Manipulation
{
    /// <summary>
    /// Situates and IMD simulation in a Unity transform and allows grab
    /// manipulations to begin and end particle interactions.
    /// </summary>
    public class ManipulableParticles
    {
        /// <summary>
        /// The force multiplier.
        /// </summary>
        public float ForceScale { get; set; } = 100f;

        private readonly Transform transform;
        private readonly ParticleInteractionCollection interactions;

        public IInteractableParticles InteractableParticles { get; set; }

        private readonly HashSet<ActiveParticleGrab> activeGrabs
            = new HashSet<ActiveParticleGrab>();

        public ManipulableParticles(Transform transform,
                                    ParticleInteractionCollection interactions,
                                    IInteractableParticles interactableParticles)
        {
            this.transform = transform;
            this.interactions = interactions;
            this.InteractableParticles = interactableParticles;
        }

        /// <summary>
        /// Start a particle grab on whatever particle falls close to the position
        /// of the given grabber. Return either the manipulation or null if there was
        /// nothing grabbable.
        /// </summary>
        public IActiveManipulation StartParticleGrab(UnitScaleTransformation grabberPose)
        {
            if (InteractableParticles.GetParticleGrab(grabberPose) is ActiveParticleGrab grab)
                return StartParticleGrab(grabberPose, grab);
            return null;
        }

        private ActiveParticleGrab StartParticleGrab(UnitScaleTransformation grabberPose,
                                                     ActiveParticleGrab grab)
        {
            grab.UpdateManipulatorPose(grabberPose);
            grab.ParticleGrabUpdated += () => OnParticleGrabUpdated(grab);
            grab.ManipulationEnded += () => EndParticleGrab(grab);
            OnParticleGrabUpdated(grab);
            activeGrabs.Add(grab);
            return grab;
        }

        private void OnParticleGrabUpdated(ActiveParticleGrab grab)
        {
            var position = transform.InverseTransformPoint(grab.GrabPosition);

            interactions.UpdateValue(grab.Id, new ParticleInteraction()
            {
                Particles = grab.ParticleIndices.ToList(),
                Position = position,
                Scale = ForceScale,
                InteractionType = "spring",
                ResetVelocities = grab.ResetVelocities
            });
        }

        private void EndParticleGrab(ActiveParticleGrab grab)
        {
            activeGrabs.Remove(grab);
            interactions.RemoveValue(grab.Id);
        }
    }
}