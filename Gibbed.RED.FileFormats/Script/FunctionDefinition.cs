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
using System.Linq;
using System.Text;
using Gibbed.RED.FileFormats.Script.Instructions;

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
        public List<int> InstructionOffsets;

        public override string ToString()
        {
            return ToString(true);
        }

        public string ToString(bool includeContainingClass)
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
            if (ContainingClass != null && includeContainingClass)
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

        public override bool Equals(object obj)
        {
            var rhs = obj as FunctionDefinition;
            if (rhs == null) return false;
            if (Instructions == null)
            {
                return rhs.Instructions == null;
            }
            if (rhs.Instructions == null)
            {
                return false;
            }
            if (Instructions.Count != rhs.Instructions.Count)
            {
                return false;
            }
            return !Instructions.Where((t, i) => t.Opcode != rhs.Instructions[i].Opcode).Any();
            // TODO check operands
        }

        public void DisassembleToConsole()
        {
            if (Instructions != null)
            {
                Console.WriteLine("{");
                foreach (var local in Locals)
                {
                    Console.WriteLine("    " + local);
                }
                if (Locals.Count > 0)
                {
                    Console.WriteLine();
                }
                for (int i = 0; i < Instructions.Count; i++)
                {
                    var instruction = Instructions[i];
                    if (instruction is Target)
                    {
                        Console.WriteLine("");
                    }
                    Console.WriteLine("    {0:D4} {1}", InstructionOffsets[i], instruction);
                }
                Console.WriteLine("}\n");
            }
        }
    }
}
