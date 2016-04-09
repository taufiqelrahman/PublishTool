using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System.Configuration;
using System.Windows.Forms;

namespace GenerateScripts
{
    class BusinessFacade
    {
        public void GenerateStoredProc(string fullpath,Server cons)
        {
           FileInfo file = new FileInfo(fullpath);
           string script = file.OpenText().ReadToEnd();
           cons.ConnectionContext.ExecuteNonQuery(script);
        }

        public List<string> GetListSortFullPathFiles(string[] pathFiles)
        {
            List<string> lstFullPathFiles = new List<string>();
            for (int i = 0; i < pathFiles.Length; i++)
            {
                lstFullPathFiles.Add(pathFiles[i]);
            }
            
           lstFullPathFiles.Sort();
            return lstFullPathFiles;
        }

        public List<string> GetListSortFullPathFiles(string[] pathFiles,string DB)
        {
            List<string> lstFullPathFiles = new List<string>();
           
            for (int i = 0; i < pathFiles.Length; i++)
            {
                string dirFile = Path.GetDirectoryName(pathFiles[i]);
                if (dirFile.ToLower().Contains(DB.ToLower()) && !dirFile.ToLower().Contains("tables"))
                    lstFullPathFiles.Add(pathFiles[i]);
                
            }

            lstFullPathFiles.Sort();
            return lstFullPathFiles;
        }

        public List<string> GetOthListSortFullPathFiles(string[] pathFiles, string DB)
        {
            List<string> lstFullPathFiles = new List<string>();

            for (int i = 0; i < pathFiles.Length; i++)
            {
                string dirFile = Path.GetDirectoryName(pathFiles[i]);
                //if (dirFile.ToLower().Contains(DB.ToLower()) && !dirFile.ToLower().Contains("tables"))
                    lstFullPathFiles.Add(pathFiles[i]);

            }

            lstFullPathFiles.Sort();
            return lstFullPathFiles;
        }

        public List<string> GetTblListSortFullPathFiles(string[] pathFiles, string DB)
        {
            List<string> lstFullPathFiles = new List<string>();

            for (int i = 0; i < pathFiles.Length; i++)
            {
                string dirFile = Path.GetDirectoryName(pathFiles[i]);
                if (dirFile.ToLower().Contains(DB.ToLower()) && dirFile.ToLower().Contains("tables"))
                    lstFullPathFiles.Add(pathFiles[i]);

            }

            lstFullPathFiles.Sort();
            return lstFullPathFiles;
        }

        public string[] GetListSortFullPathFilesConverted(string[] pathFiles, string DB)
        {

            List<string> lstFullPathFiles = new List<string>();
            string[] newpath;

            for (int i = 0; i < pathFiles.Length; i++)
            {
                string dirFile = Path.GetDirectoryName(pathFiles[i]);
                if (dirFile.ToLower().Contains(DB.ToLower()))
                    lstFullPathFiles.Add(pathFiles[i]);

            }

            lstFullPathFiles.Sort();
            newpath = lstFullPathFiles.ToArray();

            return newpath;
        }

        public void InsertAppLog(string appLog, string DB)
        {
            StreamWriter log;
            string pathlog = ConfigurationManager.AppSettings["History"];
            if (!File.Exists(pathlog))
            {
                log = new StreamWriter(pathlog);
            }
            else
            {
                log = File.AppendText(pathlog);
            }
            log.Write(DB);
            log.Write("|");
            log.Write(DateTime.Now);
            log.Write("|");
            log.Write(appLog);
            log.WriteLine();
            log.Close();
        }

        public void InsertAppLog(string appLog, string DB, string erro, CheckBox grantCheckBox)
        {
            StreamWriter log;
            string pathlog;
            if (grantCheckBox.Checked == false)
            {
                pathlog = ConfigurationManager.AppSettings["ErrLog"];
            }
            else
            {
                pathlog = ConfigurationManager.AppSettings["GrantErrLog"];
            }
            if (!File.Exists(pathlog))
            {
                log = new StreamWriter(pathlog);
            }
            else
            {
                log = File.AppendText(pathlog);
            }
            log.Write(DB);
            log.Write("|");
            log.Write(DateTime.Now);
            log.Write("|");
            log.Write(appLog);
            log.Write("|");
            log.Write(erro);
            log.WriteLine();
            log.Close();
        }
    }
}
