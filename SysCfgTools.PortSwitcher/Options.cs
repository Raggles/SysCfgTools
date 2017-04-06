using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysCfgTools.PortSwitcher
{
    class Options
    {
        [Option('u', "username", Required = true, HelpText = "Database username")]
        public string Username { get; set; }
        [Option('p', "password", Required = true, HelpText = "Database password")]
        public string Password { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            HelpText ht = HelpText.AutoBuild(new Options());
            ht.Copyright = "Copyright 2013";
            ht.Heading = "IED Copier Tool v1.0" + Environment.NewLine + "Tool for cloning IEDs (equipment) in System Configurator (currently DNP only)";

            return ht.ToString();
        }
    }
}
