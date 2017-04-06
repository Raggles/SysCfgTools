using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysCfgTools.IdfCompiler
{
    /// <summary>
    /// Represents a token in an IDF Calc
    /// </summary>
    public class IdfToken
    {
        public IdfTokenType Type { get; private set; }
        public IdfFunction Function { get; private set; }
        public IdfOperator Operator { get; private set; }
        public string Literal { get; private set; }

        /// <summary>
        /// Create an IdfToken
        /// </summary>
        /// <param name="str">The string to create the token from</param>
        /// <param name="operators">A list of valid operators</param>
        /// <param name="functions">A list of valid functions</param>
        public IdfToken(string str, List<IdfOperator> operators, List<IdfFunction> functions)
        {
            str = Literal = str.Trim();
            Function = null;
            Operator = null;

            //work out what type of token we are:
            //are we a special token?
            if (str.Length == 1)
            {
                switch (str[0])
                {
                    case '(':
                        Type = IdfTokenType.LeftParen;
                        return;
                    case ')':
                        Type = IdfTokenType.RightParen;
                        return;
                    case ',':
                        Type = IdfTokenType.ParamSeperator;
                        return;

                }
            }
            //are we an operator?
            foreach (IdfOperator op in operators)
            {
                if (op.Op.ToLower() == str.ToLower())
                {
                    Type = IdfTokenType.Operator;
                    Operator = op;
                    return;
                }
            }
            //are we a function?
            foreach (IdfFunction fn in functions)
            {
                if (fn.Name.ToLower() == str.ToLower())
                {
                    Type = IdfTokenType.Function;
                    Function = fn;
                    return;
                }
            }
            //none of the above so we must be a literal
            Type = IdfTokenType.Literal;
        }
    }
}
