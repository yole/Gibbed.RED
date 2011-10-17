using System.IO;

namespace Gibbed.RED.FileFormats.Script.Instructions
{
    public class S32: IInstruction
    {
        private readonly Opcode _opcode;
        private int _operand;

        public S32(Opcode opcode)
        {
            _opcode = opcode;
        }

        public int Deserialize(Stream input)
        {
            _operand = input.ReadValueEncodedS32();
            return 4;
        }

        public void Serialize(ICodeWriter output)
        {
            throw new System.NotImplementedException();
        }

        public override string ToString()
        {
            return _opcode + "(" + _operand + ")";
        }
    }
}