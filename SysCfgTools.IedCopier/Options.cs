using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;

namespace SysCfgTools.IedCopier
{
    class Options
    {
        [Option('i', "iedname", Required=true, HelpText="IED to copy")]
        public string OldName {get;set;}
        [Option('n', "newiedname", Required = true, HelpText = "Name for new IED")]
        public string NewName {get;set;}
        [Option('a', "address", Required = true, HelpText = "DNP Address for new IED")]
        public string NewSlaveAddress { get; set; }
        [Option('f', "findstr", Required = true, HelpText = "Substring to be replaced in tag names (separate multiple entries with comma)")]
        public string FindStr { get; set; }
        [Option('r', "replstr", Required = true, HelpText = "Substring to replace in tag names (separate multiple entries with comma)")]
        public string ReplStr { get; set; }
        [Option('v', "verbose", HelpText = "print additional information to output")]
        public bool Verbose { get; set; }
        [Option('u', "username", Required = true, HelpText = "Database username")]
        public string Username { get; set; }
        [Option('p', "password", Required = true, HelpText = "Database password")]
        public string Password { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            HelpText ht = HelpText.AutoBuild(new Options());
            ht.Copyright = "Copyright 2013";
            ht.Heading = "IED Copier Tool v0.1" + Environment.NewLine + "Tool for cloning IEDs (equipment) in System Configurator (currently DNP only)";

            return ht.ToString();
        }
    }
}
