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

namespace Gibbed.RED.FileFormats.Save
{
    public class QuestBlock : ISaveBlock
    {
        private Guid _GUID;
        private List<string> _InputNames;
        private int _ActivationState;

        public void Serialize(ISaveStream stream)
        {
            if (stream.Mode == SerializeMode.Reading)
            {
                stream.SerializeValue("GUID", ref this._GUID);

                uint inputNamesCount = 0;
                stream.SerializeValue("inputNamesCount", ref inputNamesCount);
                this._InputNames = new List<string>();
                for (uint i = 0; i < inputNamesCount; i++)
                {
                    string inputName = null;
                    stream.SerializeValue("inputName", ref inputName);
                    this._InputNames.Add(inputName);
                }

                stream.SerializeValue("activationState", ref this._ActivationState);

                // it appears there is data loaded here
                // depending on the actual quest data :(
                throw new NotImplementedException();
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
