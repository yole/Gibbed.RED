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

using System;
using System.Collections.Generic;
using Gibbed.RED.FileFormats.Serializers;

namespace Gibbed.RED.FileFormats.Game
{
    [ResourceHandler("CMesh")]
    public class CMesh : CResource
    {
        [PropertyName("materials")]
        [PropertySerializer(typeof(ArraySerializer<PointerSerializer>))]
        public List<object> Materials { get; set; }

        [PropertyName("boundingBox")]
        [PropertySerializer(typeof(StructureSerializer<Box>))]
        public Box BoundingBox { get; set; }

        [PropertyName("autoHideDistance")]
        [PropertySerializer(typeof(FloatSerializer))]
        public float AutoHideDistance { get; set; }

        [PropertyName("lodDissolveTransitionTime")]
        [PropertySerializer(typeof(FloatSerializer))]
        public float LodDissolveTransitionTime { get; set; }

        [PropertyName("importFile")]
        [PropertySerializer(typeof(StringSerializer))]
        public string ImportFile { get; set; }

        public override void Serialize(IFileStream stream)
        {
            base.Serialize(stream);
            throw new NotImplementedException();
        }
    }
}
