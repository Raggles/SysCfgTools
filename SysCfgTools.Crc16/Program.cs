using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysCfgTools.Crc16
{
    class Program
    {
        static int Main(string[] args)
        {
            try
            {
                FileStream fs = File.OpenRead(args[0]);
                Crc16 crc = new Crc16();
                byte[] bytes = new byte[fs.Length];
                fs.Read(bytes, 0, bytes.Length);
                ushort res = crc.ComputeChecksum(bytes);
                Console.WriteLine(res);
                //Console.ReadKey();
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return 1;
            }
        }
    }
}
