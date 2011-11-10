using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Gibbed.RED.FileFormats.Script;

namespace Gibbed.RED.ScriptDecompiler
{
    public abstract class Expression
    {
    }

    public abstract class Statement
    {
        public abstract void Write(TextWriter output, string indent);
    }

    public class ExpressionStatement: Statement
    {
        private readonly Expression _expression;

        public ExpressionStatement(Expression expression, int offset)
        {
            _expression = expression;
            Offset = offset;
        }

        public override string ToString()
        {
            if (!IsTarget)
            {
                return _expression.ToString();
            }
            return "[" + Offset + "] " + _expression;
        }

        public Expression Expression
        {
            get { return _expression; }
        }


        public int Offset { get; private set; }
        public bool IsTarget { get; set; }

        public override void Write(TextWriter output, string indent)
        {
            if (_expression == null)
            {
                if (!IsTarget) return;
                output.WriteLine(indent + "[" + Offset + "]");
            }
            else
            {
                output.WriteLine(indent + ToString() + ";");
            }
        }
    }

    public class BasicBlockStatement: Statement
    {
        private readonly List<BasicBlockStatement> _predecessors = new List<BasicBlockStatement>();
        private readonly List<BasicBlockStatement> _successors = new List<BasicBlockStatement>();
        protected readonly List<Statement> _statements = new List<Statement>();
        protected int _startOffset = -1;

        public BasicBlockStatement(BasicBlockStatement predecessor)
        {
            if (predecessor != null)
            {
                predecessor.AddSuccessor(this);
            }
        }

        internal void AddSuccessor(BasicBlockStatement statement)
        {
            statement._predecessors.Add(this);
            _successors.Add(statement);
        }

        internal void RemoveSelf()
        {
            foreach (var successor in _successors)
            {
                successor._predecessors.Remove(this);
            }
            foreach (var predecessor in _predecessors)
            {
                predecessor._successors.Remove(this);
            }
        }

        public override void Write(TextWriter output, string indent)
        {
            if (!ControlFlowDone && _predecessors.Count > 0)
            {
                output.WriteLine(indent + "[in " + string.Join(",", _predecessors.Select(s => s.StartOffset)) + "]");
            }
            WriteBody(output, indent);
            if (!ControlFlowDone && _successors.Count > 0)
            {
                output.WriteLine(indent + "[out " + string.Join(",", _successors.Select(s => s.StartOffset)) + "]");
            }
            output.WriteLine();
        }

        protected virtual void WriteBody(TextWriter output, string indent)
        {
            foreach (var statement in _statements)
            {
                if (ControlFlowDone && statement is ExpressionStatement)
                    ((ExpressionStatement) statement).IsTarget = false;
                statement.Write(output, indent);
            }
        }

        public List<BasicBlockStatement> Predecessors
        {
            get { return _predecessors; }
        }

        public List<BasicBlockStatement> Successors
        {
            get { return _successors; }
        }

        public void Add(Statement statement)
        {
            if (statement is ExpressionStatement && _startOffset == -1)
            {
                _startOffset = ((ExpressionStatement) statement).Offset;
            }
            _statements.Add(statement);
        }

        public int GetJumpTarget()
        {
            if (_statements.Count == 0)
            {
                return -1;
            }
            var statement = _statements.Last() as ExpressionStatement;
            if (statement != null)
            {
                var jumpExpression = statement.Expression as JumpExpression;
                if (jumpExpression != null)
                {
                    return (jumpExpression).TargetOffset;
                }
            }
            return -1;
        }

        public int StartOffset
        {
            get { return _startOffset; }
        }

        public string Condition
        {
            get
            {
                var statement = _statements.Last() as ExpressionStatement;
                if (statement == null) return null;
                var jumpExpression = statement.Expression as CondJumpExpression;
                return jumpExpression != null ? jumpExpression.Condition.ToString() : null;
            }
        }

        public bool HasPredecessors(params BasicBlockStatement[] blocks)
        {
            return _predecessors.Count == blocks.Count() && Array.TrueForAll(blocks, block => _predecessors.Contains(block));
        }

        public bool HasSuccessors(params BasicBlockStatement[] blocks)
        {
            return _successors.Count == blocks.Count() && Array.TrueForAll(blocks, block => _successors.Contains(block));
        }

        public bool ControlFlowDone
        {
            get; set;
        }
    }

    public class ControlStatement: BasicBlockStatement
    {
        private readonly string _prefix;

        public ControlStatement(BasicBlockStatement predecessor, string prefix, BasicBlockStatement body) 
            : base(predecessor)
        {
            _prefix = prefix;
            Add(body);
            body.ControlFlowDone = true;
            _startOffset = body.StartOffset;
        }

        protected override void WriteBody(TextWriter output, string indent)
        {
            output.WriteLine(indent + _prefix);
            output.WriteLine(indent + "{");
            foreach (var statement in _statements)
            {
                statement.Write(output, indent + "    ");
            }
            output.WriteLine(indent + "}");

        }
    }

    public class SimpleExpression : Expression
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
        private readonly IInstruction _instruction;

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

    internal class JumpExpression : Expression
    {
        private readonly int _targetOffset;

        public JumpExpression(int targetOffset)
        {
            _targetOffset = targetOffset;
        }

        public override string ToString()
        {
            return "jump " + _targetOffset;
        }

        public int TargetOffset
        {
            get { return _targetOffset; }
        }
    }

    internal class CondJumpExpression : JumpExpression
    {
        private readonly Expression _condition;

        public CondJumpExpression(Expression condition, int targetOffset)
            : base(targetOffset)
        {
            _condition = condition;
        }

        public override string ToString()
        {
            return "unless (" + _condition + ") jump " + TargetOffset;
        }

        public Expression Condition
        {
            get { return _condition; }
        }
    }
}