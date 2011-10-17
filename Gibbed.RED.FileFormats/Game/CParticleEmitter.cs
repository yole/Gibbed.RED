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
    [ResourceHandler("CParticleEmitter")]
    public class CParticleEmitter : CObject
    {
        [PropertyName("modules")]
        [PropertySerializer(typeof(ArraySerializer<PointerSerializer>))]
        public List<object> Modules { get; set; }

        [PropertyName("positionX")]
        [PropertySerializer(typeof(IntSerializer))]
        public int PositionX { get; set; }

        [PropertyName("positionY")]
        [PropertySerializer(typeof(IntSerializer))]
        public int PositionY { get; set; }

        [PropertyName("material")]
        [PropertySerializer(typeof(PointerSerializer))]
        public IFileObject Material { get; set; }

        [PropertyName("maxParticles")]
        [PropertySerializer(typeof(UintSerializer))]
        public uint MaxParticles { get; set; }

        [PropertyName("emitterDuration")]
        [PropertySerializer(typeof(FloatSerializer))]
        public float EmitterDuration { get; set; }

        [PropertyName("emitterLoops")]
        [PropertySerializer(typeof(IntSerializer))]
        public int EmitterLoops { get; set; }

        [PropertyName("birthRate")]
        [PropertySerializer(typeof(PointerSerializer))]
        public IFileObject BirthRate { get; set; }

        [PropertyName("particleDrawer")]
        [PropertySerializer(typeof(PointerSerializer))]
        public IFileObject ParticleDrawer { get; set; }

        [PropertyName("keepSimulationLocal")]
        [PropertySerializer(typeof(BoolSerializer))]
        public bool KeepSimulationLocal { get; set; }

        [PropertyName("envColorGroup")]
        [PropertySerializer(typeof(EnumSerializer<EEnvColorGroup>))]
        public EEnvColorGroup EnvColorGroup { get; set; }

        [PropertyName("modifierSetMask")]
        [PropertySerializer(typeof(UintSerializer))]
        public uint ModifierSetMask { get; set; }

        [PropertyName("numModifiers")]
        [PropertySerializer(typeof(UintSerializer))]
        public uint NumModifiers { get; set; }

        [PropertyName("initializerSetMask")]
        [PropertySerializer(typeof(UintSerializer))]
        public uint InitializerSetMask { get; set; }

        [PropertyName("numInitializers")]
        [PropertySerializer(typeof(UintSerializer))]
        public uint NumInitializers { get; set; }

        [PropertyName("editorName")]
        [PropertySerializer(typeof(StringSerializer))]
        public string EditorName { get; set; }

        [PropertyName("editorColor")]
        [PropertySerializer(typeof(ClassSerializer<Color>))]
        public Color EditorColor { get; set; }
    }
}
