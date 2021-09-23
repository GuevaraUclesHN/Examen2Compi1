using System;
using DotNetWeb.Core.Expressions;
using DotNetWeb.Core.Interfaces;
using System.Collections.Generic;
using System.Text;

namespace DotNetWeb.Core.Statement
{
    public class AssignationStatement : Statement
    {
        public AssignationStatement(Id id, TypedExpression expression)
        {
            Id = id;
            Expression = expression;
        }

        public Id Id { get; }
        public TypedExpression Expression { get; }

        public override string Generate(int tabs)
        {
            var code = GetCodeInit(tabs);
            code += $"{Id.Generate()} = {Expression.Generate()}{Environment.NewLine}";
            return code;
        }

        public override void Interpret()
        {
            EnvironmentManager.UpdateVariable(Id.Token.Lexeme, Expression.Evaluate());
        }

        public override void ValidateSemantic()
        {
            if (Id.GetExpressionType() != Expression.GetExpressionType())
            {
                throw new ApplicationException($"Type {Id.GetExpressionType()} is not assignable to {Expression.GetExpressionType()}");
            }
        }
    }
}
