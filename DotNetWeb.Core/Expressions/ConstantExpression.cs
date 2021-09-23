using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetWeb.Core.Expressions
{
    public class Constant : TypedExpression
    {
        public Constant(Token token, Type type)
            : base(token, type)
        {
        }

        public override dynamic Evaluate()
        {
            return Token.TokenType switch
            {
                TokenType.IntConstant => Convert.ToInt32(Token.Lexeme),
                TokenType.FloatConstant => float.Parse(Token.Lexeme),
                TokenType.StringConstant => Token.Lexeme,
                TokenType.IntListKeyword => Convert.ToInt32(Token.Lexeme),
                TokenType.FloatListKeyword => float.Parse(Token.Lexeme),
                TokenType.StringListKeyword => Token.Lexeme,
                _ => throw new NotImplementedException()
            };
        }

        public override string Generate()
        {
            return Token.Lexeme;
        }

        public override Type GetExpressionType()
        {
            return type;
        }
    }
}