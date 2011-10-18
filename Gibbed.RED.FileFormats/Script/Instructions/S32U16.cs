using System.IO;
using Gibbed.Helpers;

namespace Gibbed.RED.FileFormats.Script.Instructions
{
    public class S32U16: IInstruction
    {
        private readonly Opcode _opcode;
        private int _op0;
        private ushort _op1;

        public S32U16(Opcode opcode)
        {
            _opcode = opcode;
        }

        public int Deserialize(Stream input)
        {
            _op0 = input.ReadValueEncodedS32();
            _op1 = input.ReadValueU16();
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