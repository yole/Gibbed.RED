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
    public class CNode : CStateMachine
    {
        [PropertyName("name")]
        [PropertySerializer(typeof(StringSerializer))]
        public string Name { get; set; }

        [PropertyName("tags")]
        [PropertySerializer(typeof(TagListSerializer))]
        public List<string> Tags { get; set; }
        
        // transform
        // transformParent

        [PropertyName("guid")]
        [PropertySerializer(typeof(GuidSerializer))]
        public Guid Guid { get; set; }

        public List<IFileObject> CNodeUnknown0;
        public List<IFileObject> CNodeUnknown1;
        public ProbablyMatrix4x4 CNodeUnknown2;

        public override void Serialize(IFileStream stream)
        {
            base.Serialize(stream);
            stream.SerializePointer(ref this.CNodeUnknown0, false);
            stream.SerializePointer(ref this.CNodeUnknown1, false);
            stream.SerializeObject(ref this.CNodeUnknown2);
        }
    }
}
