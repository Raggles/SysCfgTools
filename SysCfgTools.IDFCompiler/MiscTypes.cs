using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysCfgTools.IdfCompiler
{
    public enum Associativity
    {
        Left,
        Right
    }

    public enum IdfTokenType
    {
        LeftParen,
        RightParen,
        Literal,
        Function,
        Operator,
        ParamSeperator,
    }

    [Serializable]
    public class IdfParseException : Exception
    {
        public IdfParseException(string message) : base(message) { }
    }

    public static class ExtensionMethods
    {
        public static bool IsIdfSpecialChar(this char ch)
        {
            switch (ch)
            {
                case '+':
                case '-':
                case '/':
                case '*':
                case '(':
                case ')':
                case ',':
                    return true;
                default:
                    return false;
            }
        }
    }
}
