using System;
using System.Collections.Generic;
using System.Linq;
using Gibbed.RED.FileFormats.Script;

namespace Gibbed.RED.ScriptDecompiler
{
    public abstract class Expression
    {
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
    }

    public class BinaryExpression: Expression
    {
        private readonly Expression _lhs;
        private readonly Expression _rhs;
        private readonly string _infix;
        private readonly string _suffix;

        public BinaryExpression(string infix, string suffix, Expression lhs, Expression rhs)
        {
            _infix = infix;
            _suffix = suffix;
            _lhs = lhs;
            _rhs = rhs;
        }

        public override string ToString()
        {
            return _lhs + _infix + (_rhs != null ? _rhs.ToString() : "?") + _suffix;
        }
    }

    public class UnaryExpression: Expression
    {
        private readonly Expression _operand;
        private readonly string _prefix;
        private readonly string _suffix;

        public UnaryExpression(Expression operand, string prefix, string suffix)
        {
            _operand = operand;
            _prefix = prefix;
            _suffix = suffix;
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

        public override string ToString()
        {
            return _functionName + "(" + string.Join(", ", _args) + ")";
        }
    }

    internal class CondJumpStatement: Expression
    {
        private readonly Expression _condition;
        private readonly int _targetOffset;

        public CondJumpStatement(Expression condition, int targetOffset)
        {
            _targetOffset = targetOffset;
            _condition = condition;
        }

        public override string ToString()
        {
            return "unless (" + _condition + ") jump " + _targetOffset;
        }
    }

    internal class JumpStatement: Expression
    {
        private readonly int _targetOffset;

        public JumpStatement(int targetOffset)
        {
            _targetOffset = targetOffset;
        }

        public override string ToString()
        {
            return "jump " + _targetOffset;
        }
    }

}