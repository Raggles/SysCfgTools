using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;

namespace SysCfgTools.IdfCompiler
{
    class Options
    {
        [ValueOption(0)]
        public string InFile { get; set; }
        [ValueOption(1)]
        public string OutFile { get; set; }
        [ValueOption(2)]
        public string ErrorFile { get; set; }
        
        [HelpOption]
        public string GetUsage()
        {
           string str= 
@"Open Source IDF Compiler v0.1

Usage: idfc infile outfile errfile";
            
            return str;
        }
    }
}
