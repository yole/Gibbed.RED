using System.IO;

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
            return _opcode + "(" + (_typeId == -1 ? "-1" :_scripts.TypeDefs[_typeId].Name) + ")";
        }
    }
}