using System.IO;
using Gibbed.Helpers;

namespace Gibbed.RED.FileFormats.Script.Instructions
{
    public class VirtualFunc: IInstruction
    {
        private byte _op0;
        private int _op1;

        public int Deserialize(Stream input)
        {
            _op0 = input.ReadValueU8();
            _op1 = input.ReadValueEncodedS32();
            return 5;
        }

        public void Serialize(ICodeWriter output)
        {
            throw new System.NotImplementedException();
        }

        public override string ToString()
        {
            return "VirtualFunc(" + _op0 + "," + _op1 + ")";
        }
    }
}