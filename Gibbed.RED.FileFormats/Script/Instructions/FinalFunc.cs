using System.IO;
using Gibbed.Helpers;

namespace Gibbed.RED.FileFormats.Script.Instructions
{
    public class FinalFunc: IInstruction
    {
        private readonly CompiledScriptsFile _scripts;
        private ushort _opFlags;
        private ushort _opTarget;
        private int _opFuncId;
        private int _opOperator;

        public FinalFunc(CompiledScriptsFile scripts)
        {
            _scripts = scripts;
        }

        public int Deserialize(Stream input)
        {
            _opFlags = input.ReadValueU16();
            _opTarget = input.ReadValueU16();
            _opFuncId = input.ReadValueEncodedS32();
            if (_opFuncId == -1)
            {
                _opOperator = input.ReadValueEncodedS32();
            }
            return 8;
        }

        public void Serialize(ICodeWriter output)
        {
            throw new System.NotImplementedException();
        }

        public override string ToString()
        {
            return "FinalFunc(" + _opFlags + "," + _opTarget + "," + (_opFuncId == -1 ? ((OperatorCode) _opOperator).ToString() : _scripts.FuncDefs[_opFuncId].Name) + ")";
        }
    }
}