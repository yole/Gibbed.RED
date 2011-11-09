using System.Collections.Generic;
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
                var statement = ReadNextExpression(false);
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

        private Expression ReadNextExpression(bool allowNop)
        {
            int target = -1;

            var currentInstruction = _func.Instructions[_currentIndex];
            if (currentInstruction is Target)
            {
                target = ((Target)currentInstruction).Op0;
                _currentIndex++;
                currentInstruction = _func.Instructions[_currentIndex];
            }
            if (allowNop && currentInstruction.Opcode == Opcode.OP_Nop)
            {
                _currentIndex++;
                return new SimpleExpression("null");
            }
            if (currentInstruction.Opcode == Opcode.OP_Nop || currentInstruction.Opcode == Opcode.OP_Skip)
            {
                _currentIndex++;
                if (_currentIndex == _func.Instructions.Count) return null;
                currentInstruction = _func.Instructions[_currentIndex];
            }

            var currentInstructionStart = _func.InstructionOffsets[_currentIndex];
            _currentIndex++;

            switch (currentInstruction.Opcode)
            {
                case Opcode.OP_Assign:
                    return ReadBinaryExpression(" = ", "");
                case Opcode.OP_Context:
                    return ReadBinaryExpression(".", "");
                case Opcode.OP_Return:
                    return ReadUnaryExpression("return ", "");
                case Opcode.OP_VirtualFunc:
                    {
                        var virtualFunc = (VirtualFunc)currentInstruction;
                        var args = ReadArgumentList();
                        return new CallStatement(virtualFunc.FunctionName, args);
                    }
                case Opcode.OP_EntryFunc:
                    {
                        var virtualFunc = (U16S32)currentInstruction;
                        var args = ReadArgumentList();
                        return new CallStatement("Enter " + virtualFunc.Operand, args);
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
                                    ? ReadUnaryExpression(opName, "") 
                                    : ReadBinaryExpression(" " + opName + " ", "");
                                while (NextInstruction.Opcode != Opcode.OP_ParamEnd)
                                {
                                    var next = ReadNextExpression(false);
                                    if (_incomplete) break;
                                    expression = new BinaryExpression(opName, "", expression, next);
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
                            args.Add(ReadNextExpression(false));
                            if (_incomplete) break;
                        }
                        return new CallStatement(constructor.TypeName, args);
                    }
                case Opcode.OP_New:
                    {
                        var newRef = (TypeRef) currentInstruction;
                        return ReadUnaryExpression("new " + newRef.TypeName + "(", ")");
                    }

                case Opcode.OP_TestEqual:
                    return ReadBinaryExpression(" == ", "");
                case Opcode.OP_TestNotEqual:
                    return ReadBinaryExpression(" != ", "");
                    
                case Opcode.OP_ObjectVar:
                case Opcode.OP_ParamVar:
                case Opcode.OP_LocalVar:
                    return new SimpleExpression(((TypeMember)currentInstruction).MemberName);
                case Opcode.OP_StructMember:
                    return ReadUnaryExpression("", "." + (((TypeMember) currentInstruction).MemberName));
                
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
                case Opcode.OP_GetServer:
                    return new SimpleExpression("Server");

                case Opcode.OP_This:
                    return new SimpleExpression("this");
                case Opcode.OP_Parent:
                    return ReadUnaryExpression("Parent(", ")");

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
                case Opcode.OP_ShortConst:
                    return new SimpleExpression(((ShortConst)currentInstruction).Value.ToString());
                case Opcode.OP_IntConst:
                    return new SimpleExpression(((IntConst)currentInstruction).Value.ToString());
                case Opcode.OP_FloatConst:
                    return new SimpleExpression(((FloatConst)currentInstruction).Value.ToString());
                case Opcode.OP_StringConst:
                    return new SimpleExpression("\"" + ((StringConst)currentInstruction).Value + "\"");
                case Opcode.OP_NameConst:
                    return new SimpleExpression("'" + ((NameConst)currentInstruction).Value + "'");
                
                case Opcode.OP_ArrayClear:
                    return ReadUnaryExpression("", ".Clear()");
                case Opcode.OP_ArrayLast:
                    return ReadUnaryExpression("", ".Last()");
                case Opcode.OP_ArraySize:
                    return ReadUnaryExpression("", ".Size");
                case Opcode.OP_ArrayGrow:
                    return ReadBinaryExpression(".Grow(", ")");
                case Opcode.OP_ArrayResize:
                    return ReadBinaryExpression(".Resize(", ")");
                case Opcode.OP_ArrayPushBack:
                    return ReadBinaryExpression(".PushBack(", ")");
                case Opcode.OP_ArrayRemove:
                    return ReadBinaryExpression(".Remove(", ")");
                case Opcode.OP_ArrayRemoveFast:
                    return ReadBinaryExpression(".RemoveFast(", ")");
                case Opcode.OP_ArrayFindFirstFast:
                    return ReadBinaryExpression(".FindFirstFast(", ")");
                case Opcode.OP_ArrayContains:
                    return ReadBinaryExpression(".Contains(", ")");
                case Opcode.OP_ArrayContainsFast:
                    return ReadBinaryExpression(".ContainsFast(", ")");
                case Opcode.OP_ArrayErase:
                    return ReadBinaryExpression(".Erase(", ")");
                case Opcode.OP_ArrayInsert:
                    return ReadTernaryExpression("Insert");
                case Opcode.OP_ArrayElement:
                    return ReadBinaryExpression("[", "]");

                case Opcode.OP_DynamicCast:
                    return ReadUnaryExpression("dynamic_cast<" + ((TypeRef) currentInstruction).TypeName + ">(", ")");

                case Opcode.OP_NameToString:
                case Opcode.OP_IntToString:
                case Opcode.OP_FloatToString:
                case Opcode.OP_BoolToString:
                case Opcode.OP_EnumToString:
                    return ReadUnaryExpression("(string) ", "");
                
                case Opcode.OP_ByteToInt:
                case Opcode.OP_FloatToInt:
                case Opcode.OP_StringToInt:
                    return ReadUnaryExpression("(int) ", "");
                
                case Opcode.OP_IntToByte:
                    return ReadUnaryExpression("(byte) ", "");
                case Opcode.OP_IntToFloat:
                case Opcode.OP_ByteToFloat:
                    return ReadUnaryExpression("(float) ", "");
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
                        return ReadNextExpression(false);
                    }
                    break;

                case Opcode.OP_ObjectToBool:
                case Opcode.OP_IntToBool:
                case Opcode.OP_NameToBool:
                case Opcode.OP_StringToBool:
                case Opcode.OP_FloatToBool:
                    return ReadNextExpression(false);

                case Opcode.OP_ObjectToString:
                    return ReadUnaryExpression("", ".ToString()");

                case Opcode.OP_JumpIfFalse:
                    {
                        var offset = currentInstructionStart + (short)((U16)currentInstruction).Op0;
                        var condition = ReadNextExpression(false);
                        return new CondJumpStatement(condition, offset);
                    }

                case Opcode.OP_Jump:
                    {
                        var offset = currentInstructionStart + (short)((U16)currentInstruction).Op0;
                        return new JumpStatement(offset);
                    }
            }

            _incomplete = true;
            return new UnknownExpression(currentInstruction);
        }

        private List<Expression> ReadArgumentList()
        {
            var args = new List<Expression>();
            while (NextInstruction.Opcode != Opcode.OP_ParamEnd)
            {
                var nextExpression = ReadNextExpression(true);
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

        private Expression ReadUnaryExpression(string prefix, string suffix)
        {
            var operand = ReadNextExpression(true);
            return new UnaryExpression(operand, prefix, suffix);
        }

        private Expression ReadBinaryExpression(string infix, string suffix)
        {
            var lhs = ReadNextExpression(false);
            var rhs = _incomplete ? null : ReadNextExpression(false);
            return new BinaryExpression(infix, suffix, lhs, rhs);
        }

        private Expression ReadTernaryExpression(string functionName)
        {
            var lhs = ReadNextExpression(false);
            var op1 = _incomplete ? null : ReadNextExpression(false);
            var op2 = _incomplete ? null : ReadNextExpression(false);
            var args = new List<Expression> {op1, op2};
            return new CallStatement(lhs + "." + functionName, args);
        }

        private static string GetOperator(OperatorCode code)
        {
            switch (code)
            {
                case OperatorCode.IntAdd:
                case OperatorCode.FloatAdd:
                case OperatorCode.StringAdd:
                case OperatorCode.TimeAddTime:
                case OperatorCode.TimeAddFloat:
                    return "+";

                case OperatorCode.IntSubtract:
                case OperatorCode.IntNeg:
                case OperatorCode.FloatSubtract:
                case OperatorCode.FloatNeg:
                    return "-";

                case OperatorCode.IntMultiply:
                case OperatorCode.FloatMultiply:
                    return "*";

                case OperatorCode.IntDivide:
                case OperatorCode.FloatDivide:
                    return "/";

                case OperatorCode.IntEqual:
                case OperatorCode.FloatEqual:
                case OperatorCode.UniqueIdEqual:
                    return "==";

                case OperatorCode.IntNotEqual:
                case OperatorCode.FloatNotEqual:
                case OperatorCode.UniqueIdNotEqual:
                    return "!=";

                case OperatorCode.IntLess:
                case OperatorCode.FloatLess:
                    return "<";

                case OperatorCode.IntLessEqual:
                case OperatorCode.FloatLessEqual:
                    return "<=";

                case OperatorCode.IntGreater:
                case OperatorCode.FloatGreater:
                    return ">";

                case OperatorCode.IntGreaterEqual:
                case OperatorCode.FloatGreaterEqual:
                    return ">=";

                case OperatorCode.IntAssignAdd:
                case OperatorCode.FloatAssignAdd:
                    return "+=";

                case OperatorCode.IntAssignSubtract:
                case OperatorCode.FloatAssignSubtract:
                    return "-=";

                case OperatorCode.FloatAssignMultiply:
                    return "*=";

                case OperatorCode.FloatAssignDivide:
                    return "/=";

                case OperatorCode.BoolAnd:
                    return "&&";

                case OperatorCode.BoolOr:
                    return "||";

                case OperatorCode.BoolNot:
                    return "!";

                case OperatorCode.IntAnd:
                    return "&";

                case OperatorCode.IntOr:
                    return "|";

                case OperatorCode.IntAssignOr:
                    return "|=";

                case OperatorCode.FloatOp25:
                    return "<op25>";

                case OperatorCode.ByteOp41:
                    return "<op41>";

                case OperatorCode.ByteOp48:
                    return "<op48>";

                case OperatorCode.VectorOp60:
                    return "<op60>";

                case OperatorCode.VectorOp61:
                    return "<op61>";

                case OperatorCode.VectorOp62:
                    return "<op62>";

                case OperatorCode.VectorOp65:
                    return "<op65>";

                case OperatorCode.VectorOp66:
                    return "<op66>";

                case OperatorCode.VectorOp71:
                    return "<op71>";

                case OperatorCode.VectorOp72:
                    return "<op72>";

                case OperatorCode.VectorOp75:
                    return "<op75>";

                case OperatorCode.VectorOp76:
                    return "<op76>";

                case OperatorCode.VectorOp77:
                    return "<op77>";

                case OperatorCode.TimeOp84:
                    return "<op84>";

                case OperatorCode.TimeOp85:
                    return "<op85>";
                
                case OperatorCode.TimeOp92:
                    return "<op92>";

                case OperatorCode.TimeOp93:
                    return "<op93>";

                case OperatorCode.TimeOp94:
                    return "<op94>";

                case OperatorCode.TimeOp95:
                    return "<op95>";

                case OperatorCode.TimeOp96:
                    return "<op96>";

                case OperatorCode.TimeOp97:
                    return "<op97>";

                case OperatorCode.TimeOp98:
                    return "<op98>";

                case OperatorCode.TimeOp100:
                    return "<op100>";

                case OperatorCode.TimeOp104:
                    return "<op104>";

                case OperatorCode.Op120:
                    return "<op120>";
                
                default:
                    return null;
            }
        }

        private static bool IsUnary(OperatorCode code)
        {
            return code == OperatorCode.BoolNot ||
                   code == OperatorCode.IntNeg ||
                   code == OperatorCode.FloatNeg ||
                   code == OperatorCode.VectorOp60;
        }

    }
}