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

namespace Gibbed.RED.FileFormats.Serializers
{
    public class EngineTransformSerializer : IPropertySerializer
    {
        public void Serialize(IFileStream stream, object value)
        {
            throw new NotImplementedException();
        }

        public object Deserialize(IFileStream stream)
        {
            var transform = new Game.EngineTransform();
            stream.SerializeValue(ref transform.Flags);

            if ((transform.Flags & 1) == 1)
            {
                stream.SerializeValue(ref transform.Unknown1);
                stream.SerializeValue(ref transform.Unknown2);
                stream.SerializeValue(ref transform.Unknown3);
            }

            if ((transform.Flags & 2) == 2)
            {
                stream.SerializeValue(ref transform.Unknown4);
                stream.SerializeValue(ref transform.Unknown5);
                stream.SerializeValue(ref transform.Unknown6);
            }

            if ((transform.Flags & 4) == 4)
            {
                stream.SerializeValue(ref transform.Unknown7);
                stream.SerializeValue(ref transform.Unknown8);
                stream.SerializeValue(ref transform.Unknown9);
            }

            return transform;
        }
    }
}
