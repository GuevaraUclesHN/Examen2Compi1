using DotNetWeb.Core;
using DotNetWeb.Core.Interfaces;
using DotNetWeb.Core.Statement;
using DotNetWeb.Core.Expressions;
using System;
using Type = DotNetWeb.Core.Type;

namespace DotNetWeb.Parser
{
    public class Parser : IParser
    {
        private readonly IScanner scanner;
        private Token lookAhead;
        public Parser(IScanner scanner)
        {
            this.scanner = scanner;
            this.Move();
        }
        public Statement Parse()
        {
            return Program();
        }

        private Statement Program()
        {
            return new SequenceStatement(Init(), Template());
        }

        private Statement Template()
        {
            return new SequenceStatement(Tag(), InnerTemplate());
        }

        private Statement InnerTemplate()
        {
            if (this.lookAhead.TokenType == TokenType.LessThan)
            {
                return Template();
            }
            return null;
        }
        private Statement Tag()
        {
            Match(TokenType.LessThan);
            Match(TokenType.Identifier);
            Match(TokenType.GreaterThan);
           var statement = Stmts();
            Match(TokenType.LessThan);
            Match(TokenType.Slash);
            Match(TokenType.Identifier);
            Match(TokenType.GreaterThan);
            return statement;
        }

        private Statement Stmts()
        {
            if (this.lookAhead.TokenType == TokenType.OpenBrace)
            {
                return new SequenceStatement(Stmt(), Stmts());
            }
            return null;
        }

        private Statement Stmt()
        {
            Expression expression;
     
            Match(TokenType.OpenBrace);
            switch (this.lookAhead.TokenType)
            {
                case TokenType.OpenBrace:
                    Match(TokenType.OpenBrace);
                    expression = Eq();
                    Match(TokenType.CloseBrace);
                    Match(TokenType.CloseBrace);
                    return new InnerStatement(expression as TypedExpression);
                    break;
                case TokenType.Percentage:
                   return IfStmt();
                    break;
                case TokenType.Hyphen:
                   return ForeachStatement();
                    break;
                default:
                    throw new ApplicationException("Unrecognized statement");
            }
        }

        private Statement ForeachStatement()
        {
            Match(TokenType.Hyphen);
            Match(TokenType.Percentage);
            Match(TokenType.ForEeachKeyword);

            var token1 = lookAhead;
            Match(TokenType.Identifier);
            var id1 = new Id(token1, Type.FloatList);
            EnvironmentManager.AddVariable(token1.Lexeme, id1);

            Match(TokenType.InKeyword);
            var token2 = lookAhead;
            Match(TokenType.Identifier);
            Match(TokenType.Percentage);
            Match(TokenType.CloseBrace);
            var statement = Template();
            Match(TokenType.OpenBrace);
            Match(TokenType.Percentage);
            Match(TokenType.EndForEachKeyword);
            Match(TokenType.Percentage);
            Match(TokenType.CloseBrace);

            return new ForEachStatement(token1, token2, statement);
        }

        private Statement IfStmt()
        {
            Expression expression;
            Statement statement1;

            Match(TokenType.Percentage);
            Match(TokenType.IfKeyword);
            expression = Eq();
            Match(TokenType.Percentage);
            Match(TokenType.CloseBrace);
            statement1 = Template();
            Match(TokenType.OpenBrace);
            Match(TokenType.Percentage);
            Match(TokenType.EndIfKeyword);
            Match(TokenType.Percentage);
            Match(TokenType.CloseBrace);
            return new IfStatement(expression as TypedExpression, statement1);
        }


        private Expression Eq()
        {
            var expression = Rel();
            while (this.lookAhead.TokenType == TokenType.Equal || this.lookAhead.TokenType == TokenType.NotEqual)
            {
                var token = lookAhead;
                Move();
                expression = new RelationalExpression(token, expression as TypedExpression, Rel() as TypedExpression);
            }
            return expression;
        }

        private Expression Rel()
        {
            var expression = Expr();
            if (this.lookAhead.TokenType == TokenType.LessThan
                || this.lookAhead.TokenType == TokenType.GreaterThan)
            {
                var token = lookAhead;
                Move();
                expression = new RelationalExpression(token, expression as TypedExpression, Expr() as TypedExpression);
            }
            return expression;
        }

        private Expression Expr()
        {
            var expression = Term();
            while (this.lookAhead.TokenType == TokenType.Plus || this.lookAhead.TokenType == TokenType.Hyphen)
            {
                var token = lookAhead;
                Move();
                expression = new ArithmeticOperator(token, expression as TypedExpression, Term() as TypedExpression);
            }
            return expression;
        }

        private Expression Term()
        {
            var expression = Factor();
            while (this.lookAhead.TokenType == TokenType.Asterisk || this.lookAhead.TokenType == TokenType.Slash)
            {
                var token = lookAhead;
                Move();
                expression = new ArithmeticOperator(token, expression as TypedExpression, Factor() as TypedExpression);
            }
            return expression;
        }

