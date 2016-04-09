using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace Publishing_Tools.Class
{
    class FileCopy
    {
        //string[] servers, sourceFolders;
        List <string> appFolders;
        string source, destination, tempPublishType, logData, newPath;
        string server, folderSource;
        int i, j, fullProg;
        Command myCommand;
        string[] arraySource, arrayDestination;
        ExecPowershell eps = new ExecPowershell();
        string ps1path;

        public string GetLogData()
        {
            return logData;
        }

        public void LoadBackupFiles(string publishType, ServerAndPool sap, String connectionString, string typeStatus, Button stopPoolsButton, 
            DataGridView dataGridView1, string path, string backupPath, string backupServer, string pathStg, string localPath,
            string appServer, TextBox ssisFileName, BackgroundWorker bgwLoadBackup, Label backupFileInfo)
        {
            
            if (publishType == null)
            {
                MessageBox.Show("pilih dulu apps, web apps, atau ssis!");
            }
            //else
            //{
            //    sap.getServer(connectionString, typeStatus, publishType);
            //    servers = sap.Getservers();
            //}

            if (publishType == "Web Apps")
            {
                stopPoolsButton.Invoke((MethodInvoker)delegate
                {
                    stopPoolsButton.Enabled = true;
                });
                //dataGridView1.Invoke((MethodInvoker)delegate
                //{
                //    dataGridView1.Rows.Clear();
                //});
                backupFileInfo.Invoke((MethodInvoker)delegate
                {
                    backupFileInfo.Text = "Copying files";
                });
                if (typeStatus == "PSS4W")
                {
                    //sourceFolders = new string[] { @"\pss4w", @"\tdms", @"\pss4wservice", @"\tdmsservice" };
                    server = "A000S-ITPSAP03";
                    ps1path = @"\\a000s-drrpt1\MigrationUploadData\Abud\Taufiq\Publishing tools\copyfiles.ps1";
                    eps.execToPowerShell(server, DateTime.Now.ToString("yyMMdd"), ps1path);           
                }
                else if (typeStatus == "PSS2W")
                {
                    //sourceFolders = new string[] { @"\psshsoho", @"\psshsom29", @"\pss2w", @"\pss2w1" };
                    server = "A000S-ITPSAP15";
                    ps1path = @"\\a000s-drrpt1\MigrationUploadData\Abud\Taufiq\Publishing tools\copyfiles.ps1";
                    eps.execToPowerShell(server, DateTime.Now.ToString("yyMMdd"), ps1path);    
                }
                else if (typeStatus == "TAM")
                {
                    //sourceFolders = new string[] { @"\TAM", @"\Webservice_TAM" };
                    server = "A000S-PSSAPT1";
                    ps1path = @"\\a000s-drrpt1\MigrationUploadData\Abud\Taufiq\Publishing tools\copyfiles.ps1";
                    eps.execToPowerShell(server, DateTime.Now.ToString("yyMMdd"), ps1path);    
                }
                backupFileInfo.Invoke((MethodInvoker)delegate
                {
                    backupFileInfo.Text = "Done";
                });
                //foreach (var server in servers)
                //{
                //int i = 0;
                //arraySource = new string[sourceFolders.Length];
                //arrayDestination = new string[sourceFolders.Length];
                //foreach (var srcFd in sourceFolders)
                //{
                //    if (typeStatus == "PSS4W")
                //    {
                //        server = "A000S-ITPSAP03";
                //        source = @"\\" + server + path + srcFd;
                //        destination = @"\\" + backupServer + backupPath + @"\" + server + @"\" + publishType + srcFd;
                //    }
                //    else if (typeStatus == "TAM")
                //    {
                //        server = "A000S-PSSAPT1";
                //        source = @"\\" + server + path + srcFd;
                //        destination = @"\\" + backupServer + backupPath + @"\" + server + @"\" + publishType + srcFd;
                //    }
                //    else if (typeStatus == "PSS2W")
                //    {
                //        server = "A000S-ITPSAP15";
                //        source = @"\\" + server + path + srcFd;
                //        destination = backupPath + srcFd;
                //    }
                //    if (!Directory.Exists(destination))
                //    {
                //        Directory.CreateDirectory(destination);
                //    }
                //    arraySource[i] = source;
                //    arrayDestination[i] = destination;
                //    i++;
                    
                //    //bgwLoadBackup.ReportProgress(0,"Adding security..");
                //    ////gives authorization
                //    //SetPermissions.AddDirectorySecurity(source, @"CORPAI\ciptha952839", FileSystemRights.FullControl, AccessControlType.Allow);
                //    //bgwLoadBackup.ReportProgress(0,"Listing..");

                //    //foreach (string files in Directory.GetFiles(source, "*.*", SearchOption.AllDirectories))
                //    //{
                //    //    //dataGridView1.Rows.Clear();
                //    //    //dataGridView1.Rows[i].Cells[0].Value = files;
                //    //    //dataGridView1.Rows[i].Cells[1].Value = files.Replace(source, destination);
                //    //    //i++;
                //    //    dataGridView1.Invoke((MethodInvoker)delegate
                //    //    {
                //    //        dataGridView1.Rows.Add(files, files.Replace(source, destination));
                //    //    }); 
                //    //}
                //}
                
                //if (File.Exists(Path.GetFileName(destination)))
                //{
                //    File.SetAttributes(Path.GetFileName(destination), FileAttributes.Normal);
                //    File.Delete(Path.GetFileName(destination));
                //}
                    
                    //for (i = 0; i < arraySource.Length; i++)
                    //{
                    //    backupFileInfo.Invoke((MethodInvoker)delegate
                    //    {
                    //        backupFileInfo.Text = "Copying files " + arraySource[i];
                    //    });
                    //    PowerShell ps = PowerShell.Create();
                    //    ps.AddCommand("Copy-Item");

                    //    IDictionary parameters = new Dictionary<String, String>();
                    //    parameters.Add(0,arraySource[i]);
                    //    parameters.Add(1,arrayDestination[i]);
                    //    parameters.Add(2,"-recurse");
                    //    parameters.Add(3,"-force");
                    //    parameters.Add(4,"-ErrorAction");
                    //    parameters.Add(5,"SilentlyContinue");

                    //    ps.AddParameters(parameters);

                    //    ps.Invoke();
                    //}

                    ////exec script
                    //RunspaceConfiguration runspaceConfiguration = RunspaceConfiguration.Create();
                    //Runspace runspace = RunspaceFactory.CreateRunspace(runspaceConfiguration);
                    //runspace.Open();
                    //RunspaceInvoke scriptInvoker = new RunspaceInvoke(runspace);
                    //Pipeline pipeline = runspace.CreatePipeline();
                    //myCommand = new Command(@"\\a000s-drrpt1\MigrationUploadData\Abud\Taufiq\Publishing tools\copyfiles.ps1");
                    ////myCommand.Parameters.Add("source",arraySource);
                    ////myCommand.Parameters.Add("destination",arrayDestination);

                    //myCommand.Parameters.Add("source", @"U:\tes");
                    //myCommand.Parameters.Add("destination", @"U:\tes2");
                    //pipeline.Commands.Add(myCommand);


                    //// Execute PowerShell script
                    //Collection<PSObject> results = pipeline.Invoke();


                ////double a = j++ / (sourceFolders.Length);
                //double a = 1;
                //bgwLoadBackup.ReportProgress(Convert.ToInt32(a * 100));
                //}
            }
            else if (publishType == "Apps")
            {
                dataGridView1.Invoke((MethodInvoker)delegate
                {
                    dataGridView1.Rows.Clear();
                });
                //foreach (var server in servers)
                //{

                if (typeStatus == "PSS4W")
                {
                    switch (appServer)
                    {
                        case "server2":
                            //source = @"\\" + server + path;
                            //destination = @"\\" + backupServer + backupPath + @"\" + server + @"\" + publishType;
                            //break;
                        case "server3":
                            //server = "A000S-ITPSAP03";
                            //source = @"\\" + server + path;
                            //destination = @"\\" + backupServer + backupPath + @"\" + server + @"\" + publishType;
                            //break;
                        case "server4":
                            //server = "A000S-ITPSAP04";
                            //source = @"\\" + server + path;
                            //destination = @"\\" + backupServer + backupPath + @"\" + server + @"\" + publishType;
                            //break;
                        case "server5":
                            string[] servers = { "A000S-ITPSAP02", "A000S-ITPSAP03", "A000S-ITPSAP04", "A000S-ITPSAP05"};
                            ps1path = @"\\a000s-drrpt1\MigrationUploadData\Abud\Taufiq\Publishing tools\copyfiles.ps1";
                            eps.execToPowerShell(servers, DateTime.Now.ToString("yyMMdd"), ps1path); 
                            //server = "A000S-ITPSAP05";
                            //source = @"\\" + server + path;
                            //destination = @"\\" + backupServer + backupPath + @"\" + server + @"\" + publishType;
                            break;
                        case "staging":
                            server = "A000S-PSSTG1";
                            ps1path = @"\\a000s-drrpt1\MigrationUploadData\Abud\Taufiq\Publishing tools\copyfiles.ps1";
                            eps.execToPowerShell(server, DateTime.Now.ToString("yyMMdd"), ps1path); 
                            //source = @"\\" + server + pathStg;
                            //destination = @"\\" + backupServer + backupPath + @"\" + server + @"\" + publishType;
                            break;
                        default:
                            break;
                    }
                }
                else if (typeStatus == "TAM")
                {
                    string[] servers = { "A000S-PSSAPT1", "A000S-PSSAPT3" };
                    ps1path = @"\\a000s-drrpt1\MigrationUploadData\Abud\Taufiq\Publishing tools\copyfiles.ps1";
                    eps.execToPowerShell(servers, DateTime.Now.ToString("yyMMdd"), ps1path);
                    //foreach (var server in servers)
                    //{
                    //    source = @"\\" + server + pathStg;
                    //    destination = @"\\" + backupServer + backupPath + @"\" + server + @"\" + publishType;
                    //}
                }
            
                //if (appServer == "server2")
                //{
                //    source = @"\\" + "A000S-ITPSAP2" + pathStg;
                //    destination = @"\\" + backupServer + backupPath + @"\" + server + @"\" + publishType;

                //}
                //else
                //{
                //    source = @"\\" + server + path;
                //    destination = @"\\" + backupServer + backupPath + @"\" + server + @"\" + publishType;
                //}
                //bgwLoadBackup.ReportProgress(0, "Adding security..");
                ////gives authorization
                //SetPermissions.AddDirectorySecurity(source, @"CORPAI\ciptha952839", FileSystemRights.FullControl, AccessControlType.Allow);
                //bgwLoadBackup.ReportProgress(0, "Listing..");
                //foreach (string files in Directory.GetFiles(source, "*.*", SearchOption.AllDirectories))
                //{
                //    //if (Directory.Exists(Path.GetDirectoryName(files).Replace(source, localPath)))
                //    //{
                //    //dataGridView1.Rows[i].Cells[0].Value = files;
                //    //dataGridView1.Rows[i].Cells[1].Value = files.Replace(source, destination);
                //    //i++;
                //    dataGridView1.Invoke((MethodInvoker)delegate
                //    {
                //        dataGridView1.Rows.Add(files, files.Replace(source, destination));
                //    });
                //    //}
                //}
                //}
            }
            //else if (publishType == "SSIS")
            //{
            //    tempPublishType = publishType;
            //    publishType = @"SSIS\" + publishType;
            //    dataGridView1.Invoke((MethodInvoker)delegate
            //    {
            //        dataGridView1.Rows.Clear();
            //    });
            //    foreach (var server in servers)
            //    {
            //        source = @"\\" + server + path;
            //        destination = @"\\" + backupServer + backupPath + @"\" + server + @"\" + publishType;

            //        //bgwLoadBackup.ReportProgress(0, "Adding security..");
            //        ////gives authorization
            //        //SetPermissions.AddDirectorySecurity(source, @"CORPAI\ciptha952839", FileSystemRights.FullControl, AccessControlType.Allow);
            //        bgwLoadBackup.ReportProgress(0, "Listing..");
            //        //foreach (string files in Directory.GetFiles(source, "*.*", SearchOption.AllDirectories))
            //        //{
            //        //    string filesLocal = files.Replace(source, localPath);
            //        //    if (File.Exists(filesLocal))
            //        //    {
            //        //        if (!Directory.Exists(Path.GetDirectoryName(files).Replace(source, destination)))
            //        //        {
            //        //            Directory.CreateDirectory(Path.GetDirectoryName(files).Replace(source, destination));
            //        //        }
            //        //        if (File.Exists(files.Replace(source, destination)))
            //        //        {
            //        //            File.Delete(files.Replace(source, destination));
            //        //        }
            //        //        File.Copy(files, files.Replace(source, destination), true);
            //        //        logData += files.Replace(source, destination) + "\n";
            //        //        double a = i++ / (fullProg - 1);
            //        //        //backupProgressBar.Value = Convert.ToInt32(a * 100);
            //        //    }

            //        //}
            //        foreach (string files in Directory.GetFiles(source, "*.*", SearchOption.AllDirectories))
            //        {
            //            //string configPath = source + @"\SSISConfig";
            //            //string packagePath = source + @"\SSISPackage";
            //            ssisFileName.Invoke((MethodInvoker)delegate
            //            {
            //                if (files.Contains(ssisFileName.Text))
            //                {
            //                    dataGridView1.Invoke((MethodInvoker)delegate
            //                    {
            //                        dataGridView1.Rows.Add(files, files.Replace(source, destination));
            //                    });
            //                }
            //            });
            //            //if (Directory.Exists(Path.GetDirectoryName(files).Replace(configPath, localPath)))
            //            //{
            //            //    //dataGridView1.Rows.Clear();
            //            //    //dataGridView1.Rows[i].Cells[0].Value = files;
            //            //    //dataGridView1.Rows[i].Cells[1].Value = files.Replace(source, destination);
            //            //    //i++;
            //            //    dataGridView1.Invoke((MethodInvoker)delegate
            //            //    {
            //            //        dataGridView1.Rows.Add(files, files.Replace(source, destination));
            //            //    });
            //            //}
            //            //if (Directory.Exists(Path.GetDirectoryName(files).Replace(packagePath, localPath)))
            //            //{
            //            //    //dataGridView1.Rows.Clear();
            //            //    //dataGridView1.Rows[i].Cells[0].Value = files;
            //            //    //dataGridView1.Rows[i].Cells[1].Value = files.Replace(source, destination);
            //            //    //i++;
            //            //    dataGridView1.Invoke((MethodInvoker)delegate
            //            //    {
            //            //        dataGridView1.Rows.Add(files, files.Replace(source, destination));
            //            //    });
            //            //}
            //        }
            //    }
            //    publishType = tempPublishType;
            //}
            //else
            //{
            //    dataGridView1.Invoke((MethodInvoker)delegate
            //    {
            //        dataGridView1.Rows.Clear();
            //    });
            //    foreach (var server in servers)
            //    {
            //        source = @"\\" + server + path;
            //        destination = @"\\" + backupServer + backupPath + @"\" + server + @"\" + publishType;

            //        //bgwLoadBackup.ReportProgress(0, "Adding security..");
            //        ////gives authorization
            //        //SetPermissions.AddDirectorySecurity(source, @"CORPAI\ciptha952839", FileSystemRights.FullControl, AccessControlType.Allow);
            //        bgwLoadBackup.ReportProgress(0, "Listing..");
            //        foreach (string files in Directory.GetFiles(source, "*.*", SearchOption.AllDirectories))
            //        {
            //            if (Directory.Exists(Path.GetDirectoryName(files).Replace(source, localPath)))
            //            {
            //                //dataGridView1.Rows.Clear();
            //                //dataGridView1.Rows[i].Cells[0].Value = files;
            //                //dataGridView1.Rows[i].Cells[1].Value = files.Replace(source, destination);
            //                //i++;
            //                dataGridView1.Invoke((MethodInvoker)delegate
            //                {
            //                    dataGridView1.Rows.Add(files, files.Replace(source, destination));
            //                });
            //            }
            //        }
            //    }
            //}
        }

        public void BackupWork(DataGridView dataGridView1, ProgressBar backupProgBar, string typeStatus, string publishType, BackgroundWorker bgwBackup)
        {
            try
            {
                j = 0;
                fullProg = dataGridView1.Rows.Count - 1;
                for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
                {
                    string src = dataGridView1.Rows[i].Cells[0].Value.ToString();
                    string dst = dataGridView1.Rows[i].Cells[1].Value.ToString();

                    if (!Directory.Exists(Path.GetDirectoryName(dst)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(dst));
                    }
                    if (File.Exists(dst))
                    {
                        File.SetAttributes(dst, FileAttributes.Normal);
                        File.Delete(dst);
                    }
                    File.Copy(src, dst, true);
                    logData += src + "\n";
                    double a = j++ / (fullProg - 1);
                    bgwBackup.ReportProgress(Convert.ToInt32(a * 100), i);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error : " + ex.InnerException.Message);
            }
            
        }

        public void LoadOverwriteFiles(DataGridView dataGridView2, string publishType, string typeStatus, string localPath, string path,
            string pathStg, string appServer, BackgroundWorker bgwLoadOverwrite)
        {
            try
            {
                dataGridView2.Rows.Clear();

                switch (appServer)
                {
                    case "server2":
                        server = "A000S-ITPSAP02";
                        appFolders = new List<string>{"Astra.Pss.MailingDownload","Astra.Pss.PssConsoleApps","Astra.Pss.ReportAutoDownload",
                        "Astra.Pss.RetainPPVUnit","PSS4W.ClosingApps"};
                        loadOverwriteMethod(localPath, path, dataGridView2, bgwLoadOverwrite);
                        break;
                    case "server3":
                        server = "A000S-ITPSAP03";
                        appFolders = new List<string> { "PSS4W.ReClosingApps" };
                        loadOverwriteMethod(localPath, path, dataGridView2, bgwLoadOverwrite);
                        break;
                    case "server4":
                        server = "A000S-ITPSAP04";
                        appFolders = new List<string>{"Astra.Pss.MailingDownload","Astra.Pss.PssConsoleApps","Astra.Pss.ReportAutoDownload",
                        "Astra.Pss.RetainPPVUnit","PSS4W.ClosingApps"};
                        loadOverwriteMethod(localPath, path, dataGridView2, bgwLoadOverwrite);
                        break;
                    case "server5":
                        server = "A000S-ITPSAP05";
                        appFolders = new List<string>{"Astra.Pss.MailingDownload","Astra.Pss.PssConsoleApps","Astra.Pss.ReportAutoDownload",
                        "Astra.Pss.RetainPPVUnit","PSS4W.ClosingApps"};
                        loadOverwriteMethod(localPath, path, dataGridView2, bgwLoadOverwrite);
                        break;
                    case "staging":
                        server = "A000S-PSSTG1";
                        appFolders = new List<string> { "Astra.Pss.Scheduling.BusinessLib" };
                        loadOverwriteMethod(localPath, pathStg, dataGridView2, bgwLoadOverwrite);
                        break;
                    default:
                        break;
                }

                //loadOverwriteMethod(localPath, path, dataGridView2);

                //if (publishType == "Web Apps")
                //{
                //    string waSource, wsSource, waPSS, waTDMS, waTAM, wsPSS, wsTDMS, wsTAM;
                //    if (typeStatus == "PSS4W")
                //    {
                //        dataGridView2.Rows.Clear();
                //        waSource = localPath + @"\Publish Dev";
                //        waPSS = path + @"\pss4w";
                //        waTDMS = path + @"\tdms";
                //        wsSource = localPath + @"\Web Service";
                //        wsPSS = path + @"\pss4wservice";
                //        wsTDMS = path + @"\tdmsservice";
                //        //gives authorization
                //        SetPermissions.AddDirectorySecurity(path, @"CORPAI\ciptha952839", FileSystemRights.FullControl, AccessControlType.Allow);

                //        loadOverwriteMethod(waSource, waPSS, dataGridView2);
                //        loadOverwriteMethod(waSource, waTDMS, dataGridView2);
                //        loadOverwriteMethod(wsSource, wsPSS, dataGridView2);
                //        loadOverwriteMethod(wsSource, wsTDMS, dataGridView2);
                //    }
                //    else if (typeStatus == "PSS2W")
                //    {
                //        dataGridView2.Rows.Clear();
                //        sourceFolders = new string[] {@"\psshsoho",@"\psshsom29",
                //            @"\pss2w", @"\pss2w1", @"\pss2w2", @"\pss2w3", @"\pss2w4", @"\pss2w5",
                //            @"\pss2w6", @"\pss2w7", @"\pss2w8", @"\pss2w9", @"\pss2w10", @"\pss2w11"};
                //        //gives authorization
                //        SetPermissions.AddDirectorySecurity(path, @"CORPAI\ciptha952839", FileSystemRights.FullControl, AccessControlType.Allow);
                //        foreach (var srcFd in sourceFolders)
                //        {
                //            destination = path + srcFd;
                //            loadOverwriteMethod(localPath, destination, dataGridView2);
                //        }
                //    }
                //    else if (typeStatus == "TAM")
                //    {
                //        dataGridView2.Rows.Clear();
                //        waSource = localPath + @"\Publish Dev";
                //        waTAM = path + @"\TAM";
                //        wsSource = localPath + @"\Web Service";
                //        wsTAM = path + @"\Webservice_TAM";
                //        loadOverwriteMethod(waSource, waTAM, dataGridView2);
                //        loadOverwriteMethod(wsSource, wsTAM, dataGridView2);
                //        //gives authorization
                //        SetPermissions.AddDirectorySecurity(path, @"CORPAI\ciptha952839", FileSystemRights.FullControl, AccessControlType.Allow);
                //    }
                //}
                //else if (publishType == "SSIS")
                //{
                //    dataGridView2.Rows.Clear();
                //    string destinationX, destinationConfig;
                //    foreach (var server in servers)
                //    {
                //        source = localPath;
                //        destinationX = @"\\" + server + path + @"\SSISPackage";
                //        destinationConfig = @"\\" + server + path + @"\SSISConfig";
                //        //gives authorization
                //        SetPermissions.AddDirectorySecurity(destinationX, @"CORPAI\ciptha952839", FileSystemRights.FullControl, AccessControlType.Allow);
                //        //gives authorization
                //        SetPermissions.AddDirectorySecurity(destinationConfig, @"CORPAI\ciptha952839", FileSystemRights.FullControl, AccessControlType.Allow);
                //        foreach (string files in Directory.GetFiles(source, "*.*").Where(s => s.EndsWith(".dtsx")))
                //        {
                //            //dataGridView2.Rows[i].Cells[0].Value = files;
                //            //dataGridView2.Rows[i].Cells[1].Value = files.Replace(source, destinationX);
                //            //i++;
                //            dataGridView2.Rows.Add(files, files.Replace(source, destinationX));
                //        }
                //        foreach (string files in Directory.GetFiles(source, "*.*").Where(s => s.EndsWith(".dtsConfig")))
                //        {
                //            //dataGridView2.Rows[i].Cells[0].Value = files;
                //            //dataGridView2.Rows[i].Cells[1].Value = files.Replace(source, destinationConfig);
                //            //i++;
                //            dataGridView2.Rows.Add(files, files.Replace(source, destinationConfig));
                //        }
                //    }
                //}
                //else
                //{
                //    loadOverwriteMethod(localPath, path, dataGridView2);
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error : " + ex.InnerException.Message);
            }
        }

        public void loadOverwriteMethod(string src, string dst, DataGridView dataGridView2, BackgroundWorker bgwLoadOverwrite)
        {
            try
            {
                //foreach (var server in servers)
                //{
                source = src;
                foreach (var fdr in appFolders)
                {
                    folderSource = src + fdr;

                    destination = @"\\" + server + dst;

                    //bgwLoadOverwrite.ReportProgress(0, "Adding security..");
                    ////gives authorization
                    //SetPermissions.AddDirectorySecurity(destination, @"CORPAI\ciptha952839", FileSystemRights.FullControl, AccessControlType.Allow);
                    bgwLoadOverwrite.ReportProgress(0, "Listing..");
                    foreach (string files in Directory.GetFiles(folderSource, "*.*", SearchOption.AllDirectories))
                    {
                        //dataGridView2.Rows[i].Cells[0].Value = files;
                        //dataGridView2.Rows[i].Cells[1].Value = files.Replace(source, destination);
                        //i++;
                        dataGridView2.Rows.Add(files, files.Replace(source, destination));
                    }
                }
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error : " + ex.InnerException.Message);
            }
        }

        public void OverwriteWork(DataGridView dataGridView2, ProgressBar overwriteProgBar, string typeStatus, string publishType, BackgroundWorker bgwOverwrite)
        {
            try
            {
                j = 0;
                fullProg = dataGridView2.Rows.Count - 1;
                for (int i = 0; i < dataGridView2.Rows.Count - 1; i++)
                {
                    string src = dataGridView2.Rows[i].Cells[0].Value.ToString();
                    string dst = dataGridView2.Rows[i].Cells[1].Value.ToString();
                    if (Directory.Exists(Path.GetDirectoryName(dst)))
                    {
                        if (File.Exists(dst))
                        {
                            File.SetAttributes(dst, FileAttributes.Normal);
                            File.Delete(dst);
                        }
                        File.Copy(src, dst, true);
                        logData += src + "\n";
                        double a = j++ / (fullProg - 1);
                        bgwOverwrite.ReportProgress(Convert.ToInt32(a * 100), i);
                    }
                    //double a = j++ / (fullProg - 1);
                    //overwriteProgBar.Invoke((MethodInvoker)delegate
                    //{
                    //    overwriteProgBar.Value = Convert.ToInt32(a * 100);
                    //});
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error : " + ex.InnerException.Message);
            }
        }

        public void CopyLocalWork(string sourcePath, string localPath, ProgressBar copyLocalProgBar, string typeStatus, BackgroundWorker bgwCopyToLocal)
        {
            try
            {
                i = 0;
                fullProg = Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories).Length;
                //Now Create all of the directories
                foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
                {
                    Directory.CreateDirectory(dirPath.Replace(sourcePath, localPath));
                }

                //Copy all the files & Replaces any files with the same name
                foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
                {
                    File.Copy(newPath, newPath.Replace(sourcePath, localPath), true);
                    //string b = Path.GetDirectoryName(newPath);
                    logData += newPath.Replace(sourcePath, localPath) + "\n";
                    double a = j++ / (fullProg - 1);
                    bgwCopyToLocal.ReportProgress(Convert.ToInt32(a * 100));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.InnerException.Message + "When copying : " + newPath);
            }
        }

    }
}
