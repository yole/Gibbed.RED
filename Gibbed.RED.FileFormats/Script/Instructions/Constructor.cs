using System.IO;
using Gibbed.Helpers;

namespace Gibbed.RED.FileFormats.Script.Instructions
{
    public class Constructor: IInstruction
    {
        private readonly CompiledScriptsFile _scripts;
        private byte _op0;
        private int _opTypeName;

        public Constructor(CompiledScriptsFile scripts)
        {
            _scripts = scripts;
        }

        public Opcode Opcode
        {
            get { return Opcode.OP_Constructor; }
        }

        public int Deserialize(Stream input)
        {
            _op0 = input.ReadValueU8();
            _opTypeName = input.ReadValueEncodedS32();
            return 5;
        }

        public void Serialize(ICodeWriter output)
        {
            throw new System.NotImplementedException();
        }

        public override string ToString()
        {
            return "Constructor(" + _op0 + "," + _scripts.Strings[_opTypeName].Value + ")";
        }
    }
}