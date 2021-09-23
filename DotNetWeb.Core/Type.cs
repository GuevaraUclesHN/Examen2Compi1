using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;


namespace DotNetWeb.Core
{
    public class Type : IEquatable<Type>
    {
        public string Lexeme { get; private set; }

        public TokenType TokenType { get; private set; }
        public Type(string lexeme, TokenType tokenType)
        {
            Lexeme = lexeme;
            TokenType = tokenType;
        }

        public static Type Float => new Type("float", TokenType.FloatConstant);
        public static Type Int => new Type("int", TokenType.IntConstant);
        public static Type String => new Type("string", TokenType.StringConstant);
        public static Type StringList => new Type("List<string>", TokenType.StringListKeyword);
        public static Type FloatList => new Type("List<float>", TokenType.FloatListKeyword);
        public static Type IntList => new Type("List<int>", TokenType.IntListKeyword);
        public static Type Bool => new Type("bool", TokenType.Bool);
        public static Type InKeyword => new Type("in", TokenType.InKeyword);

        public bool Equals(Type other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Lexeme == other.Lexeme && TokenType == other.TokenType;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType())
            {
                return false;
            }
            return Equals((Type)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Lexeme, (int)TokenType);
        }

        public static bool operator ==(Type a, Type b) => a.Equals(b);

        public static bool operator !=(Type a, Type b) => !a.Equals(b);

        public override string ToString()
        {
            return Lexeme;
        }
    }
}
