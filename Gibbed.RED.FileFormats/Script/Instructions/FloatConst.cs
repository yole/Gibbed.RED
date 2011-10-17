using System.IO;
using Gibbed.Helpers;

namespace Gibbed.RED.FileFormats.Script.Instructions
{
    public class FloatConst: IInstruction
    {
        private float _value;

        public int Deserialize(Stream input)
        {
            _value = input.ReadValueF32();
            return 4;
        }

        public void Serialize(ICodeWriter output)
        {
            throw new System.NotImplementedException();
        }

        public override string ToString()
        {
            return "FloatConst(" + _value + ")";
        }
    }
}