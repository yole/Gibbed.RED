using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Gibbed.RED.FileFormats;
using Gibbed.RED.FileFormats.Script;
using Gibbed.RED.FileFormats.Script.Instructions;

namespace Gibbed.RED.ScriptDecompiler
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: Gibbed.RED.ScriptDecompiler <path to compiledscripts.w2scripts>");
                return;
            }
            var input = File.OpenRead(args[0]);
            var scriptFile = new CompiledScriptsFile();
            scriptFile.Deserialize(input);
            input.Close();

            foreach(var typedef in scriptFile.TypeDefs)
            {
                if (typedef is ClassDefinition)
                {
                    DecompileClass((ClassDefinition) typedef, Console.Out);
                }
                else if (typedef is EnumDefinition)
                {
                    DecompileEnum((EnumDefinition) typedef, Console.Out);
                }
            }
        }

        private static void DecompileEnum(EnumDefinition typedef, TextWriter output)
        {
            output.WriteLine("enum " + typedef.Name);
            output.WriteLine("{");
            foreach (var constant in typedef.Constants)
            {
                output.WriteLine("    " + constant.Name + " = " + constant.Value + ",");
            }
            output.WriteLine("}\n");
        }

        private static void DecompileClass(ClassDefinition typedef, TextWriter output)
        {
            output.WriteLine("class " + typedef.Name);
            output.WriteLine("{");
            foreach(var prop in typedef.Properties.Values)
            {
                output.WriteLine("    " + prop.TypeDefinition.Name + " " + prop.Name + ";");
            }
            if (typedef.Properties.Count > 0)
            {
                output.WriteLine("");
            }
            foreach(var func in typedef.Functions.Values)
            {
                DecompileFunction(func, output, "    ");
            }
            output.WriteLine("}\n");
        }

        private static void DecompileFunction(FunctionDefinition func, TextWriter output, string indent)
        {
            output.WriteLine(indent + func.ToString(false));
            if (func.Instructions == null) return;
            output.WriteLine(indent + "{");
            foreach(var local in func.Locals)
            {
                output.WriteLine(indent + "    " + local);
            }
            if (func.Locals.Count > 0)
            {
                output.WriteLine("");
            }
            var decompiler = new FunctionDecompiler(func);
            var statements = decompiler.DecompileStatements();
            foreach(var statement in statements)
            {
                output.WriteLine(indent + "    " + statement + ";");
            }
 
            output.WriteLine(indent + "}\n");
        }
    }
}
