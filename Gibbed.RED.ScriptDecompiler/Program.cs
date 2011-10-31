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
            }
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
            var statements = DecompileStatements(func);
            foreach(var statement in statements)
            {
                output.WriteLine(indent + "    " + statement + ";");
            }
 
            output.WriteLine(indent + "}\n");
        }

        private static List<Expression> DecompileStatements(FunctionDefinition func)
        {
            var result = new List<Expression>();
            int i = 0;
            while(i < func.Instructions.Count)
            {
                var statement = ReadNextExpression(func, ref i);
                if (statement == null)
                    break;
                result.Add(statement);
                if (!statement.Complete)
                {
                    break;
                }
            }
            return result;
        }

        private static Expression ReadNextExpression(FunctionDefinition func, ref int instructionIndex)
        {
            int target = -1;
            
            var currentInstruction = func.Instructions[instructionIndex];
            if (currentInstruction is Target)
            {
                target = ((Target)currentInstruction).Op0;
                instructionIndex++;
                currentInstruction = func.Instructions[instructionIndex];
            }
            if (currentInstruction.Opcode == Opcode.OP_Nop)
            {
                instructionIndex++;
                if (instructionIndex == func.Instructions.Count) return null;
                currentInstruction = func.Instructions[instructionIndex];
            }

            instructionIndex++;
            
            switch (currentInstruction.Opcode)
            {
                case Opcode.OP_Assign:
                    return ReadBinaryExpession(func, ref instructionIndex, target, " = ");
                case Opcode.OP_Context:
                    {
                        var context = (U16U16) currentInstruction;
                        if (context.Op0 == 0)
                        {
                            return ReadBinaryExpession(func, ref instructionIndex, target, ".");
                        }
                    }
                    break;
                case Opcode.OP_Return:
                     return ReadUnaryExpression(func, ref instructionIndex, target, "return ", "");
                case Opcode.OP_VirtualFunc:
                    {
                        var virtualFunc = (VirtualFunc) currentInstruction;
                        var args = new List<Expression>();
                        bool incomplete = false;
                        while(func.Instructions[instructionIndex].Opcode != Opcode.OP_ParamEnd)
                        {
                            var nextExpression = ReadNextExpression(func, ref instructionIndex);
                            args.Add(nextExpression);
                            if (!nextExpression.Complete)
                            {
                                incomplete = true;
                                break;
                            }
                        }
                        if (!incomplete)
                            instructionIndex++;   // skip paramEnd
                        return new CallStatement(virtualFunc.FunctionName, args);
                    }
                case Opcode.OP_ObjectVar:
                case Opcode.OP_ParamVar:
                case Opcode.OP_LocalVar:
                    return new SimpleExpression(((TypeMember) currentInstruction).MemberName);
                case Opcode.OP_GetPlayer:
                    return new SimpleExpression("Player");
                case Opcode.OP_GetHud:
                    return new SimpleExpression("Hud");
                case Opcode.OP_GetGame:
                    return new SimpleExpression("Game");
                case Opcode.OP_BoolFalse:
                    return new SimpleExpression("false");
                case Opcode.OP_BoolTrue:
                    return new SimpleExpression("true");
                case Opcode.OP_IntZero:
                    return new SimpleExpression("0");
                case Opcode.OP_IntOne:
                    return new SimpleExpression("1");
                case Opcode.OP_FloatConst:
                    return new SimpleExpression(((FloatConst) currentInstruction).Value.ToString());
                case Opcode.OP_StringConst:
                    return new SimpleExpression("\"" + ((StringConst) currentInstruction).Value + "\"");
                case Opcode.OP_ArraySize:
                    return ReadUnaryExpression(func, ref instructionIndex, target, "", ".Size");
            }
            
            
            return new UnknownExpression(currentInstruction);
        }

        private static Expression ReadUnaryExpression(FunctionDefinition func, ref int instructionIndex, int target, string prefix, string suffix)
        {
            var operand = ReadNextExpression(func, ref instructionIndex);
            return new UnaryExpression(target, operand, prefix, suffix);
        }

        private static Expression ReadBinaryExpession(FunctionDefinition func, ref int instructionIndex, int target, string infix)
        {
            var lhs = ReadNextExpression(func, ref instructionIndex);
            var rhs = lhs.Complete ? ReadNextExpression(func, ref instructionIndex) : null;
            return new BinaryExpression(target, infix, lhs, rhs);
        }
    }
}
