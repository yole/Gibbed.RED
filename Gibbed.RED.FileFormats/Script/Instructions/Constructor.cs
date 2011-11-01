using System.IO;
using Gibbed.Helpers;

namespace Gibbed.RED.FileFormats.Script.Instructions
{
    public class Constructor: IInstruction
    {
        private readonly CompiledScriptsFile _scripts;
        private int _opTypeId;

        public Constructor(CompiledScriptsFile scripts)
        {
            _scripts = scripts;
        }

        public Opcode Opcode
        {
            get { return Opcode.OP_Constructor; }
        }

        public int Deserialize(Stream input)
        {
            OpArgCount = input.ReadValueU8();
            _opTypeId = input.ReadValueEncodedS32();
            return 5;
        }

        public void Serialize(ICodeWriter output)
        {
            throw new System.NotImplementedException();
        }

        public override string ToString()
        {
            return "Constructor(" + OpArgCount + "," + TypeName + ")";
        }

        public byte OpArgCount { get; private set; }

        public string TypeName
        {
            get { return _scripts.TypeDefs[_opTypeId].Name; }
        }
    }
}