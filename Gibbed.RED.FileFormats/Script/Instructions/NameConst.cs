using System.IO;

namespace Gibbed.RED.FileFormats.Script.Instructions
{
    public class NameConst: IInstruction
    {
        private readonly RawString[] _strings;

        internal NameConst(RawString[] strings)
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
            Value = _strings[index].Value;
            return 4;
        }

        public void Serialize(ICodeWriter output)
        {
            throw new System.NotImplementedException();
        }

        public override string ToString()
        {
            return "NameConst('" + Value + "')";
        }

        public string Value { get; private set; }
    }
}