using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using System.IO;

namespace SysCfgTools.IdfCompiler
{
    class Program
    {
        static void Main(string[] args)
        {
            Options options = new Options();
            if (args.Length < 3)
            {
                Console.Write(options.GetUsage());
                return;
            }
            if (Parser.Default.ParseArguments(args, options))
            {
                string input = File.ReadAllText(options.InFile);
                Compiler comp = new Compiler();
                try
                {
                    string output = comp.Compile(input);
                    File.WriteAllText(options.OutFile, output);
                    File.WriteAllText(options.ErrorFile, "");
                }
                catch (Exception ex)
                {
                    try
                    {
                        File.WriteAllText(options.ErrorFile, ex.Message);
                    }
                    catch { }
                }

                
            }
        }
    }
}
