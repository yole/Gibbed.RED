using System.IO;

namespace Gibbed.RED.FileFormats.Script.Instructions
{
    public class Simple: IInstruction
    {
        private readonly Opcode _opcode;

        public Simple(Opcode opcode)
        {
            _opcode = opcode;
        }

        public override string ToString()
        {
            return _opcode.ToString();
        }

        public Opcode Opcode
        {
            get { return _opcode; }
        }

        public int Deserialize(Stream input)
        {
            return 0;
        }

        public void Serialize(ICodeWriter output)
        {
            throw new System.NotImplementedException();
        }
    }
}