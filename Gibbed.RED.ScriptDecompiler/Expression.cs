using System;
using System.Collections.Generic;
using System.Linq;
using Gibbed.RED.FileFormats.Script;

namespace Gibbed.RED.ScriptDecompiler
{
    public abstract class Expression
    {
        public virtual bool Complete
        {
            get { return true; }
        }
    }

    public class SimpleExpression: Expression
    {
        private readonly string _text;

        public SimpleExpression(string text)
        {
            _text = text;
        }

        public override string ToString()
        {
            return _text;
        }
    }

    public class UnknownExpression: Expression
    {
        private IInstruction _instruction;

        public UnknownExpression(IInstruction instruction)
        {
            _instruction = instruction;
        }

        public override string ToString()
        {
            return "<unknown statement: " + _instruction;
        }

        public override bool Complete
        {
            get { return false; }
        }
    }

    public class BinaryExpression: Expression
    {
        private readonly Expression _lhs;
        private readonly Expression _rhs;
        private readonly string _infix;

        public BinaryExpression(int target, string infix, Expression lhs, Expression rhs)
        {
            _infix = infix;
            _lhs = lhs;
            _rhs = rhs;
        }

        public override bool Complete
        {
            get { return _lhs.Complete && _rhs != null && _rhs.Complete; }
        }

        public override string ToString()
        {
            return _lhs + _infix + (_rhs != null ? _rhs.ToString() : "?");
        }
    }

    public class UnaryExpression: Expression
    {
        private readonly Expression _operand;
        private readonly string _prefix;
        private readonly string _suffix;

        public UnaryExpression(int target, Expression operand, string prefix, string suffix)
        {
            _operand = operand;
            _prefix = prefix;
            _suffix = suffix;
        }

        public override bool Complete
        {
            get { return _operand.Complete; }
        }

        public override string ToString()
        {
            return _prefix + _operand + _suffix;
        }
    }

    internal class CallStatement : Expression
    {
        private readonly string _functionName;
        private readonly List<Expression> _args;

        public CallStatement(string functionName, List<Expression> args)
        {
            _functionName = functionName;
            _args = args;
        }

        public override bool Complete
        {
            get { return !_args.Any(arg => !arg.Complete); }
        }

        public override string ToString()
        {
            return _functionName + "(" + string.Join(", ", _args) + ")";
        }
    }

}