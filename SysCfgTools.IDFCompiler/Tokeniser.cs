using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysCfgTools.IdfCompiler
{
    /// <summary>
    /// A tokeniser class for IDF calcs
    /// </summary>
    public class IdfTokeniser
    {  
        private List<IdfFunction> _functions = new List<IdfFunction>();
        private List<IdfOperator> _operators = new List<IdfOperator>();
        private List<IdfToken> _tokens = new List<IdfToken>();
        private int _index = 0;

        public IdfTokeniser()
        {
            AddFunctions();
            AddOperators();
        }

        /// <summary>
        /// Parses a string into tokens
        /// </summary>
        /// <param name="str">the string to parse</param>
        public void ParseTokens(string str)
        {
            //clear the tokens
            _tokens.Clear();

            int i = 0;
            string currentToken = "";
            
            while (i < str.Length)
            {
                if (char.IsWhiteSpace(str[i]))
                {
                    //if we have finished a token, add it to the list
                    if (!string.IsNullOrWhiteSpace(currentToken))
                    {
                        _tokens.Add(new IdfToken(currentToken, _operators, _functions));
                        currentToken = "";
                    }
                }
                else if (str[i].IsIdfSpecialChar())
                {
                    //if we terminated a token by arriving at a special token, add it
                    if (!string.IsNullOrWhiteSpace(currentToken))
                    {
                        _tokens.Add(new IdfToken(currentToken, _operators, _functions));
                    }
                    //then add the special token
                    _tokens.Add(new IdfToken(str[i].ToString(), _operators, _functions)); 
                    currentToken = "";
                }
                else
                {
                    //else we are part of a string literal, add it to the current token
                    currentToken += str[i];
                }

                i++;
            }
            //add the final token, if it exists
            if (!string.IsNullOrWhiteSpace(currentToken))
            {
                _tokens.Add(new IdfToken(currentToken, _operators, _functions));
            }
        }

        /// <summary>
        /// Peek at the next token without advancing the index
        /// </summary>
        /// <returns>The next token in the list</returns>
        public IdfToken PeekToken()
        {
            try
            {
                return _tokens[_index];
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Return the next token and advance the index
        /// </summary>
        /// <returns></returns>
        public IdfToken NextToken()
        {
            try
            {
                return _tokens[_index++];
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Reset the index back to the start of the list of tokens
        /// </summary>
        public void Reset()
        {
            _index = 0;
        }
     
        private void AddFunctions()
        {
            _functions.Add(new IdfFunction("acib", 9, "40, \"0\""));
            _functions.Add(new IdfFunction("acob", 5, "36, \"0\""));
            _functions.Add(new IdfFunction("abs", 1, "8, \"0\""));
            _functions.Add(new IdfFunction("analogmax", 2, "59, \"x\"", true));
            _functions.Add(new IdfFunction("analogaverage", 2, "60, \"x\"", true));
            _functions.Add(new IdfFunction("analogmin", 2, "58, \"x\"", true));
            _functions.Add(new IdfFunction("analograteofchange", 2, "61, \"x\"", true));
            _functions.Add(new IdfFunction("andsoe", 3, "49, \"0\""));

            _functions.Add(new IdfFunction("attchflg", 2, "31, \"0\""));
            _functions.Add(new IdfFunction("autocontrolselect", 2, "57, \"0\""));
            _functions.Add(new IdfFunction("autoinputselect", 2, "56, \"0\""));
            _functions.Add(new IdfFunction("cibalarm", 6, "37, \"0\""));
            _functions.Add(new IdfFunction("ciboride", 4, "38, \"0\""));
            _functions.Add(new IdfFunction("clrflg", 3, "33, \"0\""));
            _functions.Add(new IdfFunction("cos", 1, "12, \"0\""));
            _functions.Add(new IdfFunction("ctrlwithfeedback", 5, "53, \"0\""));
            _functions.Add(new IdfFunction("dcib", 8, "39, \"0\""));
            _functions.Add(new IdfFunction("dcibsoe", 7, "54, \"0\""));
            _functions.Add(new IdfFunction("dcob", 5, "35, \"0\""));
            _functions.Add(new IdfFunction("defa", 1, "44, \"0\""));
            _functions.Add(new IdfFunction("defd", 1, "43, \"0\""));
            _functions.Add(new IdfFunction("dicount", 1, "50, \"0\""));
            _functions.Add(new IdfFunction("disoeinhibit", 4, "47, \"0\""));
            _functions.Add(new IdfFunction("eq", 2, "21, \"0\""));
            _functions.Add(new IdfFunction("exp", 1, "9, \"0\""));
            _functions.Add(new IdfFunction("flt2int", 1, "15, \"0\""));
            _functions.Add(new IdfFunction("formcfilter", 2, "55, \"0\""));
            _functions.Add(new IdfFunction("ge", 2, "24, \"0\""));
            _functions.Add(new IdfFunction("gt", 2, "23, \"0\""));
            _functions.Add(new IdfFunction("hialarm", 4, "30, \"0\""));
            _functions.Add(new IdfFunction("iif", 3, "27, \"0\""));
            _functions.Add(new IdfFunction("inputselect", 3, "52, \"0\""));
            _functions.Add(new IdfFunction("int2flt", 1, "14, \"0\""));
            _functions.Add(new IdfFunction("le", 2, "26, \"0\""));
            _functions.Add(new IdfFunction("linscale", 3, "28, \"0\""));

            _functions.Add(new IdfFunction("ln", 1, "10, \"0\""));
            _functions.Add(new IdfFunction("loalarm", 4, "29, \"0\""));
            _functions.Add(new IdfFunction("lt", 2, "25, \"0\""));
            _functions.Add(new IdfFunction("maskedcib", 9, "42, \"0\""));
            _functions.Add(new IdfFunction("mod", 2, "6, \"0\""));
            _functions.Add(new IdfFunction("ne", 2, "22, \"0\""));
            _functions.Add(new IdfFunction("not", 1, "20, \"0\""));
            _functions.Add(new IdfFunction("operate", 3, "34, \"0\""));
            _functions.Add(new IdfFunction("orsoe", 3, "48, \"0\""));
            _functions.Add(new IdfFunction("pscond", 2, "41, \"0\""));
            _functions.Add(new IdfFunction("pow", 2, "7, \"0\""));
            _functions.Add(new IdfFunction("readflg", 1, "46, \"0\""));
            _functions.Add(new IdfFunction("setdicount", 2, "51, \"0\""));
            _functions.Add(new IdfFunction("setflg", 3, "32, \"0\""));
            _functions.Add(new IdfFunction("sin", 1, "13, \"0\""));
            _functions.Add(new IdfFunction("sqrt", 1, "11, \"0\""));
            _functions.Add(new IdfFunction("writeflg", 2, "45, \"0\""));
        }

        private void AddOperators()
        {
            _operators.Add(new IdfOperator("+", Associativity.Left, "2, \"0\"", 4));
            _operators.Add(new IdfOperator("-", Associativity.Left, "3, \"0\"", 4));
            _operators.Add(new IdfOperator("*", Associativity.Left, "4, \"0\"", 5));
            _operators.Add(new IdfOperator("/", Associativity.Left, "5, \"0\"", 5));
            _operators.Add(new IdfOperator("and", Associativity.Left, "18, \"0\"", 3));
            _operators.Add(new IdfOperator("or", Associativity.Left, "17, \"0\"", 1));
            _operators.Add(new IdfOperator("xor", Associativity.Left, "19, \"0\"", 2));
        }
    }


}
