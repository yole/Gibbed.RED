using System.IO;
using Gibbed.Helpers;

namespace Gibbed.RED.FileFormats.Script.Instructions
{
    public class ParamEnd: IInstruction
    {
        private ushort _op0;
        private ushort _op1;
        private int _op2;
        private int _op3;

        public int Deserialize(Stream input)
        {
            _op0 = input.ReadValueU16();
            _op1 = input.ReadValueU16();
            _op2 = input.ReadValueEncodedS32();
            if (_op2 == -1)
            {
                _op3 = input.ReadValueEncodedS32();
            }
            return 8;
        }

        public void Serialize(ICodeWriter output)
        {
            throw new System.NotImplementedException();
        }

        public override string ToString()
        {
            return "ParamEnd(" + _op0 + "," + _op1 + "," + _op2 + "," + _op3 + ")";
        }
    }
}