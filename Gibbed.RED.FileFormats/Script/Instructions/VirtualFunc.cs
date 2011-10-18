﻿using System.IO;
using Gibbed.Helpers;

namespace Gibbed.RED.FileFormats.Script.Instructions
{
    public class VirtualFunc: IInstruction
    {
        private readonly CompiledScriptsFile _scripts;
        private ushort _opFlags;
        private ushort _opTargetNum;
        private int _opFuncName;

        public VirtualFunc(CompiledScriptsFile scripts)
        {
            _scripts = scripts;
        }

        public int Deserialize(Stream input)
        {
            _opFlags = input.ReadValueU16();
            _opTargetNum = input.ReadValueU16();
            _opFuncName = input.ReadValueEncodedS32();
            return 8;
        }

        public void Serialize(ICodeWriter output)
        {
            throw new System.NotImplementedException();
        }

        public override string ToString()
        {
            return "VirtualFunc(" + _opFlags + "," + _opTargetNum + "," + _scripts.Strings[_opFuncName].Value + ")";
        }
    }
}