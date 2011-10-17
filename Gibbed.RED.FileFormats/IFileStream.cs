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

namespace Gibbed.RED.FileFormats
{
    public interface IFileStream
    {
        SerializeMode Mode { get; }
        long Position { get; set; }
        long Length { get; }

        void SerializeValue(ref bool value);
        void SerializeValue(ref sbyte value);
        void SerializeValue(ref byte value);
        void SerializeValue(ref short value);
        void SerializeValue(ref ushort value);
        void SerializeValue(ref int value);
        void SerializeValue(ref uint value);
        void SerializeValue(ref float value);
        void SerializeValue(ref string value);
        void SerializeValue(ref Guid value);
        void SerializeValue(ref byte[] value, int length);
        void SerializeValue(ref byte[] value, uint length);
        void SerializeBuffer(ref byte[] value);
        void SerializeName(ref string value);
        void SerializeTagList(ref List<string> value);
        void SerializeObject<TType>(ref TType value)
            where TType : IFileObject, new();
        void SerializePointer(ref IFileObject value);
        void SerializePointer(ref List<IFileObject> value, bool encoded);
    }
}
