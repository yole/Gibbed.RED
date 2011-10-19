using System.IO;

namespace Gibbed.RED.FileFormats.Script.Instructions
{
    class TypeMember: IInstruction
    {
        private readonly Opcode _opcode;
        private readonly CompiledScriptsFile _scripts;
        private int _opNameId;
        private int _opTypeId;

        public TypeMember(Opcode opcode, CompiledScriptsFile scripts)
        {
            _opcode = opcode;
            _scripts = scripts;
        }

        public int Deserialize(Stream input)
        {
            _opNameId = input.ReadValueEncodedS32();
            _opTypeId = input.ReadValueEncodedS32();
            return 4;
        }

        public void Serialize(ICodeWriter output)
        {
            throw new System.NotImplementedException();
        }

        public override string ToString()
        {
            if (_opTypeId == -1)
            {
                return string.Format("{0}('{1}')", _opcode, _scripts.Strings[_opNameId].Value);
            }
            return string.Format("{0}('{1}', {2})", _opcode, _scripts.Strings[_opNameId].Value, _scripts.TypeDefs[_opTypeId].Name);
        }
    }
}