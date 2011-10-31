using System.IO;
using Gibbed.Helpers;

namespace Gibbed.RED.FileFormats.Script.Instructions
{
    public class FinalFunc: IInstruction
    {
        private readonly CompiledScriptsFile _scripts;
        private ushort _opFlags;
        private ushort _opTarget;

        public FinalFunc(CompiledScriptsFile scripts)
        {
            _scripts = scripts;
        }

        public Opcode Opcode
        {
            get { return Opcode.OP_FinalFunc; }
        }

        public int Deserialize(Stream input)
        {
            _opFlags = input.ReadValueU16();
            _opTarget = input.ReadValueU16();
            OpFuncId = input.ReadValueEncodedS32();
            if (OpFuncId == -1)
            {
                OpOperator = (OperatorCode) input.ReadValueEncodedS32();
            }
            return 8;
        }

        public void Serialize(ICodeWriter output)
        {
            throw new System.NotImplementedException();
        }

        public override string ToString()
        {
            if (OpFuncId == -1)
            {
                var opName = OpOperator.ToString();
                int value;
                if (int.TryParse(opName, out value))
                {
                    return "FinalFunc-UnknownOperator(" + _opFlags + "," + _opTarget + "," + opName + ")";
                }
                return "FinalFunc(" + _opFlags + "," + _opTarget + "," + opName + ")";
            }
            return string.Format("FinalFunc({0},{1},{2})", _opFlags, _opTarget, FunctionName);
        }

        public string FunctionName
        {
            get
            {
                var funcDef = _scripts.FuncDefs[OpFuncId];
                if (funcDef.ContainingClass != null)
                {
                    return funcDef.ContainingClass.Name + "::" + funcDef.Name;
                }
                return funcDef.Name;
            }
        }

        public int OpFuncId { get; private set; }
        public OperatorCode OpOperator { get; private set; }
    }
}