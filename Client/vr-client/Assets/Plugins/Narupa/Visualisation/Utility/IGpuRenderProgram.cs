// Copyright (c) Intangible Realities Lab. All rights reserved.
// Licensed under the GPL. See License.txt in the project root for license information.

namespace Narupa.Visualisation.Utility
{
    /// <summary>
    /// A GPU program that also supports keywords that modify how the shader works
    /// </summary>
    public interface IGpuRenderProgram : IGpuProgram
    {
        /// <summary>
        /// Set a keyword on the material
        /// </summary>
        void SetKeyword(string keyword, bool enabled = true);
    }
}