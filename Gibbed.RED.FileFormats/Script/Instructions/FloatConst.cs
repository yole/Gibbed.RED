using System.IO;
using Gibbed.Helpers;

namespace Gibbed.RED.FileFormats.Script.Instructions
{
    public class FloatConst: IInstruction
    {
        public Opcode Opcode
        {
            get { return Opcode.OP_FloatConst; }
        }

        public int Deserialize(Stream input)
        {
            Value = input.ReadValueF32();
            return 4;
        }

        public void Serialize(ICodeWriter output)
        {
            throw new System.NotImplementedException();
        }

        public override string ToString()
        {
            return "FloatConst(" + Value + ")";
        }

        public float Value { get; private set; }
    }
}