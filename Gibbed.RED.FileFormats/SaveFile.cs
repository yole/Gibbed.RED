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
    public class SaveFile
    {
        public uint Version;

        private Save.TimeInfo _TimeInfo;
        private Save.SaveInfo _SaveInfo;
        private Save.TimeManager _TimeManger;
        private Save.IdTagManager _IdTagManager;
        private Save.QuestSystem _QuestSystem;

        public void Deserialize(Stream input)
        {
            if (input.ReadValueU32() != 0x59564153) // SAVY
            {
                throw new FormatException("bad magic");
            }

            this.Version = input.ReadValueU32();
            var subversion = input.ReadValueU32();
            
            if (this.Version >= 2)
            {
                long position = input.Position;
                input.Seek(input.Length - 4, SeekOrigin.Begin);
                if (input.ReadValueU32() != 0x5055434B) // KCUP
                {
                    throw new FormatException("bad EOF magic");
                }
                input.Seek(position, SeekOrigin.Begin);
            }

            var offsets = new Dictionary<string, uint>();
            for (int i = 0; i < 32; i++)
            {
                var name = input.ReadString(32, true, Encoding.ASCII);
                var offset = input.ReadValueU32();

                if (string.IsNullOrEmpty(name) == false)
                {
                    offsets.Add(name, offset);
                }
            }

            var blocks = new Dictionary<string, Save.ISaveStream>();
            foreach (var kv in offsets)
            {
                var dataStart = input.Position;

                input.Seek(kv.Value, SeekOrigin.Begin);
                if (input.ReadValueU32() != 0x4B434C42) // BLCK
                {
                    throw new FormatException();
                }

                var name = input.ReadEncodedString();
                if (name != kv.Key)
                {
                    throw new FormatException();
                }

                var size = input.ReadValueU32();
                var dataEnd = input.Position + size;

                input.Seek(kv.Value, SeekOrigin.Begin);
                var data = new byte[dataEnd - dataStart];
                if (input.Read(data, 0, data.Length) != data.Length)
                {
                    throw new FormatException();
                }

                blocks.Add(name, new Save.SaveReader(data));
            }

            blocks["timeInfo"].SerializeBlock("timeInfo", ref this._TimeInfo);
            blocks["saveInfo"].SerializeBlock("saveInfo", ref this._SaveInfo);
            blocks["timeManger"].SerializeBlock("timeManger", ref this._TimeManger);
            blocks["idTagManager"].SerializeBlock("idTagManager", ref this._IdTagManager);
            blocks["questSystem"].SerializeBlock("questSystem", ref this._QuestSystem);
            throw new NotImplementedException();
        }
    }
}
