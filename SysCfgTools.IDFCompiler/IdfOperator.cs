using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysCfgTools.IdfCompiler
{
    public class IdfOperator
    {
        /// <summary>
        /// A string representation of the operator
        /// </summary>
        public string Op { get; private set; }
        
        /// <summary>
        /// The associativity of the operator
        /// </summary>
        public Associativity Associativity { get; private set; }
        
        /// <summary>
        /// The compiled form of the operator
        /// </summary>
        public string Compiled { get; private set; }
        
        /// <summary>
        /// The higher the precedence the earlier the operation is performed, ie * is higher than +
        /// </summary>
        public int Precedence { get; private set; }

        public IdfOperator(string op, Associativity assoc, string compiled, int precedence)
        {
            Op = op;
            Associativity = assoc;
            Compiled = compiled;
            Precedence = precedence;
        }
    }

}
