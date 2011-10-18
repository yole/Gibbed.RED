using System.IO;

namespace Gibbed.RED.FileFormats.Script.Instructions
{
    public class StringConst: IInstruction
    {
        private int _op0;

        public int Deserialize(Stream input)
        {
            _op0 = input.ReadValueEncodedS32();
            return 16;
        }

        public void Serialize(ICodeWriter output)
        {
            throw new System.NotImplementedException();
        }

        public override string ToString()
        {
            return "StringConst(" + _op0 + ")";
        }
    }
}