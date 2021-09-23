using System;
using System.Collections.Generic;
using System.Text;
using DotNetWeb.Core.Expressions;
namespace DotNetWeb.Core.Statement
{
    class ExpressionStatement: Statement
    {
        public Expression Expression { get; }

        public ExpressionStatement(Expression expression)
        {
            Expression = expression;
        }
        public override void ValidateSemantic()
        {
            //throw new System.NotImplementedException();
        }


        public override string Generate(int tabs)
        {
            throw new NotImplementedException();
        }

        public override void Interpret()
        {
            throw new NotImplementedException();
        }
    }
}