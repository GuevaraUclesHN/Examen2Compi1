using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetWeb.Core.Expressions
{
    public class SequenceExpression : Expression
    {
        public Expression Expression1 { get; }
        public Expression Expression2 { get; }
        public SequenceExpression(Expression expression1, Expression expression2) : base(null, null)
        {
            this.Expression1 = expression1;
            this.Expression2 = expression2;
        }

        public override string Generate()
        {
            return $"{Expression1?.Generate() + Expression1?.Generate()}";
        }
    }
}