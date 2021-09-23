using System;
using System.Collections.Generic;
using System.Text;
using DotNetWeb.Core.Expressions;

namespace DotNetWeb.Core.Statement
{
    public class InnerStatement : Statement
    {
        public InnerStatement(TypedExpression expression)
        {
            Expression = expression;
        }

        public TypedExpression Expression { get; }

        public override string Generate(int tabs)
        {
            var code = GetCodeInit(tabs);
            code += $"{Expression.Evaluate()}";
            return code;
        }

        public override void Interpret()
        {

        }

        public override void ValidateSemantic()
        {

        }
    }
}