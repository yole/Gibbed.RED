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
using System.IO;
using System.Linq;
using System.Text;
using Gibbed.Helpers;

namespace Gibbed.RED.FileFormats
{
    public class StringsFile
    {
        public uint Version;
        public uint EncryptionKey = 0;
        public Dictionary<string, uint> Keys
            = new Dictionary<string, uint>();
        public Dictionary<uint, string> Texts
            = new Dictionary<uint, string>();

        public void Serialize(Stream output)
        {
            output.WriteValueU32(this.Version);

            if (this.Version >= 114)
            {
                output.WriteValueU16((ushort)((this.EncryptionKey >> 16) & 0xFFFF));
            }

            var magic = GetRealKey(this.EncryptionKey);

            output.WriteValueEncodedS32(this.Keys.Count);
            foreach (var kv in this.Keys)
            {
                output.WriteEncodedString(kv.Key);
                output.WriteValueU32(kv.Value ^ magic);
            }

            if (this.Version >= 114)
            {
                output.WriteValueU16((ushort)((this.EncryptionKey >> 0) & 0xFFFF));
            }

            uint fileStringsHash = 0;

            var hashPosition = output.Position;
            if (this.Version >= 200)
            {
                output.WriteValueU32(0);
            }

            output.WriteValueEncodedS32(this.Texts.Count);
            foreach (var kv in this.Texts)
            {
                output.WriteValueU32(kv.Key ^ magic);

                /*
                byte[] buffer;
                if (kv.Key == 65434)
                {
                    using (var test = File.OpenRead("corrupt_string.bin"))
                    {
                        buffer = new byte[test.Length];
                        test.Read(buffer, 0, buffer.Length);
                    }
                }
                else
                {
                    buffer = Encoding.Unicode.GetBytes(kv.Value);
                }
                */
                
                var buffer = Encoding.Unicode.GetBytes(kv.Value);

                ushort stringKey = (ushort)(magic >> 8);

                for (int j = 0; j < buffer.Length; j += 2)
                {
                    if (this.Version >= 200)
                    {
                        var charKey = (ushort)(((buffer.Length / 2) + 1) * stringKey);
                        buffer[j + 0] ^= (byte)((charKey >> 0) & 0xFF);
                        buffer[j + 1] ^= (byte)((charKey >> 8) & 0xFF);
                        stringKey = stringKey.RotateLeft(1);
                    }
                    else
                    {
                        throw new NotImplementedException("string obfuscation for old strings files is untested");

                        // untested
                        buffer[j + 0] ^= (byte)((stringKey >> 0) & 0xFF);
                        buffer[j + 1] ^= (byte)((stringKey >> 8) & 0xFF);
                        stringKey++;
                    }

                    fileStringsHash += BitConverter.ToUInt16(buffer, j);
                }

                output.WriteEncodedStringBuffer(buffer);
            }

            if (this.Version >= 200)
            {
                var endPosition = output.Position;
                output.Seek(hashPosition, SeekOrigin.Begin);
                output.WriteValueU32(fileStringsHash ^ magic);
                output.Seek(endPosition, SeekOrigin.Begin);
            }
            else if (this.Version >= 114)
            {
                output.WriteValueU32(fileStringsHash);
            }
        }

        public void Deserialize(Stream input)
        {
            this.Version = input.ReadValueU32();
            this.EncryptionKey = 0;

            if (this.Version >= 114)
            {
                this.EncryptionKey |= (uint)(input.ReadValueU16() << 16);
            }

            var keyCount = input.ReadValueEncodedS32();
            this.Keys.Clear();
            for (int i = 0; i < keyCount; i++)
            {
                var key = input.ReadEncodedString();
                var index = input.ReadValueU32();
                this.Keys.Add(key, index);
            }

            if (this.Version >= 114)
            {
                this.EncryptionKey |= (uint)(input.ReadValueU16() << 0);
            }

            var magic = GetRealKey(this.EncryptionKey);

            uint fileStringsHash = 0;

            if (this.Version >= 200)
            {
                fileStringsHash = input.ReadValueU32();
                fileStringsHash ^= magic;
            }

            foreach (var key in this.Keys.Keys.ToArray())
            {
                this.Keys[key] ^= magic;
            }

            uint actualStringsHash = 0;
            var stringCount = input.ReadValueEncodedS32();
            for (int i = 0; i < stringCount; i++)
            {
                var index = input.ReadValueU32();
                index ^= magic;

                var buffer = input.ReadEncodedStringBuffer();

                ushort stringKey = (ushort)(magic >> 8);

                uint hash = 0;

                for (int j = 0; j < buffer.Length; j += 2)
                {
                    hash += BitConverter.ToUInt16(buffer, j);

                    if (this.Version >= 200)
                    {
                        var charKey = (ushort)(((buffer.Length / 2) + 1) * stringKey);
                        buffer[j + 0] ^= (byte)((charKey >> 0) & 0xFF);
                        buffer[j + 1] ^= (byte)((charKey >> 8) & 0xFF);
                        stringKey = stringKey.RotateLeft(1);
                    }
                    else
                    {
                        throw new NotImplementedException("string obfuscation for old strings files is untested");

                        // untested
                        buffer[j + 0] ^= (byte)((stringKey >> 0) & 0xFF);
                        buffer[j + 1] ^= (byte)((stringKey >> 8) & 0xFF);
                        stringKey++;
                    }
                }

                actualStringsHash += hash;

                if (index == 65434 &&
                    hash == 83394453)
                {
                    // hack to fix this dumb corrupted string in en0.w2strings
                    this.Texts.Add(index, Encoding.Unicode.GetString(buffer, 0, 104));
                }
                else
                {
                    this.Texts.Add(index, Encoding.Unicode.GetString(buffer));
                }
            }

            if (this.Version >= 114 && this.Version < 200)
            {
                fileStringsHash = input.ReadValueU32();
            }

            if (this.Version >= 114 && fileStringsHash != actualStringsHash)
            {
                throw new FormatException("hash for strings does not match");
            }
        }

        private static uint GetRealKey(uint fileKey)
        {
            /* Thanks to hhrhhr for making it obvious that the
             * keys were tied to specific language files.
             * 
             * I hadn't noticed that for some reason. :)
             */

            switch (fileKey)
            {
                case 0x18632176: return 0x16875467;
                case 0x18796651: return 0x42387566; // ES
                case 0x23863176: return 0x75921975; // FR
                case 0x24987354: return 0x21793217;
                case 0x42378932: return 0x67823218;
                case 0x43975139: return 0x79321793; // EN
                case 0x45931894: return 0x12375973; // IT
                case 0x54834893: return 0x59825646;
                case 0x63481486: return 0x42386347; // RU (1.1)
                case 0x75886138: return 0x42791159; // DE
                case 0x77932179: return 0x54932186; // RU (1.0)
                case 0x83496237: return 0x73946816; // PL
            }

            return 0;
        }
    }
}
