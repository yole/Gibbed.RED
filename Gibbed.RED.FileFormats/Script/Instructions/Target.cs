using System.IO;
using Gibbed.Helpers;

namespace Gibbed.RED.FileFormats.Script.Instructions
{
    public class Target: IInstruction
    {
        private int _op0;
        private byte _op1;

        public Opcode Opcode
        {
            get { return Opcode.OP_Target; }
        }

        public int Deserialize(Stream input)
        {
            _op0 = input.ReadValueEncodedS32();
            _op1 = input.ReadValueU8();
            return 5;
        }

        public void Serialize(ICodeWriter output)
        {
            throw new System.NotImplementedException();
        }

        public override string ToString()
        {
            return "Target(" + _op0 + "," + _op1 + ")";
        }

        public int Op0 { get { return _op0; } }
    }
}