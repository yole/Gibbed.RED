using System.IO;
using Gibbed.Helpers;

namespace Gibbed.RED.FileFormats.Script.Instructions
{
    public class U16S32: IInstruction
    {
        private readonly CompiledScriptsFile _scripts;
        private readonly Opcode _opcode;
        private ushort _op0;
        private int _op1;

        public U16S32(Opcode opcode, CompiledScriptsFile scripts)
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
            _op0 = input.ReadValueU16();
            _op1 = input.ReadValueEncodedS32();
            return 6;
        }

        public void Serialize(ICodeWriter output)
        {
            throw new System.NotImplementedException();
        }

        public override string ToString()
        {
            return _opcode + "(" + _op0 + "," + Operand + ")";
        }

        public string Operand
        {
            get { return _scripts.Strings[_op1].Value; }
        }
    }
}