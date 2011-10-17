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
    public class QuestSystem : ISaveBlock
    {
        private List<QuestExternalScenePlayer> _QuestExternalScenePlayers;
        private List<Quest> _Quests;

        public void Serialize(ISaveStream stream)
        {
            // supposedly only ever has two blocks but oh well...
            stream.SerializeBlocks(
                "questExternalScenePlayers",
                "CQuestExternalScenePlayer",
                ref this._QuestExternalScenePlayers);

            if (stream.Mode == SerializeMode.Reading)
            {
                uint numQuests = 0;
                stream.SerializeValue("numQuests", ref numQuests);

                this._Quests = new List<Quest>();
                for (uint i = 0; i < numQuests; i++)
                {
                    Quest quest = null;
                    stream.SerializeBlock("quest", ref quest);
                    this._Quests.Add(quest);
                }

                throw new NotImplementedException();
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
