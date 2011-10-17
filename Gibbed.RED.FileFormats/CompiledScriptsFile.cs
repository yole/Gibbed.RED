﻿/* Copyright (c) 2011 Rick (rick 'at' gibbed 'dot' us)
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
using Gibbed.Helpers;

namespace Gibbed.RED.FileFormats
{
    public class CompiledScriptsFile
    {
        public string Unknown0;
        public DateTime TimeStamp;
        public string Unknown2;

        public Script.TypeDefinition[] TypeDefs;
        public Script.FunctionDefinition[] FuncDefs;

        public void Deserialize(Stream input)
        {
            input.Seek(-4, SeekOrigin.End);
            
            // read strings
            var stringTableOffset = input.ReadValueU32();
            input.Seek(stringTableOffset, SeekOrigin.Begin);
            var stringCount = input.ReadValueU32();
            var strings = new Script.RawString[stringCount];
            for (int i = 0; i < strings.Length; i++)
            {
                var stringEntry = new Script.RawString();
                stringEntry.Value = input.ReadEncodedString();
                stringEntry.IsName = input.ReadValueB8();
                strings[i] = stringEntry;
            }

            input.Seek(0, SeekOrigin.Begin);
            
            // now the script data
            this.Unknown0 = input.ReadEncodedString();
            this.TimeStamp = DateTime.FromFileTime(input.ReadValueS64());
            this.Unknown2 = input.ReadEncodedString();

            var typeDefCount = input.ReadValueU32();
            var funcDefCount = input.ReadValueU32();

            var rawTypeDefs = new Script.RawTypeDefinition[typeDefCount];
            TypeDefs = new Script.TypeDefinition[typeDefCount];

            for (uint i = 0; i < typeDefCount; i++)
            {
                var rawTypeDef = new Script.RawTypeDefinition();
                rawTypeDef.Name = strings[input.ReadValueEncodedS32()].Value;
                rawTypeDef.Type = (Script.NativeType)input.ReadValueEncodedS32();
                rawTypeDef.NativePropertyCount = input.ReadValueEncodedS32();
                rawTypeDef.ScriptedPropertyCount = input.ReadValueEncodedS32();
                rawTypeDef.Flags = (Script.TypeDefinitionFlags)input.ReadValueEncodedS32();

                Script.TypeDefinition typeDef = null;

                switch (rawTypeDef.Type)
                {
                    case Script.NativeType.Simple:
                    {
                        if (rawTypeDef.NativePropertyCount > 0 ||
                            rawTypeDef.ScriptedPropertyCount > 0)
                        {
                            throw new FormatException();
                        }

                        typeDef = new Script.SimpleDefinition()
                        {
                            Name = rawTypeDef.Name,
                            Flags = rawTypeDef.Flags,
                        };

                        break;
                    }

                    case Script.NativeType.Enum:
                    {
                        if (rawTypeDef.NativePropertyCount > 0 ||
                            rawTypeDef.ScriptedPropertyCount > 0)
                        {
                            throw new FormatException();
                        }

                        typeDef = new Script.EnumDefinition()
                        {
                            Name = rawTypeDef.Name,
                            Flags = rawTypeDef.Flags,
                        };

                        break;
                    }

                    case Script.NativeType.Class:
                    {
                        typeDef = new Script.ClassDefinition()
                        {
                            Name = rawTypeDef.Name,
                            Flags = rawTypeDef.Flags,
                        };

                        break;
                    }

                    case Script.NativeType.Array:
                    {
                        if (rawTypeDef.NativePropertyCount > 0 ||
                            rawTypeDef.ScriptedPropertyCount > 0)
                        {
                            throw new FormatException();
                        }

                        typeDef = new Script.ArrayDefinition()
                        {
                            Name = rawTypeDef.Name,
                            Flags = rawTypeDef.Flags,
                        };

                        break;
                    }

                    case Script.NativeType.Pointer:
                    {
                        if (rawTypeDef.NativePropertyCount > 0 ||
                            rawTypeDef.ScriptedPropertyCount > 0)
                        {
                            throw new FormatException();
                        }

                        typeDef = new Script.PointerDefinition()
                        {
                            Name = rawTypeDef.Name,
                            Flags = rawTypeDef.Flags,
                        };

                        break;
                    }

                    case Script.NativeType.Handle:
                    {
                        if (rawTypeDef.NativePropertyCount > 0 ||
                            rawTypeDef.ScriptedPropertyCount > 0)
                        {
                            throw new FormatException();
                        }

                        typeDef = new Script.HandleDefinition()
                        {
                            Name = rawTypeDef.Name,
                            Flags = rawTypeDef.Flags,
                        };

                        break;
                    }

                    case Script.NativeType.SoftHandle:
                    {
                        if (rawTypeDef.NativePropertyCount > 0 ||
                            rawTypeDef.ScriptedPropertyCount > 0)
                        {
                            throw new FormatException();
                        }

                        typeDef = new Script.SoftHandleDefinition()
                        {
                            Name = rawTypeDef.Name,
                            Flags = rawTypeDef.Flags,
                        };

                        break;
                    }

                    case Script.NativeType.BitField:
                    {
                        if (rawTypeDef.NativePropertyCount > 0 ||
                            rawTypeDef.ScriptedPropertyCount > 0)
                        {
                            throw new FormatException();
                        }

                        typeDef = new Script.BitFieldDefinition()
                        {
                            Name = rawTypeDef.Name,
                            Flags = rawTypeDef.Flags,
                        };

                        break;
                    }

                    default:
                    {
                        throw new FormatException();
                    }
                }

                rawTypeDefs[i] = rawTypeDef;

                if (typeDef == null)
                {
                    throw new FormatException();
                }

                TypeDefs[i] = typeDef;
            }

            var rawFuncDefs = new Script.RawFunctionDefinition[funcDefCount];
            FuncDefs = new Script.FunctionDefinition[funcDefCount];

            for (uint i = 0; i < funcDefCount; i++)
            {
                var rawFuncDef = new Script.RawFunctionDefinition();
                rawFuncDef.Name = strings[input.ReadValueEncodedS32()].Value;
                rawFuncDef.DefinedOnId = input.ReadValueEncodedS32();
                rawFuncDef.Flags = input.ReadValueEncodedS32();

                var funcDef = new Script.FunctionDefinition();
                funcDef.Name = rawFuncDef.Name;

                if (rawFuncDef.DefinedOnId != 0)
                {
                    var typeDef = TypeDefs[rawFuncDef.DefinedOnId] as Script.ClassDefinition;
                    if (typeDef == null)
                        throw new FormatException("expected ClassDefinition, found " + TypeDefs[rawFuncDef.DefinedOnId]);
                    typeDef.Functions[funcDef.Name] = funcDef;
                }

                rawFuncDefs[i] = rawFuncDef;
                FuncDefs[i] = funcDef;
            }

            // parse enums
            for (int i = 0; i < rawTypeDefs.Length; i++)
            {
                var rawTypeDef = rawTypeDefs[i];

                if (rawTypeDef.Type != Script.NativeType.Enum)
                {
                    continue;
                }
                else if ((rawTypeDef.Flags &
                    Script.TypeDefinitionFlags.Scripted) == 0)
                {
                    continue;
                }

                var type = (Script.NativeType)input.ReadValueEncodedS32();
                if (rawTypeDef.Type != type)
                {
                    throw new FormatException();
                }

                var id = input.ReadValueEncodedS32();
                if (id != i)
                {
                    throw new FormatException();
                }

                var typeDef = (Script.EnumDefinition)TypeDefs[i];
                typeDef.Unknown0 = input.ReadValueEncodedS32();
                
                var constantCount = input.ReadValueEncodedS32();
                typeDef.Constants.Clear();
                for (int j = 0; j < constantCount; j++)
                {
                    var constantName = strings[input.ReadValueEncodedS32()].Value;
                    var constantValue = input.ReadValueEncodedS32();
                    typeDef.Constants.Add(constantName, constantValue);
                }
            }

            // parse classes
            for (int i = 0; i < rawTypeDefs.Length; i++)
            {
                var rawTypeDef = rawTypeDefs[i];

                if (rawTypeDef.Type != Script.NativeType.Class)
                {
                    continue;
                }
                else if ((rawTypeDef.Flags &
                    Script.TypeDefinitionFlags.Scripted) == 0)
                {
                    continue;
                }

                var type = (Script.NativeType)input.ReadValueEncodedS32();
                if (rawTypeDef.Type != type)
                {
                    throw new FormatException();
                }

                var id = input.ReadValueEncodedS32();
                if (id != i)
                {
                    throw new FormatException();
                }

                var typeDef = (Script.ClassDefinition)TypeDefs[i];

                var isExtending = input.ReadValueEncodedS32();
                if (isExtending != 0)
                {
                    var superTypeId = input.ReadValueEncodedS32();
                    var superDef = (Script.ClassDefinition)TypeDefs[superTypeId];
                    typeDef.Super = superDef;
                }

                var stateCount = input.ReadValueEncodedS32();
                typeDef.States.Clear();
                for (int j = 0; j < stateCount; j++)
                {
                    var stateName = strings[input.ReadValueEncodedS32()].Value;
                    var stateTypeId = input.ReadValueEncodedS32();
                    var stateDef = (Script.ClassDefinition)TypeDefs[stateTypeId];
                    typeDef.States.Add(stateName, stateDef);
                }

                typeDef.NativeProperties.Clear();
                for (int j = 0; j < rawTypeDef.NativePropertyCount; j++)
                {
                    var nativePropertyName = strings[input.ReadValueEncodedS32()].Value;
                    typeDef.NativeProperties.Add(nativePropertyName);
                }

                typeDef.Properties.Clear();
                for (int j = 0; j < rawTypeDef.ScriptedPropertyCount; j++)
                {
                    var propTypeId = input.ReadValueEncodedS32();
                    var propName = strings[input.ReadValueEncodedS32()].Value;
                    var propFlags = input.ReadValueEncodedS32();

                    var property = new Script.PropertyDefinition()
                    {
                        Flags = propFlags,
                        TypeDefinition = TypeDefs[propTypeId],
                    };

                    typeDef.Properties.Add(propName, property);

                    // 1 = editable
                    // 2 = const
                    // 32 = ?
                    // 32768 = saved
                }
            }

            // parse class defaults
            for (int i = 0; i < rawTypeDefs.Length; i++)
            {
                var rawTypeDef = rawTypeDefs[i];

                if (rawTypeDef.Type != Script.NativeType.Class)
                {
                    continue;
                }
                else if ((rawTypeDef.Flags &
                    Script.TypeDefinitionFlags.Scripted) == 0)
                {
                    continue;
                }

                var id = input.ReadValueEncodedS32();
                if (id != i)
                {
                    throw new FormatException();
                }

                var typeDef = (Script.ClassDefinition)TypeDefs[i];

                var defaultCount = input.ReadValueEncodedS32();
                for (int j = 0; j < defaultCount; j++)
                {
                    var propName = strings[input.ReadValueEncodedS32()].Value;

                    var dataType = input.ReadValueEncodedS32();
                    if (dataType == 0 || dataType == 1)
                    {
                        var typeName = input.ReadEncodedString();
                        var typeDataSize = input.ReadValueU32(); // size + 4
                        var typeData = new byte[typeDataSize - 4];
                        input.Read(typeData, 0, typeData.Length);

                        typeDef.Defaults.Add(propName,
                            new Script.PropertyDefault()
                            {
                                TypeName = typeName,
                                Data = typeData,
                            });
                    }
                    else
                    {
                        throw new FormatException();
                    }
                }
            }

            // parse functions (awww yeah)
            for (int i = 0; i < rawFuncDefs.Length; i++)
            {
                var rawFuncDef = rawFuncDefs[i];

                if ((rawFuncDef.Flags & 1) == 0)
                {
                    continue;
                }

                var id = input.ReadValueEncodedS32();
                if (id != i)
                {
                    throw new FormatException();
                }

                var rawFlags = input.ReadValueEncodedS32();
                var flags = (Script.FunctionFlags)rawFlags;

                var hasReturnValue = input.ReadValueB8();
                if (hasReturnValue == true)
                {
                    var returnValueTypeId = input.ReadValueEncodedS32();
                }

                var argumentCount = input.ReadValueEncodedS32();
                for (int j = 0; j < argumentCount; j++)
                {
                    var argumentTypeId = input.ReadValueEncodedS32();
                    var argumentName = strings[input.ReadValueEncodedS32()];
                    var argumentFlags = input.ReadValueEncodedS32();
                }

                var localCount = input.ReadValueEncodedS32();
                for (int j = 0; j < localCount; j++)
                {
                    var localTypeId = input.ReadValueEncodedS32();
                    var localName = strings[input.ReadValueEncodedS32()];
                }

                if ((flags & Script.FunctionFlags.Import) == 0)
                {
                    var unencodedByteCodeLength = input.ReadValueEncodedS32();
                    int read;
                    for (read = 0; read < unencodedByteCodeLength; )
                    {
                        var op = input.ReadValueU8();
                        var opcode = (Script.Opcode)op;
                        read++;

                        switch (opcode)
                        {
                            case Script.Opcode.OP_Target:
                            {
                                var op_0 = input.ReadValueEncodedS32(); read += 4;
                                var op_1 = input.ReadValueU8(); read++;
                                break;
                            }

                            case Script.Opcode.OP_ShortConst:
                            {
                                var op_0 = input.ReadValueS16(); read += 2;
                                break;
                            }

                            case Script.Opcode.OP_IntConst:
                            {
                                var op_0 = input.ReadValueEncodedS32(); read += 4;
                                break;
                            }

                            case Script.Opcode.OP_FloatConst:
                            {
                                var op_0 = input.ReadValueF32(); read += 4;
                                break;
                            }

                            case Script.Opcode.OP_StringConst:
                            {
                                var op_0 = input.ReadValueEncodedS32(); read += 16;
                                break;
                            }

                            case Script.Opcode.OP_Return:
                            {
                                var op_0 = input.ReadValueU16(); read += 2;
                                var op_1 = input.ReadValueU16(); read += 2;
                                var op_2 = input.ReadValueEncodedS32(); read += 4;
                                break;
                            }

                            case Script.Opcode.OP_TestNotEqual:
                            case Script.Opcode.OP_Jump:
                            {
                                var op_0 = input.ReadValueU16(); read += 2;
                                var op_1 = input.ReadValueU16(); read += 2;
                                break;
                            }

                            case Script.Opcode.OP_LocalVar:
                            case Script.Opcode.OP_Conditional:
                            case Script.Opcode.OP_Skip:
                            case Script.Opcode.OP_Constructor:
                            {
                                var op_0 = input.ReadValueU16(); read += 2;
                                break;
                            }

                            case Script.Opcode.OP_DefaultVar:
                            case Script.Opcode.OP_ObjectVar:
                            case Script.Opcode.OP_Switch:
                            case Script.Opcode.OP_TestEqual:
                            {
                                var op_0 = input.ReadValueEncodedS32();
                                var op_1 = input.ReadValueEncodedS32();
                                read += 4;
                                break;
                            }

                            case Script.Opcode.OP_SwitchDefault:
                            {
                                var op_0 = input.ReadValueEncodedS32(); read += 4;
                                var op_1 = input.ReadValueU16(); read += 2;
                                break;
                            }

                            case Script.Opcode.OP_VirtualFunc:
                            {
                                var op_0 = input.ReadValueU8(); read++;
                                var op_1 = input.ReadValueEncodedS32(); read += 4;
                                break;
                            }

                            case Script.Opcode.OP_New:
                            case Script.Opcode.OP_ArrayElement:
                            case Script.Opcode.OP_NameConst:
                            case Script.Opcode.OP_IntToFloat:
                            case Script.Opcode.OP_BoolToFloat:
                            case Script.Opcode.OP_StringToBool:
                            case Script.Opcode.OP_This:
                            case Script.Opcode.OP_BoolToInt:
                            case Script.Opcode.OP_EntryFunc:
                            case Script.Opcode.OP_IntToByte:
                            case Script.Opcode.OP_FloatToInt:
                            case Script.Opcode.OP_Delete:
                            case Script.Opcode.OP_NameToBool:
                            case Script.Opcode.OP_ArrayLast:
                            case Script.Opcode.OP_IntToBool:
                            case Script.Opcode.OP_BoolToString:
                            case Script.Opcode.OP_FloatToBool:
                            case Script.Opcode.OP_FloatToString:
                            case Script.Opcode.OP_ByteToInt:
                            case Script.Opcode.OP_NameToString:
                            case Script.Opcode.OP_FloatToByte:
                            case Script.Opcode.OP_Breakpoint:
                            {
                                var op_0 = input.ReadValueEncodedS32(); read += 4;
                                break;
                            }

                            case Script.Opcode.OP_ParamEnd:
                            {
                                var op_0 = input.ReadValueU16(); read += 2;
                                var op_1 = input.ReadValueU16(); read += 2;
                                var op_2 = input.ReadValueEncodedS32();
                                if (op_2 == -1)
                                {
                                    var op_3 = input.ReadValueEncodedS32();
                                }
                                read += 4;
                                break;
                            }

                            case Script.Opcode.OP_StructMember:
                            case Script.Opcode.OP_SavePointEnd:
                            {
                                var op_0 = input.ReadValueU16(); read += 2;
                                var op_1 = input.ReadValueEncodedS32(); read += 4;
                                break;
                            }

                            case Script.Opcode.OP_Nop:
                            case Script.Opcode.OP_Context:
                            case Script.Opcode.OP_IntZero:
                            case Script.Opcode.OP_IntOne:
                            case Script.Opcode.OP_BoolFalse:
                            case Script.Opcode.OP_BoolTrue:
                            case Script.Opcode.OP_Assign:
                            case Script.Opcode.OP_GetServer:
                            case Script.Opcode.OP_GetCamera:
                            case Script.Opcode.OP_ArrayPushBack:
                            case Script.Opcode.OP_GetPlayer:
                            case Script.Opcode.OP_ArrayResize:
                            case Script.Opcode.OP_SavePoint:
                            case Script.Opcode.OP_Null:
                            case Script.Opcode.OP_GetGame:
                            case Script.Opcode.OP_ArrayGrow:
                            case Script.Opcode.OP_ArrayFindFirst:
                            case Script.Opcode.OP_ArrayContains:
                            case Script.Opcode.OP_ArraySize:
                            case Script.Opcode.OP_ArrayErase:
                            case Script.Opcode.OP_JumpIfFalse:
                            case Script.Opcode.OP_ObjectToBool:
                            case Script.Opcode.OP_GetHud:
                            case Script.Opcode.OP_ArrayFindLastFast:
                            case Script.Opcode.OP_ArrayContainsFast:
                            case Script.Opcode.OP_SaveValue:
                            case Script.Opcode.OP_ArrayClear:
                            case Script.Opcode.OP_EnumToString:
                            case Script.Opcode.OP_ArrayFindFirstFast:
                            case Script.Opcode.OP_EnumToInt:
                            case Script.Opcode.OP_ArrayPopBack:
                            case Script.Opcode.OP_BoolToByte:
                            case Script.Opcode.OP_ArrayRemove:
                            case Script.Opcode.OP_GetSound:
                            {
                                break;
                            }

                            default:
                            {
                                throw new NotImplementedException("unhandled " + opcode.ToString());
                            }
                        }
                    }

                    if (read != unencodedByteCodeLength)
                    {
                        throw new InvalidOperationException();
                    }
                }
            }
        }
    }
}
