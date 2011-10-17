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
using System.Text;
using Gibbed.Helpers;

namespace Gibbed.RED.FileFormats
{
    public class PackageFile
    {
        public uint Version;
        public List<Package.Entry> Entries
            = new List<Package.Entry>();

        public ulong EntriesHash
        {
            get
            {
                var hash = 0x00000000FFFFFFFFUL;
                foreach (var entry in this.Entries)
                {
                    if (string.IsNullOrEmpty(entry.Name) == false)
                    {
                        for (int i = 0; i < entry.Name.Length; i++)
                        {
                            hash ^= (byte)entry.Name[i];
                            hash *= 0x00000100000001B3UL;
                        }

                        hash ^= (ulong)entry.Name.Length;
                        hash *= 0x00000100000001B3UL;
                    }

                    hash ^= (ulong)entry.TimeStamp.ToFileTime();
                    hash *= 0x00000100000001B3UL;
                    hash ^= (ulong)entry.UncompressedSize;
                    hash *= 0x00000100000001B3UL;
                    hash ^= (ulong)entry.Offset;
                    hash *= 0x00000100000001B3UL;
                    hash ^= (ulong)entry.CompressedSize;
                    hash *= 0x00000100000001B3UL;
                }
                return hash;
            }
        }

        public const int HeaderSize = 32;

        public void Serialize(Stream output, long entryTableOffset)
        {
            output.Seek(0, SeekOrigin.Begin);
            output.WriteValueU32(0x50495A44); // DZIP
            output.WriteValueU32(2);
            output.WriteValueS32(this.Entries.Count);
            output.WriteValueU32(0x64626267);
            output.WriteValueS64(entryTableOffset);
            output.WriteValueU64(this.EntriesHash);

            output.Seek(entryTableOffset, SeekOrigin.Begin);
            foreach (var entry in this.Entries)
            {
                output.WriteValueU16((ushort)(entry.Name.Length + 1));
                output.WriteString(entry.Name, Encoding.ASCII);
                output.WriteValueU8(0);
                
                output.WriteValueS64(entry.TimeStamp.ToFileTime());
                output.WriteValueS64(entry.UncompressedSize);
                output.WriteValueS64(entry.Offset);
                output.WriteValueS64(entry.CompressedSize);
            }
        }

        public void DeserializeWithCDKey(Stream input, string cdkey)
        {
            if (cdkey == null)
            {
                this.Deserialize(input, null);
            }
            else
            {
                var decryptKey = 0x00000000FFFFFFFFUL;
                if (string.IsNullOrEmpty(cdkey) == false)
                {
                    for (int i = 0; i < cdkey.Length; i++)
                    {
                        // probably a bug they never fixed
                        // that they use a uint instead of ulong
                        decryptKey ^= ((uint)cdkey[i] << 8 * (i % 8));
                    }
                }

                this.Deserialize(input, decryptKey);
            }
        }

        public void Deserialize(Stream input, ulong? decryptKey)
        {
            if (input.ReadValueU32() != 0x50495A44) // DZIP
            {
                throw new FormatException("bad magic");
            }

            this.Version = input.ReadValueU32();
            if (this.Version < 2)
            {
                throw new FormatException("unsupported version");
            }

            var entryCount = input.ReadValueU32();
            /*var unknown =*/ input.ReadValueU32();
            var entryTableOffset = input.ReadValueS64();
            var entryTableHash = input.ReadValueU64();

            input.Seek(entryTableOffset, SeekOrigin.Begin);
            this.Entries.Clear();
            for (uint i = 0; i < entryCount; i++)
            {
                var entry = new Package.Entry();
                var length = input.ReadValueU16();

                if (decryptKey.HasValue == false)
                {
                    entry.Name = input.ReadString(
                        length, true, Encoding.ASCII);
                }
                else
                {
                    var nameBytes = new byte[length];
                    input.Read(nameBytes, 0, nameBytes.Length);

                    for (int j = 0; j < nameBytes.Length; j++)
                    {
                        nameBytes[j] ^= (byte)(decryptKey >> j % 8);
                        decryptKey *= 0x00000100000001B3UL;
                    }

                    using (var nameStream = new MemoryStream(nameBytes))
                    {
                        entry.Name = nameStream.ReadString(
                            length, true, Encoding.ASCII);
                    }
                }
                
                //entry.TimeStamp = input.ReadValueS64();
                entry.TimeStamp = DateTime.FromFileTime(
                    input.ReadValueS64());
                entry.UncompressedSize = input.ReadValueS64();
                entry.Offset = input.ReadValueS64();
                entry.CompressedSize = input.ReadValueS64();
                this.Entries.Add(entry);
            }

            if (this.EntriesHash != entryTableHash)
            {
                throw new FormatException("bad entry table hash (wrong cdkey for DLC archives?)");
            }
        }
    }
}
