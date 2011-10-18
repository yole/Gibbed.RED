using System.IO;
using Gibbed.Helpers;

namespace Gibbed.RED.FileFormats.Script.Instructions
{
    public class ShortConst: IInstruction
    {
        private short _value;

        public int Deserialize(Stream input)
        {
            _value = input.ReadValueS16();
            return 2;
        }

        public void Serialize(ICodeWriter output)
        {
            throw new System.NotImplementedException();
        }

        public override string ToString()
        {
            return "ShortConst(" + _value + ")";
        }
    }
}