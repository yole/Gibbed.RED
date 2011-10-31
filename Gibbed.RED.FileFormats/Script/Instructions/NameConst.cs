using System.IO;

namespace Gibbed.RED.FileFormats.Script.Instructions
{
    class NameConst: IInstruction
    {
        private readonly RawString[] _strings;
        private string _value;

        public NameConst(RawString[] strings)
        {
            _strings = strings;
        }

        public Opcode Opcode
        {
            get { return Opcode.OP_NameConst; }
        }

        public int Deserialize(Stream input)
        {
            int index = input.ReadValueEncodedS32();
            _value = _strings[index].Value;
            return 4;
        }

        public void Serialize(ICodeWriter output)
        {
            throw new System.NotImplementedException();
        }

        public override string ToString()
        {
            return "NameConst('" + _value + "')";
        }
    }
}