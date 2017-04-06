using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysCfgTools.PortSwitcher
{
    class Program
    {
        enum Operation
        {
            MasterGroup,
            SlaveGroup,
            Master,
            Slave
        };

        static void Main(string[] args)
        {
            try
            {
                var options = new Options();
                if (CommandLine.Parser.Default.ParseArguments(args, options))
                {

                    Console.WriteLine("Port switcher tool v0.1");
                    Console.WriteLine("Choose from the following:");
                    Console.WriteLine("    1. Move a Master Protocol Group to another comms port");
                    Console.WriteLine("    2. Move a Slave Protocol Group to another comms port");
                    Console.WriteLine("    3. Move a Master to another Protocol Group");
                    Console.WriteLine("    4. Move a Salve to another Protocol Group");

                    ConsoleKeyInfo? key = null;
                    int choice;
                    do
                    {
                        if (key != null) Console.WriteLine("Please select a valid choice");
                        key = Console.ReadKey();
                        Console.WriteLine();
                        choice = int.Parse(key.Value.KeyChar.ToString());
                    } while (choice >= 4 || choice < 0);

                    switch (choice)
                    {
                        case 1:
                            MoveMasterGroup(options);
                            break;
                        case 2:
                            MoveSlaveGroup(options);
                            break;
                        case 3:
                            MoveMaster(options);
                            break;
                        case 4:
                            MoveSlave(options);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(ex.ToString());
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
        private static void MoveMasterGroup(Options options)
        {

            MoveAtoB(options, Operation.MasterGroup);
        }

        private static void MoveSlaveGroup(Options options)
        {
            MoveAtoB(options, Operation.SlaveGroup);
        }

        private static void MoveMaster(Options options)
        {
            MoveAtoB(options, Operation.Master);
        }

        private static void MoveSlave(Options options)
        {
            MoveAtoB(options, Operation.Slave);
        }
        
        private static void MoveAtoB(Options options, Operation op)
        {
            //these strings are used in sql queries (table names) and console prompts, and change depending on the type of operation we are doing
            string string1, string2, string3, string4, string5;
            
            switch (op)
            {
                case Operation.Master:
                    string1 = "Master";
                    string2 = "protocol_master";
                    string3 = "master group";
                    string4 = "protocol_master_grp";
                    string5 = "protocol_grp_oid";
                    break;
                default:
                case Operation.MasterGroup:
                    string1 = "Master Group";
                    string2 = "protocol_master_grp";
                    string3 = "serial port";
                    string4 = "comms_port";
                    string5 = "comms_port_oid";
                    break;
                case Operation.Slave:
                    string1 = "Slave";
                    string2 = "protocol_slave";
                    string3 = "slave group";
                    string4 = "protocol_slave_grp";
                    string5 = "protocol_grp_oid";
                    break;
                case Operation.SlaveGroup:
                    string1 = "Slave Group";
                    string2 = "protocol_slave_grp";
                    string3 = "serial port";
                    string4 = "comms_port";
                    string5 = "comms_port_oid";
                    break;
            }

            Console.WriteLine(string.Format("Choose the {0} to move:", string1));
            
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();

            //database connection string
            string connectionString = string.Format("Server=localhost;port=5432;User Id={0};Password={1};Database=SYSCFG;", options.Username, options.Password);
            NpgsqlConnection conn = new NpgsqlConnection(connectionString);
            conn.Open();
            string sql = string.Format("SELECT * FROM {0}", string2);
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(sql, conn);
            ds.Reset();
            da.Fill(ds);
            dt = ds.Tables[0];
            Console.WriteLine(String.Format("|{0,-5}|{1,-30}|{2,-30}|{3,-100}|", "index", "object_type", "entity_name", "description"));
            Console.WriteLine(String.Format("|{0,-5}|{1,-30}|{2,-30}|{3,-100}|", "-----", "-----------", "-----------", "-----------"));
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Console.WriteLine(String.Format("|{0,-5}|{1,-30}|{2,-30}|{3,-100}|", i, dt.Rows[i]["object_type"], dt.Rows[i]["entity_name"], dt.Rows[i]["description"]));
            }
            string input = null;
            int groupIndex;
            //loop until we get some valid input
            do
            {
                if (input != null) Console.WriteLine("Please select a valid index");
                input = Console.ReadLine();
                Console.WriteLine();
                groupIndex = int.Parse(input);

            } while (groupIndex >= dt.Rows.Count || groupIndex < 0);
            string grp_oid = dt.Rows[groupIndex]["oid"] as string;

            Console.WriteLine(string.Format("Choose the new {0}:", string3));
            sql = string.Format("SELECT * FROM {0}", string4);
            da = new NpgsqlDataAdapter(sql, conn);
            ds.Reset();
            da.Fill(ds);
            dt = ds.Tables[0];
            Console.WriteLine(String.Format("|{0,-5}|{1,-30}|{2,-30}|{3,-100}|", "index", "object_type", "entity_name", "description"));
            Console.WriteLine(String.Format("|{0,-5}|{1,-30}|{2,-30}|{3,-100}|", "-----", "-----------", "-----------", "-----------"));
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Console.WriteLine(String.Format("|{0,-5}|{1,-30}|{2,-30}|{3,-100}|", i, dt.Rows[i]["object_type"], dt.Rows[i]["entity_name"], dt.Rows[i]["description"]));
            }
            input = null;
            int portIndex;
            //loop until we get some valid input
            do
            {
                if (input != null) Console.WriteLine("Please select a valid index");
                input = Console.ReadLine();
                Console.WriteLine();
                portIndex = int.Parse(input);

            } while (portIndex >= dt.Rows.Count || portIndex < 0);
            //the new oid of the thing we are moving
            string port_oid = dt.Rows[portIndex]["oid"] as string;

            NpgsqlCommand cmd = new NpgsqlCommand(string.Format("update {0} SET {1}='" + port_oid + "' WHERE oid ='" + grp_oid + "'", string2, string5), conn);
            cmd.ExecuteNonQuery();
            Console.WriteLine("Done.");
        }
    }
}
