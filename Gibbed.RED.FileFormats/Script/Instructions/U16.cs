using System.IO;
using Gibbed.Helpers;

namespace Gibbed.RED.FileFormats.Script.Instructions
{
    public class U16: IInstruction
    {
        private readonly Opcode _opcode;
        private ushort _op0;

        public U16(Opcode opcode)
        {
            _opcode = opcode;
        }

        public int Deserialize(Stream input)
        {
            _op0 = input.ReadValueU16();
            return 2;
        }

        public void Serialize(ICodeWriter output)
        {
            throw new System.NotImplementedException();
        }

        public override string ToString()
        {
            return _opcode + "(" + _op0 + ")";
        }
    }
}