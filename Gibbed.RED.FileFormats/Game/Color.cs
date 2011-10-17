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

using Gibbed.RED.FileFormats.Serializers;

namespace Gibbed.RED.FileFormats.Game
{
    public class Color : TTypedClass
    {
        [PropertyName("Red")]
        [PropertySerializer(typeof(Uint8Serializer))]
        public byte Red { get; set; }

        [PropertyName("Green")]
        [PropertySerializer(typeof(Uint8Serializer))]
        public byte Green { get; set; }

        [PropertyName("Blue")]
        [PropertySerializer(typeof(Uint8Serializer))]
        public byte Blue { get; set; }

        [PropertyName("Alpha")]
        [PropertySerializer(typeof(Uint8Serializer))]
        public byte Alpha { get; set; }

        public override string ToString()
        {
            return string.Format("#{0:X2}{1:X2}{2:X2}:{3:X2}",
                this.Red, this.Green, this.Blue, this.Alpha);
        }
    }
}
