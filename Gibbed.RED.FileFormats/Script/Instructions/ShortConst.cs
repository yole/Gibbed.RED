using System.IO;
using Gibbed.Helpers;

namespace Gibbed.RED.FileFormats.Script.Instructions
{
    public class ShortConst: IInstruction
    {
        public Opcode Opcode
        {
            get { return Opcode.OP_ShortConst; }
        }

        public int Deserialize(Stream input)
        {
            Value = input.ReadValueS16();
            return 2;
        }

        public void Serialize(ICodeWriter output)
        {
            throw new System.NotImplementedException();
        }

        public override string ToString()
        {
            return "ShortConst(" + Value + ")";
        }

        public short Value { get; private set; }
    }
}