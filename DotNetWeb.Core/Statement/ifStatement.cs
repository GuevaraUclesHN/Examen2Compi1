using System;
using DotNetWeb.Core.Expressions;
using System.Collections.Generic;
using System.Text;

namespace DotNetWeb.Core.Statement
{
    public class IfStatement : Statement
    {
        public IfStatement(TypedExpression expression, Statement statement)
        {
            Expression = expression;
            Statement = statement;
        }

        public TypedExpression Expression { get; }
        public Statement Statement { get; }

        public override string Generate(int tabs)
        {
            var code = GetCodeInit(tabs);
            code += $"< IF {Expression.Generate()}> <{Environment.NewLine}";
            code += $"< THEN {Statement.Generate(tabs + 1)}/THEN>{Environment.NewLine}";
            return code;
        }

        public override void Interpret()
        {
            if (Expression.Evaluate())
            {
                Statement.Interpret();
            }
        }

        public override void ValidateSemantic()
        {
            if (Expression.GetExpressionType() != Type.Bool)
            {
                throw new ApplicationException("A boolean is required in ifs");
            }
        }
    }
}