using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Gibbed.RED.FileFormats;
using Gibbed.RED.FileFormats.Script;

namespace Gibbed.RED.Diff
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: Gibbed.RED.Diff <old.w2scripts> <new.w2scripts>");
                return;
            }
            var oldScripts = new CompiledScriptsFile();
            var newScripts = new CompiledScriptsFile();
            using(var oldInput = File.OpenRead(args[0]))
            {
                oldScripts.Deserialize(oldInput);
            }
            using(var newInput = File.OpenRead(args[1]))
            {
                newScripts.Deserialize(newInput);
            }

            var oldFunctions = new Dictionary<string, FunctionDefinition>();
            foreach(var def in oldScripts.FuncDefs)
            {
                oldFunctions[def.ToString()] = def;
            }

            foreach (var def in newScripts.FuncDefs)
            {
                if (!oldFunctions.ContainsKey(def.ToString()))
                {
                    Console.WriteLine("\n\n\nNew function: " + def);
                    def.DisassembleToConsole();
                }
                else
                {
                    var oldDef = oldFunctions[def.ToString()];
                    if (!oldDef.Equals(def))
                    {
                        Console.WriteLine("\n\n\nChanged function: " + def);
                        Console.WriteLine("Old bytecode: ");
                        oldDef.DisassembleToConsole();
                        Console.WriteLine("New bytecode: ");
                        def.DisassembleToConsole();
                    }
                }
            }
        }
    }
}
