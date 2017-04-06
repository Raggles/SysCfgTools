using System;
using System.Collections;
using System.Collections.Generic;


namespace SysCfgTools.IdfCompiler
{
    /// <summary>
    /// Represents a mapping from a function name to a IDF function code
    /// </summary>
    public class IdfFunction
    {
        /// <summary>
        /// The function name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The number of arguments the function expects
        /// </summary>
        public int ArgCount { get; private set; }

        /// <summary>
        /// The compiled form of the function
        /// </summary>
        private string Compiled { get;  set; }

        /// <summary>
        /// If the compiled version has the output point in the second parameter
        /// Not sure what the purpose of this is but its how it seems to work...
        /// </summary>
        private bool IsWeird {get; set;}
        
        public IdfFunction(string name, int argCount, string compiled, bool weird = false)
        {
            Name = name;
            ArgCount = argCount;
            Compiled = compiled;
            IsWeird = weird;
        }

        /// <summary>
        /// Generates the instruction for this function
        /// </summary>
        /// <param name="outArg">Should supply the name of the point being assigned in the IDF Calc if the funcion IsWeird</param>
        /// <returns>Compiled form of the function</returns>
        public string GenerateOutput(string outArg = "")
        {
            if (IsWeird)
            {
                return Compiled.Replace("x", outArg);
            }
            else
            {
                return Compiled;
            }
        }


    }
}