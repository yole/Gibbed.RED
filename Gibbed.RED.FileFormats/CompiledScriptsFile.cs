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
using System.IO;
using Gibbed.Helpers;
using Gibbed.RED.FileFormats.Script;
using Gibbed.RED.FileFormats.Script.Instructions;

namespace Gibbed.RED.FileFormats
{
    public class CompiledScriptsFile
    {
        public string Unknown0;
        public DateTime TimeStamp;
        public string Unknown2;

        public Script.TypeDefinition[] TypeDefs;
        public Script.FunctionDefinition[] FuncDefs;
        internal RawString[] Strings;
        private RawFunctionDefinition[] _rawFuncDefs;

        public void Deserialize(Stream input)
        {
            input.Seek(-4, SeekOrigin.End);
            
            // read strings
            var stringTableOffset = input.ReadValueU32();
            input.Seek(stringTableOffset, SeekOrigin.Begin);
            var stringCount = input.ReadValueU32();
            Strings = new Script.RawString[stringCount];
            for (int i = 0; i < Strings.Length; i++)
            {
                var stringEntry = new Script.RawString();
                stringEntry.Value = input.ReadEncodedString();
                stringEntry.IsName = input.ReadValueB8();
                Strings[i] = stringEntry;
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
                rawTypeDef.Name = Strings[input.ReadValueEncodedS32()].Value;
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

            _rawFuncDefs = new Script.RawFunctionDefinition[funcDefCount];
            FuncDefs = new Script.FunctionDefinition[funcDefCount];

            for (uint i = 0; i < funcDefCount; i++)
            {
                var rawFuncDef = new Script.RawFunctionDefinition();
                rawFuncDef.Name = Strings[input.ReadValueEncodedS32()].Value;
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
                    funcDef.ContainingClass = typeDef;
                }

                _rawFuncDefs[i] = rawFuncDef;
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
                    var constantName = Strings[input.ReadValueEncodedS32()].Value;
                    var constantValue = input.ReadValueEncodedS32();
                    typeDef.Constants.Add(constantName, constantValue);
                }
            }

            // parse classes
            for (int i = 0; i < rawTypeDefs.Length; i++)
            {
                ParseClass(input, rawTypeDefs, i);
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
                    var propName = Strings[input.ReadValueEncodedS32()].Value;

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
            for (int i = 0; i < _rawFuncDefs.Length; i++)
            {
                ParseFunction(input, i);
            }
        }

