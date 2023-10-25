// Copyright (c) Intangible Realities Laboratory. All rights reserved.
// Licensed under the GPL. See License.txt in the project root for license information.

/// Contains methods for lighting

#ifndef LIGHTING_CGINC_INCLUDED

    #define LIGHTING_CGINC_INCLUDED
    
    #define DIFFUSE(c, n, l, d) c * saturate(lerp(1, dot(n, l), d));
    
#endif