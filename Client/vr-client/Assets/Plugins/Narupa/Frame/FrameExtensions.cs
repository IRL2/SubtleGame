// Copyright (c) Intangible Realities Lab. All rights reserved.
// Licensed under the GPL. See License.txt in the project root for license information.

using System.Linq;
using Narupa.Core.Science;

namespace Narupa.Frame
{
    public static class FrameExtensions
    {
        /// <summary>
        /// Recenter particles about the origin.
        /// </summary>
        public static void RecenterAroundOrigin(this Frame frame, bool massWeighted = false)
        {
            if (frame.Particles.Count == 0)
                return;

            var total = frame.Particles
                             .Select(p => massWeighted
                                              ? p.Position * GetMass(p)
                                              : p.Position)
                             .Aggregate((v, w) => v + w);
            total /= frame.Particles
                          .Select(p => massWeighted ? GetMass(p) : 1).Sum();
            for (var i = 0; i < frame.ParticlePositions.Length; i++)
                frame.ParticlePositions[i] -= total;
        }

        private static float GetMass(IParticle particle)
        {
            if (particle is ParticleReference reference)
                return reference.Element?.GetStandardAtomicWeight() ?? 1;
            return 1;
        }
    }
}