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

using System.Collections.Generic;
using Gibbed.RED.FileFormats.Serializers;

namespace Gibbed.RED.FileFormats.Game
{
    [ResourceHandler("CEntityTemplate")]
    public class CEntityTemplate : CResource
    {
        [PropertyName("backgroundOffset")]
        [PropertySerializer(typeof(StructureSerializer<Vector>))]
        public Vector BackgroundOffset { get; set; }

        [PropertyName("entityClass")]
        [PropertySerializer(typeof(CNameSerializer))]
        public string EntityClass { get; set; }

        [PropertyName("entityObject")]
        [PropertySerializer(typeof(PointerSerializer))]
        public IFileObject EntityObject { get; set; }

        [PropertyName("cookedEntityObject")]
        [PropertySerializer(typeof(PointerSerializer))]
        public IFileObject CookedEntityObject { get; set; }

        [PropertyName("bodyParts")]
        [PropertySerializer(typeof(ArraySerializer<CEntityBodyPart, StructureSerializer<CEntityBodyPart>>))]
        public List<CEntityBodyPart> BodyParts { get; set; }

        [PropertyName("appearances")]
        [PropertySerializer(typeof(ArraySerializer<CEntityAppearance, StructureSerializer<CEntityAppearance>>))]
        public List<CEntityAppearance> Appearances { get; set; }

        [PropertyName("usedAppearances")]
        [PropertySerializer(typeof(ArraySerializer<string, CNameSerializer>))]
        public List<string> UsedAppearances { get; set; }

        [PropertyName("voicetagAppearances")]
        [PropertySerializer(typeof(ArraySerializer<VoicetagAppearancePair, StructureSerializer<VoicetagAppearancePair>>))]
        public List<VoicetagAppearancePair> VoicetagAppearances { get; set; }

        [PropertyName("effects")]
        [PropertySerializer(typeof(ArraySerializer<PointerSerializer>))]
        public List<object> Effects { get; set; }

        [PropertyName("slots")]
        [PropertySerializer(typeof(ArraySerializer<EntitySlot, StructureSerializer<EntitySlot>>))]
        public List<EntitySlot> Slots { get; set; }

        [PropertyName("templateParams")]
        [PropertySerializer(typeof(ArraySerializer<PointerSerializer>))]
        public List<object> TemplateParams { get; set; }

        [PropertyName("wasMerged")]
        [PropertySerializer(typeof(BoolSerializer))]
        public bool WasMerged { get; set; }

        // Nested ResourceFile? :wtc:
        public byte[] CEntityTemplateUnknown0;

        public override void Serialize(IFileStream stream)
        {
            base.Serialize(stream);
            stream.SerializeBuffer(ref this.CEntityTemplateUnknown0);
        }
    }
}
