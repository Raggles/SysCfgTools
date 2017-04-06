using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SysCfgTools.UtilLib;

namespace SysCfgTools.BackupDeversioner
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    Console.Write(@"
SyscfgTools.BackupDeversioner Tool.
Converts backup files created with postgresql version 8.2.11 to a format
compatible with version 7.4.1.

Copyright © Hamish Cumming 2011
Contact hamish.cumming@gmail.com

Usage:

SysCfgTools.BackupDeversioner infile [outfile]

where:
    infile  The file to convert
    outfile An optional output filename.  If not specified the input file
            will be used, with '_out' appended.

Press any key to quit...
");
                    Console.ReadKey();
                    return;
                }
                else
                {
                    Console.Write(@"
SyscfgTools.BackupDeversioner Tool.
Copyright © Hamish Cumming 2011
");
                }
                
                string inFile = args[0];
                string outFile;
                if (args.Length < 2)
                {
                    outFile = args[0] + "_out";
                }
                else
                {
                    outFile = args[1];
                }

                Utils.DeversionBackupFile(inFile, outFile, true);

                Console.WriteLine("Press any key to quit...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Crap!  Something went wrong:");
                Console.WriteLine();
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(ex.ToString());
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("Press any key to quit...");
                Console.ReadKey();
            }
        }

        
    }


}
