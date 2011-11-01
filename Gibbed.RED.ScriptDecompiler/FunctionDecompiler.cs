﻿using System.Collections.Generic;
using Gibbed.RED.FileFormats.Script;
using Gibbed.RED.FileFormats.Script.Instructions;

namespace Gibbed.RED.ScriptDecompiler
{
    public class FunctionDecompiler
    {
        private readonly FunctionDefinition _func;
        private int _currentIndex;
        private bool _incomplete = false;

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
                if (_incomplete)
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
                    return ReadBinaryExpression(target, " = ", "");
                case Opcode.OP_Context:
                    {
                        var context = (U16U16)currentInstruction;
                        if (context.Op0 == 0)
                        {
                            return ReadBinaryExpression(target, ".", "");
                        }
                    }
                    break;
                case Opcode.OP_Return:
                    return ReadUnaryExpression(target, "return ", "");
                case Opcode.OP_VirtualFunc:
                    {
                        var virtualFunc = (VirtualFunc)currentInstruction;
                        var args = ReadArgumentList();
                        return new CallStatement(virtualFunc.FunctionName, args);
                    }
                case Opcode.OP_FinalFunc:
                    {
                        var finalFunc = (FinalFunc) currentInstruction;
                        if (finalFunc.OpFuncId != -1)
                        {
                            var args = ReadArgumentList();
                            return new CallStatement(finalFunc.FunctionName, args);
                        }
                        else
                        {
                            string opName = GetOperator(finalFunc.OpOperator);
                            if (opName != null)
                            {
                                var expression = IsUnary(finalFunc.OpOperator)
                                    ? ReadUnaryExpression(target, opName, "") 
                                    : ReadBinaryExpression(target, " " + opName + " ", "");
                                while (NextInstruction.Opcode != Opcode.OP_ParamEnd)
                                {
                                    var next = ReadNextExpression();
                                    if (_incomplete) break;
                                    expression = new BinaryExpression(target, opName, "", expression, next);
                                }
                                if (!_incomplete) _currentIndex++;   // skip param end
                                return expression;
                            }
                        }
                    }
                    break;
                case Opcode.OP_Constructor:
                    {
                        var constructor = (Constructor) currentInstruction;
                        var args = new List<Expression>();
                        for(int i=0; i<constructor.OpArgCount; i++)
                        {
                            args.Add(ReadNextExpression());
                            if (_incomplete) break;
                        }
                        return new CallStatement(constructor.TypeName, args);

                    }
                case Opcode.OP_ObjectVar:
                case Opcode.OP_ParamVar:
                case Opcode.OP_LocalVar:
                    return new SimpleExpression(((TypeMember)currentInstruction).MemberName);
                case Opcode.OP_StructMember:
                    return ReadUnaryExpression(target, "", "." + (((TypeMember) currentInstruction).MemberName));
                
                case Opcode.OP_GetPlayer:
                    return new SimpleExpression("Player");
                case Opcode.OP_GetHud:
                    return new SimpleExpression("Hud");
                case Opcode.OP_GetGame:
                    return new SimpleExpression("Game");
                case Opcode.OP_GetSound:
                    return new SimpleExpression("Sound");
                case Opcode.OP_GetCamera:
                    return new SimpleExpression("Camera");

                case Opcode.OP_BoolFalse:
                    return new SimpleExpression("false");
                case Opcode.OP_BoolTrue:
                    return new SimpleExpression("true");
                case Opcode.OP_IntZero:
                    return new SimpleExpression("0");
                case Opcode.OP_IntOne:
                    return new SimpleExpression("1");
                case Opcode.OP_Null:
                    return new SimpleExpression("null");
                case Opcode.OP_IntConst:
                    return new SimpleExpression(((IntConst)currentInstruction).Value.ToString());
                case Opcode.OP_FloatConst:
                    return new SimpleExpression(((FloatConst)currentInstruction).Value.ToString());
                case Opcode.OP_StringConst:
                    return new SimpleExpression("\"" + ((StringConst)currentInstruction).Value + "\"");
                case Opcode.OP_NameConst:
                    return new SimpleExpression("'" + ((NameConst)currentInstruction).Value + "'");
                
