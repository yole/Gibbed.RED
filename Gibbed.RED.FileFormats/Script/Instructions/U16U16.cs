using System.IO;
using Gibbed.Helpers;

namespace Gibbed.RED.FileFormats.Script.Instructions
{
    public class U16U16: IInstruction
    {
        private readonly Opcode _opcode;
        private ushort _op0;
        private ushort _op1;

        public U16U16(Opcode opcode)
        {
            _opcode = opcode;
        }

        public Opcode Opcode
        {
            get { return _opcode; }
        }

        public ushort Op0
        {
            get { return _op0; }
        }

        public int Deserialize(Stream input)
        {
            _op0 = input.ReadValueU16();
            _op1 = input.ReadValueU16();
            return 4;
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