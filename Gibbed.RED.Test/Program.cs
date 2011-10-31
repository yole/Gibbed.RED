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
using System.IO;
using Gibbed.RED.FileFormats;
using Gibbed.RED.FileFormats.Script.Instructions;

namespace Gibbed.RED.Test
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            using (var input = File.OpenRead(@"C:\src\w2\compiledscripts.w2scripts"))
            {
                var test = new CompiledScriptsFile();
                test.Deserialize(input);

                for (int i = 0; i < test.TypeDefs.Length; i++)
                {
                    Console.WriteLine(i + ". " + test.TypeDefs[i]);
                    if (test.TypeDefs[i] is FileFormats.Script.ClassDefinition)
                    {
                        var classDef = test.TypeDefs[i] as FileFormats.Script.ClassDefinition;
                        foreach (var propDef in classDef.Properties)
                        {
                            Console.WriteLine("    " + propDef.Value);
                        }
                        Console.WriteLine();
                        foreach (var funcDef in classDef.Functions)
                        {
                            Console.WriteLine("    " + funcDef.Value);
                        }
                    }
                }

                Console.WriteLine("\n\nFUNCTIONS:");
                for (int index = 0; index < test.FuncDefs.Length; index++)
                {
                    var funcDef = test.FuncDefs[index];
                    Console.WriteLine(index + ". " + funcDef);
                    if (funcDef.Instructions != null)
                    {
                        Console.WriteLine("{");
                        foreach (var local in funcDef.Locals)
                        {
                            Console.WriteLine("    " + local);
                        }
                        if (funcDef.Locals.Count > 0)
                        {
                            Console.WriteLine();
                        }
                        for (int i = 0; i < funcDef.Instructions.Count; i++)
                        {
                            var instruction = funcDef.Instructions[i];
                            if (instruction is Target)
                            {
                                Console.WriteLine("");
                            }
                            Console.WriteLine("    {0:D4} {1}", funcDef.InstructionOffsets[i], instruction);
                        }
                        Console.WriteLine("}\n");
                    }
                }
            }
        }
    }
}