                case Opcode.OP_ArrayClear:
                    return ReadUnaryExpression(target, "", ".Clear()");
                case Opcode.OP_ArraySize:
                    return ReadUnaryExpression(target, "", ".Size");
                case Opcode.OP_ArrayPushBack:
                    return ReadBinaryExpression(target, ".PushBack(", ")");
                case Opcode.OP_ArrayRemoveFast:
                    return ReadBinaryExpression(target, ".RemoveFast(", ")");
                case Opcode.OP_ArrayContainsFast:
                    return ReadBinaryExpression(target, ".ContainsFast(", ")");
                case Opcode.OP_ArrayElement:
                    return ReadBinaryExpression(target, "[", "]");

                case Opcode.OP_DynamicCast:
                    return ReadUnaryExpression(target, "dynamic_cast<" + ((TypeRef) currentInstruction).TypeName + ">(", ")");

                case Opcode.OP_NameToString:
                    return ReadUnaryExpression(target, "(string) ", "");
                case Opcode.OP_ByteToInt:
                    return ReadUnaryExpression(target, "(int) ", "");
                case Opcode.OP_IntToByte:
                    return ReadUnaryExpression(target, "(byte) ", "");
                case Opcode.OP_IntToFloat:
                    return ReadUnaryExpression(target, "(float) ", "");
                case Opcode.OP_EnumToInt:
                    if (NextInstruction is ShortConst)
                    {
                        var typeDef = ((TypeRef) currentInstruction).TypeDef;
                        var value = ((ShortConst) NextInstruction).Value;
                        if (typeDef is EnumDefinition)
                        {
                            _currentIndex++;
                            return new SimpleExpression(((EnumDefinition) typeDef).FindByValue(value));
                        }
                    }
                    else
                    {
                        return ReadNextExpression();
                    }
                    break;

            }

            _incomplete = true;
            return new UnknownExpression(currentInstruction);
        }

        private List<Expression> ReadArgumentList()
        {
            var args = new List<Expression>();
            while (NextInstruction.Opcode != Opcode.OP_ParamEnd)
            {
                if (NextInstruction.Opcode == Opcode.OP_Nop)
                {
                    args.Add(new SimpleExpression("null"));
                    _currentIndex++;
                    continue;
                }
                var nextExpression = ReadNextExpression();
                args.Add(nextExpression);
                if (_incomplete)
                {
                    break;
                }
            }
            if (!_incomplete)
                _currentIndex++; // skip paramEnd
            return args;
        }

        private IInstruction NextInstruction
        {
            get { return _func.Instructions[_currentIndex]; }
        }

        private Expression ReadUnaryExpression(int target, string prefix, string suffix)
        {
            var operand = ReadNextExpression();
            return new UnaryExpression(target, operand, prefix, suffix);
        }

        private Expression ReadBinaryExpression(int target, string infix, string suffix)
        {
            var lhs = ReadNextExpression();
            var rhs = _incomplete ? null : ReadNextExpression();
            return new BinaryExpression(target, infix, suffix, lhs, rhs);
        }

        private string GetOperator(OperatorCode code)
        {
            switch (code)
            {
                case OperatorCode.IntAdd:
                case OperatorCode.FloatAdd:
                case OperatorCode.StringAdd:
                    return "+";

                case OperatorCode.IntSubtract:
                case OperatorCode.IntNeg:
                case OperatorCode.FloatSubtract:
                    return "-";

                case OperatorCode.IntMultiply:
                case OperatorCode.FloatMultiply:
                    return "*";

                case OperatorCode.IntDivide:
                case OperatorCode.FloatDivide:
                    return "/";

                case OperatorCode.IntEqual:
                case OperatorCode.FloatEqual:
                    return "==";

                case OperatorCode.IntNotEqual:
                case OperatorCode.FloatNotEqual:
                    return "!=";

                case OperatorCode.IntAssignAdd:
                case OperatorCode.FloatAssignAdd:
                    return "+=";

                case OperatorCode.IntAssignSubtract:
                case OperatorCode.FloatAssignSubtract:
                    return "-=";

                case OperatorCode.BoolAnd:
                    return "&&";

                case OperatorCode.BoolOr:
                    return "||";

                case OperatorCode.BoolNot:
                    return "!";

                default:
                    return null;
            }
        }

        private bool IsUnary(OperatorCode code)
        {
            return code == OperatorCode.BoolNot || code == OperatorCode.IntNeg;
        }

    }
}