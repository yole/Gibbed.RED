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
using System.Text;

namespace Gibbed.RED.FileFormats.Script
{
    public class FunctionDefinition
    {
        public string Name;
        public TypeDefinition ContainingClass;
        public TypeDefinition ReturnValue;
        public readonly List<ArgumentDefinition> Arguments = new List<ArgumentDefinition>();
        public readonly List<LocalDefinition> Locals = new List<LocalDefinition>();
        public List<IInstruction> Instructions;

        public override string ToString()
        {
            var result = new StringBuilder();
            if (ReturnValue != null)
            {
                result.Append(ReturnValue.Name);
            }
            else
            {
                result.Append("void");
            }
            result.Append(" ");
            if (ContainingClass != null)
            {
                result.Append(ContainingClass.Name).Append("::");
            }
            result.Append(Name).Append("(");
            for (int i = 0; i < Arguments.Count; i++)
            {
                if (i > 0)
                {
                    result.Append(", ");
                }
                result.Append(Arguments[i].Type.Name).Append(" ").Append(Arguments[i].Name);
            }
            result.Append(")");
            return result.ToString();
        }
    }
}
