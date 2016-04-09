using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Publishing_Tools
{
    class backupOverwrite : Form1
    {
        
        string[] sourceFolders;
        string[] servers;
        string waSource, wsSource, waPSS, waTDMS, waTAM, wsPSS, wsTDMS, wsTAM;
        string source, destination, local;
        string logData;
        double fullProg, i;

        //public App(string[] servers, string backupServer, string publishType, Progress log, string typeStatus)
        public backupOverwrite(string[] servers, string logdata, string typeStatus)
        {
            this.servers = servers;
            this.logData = logdata;
            //this.backupServer = backupServer;
            //this.publishType = publishType;
            //this.log = log;
            this.typeStatus = typeStatus;
        }

        public string getLogData()
        {
            return logData;
        }

        public void setLogData(string logData)
        {
            this.logData = logData;
        }

        public void backupWebApps(string src, string dst, string lcl)
        {
            try
            {
                if (typeStatus == "PSS4W")
                {
                    sourceFolders = new string[] { @"\pss4w", @"\tdms", @"\pss4wservice", @"\tdmsservice" };

                }
                else if (typeStatus == "PSS2W")
                {
                    sourceFolders = new string[] {@"\psshsoho",@"\psshsom29",
                        @"\pss2w", @"\pss2w1", @"\pss2w2", @"\pss2w3", @"\pss2w4", @"\pss2w5",
                        @"\pss2w6", @"\pss2w7", @"\pss2w8", @"\pss2w9", @"\pss2w10", @"\pss2w11"};
                }
                else if (typeStatus == "TAM")
                {
                    sourceFolders = new string[] { @"\TAM", @"\Webservice_TAM" };
                }


                foreach (var server in servers)
                {
                    foreach (var srcFd in sourceFolders)
                    {
                        source = @"\\" + server + src + srcFd;
                        if (typeStatus == "PSS4W")
                        {
                            destination = @"\\" + backupServer + dst + @"\" + server + @"\" + publishType + srcFd;
                        }
                        else if (typeStatus == "PSS2W")
                        {
                            destination = dst + srcFd;
                        }
                        local = lcl;
                        fullProg = Directory.GetFiles(source, "*.*", SearchOption.AllDirectories).Length;
                        i = 0;

                        //bikin folder dulu
                        if (!Directory.Exists(destination))
                        {
                            Directory.CreateDirectory(destination);
                        }

                        //cek direktorinya
                        foreach (string dirPath in Directory.GetDirectories(source, "*", SearchOption.AllDirectories))
                        {
                            //cek di tempat backup ada gak
                            //kalo ga ada di tempat backup
                            if (!Directory.Exists(dirPath.Replace(source, destination)))
                            {
                                //bikin direktori di tempat backup
                                Directory.CreateDirectory(dirPath.Replace(source, destination));
                            }
                            //ambil file dari alamat yg sudah dikonfirm tadi
                            foreach (string newPath in Directory.GetFiles(dirPath))
                            {
                                if (File.Exists(newPath.Replace(source, destination)))
                                {
                                    File.Delete(newPath.Replace(source, destination));
                                }
                                File.Copy(newPath, newPath.Replace(source, destination), true);
                                logData += newPath.Replace(source, destination) + "\n";
                                double a = i++ / (fullProg - 1);
                                //backupProgressBar.Value = Convert.ToInt32(a * 100);

                            }
                        }

                        //copy file di root
                        foreach (string newPath in Directory.GetFiles(source))
                        {
                            if (File.Exists(newPath.Replace(source, lcl)))
                            {
                                if (File.Exists(newPath.Replace(source, destination)))
                                {
                                    File.Delete(newPath.Replace(source, destination));
                                }
                                File.Copy(newPath, newPath.Replace(source, destination), true);
                                logData += newPath.Replace(source, destination) + "\n";
                                double a = i++ / (fullProg - 1);
                                //backupProgressBar.Value = Convert.ToInt32(a * 100);
                            }

                        }
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error : " + ex);
            }

        }

        public void overwriteWebApps(string src, string dst)
        {
            if (typeStatus == "PSS4W")
            {
                waSource = src + @"\Publish Dev";
                waPSS = dst + @"\pss4w";
                waTDMS = dst + @"\tdms";
                wsSource = src + @"\Web Service";
                wsPSS = dst + @"\pss4wservice";
                wsTDMS = dst + @"\tdmsservice";
                overwriteFiles(waSource, waPSS);
                overwriteFiles(waSource, waTDMS);
                overwriteFiles(wsSource, wsPSS);
                overwriteFiles(wsSource, wsTDMS);
            }
            else if (typeStatus == "PSS2W")
            {
                sourceFolders = new string[] {@"\psshsoho",@"\psshsom29",
                        @"\pss2w", @"\pss2w1", @"\pss2w2", @"\pss2w3", @"\pss2w4", @"\pss2w5",
                        @"\pss2w6", @"\pss2w7", @"\pss2w8", @"\pss2w9", @"\pss2w10", @"\pss2w11"};
                foreach (var srcFd in sourceFolders)
                {
                    destination = dst + srcFd;
                    overwriteFiles(src, destination);
                }
            }
            else if (typeStatus == "TAM")
            {
                waSource = src + @"\Publish Dev";
                waTAM = dst + @"\TAM";
                wsSource = src + @"\Web Service";
                wsTAM = dst + @"\Webservice_TAM";
                overwriteFiles(waSource, waTAM);
                overwriteFiles(waSource, wsTAM);
            }
        }

        public void backupStgFiles(string src, string dst, string lcl)
        {
            try
            {
                //src = path, dst = backupPath
                string source, destination, local;
                foreach (var server in servers)
                {
                    source = @"\\" + server + src;
                    destination = @"\\" + backupServer + dst + @"\" + server + @"\" + publishType;
                    local = lcl;
                    fullProg = Directory.GetFiles(source, "*.*", SearchOption.AllDirectories).Length;
                    i = 0;

                    //bikin folder dulu
                    if (!Directory.Exists(destination))
                    {
                        Directory.CreateDirectory(destination);
                    }

                    //cek direktorinya
                    foreach (string dirPath in Directory.GetDirectories(source, "*", SearchOption.AllDirectories))
                    {
                        //direktorinya ada gak di local
                        if (Directory.Exists(dirPath.Replace(source, local)))
                        {
                            //kalo ada di source, di backup
                            //cek di tempat backup ada gak
                            //kalo ga ada di tempat backup
                            if (!Directory.Exists(dirPath.Replace(source, destination)))
                            {
                                //bikin direktori di tempat backup
                                Directory.CreateDirectory(dirPath.Replace(source, destination));
                            }
                            //ambil file dari alamat yg sudah dikonfirm tadi
                            foreach (string newPath in Directory.GetFiles(dirPath))
                            {
                                if (File.Exists(newPath.Replace(source, destination)))
                                {
                                    File.Delete(newPath.Replace(source, destination));
                                }
                                File.Copy(newPath, newPath.Replace(source, destination), true);
                                logData += newPath.Replace(source, destination) + "\n";
                                double a = i++ / (fullProg - 1);
                                //backupProgressBar.Value = Convert.ToInt32(a * 100);

                            }
                        }
                    }

                    //copy file di root
                    foreach (string newPath in Directory.GetFiles(source))
                    {
                        if (File.Exists(newPath.Replace(source, lcl)))
                        {
                            if (File.Exists(newPath.Replace(source, destination)))
                            {
                                File.Delete(newPath.Replace(source, destination));
                            }
                            File.Copy(newPath, newPath.Replace(source, destination), true);
                            logData += newPath.Replace(source, destination) + "\n";
                            double a = i++ / (fullProg - 1);
                            //backupProgressBar.Value = Convert.ToInt32(a * 100);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error : " + ex);
            }
        }

        public void backupFiles(string src, string dst, string lcl)
        {
            try
            {
                //src = path, dst = backupPath
                string source, destination, local;
                foreach (var server in servers)
                {
                    source = @"\\" + server + src;
                    destination = @"\\" + backupServer + dst + @"\" + server + @"\" + publishType;
                    local = lcl;
                    fullProg = Directory.GetFiles(source, "*.*", SearchOption.AllDirectories).Length;
                    i = 0;

                    //cek jika source exists
                    if (Directory.Exists(source))
                    {
                        //bikin folder dulu
                        if (!Directory.Exists(destination))
                        {
                            Directory.CreateDirectory(destination);
                        }

                        //cek direktorinya
                        foreach (string dirPath in Directory.GetDirectories(source, "*", SearchOption.AllDirectories))
                        {
                            //direktorinya ada gak di local
                            if (Directory.Exists(dirPath.Replace(source, local)))
                            {
                                //kalo ada di source, di backup
                                //cek di tempat backup ada gak
                                //kalo ga ada di tempat backup
                                if (!Directory.Exists(dirPath.Replace(source, destination)))
                                {
                                    //bikin direktori di tempat backup
                                    Directory.CreateDirectory(dirPath.Replace(source, destination));
                                }
                                //ambil file dari alamat yg sudah dikonfirm tadi
                                foreach (string newPath in Directory.GetFiles(dirPath))
                                {
                                    if (File.Exists(newPath.Replace(source, destination)))
                                    {
                                        File.Delete(newPath.Replace(source, destination));
                                    }
                                    File.Copy(newPath, newPath.Replace(source, destination), true);
                                    logData += newPath.Replace(source, destination) + "\n";
                                    double a = i++ / (fullProg - 1);
                                    //backupProgressBar.Value = Convert.ToInt32(a * 100);

                                }
                                
                            }
                        }

                        //copy file di root
                        foreach (string newPath in Directory.GetFiles(source))
                        {
                            if (File.Exists(newPath.Replace(source, lcl)))
                            {
                                if (File.Exists(newPath.Replace(source, destination)))
                                {
                                    File.Delete(newPath.Replace(source, destination));
                                }
                                File.Copy(newPath, newPath.Replace(source, destination), true);
                                logData += newPath.Replace(source, destination) + "\n";
                                double a = i++ / (fullProg - 1);
                                //backupProgressBar.Value = Convert.ToInt32(a * 100);
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error : " + ex);
            }
        }

        public void backupSSISFiles(string src, string dst, string lcl)
        {
            try
            {
                //src = path, dst = backupPath
                string source, destination, local;
                foreach (var server in servers)
                {
                    source = @"\\" + server + src;
                    destination = @"\\" + backupServer + dst + @"\" + server + @"\" + publishType;
                    local = lcl;
                    fullProg = Directory.GetFiles(source, "*.*", SearchOption.AllDirectories).Length;
                    i = 0;

                    //cek jika source exists
                    //if (Directory.Exists(source))
                    //{
                    //bikin folder dulu
                    if (!Directory.Exists(destination))
                    {
                        Directory.CreateDirectory(destination);
                    }

                    foreach (string files in Directory.GetFiles(source, "*.*", SearchOption.AllDirectories))
                    {
                        string filesLocal = files.Replace(source,local);
                        if (File.Exists(filesLocal))
                        {
                            if(!Directory.Exists(Path.GetDirectoryName(files).Replace(source, destination)))
                            {
                                Directory.CreateDirectory(Path.GetDirectoryName(files).Replace(source, destination));
                            }
                            if (File.Exists(files.Replace(source, destination)))
                            {
                                File.Delete(files.Replace(source, destination));
                            }
                            File.Copy(files, files.Replace(source, destination), true);
                            logData += files.Replace(source, destination) + "\n";
                            double a = i++ / (fullProg - 1);
                            //backupProgressBar.Value = Convert.ToInt32(a * 100);
                        }

                    }

                        ////cek direktorinya
                        //foreach (string dirPath in Directory.GetDirectories(source, "*", SearchOption.AllDirectories))
                        //{
                        //    //direktorinya ada gak di local
                        //    if (Directory.Exists(dirPath.Replace(source, local)))
                        //    {
                        //        //kalo ada di source, di backup
                        //        //cek di tempat backup ada gak
                        //        //kalo ga ada di tempat backup
                        //        if (!Directory.Exists(dirPath.Replace(source, destination)))
                        //        {
                        //            //bikin direktori di tempat backup
                        //            Directory.CreateDirectory(dirPath.Replace(source, destination));
                        //        }
                        //        //ambil file dari alamat yg sudah dikonfirm tadi
                        //        foreach (string newPath in Directory.GetFiles(dirPath))
                        //        {
                        //            if (File.Exists(newPath.Replace(source, destination)))
                        //            {
                        //                File.Delete(newPath.Replace(source, destination));
                        //            }
                        //            File.Copy(newPath, newPath.Replace(source, destination), true);
                        //            logData += newPath.Replace(source, destination) + "\n";
                        //            double a = i++ / (fullProg - 1);
                        //            backupProgressBar.Value = Convert.ToInt32(a * 100);

                        //        }

                        //    }
                        //}

                        ////copy file di root
                        //foreach (string newPath in Directory.GetFiles(source))
                        //{
                        //    if (File.Exists(newPath.Replace(source, lcl)))
                        //    {
                        //        if (File.Exists(newPath.Replace(source, destination)))
                        //        {
                        //            File.Delete(newPath.Replace(source, destination));
                        //        }
                        //        File.Copy(newPath, newPath.Replace(source, destination), true);
                        //        logData += newPath.Replace(source, destination) + "\n";
                        //        double a = i++ / (fullProg - 1);
                        //        backupProgressBar.Value = Convert.ToInt32(a * 100);
                        //    }

                        //}
                    }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error : " + ex);
            }
        }

        public void overwriteFiles(string src, string dst)
        {
            try
            {
                //src = localpath; dst = path
                //apps
                //folder harus sama
                string source, destination;
                foreach (var server in servers)
                {
                    source = src;
                    destination = @"\\" + server + dst;
                    fullProg = Directory.GetFiles(source, "*.*", SearchOption.AllDirectories).Length;
                    i = 0;

                    //Now Create all of the directories
                    foreach (string dirPath in Directory.GetDirectories(source, "*", SearchOption.AllDirectories))
                    {
                        if (Directory.Exists(dirPath.Replace(source, destination)))
                        {
                            //Directory.CreateDirectory(dirPath.Replace(source, destination));
                            foreach (string newPath in Directory.GetFiles(dirPath))
                            {
                                if (File.Exists(newPath.Replace(source, destination)))
                                {
                                    File.Delete(newPath.Replace(source, destination));
                                }
                                File.Copy(newPath, newPath.Replace(source, destination), true);
                                logData += newPath.Replace(source, destination) + "\n";
                                double a = i++ / (fullProg - 1);
                                //overwriteProgressBar.Value = Convert.ToInt32(a * 100);
                            }
                        }
                    }

                    //copy file di root
                    foreach (string newPath in Directory.GetFiles(source))
                    {
                        if (File.Exists(newPath.Replace(source, destination)))
                        {
                            File.Delete(newPath.Replace(source, destination));
                        }
                        File.Copy(newPath, newPath.Replace(source, destination), true);
                        logData += newPath.Replace(source, destination) + "\n";
                        double a = i++ / (fullProg - 1);
                        //overwriteProgressBar.Value = Convert.ToInt32(a * 100);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error : " + ex);
            }
            
        }

        public void overwriteSSISFiles(string src, string dst)
        {
            try
            {
                //src = localpath; dst = path
                //apps
                //folder harus sama
                string source, destinationX, destinationConfig;
                foreach (var server in servers)
                {
                    source = src;
                    destinationX = @"\\" + server + dst + @"\SSISPackage";
                    destinationConfig = @"\\" + server + dst + @"\SSISConfig";
                    fullProg = Directory.GetFiles(source, "*.*", SearchOption.AllDirectories).Length;
                    i = 0;

                    //Now Create all of the directories
                    foreach (string dirPath in Directory.GetDirectories(source, "*", SearchOption.AllDirectories))
                    {
                        if (Directory.Exists(dirPath.Replace(source, destination)))
                        {
                            //Directory.CreateDirectory(dirPath.Replace(source, destination));
                            foreach (string newPath in Directory.GetFiles(dirPath, "*.*").Where(s => s.EndsWith(".dtsx")))
                            {
                                if (File.Exists(newPath.Replace(source, destinationX)))
                                {
                                    File.Delete(newPath.Replace(source, destinationX));
                                }
                                File.Copy(newPath, newPath.Replace(source, destinationX), true);
                                logData += newPath.Replace(source, destinationX) + "\n";
                                double a = i++ / (fullProg - 1);
                                //overwriteProgressBar.Value = Convert.ToInt32(a * 100);
                            }
                            foreach (string newPath in Directory.GetFiles(dirPath, "*.*").Where(s => s.EndsWith(".dtsConfig")))
                            {
                                if (File.Exists(newPath.Replace(source, destinationConfig)))
                                {
                                    File.Delete(newPath.Replace(source, destinationConfig));
                                }
                                File.Copy(newPath, newPath.Replace(source, destinationConfig), true);
                                logData += newPath.Replace(source, destinationConfig) + "\n";
                                double a = i++ / (fullProg - 1);
                                //overwriteProgressBar.Value = Convert.ToInt32(a * 100);
                            }
                        }
                    }

                    ////copy file di root
                    //foreach (string newPath in Directory.GetFiles(source))
                    //{
                    //    if (File.Exists(newPath.Replace(source, destination)))
                    //    {
                    //        File.Delete(newPath.Replace(source, destination));
                    //    }
                    //    File.Copy(newPath, newPath.Replace(source, destination), true);
                    //    logData += newPath.Replace(source, destination) + "\n";
                    //}
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error : " + ex);
            }

        }
            
    }
}
