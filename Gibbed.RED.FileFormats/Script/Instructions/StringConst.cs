using System.IO;

namespace Gibbed.RED.FileFormats.Script.Instructions
{
    public class StringConst: IInstruction
    {
        private readonly CompiledScriptsFile _scripts;
        private int _op0;

        public StringConst(CompiledScriptsFile scripts)
        {
            _scripts = scripts;
        }

        public Opcode Opcode
        {
            get { return Opcode.OP_StringConst; }
        }

        public int Deserialize(Stream input)
        {
            _op0 = input.ReadValueEncodedS32();
            return 16;
        }

        public void Serialize(ICodeWriter output)
        {
            throw new System.NotImplementedException();
        }

        public override string ToString()
        {
            return "StringConst('" + Value + "')";
        }

        public string Value
        {
            get { return _scripts.Strings[_op0].Value; }
        }
    }
}