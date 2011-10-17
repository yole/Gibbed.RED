/* Copyright (c) 2011 Rick (rick 'at' gibbed 'dot' us)
 * 
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would
 *    be appreciated but is not required.
 * 
 * 2. Altered source versions must be plainly marked as such, and must not
 *    be misrepresented as being the original software.
 * 
 * 3. This notice may not be removed or altered from any source
 *    distribution.
 */

namespace Gibbed.RED.FileFormats.Game
{
    public enum ERenderingSortGroup
    {
        RSG_DebugUnlit = 0,
        RSG_Unlit = 1,
        RSG_LitOpaque = 2,
        RSG_LitOpaqueWithEmissive = 3,
        RSG_DecalModulativeColor = 4,
        RSG_DecalBlendedColor = 5,
        RSG_DecalBlendedNormalsColor = 6,
        RSG_DecalBlendedNormals = 7,
        RSG_Sprites = 8,
        RSG_RefractiveBackground = 9,
        RSG_RefractiveBackgroundDepthWrite = 10,
        RSG_Transparent = 11,
        RSG_DebugTransparent = 12,
        RSG_DebugOverlay = 13,
        RSG_2D = 14,
        RSG_Prepare = 15,
        RSG_Skin = 16,
        RSG_Max = 17,
    }
}
