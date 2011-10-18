using System.IO;
using Gibbed.Helpers;

namespace Gibbed.RED.FileFormats.Script.Instructions
{
    public class U16S32: IInstruction
    {
        private Opcode _opcode;
        private ushort _op0;
        private int _op1;

        public U16S32(Opcode opcode)
        {
            _opcode = opcode;
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
            return _opcode + "(" + _op0 + "," + _op1 + ")";
        }
    }
}