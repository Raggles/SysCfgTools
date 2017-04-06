using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using System.Data;
using System.Data.SqlClient;
using System.Threading;


namespace SysCfgTools.IedCopier
{
    /// <summary>
    /// TODO: Tidy this disgraceful code
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var options = new Options();
                if (CommandLine.Parser.Default.ParseArguments(args, options))
                {

                    string oldName = options.OldName;
                    string newName = options.NewName;
                    string newSlaveOid = "";
                    string newSlaveAddress = options.NewSlaveAddress;
                    string oldSlaveOid = "";
                    string findStr = options.FindStr;
                    string replStr = options.ReplStr;
                    Dictionary<string, string> findreplace = new Dictionary<string, string>();
                    var find = findStr.Split(',');
                    var replace = replStr.Split(',');
                    if (find.Length != replace.Length)
                    {
                        throw new Exception("Must have equal numbers of find and replace tokens");
                    }
                    for (int i = 0; i < find.Length; i++)
                    {
                        findreplace.Add(find[i], replace[i]);
                    }

                    DataSet ds = new DataSet();
                    DataTable dt = new DataTable();

                    //our database connection string
                    string connectionString = string.Format("Server=localhost;port=5432;User Id={0};Password={1};Database=SYSCFG;", options.Username, options.Password);
                    NpgsqlConnection conn = new NpgsqlConnection(connectionString);
                    PrintVerbose("Connecting to database...", options.Verbose);
                    conn.Open();
                    //find the slave with the name that we want to copy
                    string sql = "SELECT * FROM protocol_slave_dnp WHERE entity_name = '" + oldName + "'";

                    NpgsqlDataAdapter da = new NpgsqlDataAdapter(sql, conn);
                    //get the slave data
                    ds.Reset();
                    da.Fill(ds);
                    dt = ds.Tables[0];
                    //copy the slave
                    PrintVerbose("Creating the new IED...", options.Verbose);
                    DataRow row = dt.NewRow();
                    row.ItemArray = dt.Rows[0].ItemArray.Clone() as object[];
                    oldSlaveOid = row["oid"] as string; //remember the oid, we'll need it later
                    row["oid"] = newSlaveOid = GenerateOid();
                    row["entity_name"] = newName;
                    row["slave_address"] = newSlaveAddress;
                    dt.Rows.Add(row);
                    NpgsqlCommandBuilder comb = new NpgsqlCommandBuilder(da);
                    NpgsqlCommand insCommand = comb.GetInsertCommand(true);
                    da.InsertCommand = insCommand;
                    //insert the slave
                    da.Update(ds);
                    PrintVerbose("Done.", options.Verbose);
                    ////////////////////////////////////////////////////////
                    //now find the protocol address groups and copy them too
                    PrintVerbose("Getting address groups", options.Verbose);
                    sql = "SELECT * FROM protocol_address_grp_dnp WHERE protocol_slave_oid = '" + oldSlaveOid + "'";
                    da = new NpgsqlDataAdapter(sql, conn);
                    ds.Reset();
                    da.Fill(ds);
                    dt = ds.Tables[0];
                    //this is a map of old oids to new ids, so we can copy all the points in the address groups later on
                    Dictionary<string, string> prAddrGrpOids = new Dictionary<string, string>();
                    List<DataRow> rowsToAdd = new List<DataRow>();
                    //get all the address groups and make a copy
                    PrintVerbose("Copying address groups...", options.Verbose);
                    foreach (DataRow r in dt.Rows)
                    {
                        prAddrGrpOids.Add(r["oid"] as string, GenerateOid());
                        row = dt.NewRow();
                        row.ItemArray = r.ItemArray.Clone() as object[];
                        row["oid"] = prAddrGrpOids[r["oid"] as string];
                        row["prot_poll_grp_oid"] = null;
                        row["protocol_slave_oid"] = newSlaveOid;
                        rowsToAdd.Add(row);
                    }
                    //then insert the copies back into the database
                    foreach (DataRow r in rowsToAdd)
                    {
                        dt.Rows.Add(r);
                    }
                    comb = new NpgsqlCommandBuilder(da);
                    insCommand = comb.GetInsertCommand(true);
                    da.InsertCommand = insCommand;
                    da.Update(ds);
                    PrintVerbose("Done.", options.Verbose);
                    ///////////////////////////////////////////////////////////////
                    //get a list of points from protocol_address_slave that match the above protocol address group
                    PrintVerbose("Getting address group points", options.Verbose);
                    sql = "SELECT * FROM protocol_address_dnp WHERE prot_addr_grp_oid IN(" + InifyKeys(prAddrGrpOids) + ")";
                    da = new NpgsqlDataAdapter(sql, conn);
                    ds.Reset();
                    da.Fill(ds);
                    dt = ds.Tables[0];

                    //this is a map of old oids to new ids, so we can copy all the points in the address groups later on
                    Dictionary<string, string> prPointAddrOids = new Dictionary<string, string>();
                    Dictionary<string, string> prPointOids = new Dictionary<string, string>();
                    rowsToAdd = new List<DataRow>();
                    //get all the points we need to copy, and copy the new address group-point combos in advance
                    PrintVerbose("Copying group-point relationships...", options.Verbose);
                    foreach (DataRow r in dt.Rows)
                    {
                        prPointAddrOids.Add(r["oid"] as string, GenerateOid());
                        prPointOids.Add(r["slave_point_oid"] as string, GenerateOid());
                        row = dt.NewRow();
                        row.ItemArray = r.ItemArray.Clone() as object[];
                        row["oid"] = GenerateOid(); //I dont think we care about this oid
                        row["prot_addr_grp_oid"] = prAddrGrpOids[r["prot_addr_grp_oid"] as string];
                        row["slave_point_oid"] = prPointOids[r["slave_point_oid"] as string];
                        rowsToAdd.Add(row);
                    }
                    //add new rows
                    foreach (DataRow r in rowsToAdd)
                    {
                        dt.Rows.Add(r);
                    }
                    comb = new NpgsqlCommandBuilder(da);
                    insCommand = comb.GetInsertCommand(true);
                    da.InsertCommand = insCommand;
                    da.Update(ds);
                    PrintVerbose("Done", options.Verbose);
                    //////////////////////////////////////////////////////////////
                    //copy all the analog input points
                    PrintVerbose("Copying AIs...", options.Verbose);
                    sql = "SELECT * FROM point_analog_input WHERE oid IN(" + InifyKeys(prPointOids) + ")";
                    da = new NpgsqlDataAdapter(sql, conn);
                    ds.Reset();
                    da.Fill(ds);
                    dt = ds.Tables[0];
                    //copy of points to add
                    rowsToAdd = new List<DataRow>();
                    //get all the address groups and make a copy
                    foreach (DataRow r in dt.Rows)
                    {
                        row = dt.NewRow();
                        row.ItemArray = r.ItemArray.Clone() as object[];
                        row["oid"] = prPointOids[r["oid"] as string];
                        //replace tokens in point names
                        foreach (var kvp in findreplace)
                        {
                            row["entity_name"] = (row["entity_name"] as string).Replace(kvp.Key, kvp.Value);
                        }
                        rowsToAdd.Add(row);
                    }
                    //then insert the copies back into the database
                    foreach (DataRow r in rowsToAdd)
                    {
                        dt.Rows.Add(r);
                    }
                    comb = new NpgsqlCommandBuilder(da);
                    insCommand = comb.GetInsertCommand(true);
                    da.InsertCommand = insCommand;
                    da.Update(ds);
                    PrintVerbose("Done", options.Verbose);
                    //////////////////////////////////////////////////////////////
                    //copy all the analog output points
                    PrintVerbose("Copying AOs...", options.Verbose);
                    sql = "SELECT * FROM point_analog_output WHERE oid IN(" + InifyKeys(prPointOids) + ")";
                    da = new NpgsqlDataAdapter(sql, conn);
                    ds.Reset();
                    da.Fill(ds);
                    dt = ds.Tables[0];
                    //copy of points to add
                    rowsToAdd = new List<DataRow>();
                    //get all the address groups and make a copy
                    foreach (DataRow r in dt.Rows)
                    {
                        row = dt.NewRow();
                        row.ItemArray = r.ItemArray.Clone() as object[];
                        row["oid"] = prPointOids[r["oid"] as string];
                        //replace tokens in point names
                        foreach (var kvp in findreplace)
                        {
                           row["entity_name"] = (row["entity_name"] as string).Replace(kvp.Key, kvp.Value);
                        } 
                        rowsToAdd.Add(row);
                    }
                    //then insert the copies back into the database
                    foreach (DataRow r in rowsToAdd)
                    {
                        dt.Rows.Add(r);
                    }
                    comb = new NpgsqlCommandBuilder(da);
                    insCommand = comb.GetInsertCommand(true);
                    da.InsertCommand = insCommand;
                    da.Update(ds);
                    PrintVerbose("Done", options.Verbose);
                    //////////////////////////////////////////////////////////////
                    //copy all the digital input points
                    PrintVerbose("Copying DIs...", options.Verbose);
                    sql = "SELECT * FROM point_digital_input WHERE oid IN(" + InifyKeys(prPointOids) + ")";
                    da = new NpgsqlDataAdapter(sql, conn);
                    ds.Reset();
                    da.Fill(ds);
                    dt = ds.Tables[0];
                    //copy of points to add
                    rowsToAdd = new List<DataRow>();
                    //get all the address groups and make a copy
                    foreach (DataRow r in dt.Rows)
                    {
                        row = dt.NewRow();
                        row.ItemArray = r.ItemArray.Clone() as object[];
                        row["oid"] = prPointOids[r["oid"] as string];
                        //replace tokens in point names
                        foreach (var kvp in findreplace)
                        {
                           row["entity_name"] = (row["entity_name"] as string).Replace(kvp.Key, kvp.Value);
                        } 
                        rowsToAdd.Add(row);
                    }
                    //then insert the copies back into the database
                    foreach (DataRow r in rowsToAdd)
                    {
                        dt.Rows.Add(r);
                    }
                    comb = new NpgsqlCommandBuilder(da);
                    insCommand = comb.GetInsertCommand(true);
                    da.InsertCommand = insCommand;
                    da.Update(ds);
                    PrintVerbose("Done", options.Verbose);

                    //////////////////////////////////////////////////////////////
                    //copy all the digital output points
                    PrintVerbose("Copying DOs...", options.Verbose);
                    sql = "SELECT * FROM point_digital_output WHERE oid IN(" + InifyKeys(prPointOids) + ")";
                    da = new NpgsqlDataAdapter(sql, conn);
                    ds.Reset();
                    da.Fill(ds);
                    dt = ds.Tables[0];
                    //copy of points to add
                    rowsToAdd = new List<DataRow>();
                    //get all the address groups and make a copy
                    foreach (DataRow r in dt.Rows)
                    {
                        row = dt.NewRow();
                        row.ItemArray = r.ItemArray.Clone() as object[];
                        row["oid"] = prPointOids[r["oid"] as string];
                        //replace tokens in point names
                        foreach (var kvp in findreplace)
                        {
                           row["entity_name"] = (row["entity_name"] as string).Replace(kvp.Key, kvp.Value);
                        }
                        rowsToAdd.Add(row);
                    }
                    //then insert the copies back into the database
                    foreach (DataRow r in rowsToAdd)
                    {
                        dt.Rows.Add(r);
                    }
                    comb = new NpgsqlCommandBuilder(da);
                    insCommand = comb.GetInsertCommand(true);
                    da.InsertCommand = insCommand;
                    da.Update(ds);
                    PrintVerbose("Done", options.Verbose);
                    PrintVerbose("Complete.", options.Verbose);
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(ex.ToString());
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        public static string GenerateOid()
        {
            Thread.Sleep(10);
            DateTime now = DateTime.Now;
            return now.Year.ToString() + now.Month.ToString("D2") + now.Day.ToString("D2") + now.Ticks.ToString().Substring(6, 10);
        }

        public static string InifyStrings(List<string> str)
        {
            string ret = "";
            foreach (string s in str)
            {
                ret += "'" + s + "',";
            }
            ret = ret.TrimEnd(',');
            return ret;
        }

        public static string InifyKeys(Dictionary<string, string> dict)
        {
            string ret = "";
            foreach (KeyValuePair<string, string> kvp in dict)
            {
                ret += "'" + kvp.Key + "',";
            }
            ret = ret.TrimEnd(',');
            return ret;
        }

        public static void PrintVerbose(string message, bool isVerbose)
        {
            if (isVerbose)
            {
                Console.WriteLine(message);
            }
        }
    }
}
