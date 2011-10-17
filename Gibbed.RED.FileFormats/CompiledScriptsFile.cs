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
using Gibbed.Helpers;

namespace Gibbed.RED.FileFormats
{
    public class CompiledScriptsFile
    {
        public string Unknown0;
        public DateTime TimeStamp;
        public string Unknown2;

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
            var typeDefs = new Script.TypeDefinition[typeDefCount];

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

                typeDefs[i] = typeDef;
            }

            var rawFuncDefs = new Script.RawFunctionDefinition[funcDefCount];
            var funcDefs = new Script.FunctionDefinition[funcDefCount];

            for (uint i = 0; i < funcDefCount; i++)
            {
                var rawFuncDef = new Script.RawFunctionDefinition();
                rawFuncDef.Name = strings[input.ReadValueEncodedS32()].Value;
                rawFuncDef.DefinedOnId = input.ReadValueEncodedS32();
                rawFuncDef.Flags = input.ReadValueEncodedS32();

                var funcDef = new Script.FunctionDefinition();

                if (rawFuncDef.DefinedOnId == 0)
                {
                }

                rawFuncDefs[i] = rawFuncDef;
                funcDefs[i] = funcDef;
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

                var typeDef = (Script.EnumDefinition)typeDefs[i];
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

                var typeDef = (Script.ClassDefinition)typeDefs[i];

                var isExtending = input.ReadValueEncodedS32();
                if (isExtending != 0)
                {
                    var superTypeId = input.ReadValueEncodedS32();
                    var superDef = (Script.ClassDefinition)typeDefs[superTypeId];
                    typeDef.Super = superDef;
                }

                var stateCount = input.ReadValueEncodedS32();
                typeDef.States.Clear();
                for (int j = 0; j < stateCount; j++)
                {
                    var stateName = strings[input.ReadValueEncodedS32()].Value;
                    var stateTypeId = input.ReadValueEncodedS32();
                    var stateDef = (Script.ClassDefinition)typeDefs[stateTypeId];
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
                        TypeDefinition = typeDefs[propTypeId],
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

                var typeDef = (Script.ClassDefinition)typeDefs[i];

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
                            case Script.Opcode.U12:
                            {
                                var op_0 = input.ReadValueEncodedS32(); read += 4;
                                var op_1 = input.ReadValueU8(); read++;
                                break;
                            }

                            case Script.Opcode.U05:
                            {
                                var op_0 = input.ReadValueS16(); read += 2;
                                break;
                            }

                            case Script.Opcode.U04:
                            {
                                var op_0 = input.ReadValueEncodedS32(); read += 4;
                                break;
                            }

                            case Script.Opcode.U06:
                            {
                                var op_0 = input.ReadValueF32(); read += 4;
                                break;
                            }

                            case Script.Opcode.U07:
                            {
                                var op_0 = input.ReadValueEncodedS32(); read += 16;
                                break;
                            }

                            case Script.Opcode.U28:
                            {
                                var op_0 = input.ReadValueU16(); read += 2;
                                var op_1 = input.ReadValueU16(); read += 2;
                                var op_2 = input.ReadValueEncodedS32(); read += 4;
                                break;
                            }

                            case Script.Opcode.U33:
                            case Script.Opcode.U20:
                            {
                                var op_0 = input.ReadValueU16(); read += 2;
                                var op_1 = input.ReadValueU16(); read += 2;
                                break;
                            }

                            case Script.Opcode.U13:
                            case Script.Opcode.U23:
                            case Script.Opcode.U22:
                            case Script.Opcode.U24:
                            {
                                var op_0 = input.ReadValueU16(); read += 2;
                                break;
                            }

                            case Script.Opcode.U15:
                            case Script.Opcode.U16:
                            case Script.Opcode.U17:
                            case Script.Opcode.U32:
                            {
                                var op_0 = input.ReadValueEncodedS32();
                                var op_1 = input.ReadValueEncodedS32();
                                read += 4;
                                break;
                            }

                            case Script.Opcode.U19:
                            {
                                var op_0 = input.ReadValueEncodedS32(); read += 4;
                                var op_1 = input.ReadValueU16(); read += 2;
                                break;
                            }

                            case Script.Opcode.U26:
                            {
                                var op_0 = input.ReadValueU8(); read++;
                                var op_1 = input.ReadValueEncodedS32(); read += 4;
                                break;
                            }

                            case Script.Opcode.U34:
                            case Script.Opcode.U86:
                            case Script.Opcode.U08:
                            case Script.Opcode.U52:
                            case Script.Opcode.U44:
                            case Script.Opcode.U60:
                            case Script.Opcode.U36:
                            case Script.Opcode.U43:
                            case Script.Opcode.U87:
                            case Script.Opcode.U51:
                            case Script.Opcode.U56:
                            case Script.Opcode.U35:
                            case Script.Opcode.U58:
                            case Script.Opcode.U85:
                            case Script.Opcode.U50:
                            case Script.Opcode.U45:
                            case Script.Opcode.U54:
                            case Script.Opcode.U57:
                            case Script.Opcode.U47:
                            case Script.Opcode.U59:
                            case Script.Opcode.U55:
                            case Script.Opcode.U41:
                            {
                                var op_0 = input.ReadValueEncodedS32(); read += 4;
                                break;
                            }

                            case Script.Opcode.U27:
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

                            case Script.Opcode.U29:
                            case Script.Opcode.U40:
                            {
                                var op_0 = input.ReadValueU16(); read += 2;
                                var op_1 = input.ReadValueEncodedS32(); read += 4;
                                break;
                            }

                            case Script.Opcode.U00:
                            case Script.Opcode.U30:
                            case Script.Opcode.U03:
                            case Script.Opcode.U02:
                            case Script.Opcode.U11:
                            case Script.Opcode.U10:
                            case Script.Opcode.U31:
                            case Script.Opcode.U89:
                            case Script.Opcode.U91:
                            case Script.Opcode.U78:
                            case Script.Opcode.U90:
                            case Script.Opcode.U71:
                            case Script.Opcode.U38:
                            case Script.Opcode.U01:
                            case Script.Opcode.U88:
                            case Script.Opcode.U83:
                            case Script.Opcode.U72:
                            case Script.Opcode.U76:
                            case Script.Opcode.U70:
                            case Script.Opcode.U84:
                            case Script.Opcode.U21:
                            case Script.Opcode.U64:
                            case Script.Opcode.U92:
                            case Script.Opcode.U75:
                            case Script.Opcode.U77:
                            case Script.Opcode.U39:
                            case Script.Opcode.U69:
                            case Script.Opcode.U66:
                            case Script.Opcode.U73:
                            case Script.Opcode.U67:
                            case Script.Opcode.U79:
                            case Script.Opcode.U42:
                            case Script.Opcode.U81:
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
