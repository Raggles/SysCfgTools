using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Core;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace SysCfgTools.UtilLib
{
    public class Utils
    {
        private static string _replacementText = @"--
-- PostgreSQL database dump
-- Deversioned By HC's SysCfgBackupDeversioner Tool v0.1
--

--
-- Name: SCHEMA public; Type: COMMENT; Schema: -; Owner: -
--

COMMENT ON SCHEMA public IS 'Standard public schema';


SET search_path = public, pg_catalog;

--
-- Name: plpgsql_call_handler(); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION plpgsql_call_handler() RETURNS language_handler
    AS '$libdir/plpgsql', 'plpgsql_call_handler'
    LANGUAGE c;

--
-- Name: plpgsql; Type: PROCEDURAL LANGUAGE; Schema: -; Owner: -
--


CREATE TRUSTED PROCEDURAL LANGUAGE plpgsql HANDLER plpgsql_call_handler;

";
        public static event Action BackupCompleted;
        public static event Action RestoreComplete;
        public static string StandardOutput
        {
            get { return _standardOutput; }
            set { _standardOutput = value; }
        }

        private static string _standardOutput = "";
        //this is the path to the file we want to backup/restore from/to
        private static string _fileName = null;
        //handle to the bash process doing the work
        private static Process _p = null;
        //this is the path to the temporary file
        private static string _tmpFileName = (System.IO.Path.GetTempPath() + "SysCfgTempFile").Replace("\\", "/");
        //deversion the file after backup
        private static bool _deversionBackup = false;

        public static void DeversionBackupFile(string inputFile, string outputFile, bool verbose)
        {
            WriteLineToConsoleIfVerbose("Reading input file (" + inputFile + ")...", verbose);
            ZipFile zip = new ZipFile(inputFile);
            WriteLineToConsoleIfVerbose("Finding dump script...", verbose);
            ZipEntry entry = zip.GetEntry("temp\\users_pgsql_data.dat");
            byte[] bytes;

            WriteLineToConsoleIfVerbose("Reading data...", verbose);
            using (var s = zip.GetInputStream(entry))
            {
                bytes = new byte[entry.Size];
                StreamUtils.ReadFully(s, bytes);
            }
            string file = Encoding.ASCII.GetString(bytes);

            int startIndex = 0;

            WriteLineToConsoleIfVerbose("Replacing config info...", verbose);
            //remove first 40 lines
            for (int i = 0; i < 38; i++)
            {
                startIndex = file.IndexOf("\n", startIndex + 1, file.Length - startIndex - 1);
            }
            file = file.Remove(0, startIndex);
            file = file.Insert(0, _replacementText);
            startIndex = 0;

            WriteLineToConsoleIfVerbose("Appending 'WITHOUT OIDS' to CREATE TABLE statements...", verbose);
            while (true)
            {

                int indexOf = file.IndexOf("CREATE TABLE", startIndex, file.Length - startIndex);

                if (indexOf < 0)
                {
                    break;
                }

                startIndex = indexOf;
                int nextIndexOf = file.IndexOf(";", startIndex, file.Length - startIndex);
                file = file.Insert(nextIndexOf, " WITHOUT OIDS");
                startIndex = nextIndexOf;

            }

            WriteLineToConsoleIfVerbose("Creating output file (" + outputFile + ")...", verbose);
            byte[] newBytes = Encoding.ASCII.GetBytes(file);
            MemoryStream ms = new MemoryStream(newBytes);
            FileStream fs = new FileStream(outputFile, FileMode.Create);
            ZipOutputStream zipStream = new ZipOutputStream(fs);
            zipStream.UseZip64 = UseZip64.Off;
            zipStream.SetLevel(3); //0-9, 9 being the highest level of compression
            //ZipEntry temp = new ZipEntry("temp/");
            //zipStream.PutNextEntry(temp);
            ZipEntry newEntry = new ZipEntry("temp/users_pgsql_data.dat");
            newEntry.DateTime = DateTime.Now;
            zipStream.PutNextEntry(newEntry);

            WriteLineToConsoleIfVerbose("Copying data to output stream...", verbose);
            StreamUtils.Copy(ms, zipStream, new byte[4096]);
            zipStream.CloseEntry();

            zipStream.IsStreamOwner = false;	// False stops the Close also Closing the underlying stream.
            zipStream.Close();			// Must finish the ZipOutputStream before using outputMemStream.
            fs.Position = 0;
            fs.Close();
            zip.Close();
            WriteLineToConsoleIfVerbose("Done!", verbose);
        }

        private static void WriteLineToConsoleIfVerbose(string message, bool verbose)
        {
            if (verbose)
            {
                Console.WriteLine(message);
            }
        }

        public static void BackupFile(string bashPath, string scriptPath, string file, bool deversion = false)
        {
            _deversionBackup = deversion;
            //change to script directory
            Directory.SetCurrentDirectory(Directory.GetParent(scriptPath).FullName);

            string backupScript = System.IO.Path.GetFileName(scriptPath);

            _fileName = file;
            //cygwin doesnt like windows pathnames
            _fileName = _fileName.Replace("\\", "/");

            //create a new bash process
            //command is of form bash.exe -c sh database_maintenance.sh --backup|restore <file>
            ProcessStartInfo pi = new ProcessStartInfo();
            pi.FileName = bashPath;
            //as a literal string this should look like: bash -c "sh database_maintenance.sh --backup \"filename\" "
            pi.Arguments = "-c \"./" + backupScript + " --backup \\\"" + _tmpFileName + "\\\" \"";
            pi.UseShellExecute = false;
            pi.CreateNoWindow = true;
            pi.RedirectStandardOutput = true;

            _p = new Process();
            _p.EnableRaisingEvents = true;
            _p.StartInfo = pi;
            _p.Exited += new EventHandler(p_Exited_Backup);
            //lets not try do two things at once
            _p.Start();
            GetOutput(_p);
        }

        private static void GetOutput(Process _p)
        {
            StandardOutput = "";
            Thread t = new Thread((ThreadStart)delegate
            {
                string str;
                while ((str = _p.StandardOutput.ReadLine()) != null)
                {
                    StandardOutput += str + Environment.NewLine;
                }
            });
            t.Start();
        }

        public static void RestoreFile(string bashPath, string scriptPath, string file)
        {
            //change to script directory
            Directory.SetCurrentDirectory(Directory.GetParent(scriptPath).FullName);

            string backupScript = System.IO.Path.GetFileName(scriptPath);

            _fileName = file;
            _fileName = _fileName.Replace("\\", "/");
            File.Copy(_fileName, _tmpFileName, true);

            ProcessStartInfo pi = new ProcessStartInfo();
            pi.FileName = bashPath;
            pi.Arguments = "-c \"./" + backupScript + " --restore \\\"" + _tmpFileName + "\\\" \"";
            pi.CreateNoWindow = true;
            pi.UseShellExecute = false;
            pi.RedirectStandardOutput = true;

            _p = new Process();
            _p.EnableRaisingEvents = true;
            _p.StartInfo = pi;
            _p.Exited += new EventHandler(p_Exited_Restore);
            _p.Start();
            GetOutput(_p);
        }

        /// <summary>
        /// Called after the backup process has completed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void p_Exited_Backup(object sender, EventArgs e)
        {
            try
            {
                if (_deversionBackup)
                {
                    //deversion the backup
                    DeversionBackupFile(_tmpFileName, _fileName, false);
                }
                else
                {
                    //otherwise just copy it
                    File.Copy(_tmpFileName, _fileName, true);
                }
                //delete the tmp file
                File.Delete(_tmpFileName);
            }
            catch
            {
                
            }
            finally
            {
                if (BackupCompleted != null)
                {
                    BackupCompleted();
                }
            }
        }

        /// <summary>
        /// Triggered after the restore process has completed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void p_Exited_Restore(object sender, EventArgs e)
        {
            try
            {
                //delete the temp file
                File.Delete(_tmpFileName);
            }
            catch
            {
                
            }
            finally
            {
                if (RestoreComplete != null)
                {
                    RestoreComplete();
                }
            }
        }
    }
}
