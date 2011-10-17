using System.IO;

namespace Gibbed.RED.FileFormats.Script.Instructions
{
    public class S32S32: IInstruction
    {
        private readonly Opcode _opcode;
        private int _op0;
        private int _op1;

        public S32S32(Opcode opcode)
        {
            _opcode = opcode;
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
            return _opcode + "(" + _op0 + "," + _op1 + ")";
        }
    }
}