        private Expression Factor()
        {
            switch (this.lookAhead.TokenType)
            {
                case TokenType.LeftParens:
                    {
                        Match(TokenType.LeftParens);
                        var expression = Eq();
                        Match(TokenType.RightParens);
                        return expression;
                    }
                    break;
                case TokenType.IntConstant:
                   var constant = new Constant(lookAhead, Type.Int);
                    Match(TokenType.IntConstant);
                    return constant;
                case TokenType.FloatConstant:
                    constant = new Constant(lookAhead, Type.Float);
                    Match(TokenType.FloatConstant);
                    return constant;
                case TokenType.StringConstant:
                    constant = new Constant(lookAhead, Type.String);
                    Match(TokenType.StringConstant);
                    return constant;
                case TokenType.OpenBracket:
                    Match(TokenType.OpenBracket);
                    var arr = ExprList();
                    Match(TokenType.CloseBracket);
                    return arr;
                default:
                    var symbol = EnvironmentManager.GetSymbol(this.lookAhead.Lexeme);
                    Match(TokenType.Identifier);
                    return symbol.Id;
            }
        }

        private Expression ExprList()
        {
            var expression = Eq();
            if (this.lookAhead.TokenType != TokenType.Comma)
            {
                return expression;
            }
            Match(TokenType.Comma);
            return new SequenceExpression(expression, ExprList());
        }

        private Statement Init()
        {
            Match(TokenType.OpenBrace);
            Match(TokenType.Percentage);
            Match(TokenType.InitKeyword);
            var statements = Code();
            Match(TokenType.Percentage);
            Match(TokenType.CloseBrace);
            return statements;
        }

        private Statement Code()
        {
            Decls();
            return Assignations();
        }

        private Statement Assignations()
        {
            if (this.lookAhead.TokenType == TokenType.Identifier)
            {
                var symbol = EnvironmentManager.GetSymbol(this.lookAhead.Lexeme);
                return new SequenceStatement(Assignation(symbol.Id), Assignations());
            }

            return null;
        }

        private Statement Assignation(Id id)
        {
            Match(TokenType.Identifier);
            Match(TokenType.Assignation);
            var expression = Eq();
            Match(TokenType.SemiColon);
            return new AssignationStatement(id, expression as TypedExpression);
        }

        private void Decls()
        {
            Decl();
            InnerDecls();
        }

        private void InnerDecls()
        {
            if (this.LookAheadIsType())
            {
                Decls();
            }
        }

        private void Decl()
        {
            switch (this.lookAhead.TokenType)
            {
                case TokenType.FloatKeyword:
                    Match(TokenType.FloatKeyword);
                    var token = lookAhead;
                    Match(TokenType.Identifier);
                    Match(TokenType.SemiColon);
                    var id = new Id(token, Type.Float);
                    EnvironmentManager.AddVariable(token.Lexeme, id);
                    break;
                case TokenType.StringKeyword:
                    Match(TokenType.StringKeyword);
                    token = lookAhead;
                    Match(TokenType.Identifier);
                    Match(TokenType.SemiColon);
                    id = new Id(token, Type.String);
                    EnvironmentManager.AddVariable(token.Lexeme, id);
                    break;
                case TokenType.IntKeyword:
                    Match(TokenType.IntKeyword);
                    token = lookAhead;
                    Match(TokenType.Identifier);
                    Match(TokenType.SemiColon);
                    id = new Id(token, Type.Int);
                    EnvironmentManager.AddVariable(token.Lexeme, id);
                    break;
                case TokenType.FloatListKeyword:
                    Match(TokenType.FloatListKeyword);
                    token = lookAhead;
                    Match(TokenType.Identifier);
                    Match(TokenType.SemiColon);
                    id = new Id(token, Type.Int);
                    EnvironmentManager.AddVariable(token.Lexeme, id);
                    break;
                case TokenType.IntListKeyword:
                    Match(TokenType.IntListKeyword);
                    token = lookAhead;
                    Match(TokenType.Identifier);
                    Match(TokenType.SemiColon);
                    id = new Id(token, Type.Int);
                    EnvironmentManager.AddVariable(token.Lexeme, id);
                    break;
                case TokenType.StringListKeyword:
                    Match(TokenType.StringListKeyword);
                    token = lookAhead;
                    Match(TokenType.Identifier);
                    Match(TokenType.SemiColon);
                    id = new Id(token, Type.Int);
                    EnvironmentManager.AddVariable(token.Lexeme, id);
                    break;
                default:
                    throw new ApplicationException($"Unsupported type {this.lookAhead.Lexeme}");
            }
        }

        private void Move()
        {
            this.lookAhead = this.scanner.GetNextToken();
        }

        private void Match(TokenType tokenType)
        {
            if (this.lookAhead.TokenType != tokenType)
            {
                throw new ApplicationException($"Syntax error! expected token {tokenType} but found {this.lookAhead.TokenType}. Line: {this.lookAhead.Line}, Column: {this.lookAhead.Column}");
            }
            this.Move();
        }

        private bool LookAheadIsType()
        {
            return this.lookAhead.TokenType == TokenType.IntKeyword ||
                this.lookAhead.TokenType == TokenType.StringKeyword ||
                this.lookAhead.TokenType == TokenType.FloatKeyword ||
                this.lookAhead.TokenType == TokenType.IntListKeyword ||
                this.lookAhead.TokenType == TokenType.FloatListKeyword ||
                this.lookAhead.TokenType == TokenType.StringListKeyword;

        }
    }
}
