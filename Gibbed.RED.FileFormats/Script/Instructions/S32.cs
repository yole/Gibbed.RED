using System.IO;

namespace Gibbed.RED.FileFormats.Script.Instructions
{
    public class S32: IInstruction
    {
        private readonly CompiledScriptsFile _scripts;
        private readonly Opcode _opcode;
        private int _operand;

        public S32(Opcode opcode, CompiledScriptsFile scripts)
        {
            _opcode = opcode;
            _scripts = scripts;
        }

        public int Deserialize(Stream input)
        {
            _operand = input.ReadValueEncodedS32();
            return 4;
        }

        public void Serialize(ICodeWriter output)
        {
            throw new System.NotImplementedException();
        }

        public override string ToString()
        {
            if (_opcode == Opcode.OP_EnumToInt)
            {
                return _opcode + "(" + _scripts.TypeDefs[_operand].Name + ")";
            }
            return _opcode + "(" + _operand + ")";
        }
    }
}