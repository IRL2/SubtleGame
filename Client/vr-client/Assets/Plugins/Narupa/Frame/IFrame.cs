// Copyright (c) Intangible Realities Laboratory. All rights reserved.
// Licensed under the GPL. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Narupa.Frame
{
    /// <summary>
    /// A single frame with a trajectory. It contains a snapshot of the system, with a
    /// list of particles with types, positions etc.
    /// </summary>
    public interface IFrame
    {
        /// <summary>
        /// List of particles in the current frame.
        /// </summary>
        IReadOnlyList<IParticle> Particles { get; }

        /// <summary>
        /// List of bonds between particles.
        /// </summary>
        IReadOnlyList<BondPair> Bonds { get; }

        /// <summary>
        /// General data associated with the current frame.
        /// </summary>
        IDictionary<string, object> Data { get; }
    }
}