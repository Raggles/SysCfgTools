using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysCfgTools.IdfCompiler
{
    /// <summary>
    /// The IDF Compiler
    /// </summary>
    public class Compiler
    {
        private string _assignee = "";

        /// <summary>
        /// Compile a IDF string
        /// </summary>
        /// <param name="str">The IDF string</param>
        /// <returns>The Compiled IDF</returns>
        public string Compile(string str)
        {
            //find equals and just send the right side to the tokeniser
            int eqIndex = str.IndexOf('=');
            _assignee = str.Substring(0, eqIndex).Trim();
            //TODO: check no white space inside assignee
            string right = str.Substring(eqIndex+1, str.Length - eqIndex-1);

            IdfTokeniser tk = new IdfTokeniser();
            tk.ParseTokens(right);

            //Check our tokenised equation for validity
            CheckSyntaxRules(tk);
            CheckFunctionRules(tk);

            //convert to RPN and return as compile IDF
            return GenerateOutput(ShuntingYard(tk));
        }

        /// <summary>
        /// Given a queue of tokens in Reverse Polish Notation generates a compiled IDF string
        /// </summary>
        /// <param name="list">Queue of IdfTokens</param>
        /// <returns>Compiled IDF</returns>
        private string GenerateOutput(Queue<IdfToken> list)
        {
            string output = "";
            while (list.Count > 0)
            {
                IdfToken tk = list.Dequeue();
                switch (tk.Type)
                {
                    case IdfTokenType.Function:
                        output += tk.Function.GenerateOutput(_assignee) + Environment.NewLine;
                        break;
                    case IdfTokenType.Literal:
                        output += "0, \"" + tk.Literal + "\"" + Environment.NewLine;
                        break;
                    case IdfTokenType.Operator:
                        output += tk.Operator.Compiled + Environment.NewLine;
                        break;
                    default:
                        throw new IdfParseException("Invalid token type");
                }
            }

            output += "1, \"" + _assignee + "\"";
            return output;
        }

        /// <summary>
        /// Convert infix to Reverse Polish Notation
        /// </summary>
        /// <param name="tks">IdfTokeniser that has parsed its tokens already</param>
        /// <returns></returns>
        private Queue<IdfToken> ShuntingYard(IdfTokeniser tks)
        {
            Stack<IdfToken> stack = new Stack<IdfToken>();
            Queue<IdfToken> output = new Queue<IdfToken>();

            IdfToken tk = null;
            //reset the tokeniser to the start of its list
            tks.Reset();
            //see wikipeia Shunting Yard Article for details
            while (tks.PeekToken() != null)
            {
                tk = tks.NextToken();
                switch (tk.Type)
                {
                    case IdfTokenType.Literal:
                        output.Enqueue(tk);
                        break;
                    case IdfTokenType.Function:
                        stack.Push(tk);
                        break;
                    case IdfTokenType.ParamSeperator:
                        while (stack.Peek().Type != IdfTokenType.LeftParen)
                        {
                            output.Enqueue(stack.Pop());
                            if (stack.Count == 0)
                            {
                                throw new IdfParseException("Mismatched parens or bad param separator");
                            }
                        }
                        break;
                    case IdfTokenType.Operator:
                        if (stack.Count != 0)
                        {
                            while (stack.Peek().Type == IdfTokenType.Operator)
                            {
                                if ((tk.Operator.Precedence < stack.Peek().Operator.Precedence) || (tk.Operator.Associativity == Associativity.Left && tk.Operator.Precedence <= stack.Peek().Operator.Precedence))
                                {
                                    output.Enqueue(stack.Pop());
                                }
                                else
                                {
                                    break;
                                }
                                if (stack.Count == 0)
                                    break;
                            }
                        }
                        stack.Push(tk);

                        break;
                    case IdfTokenType.LeftParen:
                        stack.Push(tk);
                        break;
                    case IdfTokenType.RightParen:
                        while (stack.Peek().Type != IdfTokenType.LeftParen)
                        {
                            output.Enqueue(stack.Pop());
                            if (stack.Count == 0)
                            {
                                throw new IdfParseException("Mismatched parens");
                            }
                        }
                        stack.Pop();
                        if (stack.Count != 0)
                        {
                            if (stack.Peek().Type == IdfTokenType.Function)
                            {
                                output.Enqueue(stack.Pop());
                            }
                        }
                        break;
                }
                
            }
            while (stack.Count != 0)
            {
                if (stack.Peek().Type == IdfTokenType.LeftParen || stack.Peek().Type == IdfTokenType.RightParen)
                {
                    throw new IdfParseException("Mismatched parens");
                }
                output.Enqueue(stack.Pop());
            }
            return output;
        }

        /// <summary>
        /// Check that all the functions have the right number of parameters
        /// Check that literals dont have function names
        /// </summary>
        /// <param name="tokeniser"></param>
        private void CheckFunctionRules(IdfTokeniser tokeniser)
        {
            //TODO: implementation
        }

        /// <summary>
        /// Check basic syntax rules
        /// </summary>
        /// <param name="tokeniser"></param>
        private void CheckSyntaxRules(IdfTokeniser tokeniser)
        {
            //TODO: implementation
        }

    }
}
