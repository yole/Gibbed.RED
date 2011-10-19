using System.IO;
using Gibbed.Helpers;

namespace Gibbed.RED.FileFormats.Script.Instructions
{
    public class Switch: IInstruction
    {
        private int _op0;
        private ushort _op1;

        public Switch()
        {
        }

        public int Deserialize(Stream input)
        {
            _op0 = input.ReadValueEncodedS32();
            _op1 = input.ReadValueU16();
            return 6;
        }

        public void Serialize(ICodeWriter output)
        {
            throw new System.NotImplementedException();
        }

        public override string ToString()
        {
            return "Switch(" + _op0 + "," + _op1 + ")";
        }
    }
}