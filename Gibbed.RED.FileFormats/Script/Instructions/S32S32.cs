using System.IO;

namespace Gibbed.RED.FileFormats.Script.Instructions
{
    class S32S32: IInstruction
    {
        private readonly Opcode _opcode;
        private readonly CompiledScriptsFile _scripts;
        private int _op0;
        private int _op1;

        public S32S32(Opcode opcode, CompiledScriptsFile scripts)
        {
            _opcode = opcode;
            _scripts = scripts;
        }

        public int Deserialize(Stream input)
        {
            _op0 = input.ReadValueEncodedS32();
            _op1 = input.ReadValueEncodedS32();
            return 4;
        }

        public void Serialize(ICodeWriter output)
        {
            throw new System.NotImplementedException();
        }

        public override string ToString()
        {
            if (_op1 == -1)
            {
                return string.Format("{0}('{1}')", _opcode, _scripts.Strings[_op0].Value);
            }
            return string.Format("{0}('{1}', {2})", _opcode, _scripts.Strings[_op0].Value, _scripts.TypeDefs[_op1].Name);
        }
    }
}