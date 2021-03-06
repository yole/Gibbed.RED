﻿using System.IO;

namespace Gibbed.RED.FileFormats.Script.Instructions
{
    public class TypeRef: IInstruction
    {
        private readonly CompiledScriptsFile _scripts;
        private readonly Opcode _opcode;
        private int _typeId;

        public TypeRef(Opcode opcode, CompiledScriptsFile scripts)
        {
            _opcode = opcode;
            _scripts = scripts;
        }

        public Opcode Opcode
        {
            get { return _opcode; }
        }

        public int Deserialize(Stream input)
        {
            _typeId = input.ReadValueEncodedS32();
            return 4;
        }

        public void Serialize(ICodeWriter output)
        {
            throw new System.NotImplementedException();
        }

        public override string ToString()
        {
            if (_opcode == Opcode.OP_SaveValue)
            {
                return _opcode + "(" + _scripts.Strings[_typeId].Value + ")";
            }
            return _opcode + "(" + TypeName + ")";
        }

        public string TypeName
        {
            get { return (_typeId == -1 ? "-1" : TypeDef.Name); }
        }

        public TypeDefinition TypeDef
        {
            get { return _scripts.TypeDefs[_typeId]; }
        }
    }
}