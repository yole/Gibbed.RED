using System.IO;
using Gibbed.Helpers;

namespace Gibbed.RED.FileFormats.Script.Instructions
{
    public class U16: IInstruction
    {
        private readonly Opcode _opcode;

        public U16(Opcode opcode)
        {
            _opcode = opcode;
        }

        public Opcode Opcode
        {
            get { return _opcode; }
        }

        public int Deserialize(Stream input)
        {
            Op0 = input.ReadValueU16();
            return 2;
        }

        public void Serialize(ICodeWriter output)
        {
            throw new System.NotImplementedException();
        }

        public override string ToString()
        {
            return _opcode + "(" + Op0 + ")";
        }

        public ushort Op0 { get; private set; }
    }
}