        private void ParseClass(Stream input, RawTypeDefinition[] rawTypeDefs, int i)
        {
            var rawTypeDef = rawTypeDefs[i];

            if (rawTypeDef.Type != Script.NativeType.Class)
            {
                return;
            }
            else if ((rawTypeDef.Flags &
                      Script.TypeDefinitionFlags.Scripted) == 0)
            {
                return;
            }

            var type = (Script.NativeType) input.ReadValueEncodedS32();
            if (rawTypeDef.Type != type)
            {
                throw new FormatException();
            }

            var id = input.ReadValueEncodedS32();
            if (id != i)
            {
                throw new FormatException();
            }

            var typeDef = (Script.ClassDefinition) TypeDefs[i];

            var isExtending = input.ReadValueEncodedS32();
            if (isExtending != 0)
            {
                var superTypeId = input.ReadValueEncodedS32();
                var superDef = (Script.ClassDefinition) TypeDefs[superTypeId];
                typeDef.Super = superDef;
            }

            var stateCount = input.ReadValueEncodedS32();
            typeDef.States.Clear();
            for (int j = 0; j < stateCount; j++)
            {
                var stateName = Strings[input.ReadValueEncodedS32()].Value;
                var stateTypeId = input.ReadValueEncodedS32();
                var stateDef = (Script.ClassDefinition) TypeDefs[stateTypeId];
                typeDef.States.Add(stateName, stateDef);
            }

            typeDef.NativeProperties.Clear();
            for (int j = 0; j < rawTypeDef.NativePropertyCount; j++)
            {
                var nativePropertyName = Strings[input.ReadValueEncodedS32()].Value;
                typeDef.NativeProperties.Add(nativePropertyName);
            }

            typeDef.Properties.Clear();
            for (int j = 0; j < rawTypeDef.ScriptedPropertyCount; j++)
            {
                var propTypeId = input.ReadValueEncodedS32();
                var propName = Strings[input.ReadValueEncodedS32()].Value;
                var propFlags = input.ReadValueEncodedS32();

                var property = new Script.PropertyDefinition()
                                   {
                                       Name = propName,
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

        private void ParseFunction(Stream input, int index)
        {
            var rawFuncDef = _rawFuncDefs[index];
            var funcDef = FuncDefs[index];

            if ((rawFuncDef.Flags & 1) == 0)
            {
                return;
            }

            var id = input.ReadValueEncodedS32();
            if (id != index)
            {
                throw new FormatException();
            }

            var rawFlags = input.ReadValueEncodedS32();
            var flags = (Script.FunctionFlags) rawFlags;

            var hasReturnValue = input.ReadValueB8();
            if (hasReturnValue == true)
            {
                var returnValueTypeId = input.ReadValueEncodedS32();
                funcDef.ReturnValue = TypeDefs[returnValueTypeId];
            }

            var argumentCount = input.ReadValueEncodedS32();
            for (int j = 0; j < argumentCount; j++)
            {
                var argumentTypeId = input.ReadValueEncodedS32();
                var argumentName = Strings[input.ReadValueEncodedS32()];
                var argumentFlags = input.ReadValueEncodedS32();

                ArgumentDefinition argumentDefinition = new ArgumentDefinition();
                argumentDefinition.Name = argumentName.Value;
                argumentDefinition.Type = TypeDefs[argumentTypeId];
                argumentDefinition.Flags = argumentFlags;

                funcDef.Arguments.Add(argumentDefinition);
            }

            var localCount = input.ReadValueEncodedS32();
            for (int j = 0; j < localCount; j++)
            {
                var localTypeId = input.ReadValueEncodedS32();
                var localName = Strings[input.ReadValueEncodedS32()];

                var localDefinition = new LocalDefinition()
                                          {
                                              Name = localName.Value,
                                              Type = TypeDefs[localTypeId]
                                          };
                funcDef.Locals.Add(localDefinition);
            }

            if ((flags & Script.FunctionFlags.Import) == 0)
            {
                ReadBytecode(input, funcDef);
            }
        }

        private void ReadBytecode(Stream input, FunctionDefinition funcDef)
        {
            funcDef.Instructions = new List<IInstruction>();
            funcDef.InstructionOffsets = new List<int>();
            var unencodedByteCodeLength = input.ReadValueEncodedS32();
            int read;
            for (read = 0; read < unencodedByteCodeLength;)
            {
                funcDef.InstructionOffsets.Add(read);
                var op = input.ReadValueU8();
                var opcode = (Script.Opcode) op;
                
                read++;
                Script.IInstruction instruction = null;

                switch (opcode)
                {
                    case Script.Opcode.OP_Target:
                        {
                            instruction = new Target();
                            break;
                        }

                    case Script.Opcode.OP_ShortConst:
                        {
                            instruction = new ShortConst();
                            break;
                        }

                    case Script.Opcode.OP_IntConst:
                        {
                            instruction = new IntConst();
                            break;
                        }

                    case Script.Opcode.OP_FloatConst:
                        {
                            instruction = new FloatConst();
                            break;
                        }

                    case Script.Opcode.OP_StringConst:
                        {
                            instruction = new StringConst(this);
                            break;
                        }

                    case Script.Opcode.OP_VirtualFunc:
                        {
                            instruction = new VirtualFunc(this);
                            break;
                        }

                    case Script.Opcode.OP_Context:
                    case Script.Opcode.OP_SwitchLabel:
                        {
                            instruction = new U16U16(opcode);
                            break;
                        }

                    case Script.Opcode.OP_Assign:
                    case Script.Opcode.OP_JumpIfFalse:
                    case Script.Opcode.OP_Jump:
                    case Script.Opcode.OP_Skip:
                        {
                            instruction = new U16(opcode);
                            break;
                        }

                    case Script.Opcode.OP_LocalVar:
                    case Script.Opcode.OP_ObjectVar:
                    case Script.Opcode.OP_ParamVar:
                    case Script.Opcode.OP_StructMember:
                        {
                            instruction = new TypeMember(opcode, this);
                            break;
                        }

                    case Script.Opcode.OP_Switch:
                        {
                            instruction = new Switch();
                            break;
                        }

                    case Script.Opcode.OP_Constructor:
                        {
                            instruction = new Constructor(this);
                            break;
                        }

                    case Script.Opcode.OP_TestEqual:
                    case Script.Opcode.OP_EnumToInt:
                    case Script.Opcode.OP_ArrayPushBack:
                    case Script.Opcode.OP_ArraySize:
                    case Script.Opcode.OP_ArrayElement:
                    case Script.Opcode.OP_New:
                    case Script.Opcode.OP_ArrayClear:
                    case Script.Opcode.OP_DynamicCast:
                    case Script.Opcode.OP_ArrayContainsFast:
                    case Script.Opcode.OP_ArrayRemoveFast:
                    case Script.Opcode.OP_TestNotEqual:
                    case Script.Opcode.OP_ArrayErase:
                    case Script.Opcode.OP_EnumToString:
                    case Script.Opcode.OP_ArrayContains:
                    case Script.Opcode.OP_ArrayResize:
                    case Script.Opcode.OP_ArrayInsert:
                    case Script.Opcode.OP_ArrayGrow:
                    case Script.Opcode.OP_ArrayFindFirstFast:
                    case Script.Opcode.OP_ArrayLast:
                    case Script.Opcode.OP_ArrayRemove:
                    case Script.Opcode.OP_SaveValue:
                        {
                            instruction = new TypeRef(opcode, this);
                            break;
                        }

                    case Script.Opcode.OP_NameConst:
                        {
                            instruction = new NameConst(Strings);
                            break;
                        }

                    case Script.Opcode.OP_FinalFunc:
                        {
                            instruction = new FinalFunc(this);
                            break;
                        }

                    case Script.Opcode.OP_EntryFunc:
                    case Script.Opcode.OP_SavePoint:
                        {
                            instruction = new U16S32(opcode, this);
                            break;
                        }

                    case Script.Opcode.OP_Nop:
                    case Script.Opcode.OP_ParamEnd:
                    case Script.Opcode.OP_IntZero:
                    case Script.Opcode.OP_IntOne:
                    case Script.Opcode.OP_BoolFalse:
                    case Script.Opcode.OP_BoolTrue:
                    case Script.Opcode.OP_Return:
                    case Script.Opcode.OP_GetServer:
                    case Script.Opcode.OP_GetCamera:
                    case Script.Opcode.OP_NameToString:
                    case Script.Opcode.OP_GetPlayer:
                    case Script.Opcode.OP_IntToFloat:
                    case Script.Opcode.OP_This:
                    case Script.Opcode.OP_Null:
                    case Script.Opcode.OP_GetGame:
                    case Script.Opcode.OP_ObjectToBool:
                    case Script.Opcode.OP_IntToString:
                    case Script.Opcode.OP_FloatToString:
                    case Script.Opcode.OP_IntToByte:
                    case Script.Opcode.OP_ObjectToString:
                    case Script.Opcode.OP_SwitchDefault:
                    case Script.Opcode.OP_BoolToString:
                    case Script.Opcode.OP_GetHud:
                    case Script.Opcode.OP_FloatToInt:
                    case Script.Opcode.OP_NameToBool:
                    case Script.Opcode.OP_Parent:
                    case Script.Opcode.OP_IntToBool:
                    case Script.Opcode.OP_ByteToInt:
                    case Script.Opcode.OP_FloatToBool:
                    case Script.Opcode.OP_ByteToFloat:
                    case Script.Opcode.OP_StringToBool:
                    case Script.Opcode.OP_SavePointEnd:
                    case Script.Opcode.OP_StringToInt:
                    case Script.Opcode.OP_GetSound:
                        {
                            instruction = new Simple(opcode);
                            break;
                        }

                    default:
                        {
                            throw new NotImplementedException("unhandled " + opcode.ToString());
                        }
                }
                read += instruction.Deserialize(input);
                funcDef.Instructions.Add(instruction);
            }

            if (read != unencodedByteCodeLength)
            {
                throw new InvalidOperationException();
            }
        }
    }
}
