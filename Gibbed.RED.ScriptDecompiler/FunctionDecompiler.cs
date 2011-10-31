using System.Collections.Generic;
using Gibbed.RED.FileFormats.Script;
using Gibbed.RED.FileFormats.Script.Instructions;

namespace Gibbed.RED.ScriptDecompiler
{
    public class FunctionDecompiler
    {
        private readonly FunctionDefinition _func;
        private int _currentIndex;

        public FunctionDecompiler(FunctionDefinition func)
        {
            _func = func;
            _currentIndex = 0;
        }

        public List<Expression> DecompileStatements()
        {
            var result = new List<Expression>();
            _currentIndex = 0;
            while (_currentIndex < _func.Instructions.Count)
            {
                var statement = ReadNextExpression();
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

        private Expression ReadNextExpression()
        {
            int target = -1;

            var currentInstruction = _func.Instructions[_currentIndex];
            if (currentInstruction is Target)
            {
                target = ((Target)currentInstruction).Op0;
                _currentIndex++;
                currentInstruction = _func.Instructions[_currentIndex];
            }
            if (currentInstruction.Opcode == Opcode.OP_Nop)
            {
                _currentIndex++;
                if (_currentIndex == _func.Instructions.Count) return null;
                currentInstruction = _func.Instructions[_currentIndex];
            }

            _currentIndex++;

            switch (currentInstruction.Opcode)
            {
                case Opcode.OP_Assign:
                    return ReadBinaryExpession(target, " = ");
                case Opcode.OP_Context:
                    {
                        var context = (U16U16)currentInstruction;
                        if (context.Op0 == 0)
                        {
                            return ReadBinaryExpession(target, ".");
                        }
                    }
                    break;
                case Opcode.OP_Return:
                    return ReadUnaryExpression(target, "return ", "");
                case Opcode.OP_VirtualFunc:
                    {
                        var virtualFunc = (VirtualFunc)currentInstruction;
                        var args = new List<Expression>();
                        bool incomplete = false;
                        while (_func.Instructions[_currentIndex].Opcode != Opcode.OP_ParamEnd)
                        {
                            var nextExpression = ReadNextExpression();
                            args.Add(nextExpression);
                            if (!nextExpression.Complete)
                            {
                                incomplete = true;
                                break;
                            }
                        }
                        if (!incomplete)
                            _currentIndex++;   // skip paramEnd
                        return new CallStatement(virtualFunc.FunctionName, args);
                    }
                case Opcode.OP_ObjectVar:
                case Opcode.OP_ParamVar:
                case Opcode.OP_LocalVar:
                    return new SimpleExpression(((TypeMember)currentInstruction).MemberName);
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
                    return new SimpleExpression(((FloatConst)currentInstruction).Value.ToString());
                case Opcode.OP_StringConst:
                    return new SimpleExpression("\"" + ((StringConst)currentInstruction).Value + "\"");
                case Opcode.OP_ArraySize:
                    return ReadUnaryExpression(target, "", ".Size");
            }


            return new UnknownExpression(currentInstruction);
        }

        private Expression ReadUnaryExpression(int target, string prefix, string suffix)
        {
            var operand = ReadNextExpression();
            return new UnaryExpression(target, operand, prefix, suffix);
        }

        private Expression ReadBinaryExpession(int target, string infix)
        {
            var lhs = ReadNextExpression();
            var rhs = lhs.Complete ? ReadNextExpression() : null;
            return new BinaryExpression(target, infix, lhs, rhs);
        }
    }
}