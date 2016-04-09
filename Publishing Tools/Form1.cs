using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.Management.Automation.Runspaces;
using System.Management.Automation;
using System.Collections.ObjectModel;
using Excel = Microsoft.Office.Interop.Excel;
using System.Data.SqlClient;
using System.Configuration;
using GenerateScripts;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Server;
using Microsoft.SqlServer.Management.Common;
using System.Threading;
using System.Security.AccessControl;
using Publishing_Tools.Class;
using System.Collections;
using System.Diagnostics;
using System;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Publishing_Tools
{
    public partial class Form1 : Form
    {
        private static Timer aTimer;
        string constring = ConfigurationManager.ConnectionStrings["connString"].ToString();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //ToggleConfigEncryption("Publishing Tools.exe");
            startPoolsButton.Enabled = false;
            stopPoolsButton.Enabled = false;
            //overwriteButton.Enabled = false;
            //loadOverwriteButton.Enabled = false;
            //copyLocalButton.Enabled = false;
            //backupAppsButton.Enabled = false;
            //loadButton.Enabled = false;
            //publishTypeBox.Enabled = false;

            //read xml file
            //po = XDocument.Load(@"\\a000s-drrpt1\MigrationUploadData\Abud\Taufiq\tes.xml");
            //readXmlFile(po);

            //tab DB
            ListServer();
            ListAuthentication();
            ListDatabase();
            execOptions.SelectedIndex = 0;
            execOptions.Enabled = false;
            otherServer.Enabled = false;

            //tab manualTask
            updateValueButton.Enabled = false;
            addXMLButton.Enabled = false;
            newKeyText.Enabled = false;
            newValueText.Enabled = false;

            //DataGridViewColumn dgvSource = dgvPublishLog.Columns[1];
            //DataGridViewColumn dgvDestination = dgvPublishLog.Columns[2];
            //dgvSource.Width = 60;
            //dgvSource.Width = 60;


            timer.RunWorkerAsync();
        }

        private void timer_DoWork(object sender, DoWorkEventArgs e)
        {
            // Create a timer with a two second interval.
            //aTimer = new System.Timers.Timer(2000);
            // Hook up the Elapsed event for the timer. 
            //aTimer.Elapsed += OnTimedEvent;
            //aTimer.Enabled = true;
        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            ToggleQuote(quote1);
            ToggleQuote(quote2);
            //Console.WriteLine("The Elapsed event was raised at {0}", e.SignalTime);
            /*using (SqlConnection sqlConn = new SqlConnection(constring))
            {
                string sqlQuery = @"select Type, Source, Destination, Createdtime as Time from publishlog order by createdtime desc";
                SqlCommand cmd = new SqlCommand(sqlQuery, sqlConn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable table = new DataTable();
                da.Fill(table);
                dgvPublishLog.Invoke((MethodInvoker)delegate
                {
                    dgvPublishLog.DataSource = new BindingSource(table, null);
                });

                string sqlQuery2 = @"select servername as 'Instance Name', DatabaseName as 'Database Name', Query, querypath as Path, createdtime as Time from execscriptlog order by createdtime desc";
                SqlCommand cmd2 = new SqlCommand(sqlQuery2, sqlConn);
                SqlDataAdapter da2 = new SqlDataAdapter(cmd2);
                DataTable table2 = new DataTable();
                da2.Fill(table2);
                dgvDBlog.Invoke((MethodInvoker)delegate
                {
                    dgvDBlog.DataSource = new BindingSource(table2, null);
                });
            }*/
        }


        private void refresha_Click(object sender, System.EventArgs e)
        {
            using (SqlConnection sqlConn = new SqlConnection(constring))
            {
                string sqlQuery = @"select top 500 Type, Source, Destination, Createdtime as Time from publishlog order by createdtime desc";
                SqlCommand cmd = new SqlCommand(sqlQuery, sqlConn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable table = new DataTable();
                da.Fill(table);
                dgvPublishLog.Invoke((MethodInvoker)delegate
                {
                    dgvPublishLog.DataSource = new BindingSource(table, null);
                });
            }

        }

        private void refreshd_Click(object sender, System.EventArgs e)
        {
            using (SqlConnection sqlConn = new SqlConnection(constring))
            {
                string sqlQuery2 = @"select top 500 servername as 'Instance Name', DatabaseName as 'Database Name', Query, querypath as Path, createdtime as Time from execscriptlog order by createdtime desc";
                SqlCommand cmd2 = new SqlCommand(sqlQuery2, sqlConn);
                SqlDataAdapter da2 = new SqlDataAdapter(cmd2);
                DataTable table2 = new DataTable();
                da2.Fill(table2);
                dgvDBlog.Invoke((MethodInvoker)delegate
                {
                    dgvDBlog.DataSource = new BindingSource(table2, null);
                });
            }

        }


        private void ToggleQuote(Label quote)
        {
            quote.Invoke((MethodInvoker)delegate
                {
                    if (quote.Visible == true)
                    {
                        quote.Visible = false;
                    }
                    else
                    {
                        quote.Visible = true;
                    }
                });
        }

        #region apps

        protected string typeStatus = null;
        protected string publishType = null;
        //string tempPublishType;
        string empty = string.Empty;
        string logData;
        string backupLogPath, overwriteLogPath, copyLogPath, poolLogPath, plantLogPath;
        Progress log;
        //protected string logBackup;
        //protected string logOverwrite;
        //string logCopy;
        //string logPool;
        //Progress logB;
        //Progress logO;
        //Progress logC;
        //Progress logP;
        //backupOverwrite bO;
        //double fullProg, i, j;

        string appServer = "none";
        string localPath;
        string backupPath, path, pathStg, sourcePath;
        string[] servers;
        string[] tempListServer;

        Command myCommand;
        //string[] pools;
        //besok diganti a000s-psstg3
        //testing jerie
        //protected string backupServer = "Z000S-ITPSAP25";
        protected string backupServer = ConfigurationManager.AppSettings["backupServer"].ToString();

        //string[] sourceFolders;
        //string source;
        //string destination;
        //string newPath, spPSS, spWebApps, spWebService, spClosingApps, spRetainPPVUnit;
        //DirectoryInfo di;
        //DirectoryInfo diApps;

        //connection to SQL;
        SqlConnection connection;
        SqlCommand command;
        string sql = null;
        SqlDataReader dataReader;

        //initialize objects
        ExecPowershell eps = new ExecPowershell();
        ServerAndPool sap = new ServerAndPool();
        FileCopy fc = new FileCopy();
        StartStopApplicationPools ssap = new StartStopApplicationPools();
        RenamePath rp = new RenamePath();

        string currentDate = DateTime.Now.ToString("yyMMdd");
        
        private void pss4wButton_Clicked(object sender, EventArgs e)
        {
            if (pss4wButton.Checked == true)
            {
                DialogResult rs1 = MessageBox.Show("yakin?", "yakin?",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);

                if (rs1 == DialogResult.OK)
                {
                    typeStatus = "PSS4W";
                    localPath = ConfigurationManager.AppSettings["localPath_PSS4W"].ToString() + currentDate;
                    //localPath = @"\\A000S-DRRPT1\MigrationUploadData\Abud\Taufiq\Publishing tools\testsource";
                    //testingjerie
                    //localPath = @"C:\Deploy\Source\Publish Prod " + DateTime.Now.ToString("yyMMdd");
                    localPathBox.Text = string.Empty;
                    localPathBox.Text = localPath;
                    appsButton.Enabled = true;
                    webAppsButton.Enabled = true;
                    ssisButton.Enabled = true;
                    copyLocalButton.Enabled = true;
                }
                else if (rs1 == DialogResult.Cancel)
                {
                    typeStatusSwitch();
                }
            }
        }

        private void tamButton_Clicked(object sender, EventArgs e)
        {
            if (tamButton.Checked == true)
            {
                DialogResult rs1 = MessageBox.Show("yakin?", "yakin?",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);

                if (rs1 == DialogResult.OK)
                {
                    typeStatus = "TAM";

                    //localPath = ConfigurationManager.AppSettings["localPath_TAM"].ToString() + currentDate;
                    //localPath = @"\\A000S-DRRPT1\MigrationUploadData\Abud\Taufiq\Publishing tools\testsource";
                    //testingjerie
                    //localPath = @"C:\Deploy\Source\Publish Prod " + DateTime.Now.ToString("yyMMdd");
                    localPathBox.Text = string.Empty;
                    localPathBox.Text = localPath;
                    appsButton.Enabled = true;
                    webAppsButton.Enabled = true;
                    ssisButton.Enabled = true;
                    copyLocalButton.Enabled = false;
                }
                else if (rs1 == DialogResult.Cancel)
                {
                    typeStatusSwitch();
                }
            }
        }

        private void pss2wButton_Clicked(object sender, EventArgs e)
        {
            //hanya untuk web apps
            if (pss2wButton.Checked == true)
            {
                DialogResult rs1 = MessageBox.Show("yakin?", "yakin?",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);

                if (rs1 == DialogResult.OK)
                {
                    typeStatus = "PSS2W";
                    localPath = ConfigurationManager.AppSettings["localPath_PSS2W"].ToString() + currentDate + @"\APP\PSS";
                    //localPath = @"\\A000S-DRRPT1\MigrationUploadData\Abud\Taufiq\Publishing tools\testsource";
                    //testingjerie
                    //localPath = @"C:\BACKUP & PUBLISH NPC\PUBLISH\" + DateTime.Now.ToString("yyMMdd") + @"\APP\PSS";
                    localPathBox.Text = string.Empty;
                    localPathBox.Text = localPath;
                    appsButton.Enabled = false;
                    ssisButton.Enabled = false;
                    webAppsButton.Enabled = true;
                    copyLocalButton.Enabled = true;
                }
                else if (rs1 == DialogResult.Cancel)
                {
                    typeStatusSwitch();
                }
            }
        }

        private void typeStatusSwitch()
        {
            switch (typeStatus)
            {
                case "PSS4W":
                    pss4wButton.Checked = true;
                    break;
                case "TAM":
                    tamButton.Checked = true;
                    break;
                case "PSS2W":
                    pss2wButton.Checked = true;
                    break;
            }
        }

        private void appsButton_Clicked(object sender, EventArgs e)
        {
            backupPath = ConfigurationManager.AppSettings["backupPath_apps"].ToString() + currentDate;
            //backupPath = @"\MigrationUploadData\Abud\Taufiq\Publishing tools\testcopy";
            //path = @"\e$\apps";
            //pathStg = @"\s$\apps";
            //path = @"\MigrationUploadData\Abud\Taufiq\Publishing tools\testpath";
            //pathStg = @"\MigrationUploadData\Abud\Taufiq\Publishing tools\testpathStg";
            localPath = localPath + ConfigurationManager.AppSettings["localPath_apps"].ToString();

            //testingjerie
            //backupPath = @"\c$\BAK\" + DateTime.Now.ToString("yyMMdd");
            path = ConfigurationManager.AppSettings["path_apps"].ToString();
            pathStg = ConfigurationManager.AppSettings["path_appsStg"].ToString();
            //localPath = localPath + @"\PSSApps";

            publishType = "Apps";
            backupPathBox.Text = backupServer + backupPath;
        }

        private void server2Button_CheckedChanged(object sender, EventArgs e)
        {
            appServer = "server2";
        }

        private void server3Button_CheckedChanged(object sender, EventArgs e)
        {
            appServer = "server3";
        }

        private void server4Button_CheckedChanged(object sender, EventArgs e)
        {
            appServer = "server4";
        }

        private void server5Button_CheckedChanged(object sender, EventArgs e)
        {
            appServer = "server5";
        }

        private void stagingButton_CheckedChanged(object sender, EventArgs e)
        {
            appServer = "staging";
        }

        private void webAppsButton_Clicked(object sender, EventArgs e)
        {
            switch (typeStatus)
            {
                case "PSS4W":
                    backupPath = ConfigurationManager.AppSettings["backupPath_PSS4W_webApps"].ToString() + currentDate;
                    //backupPath = @"\MigrationUploadData\Abud\Taufiq\Publishing tools\testcopy";
                    path = ConfigurationManager.AppSettings["path_PSS4W_webApps"].ToString();
                    //path = @"\MigrationUploadData\Abud\Taufiq\Publishing tools\testpath";
                    //testingjerie
                    //backupPath = @"\c$\BAK\" + DateTime.Now.ToString("yyMMdd");
                    //path = @"\c$\_webapps";
                    break;
                case "PSS2W":
                    backupPath = ConfigurationManager.AppSettings["backupPath_PSS2W_webApps"].ToString() + currentDate;
                    //backupPath = @"\MigrationUploadData\Abud\Taufiq\Publishing tools\testcopy";
                    path = ConfigurationManager.AppSettings["path_PSS2W_webApps"].ToString();
                    //path = @"\MigrationUploadData\Abud\Taufiq\Publishing tools\testpath";
                    //testingjerie
                    //backupPath = @"C:\BACKUP & PUBLISH NPC\BACKUP\" + DateTime.Now.ToString("yyMMdd");
                    //path = @"\c$\_webapps";
                    break;
                case "TAM":
                    backupPath = ConfigurationManager.AppSettings["backupPath_TAM_webApps"].ToString() + currentDate;
                    path = ConfigurationManager.AppSettings["path_TAM_webApps"].ToString();
                    //backupPath = @"\c$\BAK\" + DateTime.Now.ToString("yyMMdd");
                    //path = @"\c$\_webapps";
                    break;
            }

            publishType = "Web Apps";
            backupPathBox.Text = backupServer + backupPath;
        }
        
        private void webAppsButton_CheckedChanged(object sender, EventArgs e)
        {
            if (webAppsButton.Checked)
            {
                startPoolsButton.Enabled = true;
                stopPoolsButton.Enabled = true;
            }
            else if (!webAppsButton.Checked)
            {
                startPoolsButton.Enabled = false;
                stopPoolsButton.Enabled = false;
            }
        }

        private void ssisButton_Clicked(object sender, EventArgs e)
        {
            backupPath = ConfigurationManager.AppSettings["backupPath_SSIS"].ToString() + currentDate;
            //backupPath = @"\MigrationUploadData\Abud\Taufiq\Publishing tools\testcopy";
            //testingjerie
            //backupPath = @"\c$\BAK\" + DateTime.Now.ToString("yyMMdd");

            if (typeStatus == "PSS4W")
            {
                path = ConfigurationManager.AppSettings["path_PSS4W_SSIS"].ToString();
                //path = @"\MigrationUploadData\Abud\Taufiq\Publishing tools\testpath";
                //testingjerie
                //path = @"\C$\SynchronizationPackages\Astra";
            }
            else if (typeStatus == "TAM")
            {
                path = ConfigurationManager.AppSettings["path_TAM_SSIS"].ToString();
                //path = @"\MigrationUploadData\Abud\Taufiq\Publishing tools\testpath";
                //testingjerie
                //path = @"\C$\SynchronizationPackages\TAM";
            }
            localPath = localPath + ConfigurationManager.AppSettings["localPath_SSIS"].ToString();
            publishType = "SSIS";
            backupPathBox.Text = backupServer + backupPath;
        }

        private void loadButton_Click(object sender, EventArgs e)
        {
            backupAppsButton.Enabled = true;
            bgwLoadBackup.RunWorkerAsync();
        }


        private void bgwLoadBackup_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                fc.LoadBackupFiles(publishType, sap, ConfigurationManager.ConnectionStrings["connString"].ToString(), typeStatus, stopPoolsButton, dataGridView1, path, backupPath,
                    backupServer, pathStg, localPath, appServer, ssisFileName, sender as BackgroundWorker, backupFileInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error : " + ex.InnerException.Message);
            }
        }

        private void bgwLoadBackup_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //lastActivity.Text = e.UserState as string;
            backupProgBar.Invoke((MethodInvoker)delegate
            {
               backupProgBar.Value = e.ProgressPercentage;
            });
        }


        private void backupAppsButton_Click(object sender, EventArgs e)
        {
            bgwBackup.RunWorkerAsync();
        }

        private void bgwBackup_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                fc.BackupWork(dataGridView1, backupProgBar, typeStatus, publishType, sender as BackgroundWorker);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error : " + ex.InnerException.Message);
            }
        }

        private void bgwBackup_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //dataGridView1.Rows[i].Cells[2].Value = "Success";
            dataGridView1.Rows[Convert.ToInt32(e.UserState)].Cells[2].Value = "Success";
            backupProgBar.Invoke((MethodInvoker)delegate
            {
                backupProgBar.Value = e.ProgressPercentage;
            });
        }

        private void bgwBackup_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                backupLogPath = ConfigurationManager.AppSettings["appsLog"].ToString() + "backupLog " + typeStatus + " " + publishType + ".txt";
                logData = fc.GetLogData();
                log = new Progress(logData, backupLogPath);
                log.Show();
                logData = string.Empty;

                overwriteButton.Enabled = true;
                loadOverwriteButton.Enabled = true;
                lastActivity.Text = "Backup " + typeStatus + " " + publishType;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error : " + ex.InnerException.Message);
            }
        }

        private void server2OWButton_CheckedChanged(object sender, EventArgs e)
        {
            appServer = "server2";
        }

        private void server3OWButton_CheckedChanged(object sender, EventArgs e)
        {
            appServer = "server3";
        }

        private void server4OWButton_CheckedChanged(object sender, EventArgs e)
        {
            appServer = "server4";
        }

        private void server5OWButton_CheckedChanged(object sender, EventArgs e)
        {
            appServer = "server5";
        }

        private void stagingOWButton_CheckedChanged(object sender, EventArgs e)
        {
            appServer = "staging";
        }

        private void loadOverwriteButton_Click(object sender, EventArgs e)
        {
            bgwLoadOverwrite.RunWorkerAsync();
        }

        private void bgwLoadOverwrite_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                fc.LoadOverwriteFiles(dataGridView2, publishType, typeStatus, localPath, path, pathStg, appServer, sender as BackgroundWorker);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error : " + ex.InnerException.Message);
            }
        }

        private void bgwLoadOverwrite_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            lastActivity.Text = e.UserState as string;
        }

        private void overwriteButton_Click(object sender, EventArgs e)
        {
            DialogResult rs1 = MessageBox.Show("yakin?", "yakin?",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);
            if (rs1 == DialogResult.Cancel)
            {
                return;
            }
            bgwOverwrite.RunWorkerAsync();
        }

        private void bgwOverwrite_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                fc.OverwriteWork(dataGridView2, overwriteProgBar, typeStatus, publishType, sender as BackgroundWorker);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error : " + ex.InnerException.Message);
            }
        }

        private void bgwOverwrite_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //dataGridView2.Rows[i].Cells[2].Value = "Success";
            dataGridView2.Rows[Convert.ToInt32(e.UserState)].Cells[2].Value = "success";
            overwriteProgBar.Invoke((MethodInvoker)delegate
            {
                overwriteProgBar.Value = e.ProgressPercentage;
            });
        }

        private void bgwOverwrite_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                overwriteLogPath = ConfigurationManager.AppSettings["appsLog"].ToString() + "overwriteLog " + typeStatus + " " + publishType + ".txt";
                logData = fc.GetLogData();
                log = new Progress(logData, overwriteLogPath);
                log.Show();
                logData = string.Empty;
                lastActivity.Text = "Overwrite " + typeStatus + " " + publishType;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error : " + ex.InnerException.Message);
            }
        }

        private void webAppsOWButton_Click(object sender, EventArgs e)
        {
            logData = string.Empty;
            bgwOWWebApps.RunWorkerAsync();
        }


        private void bgwOWWebApps_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {

                if (typeStatus == "PSS4W")
                {
                    //copy from local to deployFolder
                    string destination = @"\\A000S-ITPSAP03\c$\Deploy\Source\Web\";
                    File.Copy(localPath, destination, true);
                    string destinationWS = @"\\A000S-ITPSAP03\c$\Deploy\Source\WS\";
                    File.Copy(localPath, destinationWS, true);
                }

                //exec script
                RunspaceConfiguration runspaceConfiguration = RunspaceConfiguration.Create();
                Runspace runspace = RunspaceFactory.CreateRunspace(runspaceConfiguration);
                runspace.Open();
                RunspaceInvoke scriptInvoker = new RunspaceInvoke(runspace);
                Pipeline pipeline = runspace.CreatePipeline();

                if (typeStatus == "PSS4W")
                {
                    //Here's how you add a new script with arguments
                    //myCommand = new Command(@"\\a000s-drrpt1\MigrationUploadData\Abud\Taufiq\Publishing tools\OWWebApps.ps1");
                    //testing
                    myCommand = new Command(@"\\a000s-drrpt1\MigrationUploadData\Abud\Taufiq\Publishing tools\OWWebApps - testing.ps1");
                    //CommandParameter a = new CommandParameter("server", servers);
                    //CommandParameter b = new CommandParameter("appPool", pools);
                    //CommandParameter c = new CommandParameter("type", type);
                    //myCommand.Parameters.Add(a);
                    //myCommand.Parameters.Add(b);
                    //myCommand.Parameters.Add(c);
                }
                else if (typeStatus == "PSS2W")
                {
                    //testing
                    //myCommand = new Command(@"\\a000s-drrpt1\MigrationUploadData\Abud\Taufiq\Publishing tools\OWWebApps.ps1");
                    CommandParameter a = new CommandParameter("SourcePath", localPath);
                    myCommand.Parameters.Add(a);
                }
                pipeline.Commands.Add(myCommand);

                // Execute PowerShell script
                Collection<PSObject> results = pipeline.Invoke();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error : " + ex.InnerException.Message);
            }
        }

        private void bgwOWWebApps_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            overwriteLogPath = ConfigurationManager.AppSettings["appsLog"].ToString() + "overwriteLog " + typeStatus + " " + publishType + ".txt";
            logData = "Overwrited Web Apps " + typeStatus;
            log = new Progress(logData, overwriteLogPath);
            log.Show();
            logData = string.Empty;
            lastActivity.Text = "Overwrited Web Apps " + typeStatus;
        }

        private void ssisOWButton_Click(object sender, EventArgs e)
        {
            //belum
        }

        private void copyLocalButton_Click(object sender, EventArgs e)
        {
            sourcePath = sourceBox.Text;
            bgwCopyToLocal.RunWorkerAsync();
        }

        private void bgwCopyToLocal_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                fc.CopyLocalWork(sourcePath, localPath, copyLocalProgBar, typeStatus, sender as BackgroundWorker);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error : " + ex.InnerException.Message);
            }
        }

        private void bgwCopyToLocal_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            copyLocalProgBar.Invoke((MethodInvoker)delegate
            {
                copyLocalProgBar.Value = e.ProgressPercentage;
            });
        }

        private void bgwCopyToLocal_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                copyLogPath = ConfigurationManager.AppSettings["appsLog"].ToString() + "copyLocalLog " + typeStatus + ".txt";
                logData = fc.GetLogData();
                log = new Progress(logData, copyLogPath);
                log.Show();
                logData = string.Empty;

                rp.renamePathRule(localPath);
                publishTypeBox.Enabled = true;
                loadButton.Enabled = true;
                lastActivity.Text = "Copy to local " + typeStatus + " " + publishType;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error : " + ex.InnerException.Message);
            }
        }

        private void stopPoolsButton_Click(object sender, EventArgs e)
        {
            DialogResult rs1 = MessageBox.Show("yakin?", "yakin?",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);
            if (rs1 == DialogResult.Cancel)
            {
                return;
            }
            try
            {
                logData = string.Empty;
                ssap.startStopPool(sap, ConfigurationManager.ConnectionStrings["connString"].ToString(), typeStatus,
                    publishType, ConfigurationManager.AppSettings["poolPowershell"].ToString(), "stop");
                startPoolsButton.Enabled = true;
                stopProgressBar.Value = 100;

                poolLogPath = ConfigurationManager.AppSettings["appsLog"].ToString() + "stopPoolsLog.txt";
                logData = ssap.GetLogData();

                //write to event viewer
                //EventLog appLog = new EventLog();
                //appLog.Source = "Application Pool";
                //appLog.WriteEntry("These application pools are stopped: " + logData + ".");

                log = new Progress(logData, poolLogPath);
                log.Show();
                logData = string.Empty;
                lastActivity.Text = "Stop pool " + typeStatus + " " + publishType;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error : " + ex.InnerException.Message);
            }
        }

        private void startPoolsButton_Click(object sender, EventArgs e)
        {
            try
            {
                logData = string.Empty;
                ssap.startStopPool(sap, ConfigurationManager.ConnectionStrings["connString"].ToString(), typeStatus,
                    publishType, ConfigurationManager.AppSettings["poolPowershell"].ToString(), "start");
                startProgressBar.Value = 100;
                poolLogPath = ConfigurationManager.AppSettings["appsLog"].ToString() + "startPoolsLog.txt";
                logData = ssap.GetLogData();

                log = new Progress(logData, poolLogPath);
                log.Show();
                logData = string.Empty;
                lastActivity.Text = "Start pool " + typeStatus + " " + publishType;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error : " + ex.InnerException.Message);
            }
        }

        private void sourceBox_TextChanged(object sender, EventArgs e)
        {
            copyLocalButton.Enabled = true;
        }

        private void refreshInfoButton_Click(object sender, EventArgs e)
        {
            infoLocalPath.Text = localPath;
        }

        private void refreshInfoButton2_Click(object sender, EventArgs e)
        {
            infoBackupPath.Text = backupPath;
        }

        private void refreshInfoButton3_Click(object sender, EventArgs e)
        {
            infoPath.Text = path;
        }

        private void backupWebAppsButton_Click(object sender, EventArgs e)
        {
            bwebapp.RunWorkerAsync(); 
        }

        private void backupAppsButtonN_Click(object sender, EventArgs e)
        {
            bapp.RunWorkerAsync();
        }

        private void PublishWebAppsButton_Click(object sender, EventArgs e)
        {
            pwebapp.RunWorkerAsync();
        }

        private void publishAppsButton_Click(object sender, EventArgs e)
        {
            papp.RunWorkerAsync();
        }

        private void cancelbwebapp_Click(object sender, System.EventArgs e)
        {
            if (bwebapp.IsBusy)
            {
                bwebapp.CancelAsync();
                progressUpdate.Text = "Cancelled";
            }
        }

        private void cancelpwebapp_Click(object sender, System.EventArgs e)
        {
            if (pwebapp.IsBusy)
            {
                pwebapp.CancelAsync();
                progressUpdate.Text = "Cancelled";
            }
        }

        private void cancelbapp_Click(object sender, System.EventArgs e)
        {
            if (bapp.IsBusy)
            {
                bapp.CancelAsync();
                progressUpdate.Text = "Cancelled";
            }
        }

        private void cancelpapp_Click(object sender, System.EventArgs e)
        {
            if (papp.IsBusy)
            {
                papp.CancelAsync();
                progressUpdate.Text = "Cancelled";
            }
        }

        private void bwebapp_DoWork(object sender, DoWorkEventArgs e)
        {
            progressUpdate.Invoke((MethodInvoker)delegate
            {
                progressUpdate.Text = "Running powershell file ...";
            });
            if (typeStatus == "PSS4W")
            {
                string[] sourceFolders = new string[] { "pss4w", "tdms", "pss4wservice", "tdmsservice" };
                connection = new SqlConnection(constring);
                connection.Open();
                foreach (string src in sourceFolders)
                {
                    sql = @"UPDATE DBA.dbo.PublishPath SET BackupPath = '\\"
                        + backupServer
                        + ConfigurationManager.AppSettings["backupPath_PSS4W_webApps"].ToString()
                        + currentDate + @"\Web Apps\" + src
                        + "' WHERE ProdPath like '%" + src + "' and IsBackedUp = 1";
                    command = new SqlCommand(sql, connection);
                    command.ExecuteNonQuery();
                }
                connection.Close();
                //sap.getServer(constring, typeStatus, publishType);
                //string[] servers = sap.Getservers();

                string query = "select prodpath,backuppath from publishpath where psstype = 'pss4w' and publishtype = 'web apps' and isbackedup = 1";
                //string query = "select top 1 prodpath,backuppath from publishpath where psstype = 'pss4w' and publishtype = 'web apps' and isbackedup = 1";
                eps.execToPowerShell("A000S-ITPSAP03", query, @"\\a000s-drrpt1\MigrationUploadData\Abud\Taufiq\Publishing tools\backuppss4wwa.ps1");
                //eps.execToPowerShell("A000S-ITPSAP03", query, @"E:\Script\backuppss4wwa.ps1");
            }
            else if (typeStatus == "PSS2W")
            {
                string[] sourceFolders = new string[] { "psshsoho", "psshsom29", "pss2w", "pss2w1" };
                connection = new SqlConnection(constring);
                connection.Open();
                foreach (string src in sourceFolders)
                {
                    sql = @"UPDATE DBA.dbo.PublishPath SET BackupPath = '"
                        + ConfigurationManager.AppSettings["backupPath_PSS2W_webApps"].ToString()
                        + currentDate + @"\" + src
                        + "' WHERE ProdPath like '%" + src + "' and isbackedup = 1";
                    command = new SqlCommand(sql, connection);
                    command.ExecuteNonQuery();
                }
                connection.Close();
                //sap.getServer(constring, typeStatus, publishType);
                //string[] servers = sap.Getservers();

                string query = "select prodpath,backuppath from publishpath where psstype = 'pss2w' and publishtype = 'web apps' and isbackedup = 1";
                eps.execToPowerShell("A000S-ITPSAP15", query, @"\\a000s-drrpt1\MigrationUploadData\Abud\Taufiq\Publishing tools\backuppss4wwa.ps1");
                
            }
            else if (typeStatus == "TAM")
            {
                string[] sourceFolders = new string[] { "TAM","Webservice_TAM" };
                //string[] sourceServers = new string[] { "PSSAPT1", "PSSAPT3" };
                connection = new SqlConnection(constring);
                connection.Open();
                //foreach (string srv in sourceServers)
                //{
                    foreach (string src in sourceFolders)
                    {
                        sql = @"UPDATE DBA.dbo.PublishPath SET BackupPath = '\\"
                            + backupServer
                            + ConfigurationManager.AppSettings["backupPath_TAM_webApps"].ToString()
                            + currentDate + @"\Web Apps\" + src
                            + "' WHERE ProdPath like '%" + src + "' and IsBackedUp = 1";
                        command = new SqlCommand(sql, connection);
                        command.ExecuteNonQuery();
                    }
                //}
                connection.Close();
                string query = "select prodpath,backuppath from publishpath where psstype = 'tam' and publishtype = 'web apps' and isbackedup = 1";
                eps.execToPowerShell("A000S-PSSAPT1", query, @"\\a000s-drrpt1\MigrationUploadData\Abud\Taufiq\Publishing tools\backuppss4wwa.ps1");
            }
        }

        private void bwebapp_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressUpdate.Text = "Done";
        }

        private void pwebapp_DoWork(object sender, DoWorkEventArgs e)
        {
            progressUpdate.Invoke((MethodInvoker)delegate
            {
                progressUpdate.Text = "Running powershell file ...";
            });
            sap.getServer(constring, typeStatus, publishType);
            servers = sap.Getservers();
            if (typeStatus == "PSS4W")
            {
                
                connection = new SqlConnection(constring);
                connection.Open();

                //update webapp
                sql = @"UPDATE DBA.dbo.PublishPath SET LocalPath = '"
                    + ConfigurationManager.AppSettings["localPath_PSS4W"].ToString()
                    + currentDate + @"\Publish Dev"
                    + "' WHERE IsWebApp = 1";
                command = new SqlCommand(sql, connection);
                command.ExecuteNonQuery();

                //update webservice
                sql = @"UPDATE DBA.dbo.PublishPath SET LocalPath = '"
                    + ConfigurationManager.AppSettings["localPath_PSS4W"].ToString()
                    + currentDate + @"\Web Service"
                    + "' WHERE IsWebService = 1";
                command = new SqlCommand(sql, connection);
                command.ExecuteNonQuery();

                connection.Close();

                string query = "select localpath, prodpath from publishpath where psstype = 'pss4w' and publishtype = 'web apps' and (IsWebApp = 1 or IsWebService = 1)";
                eps.execToPowerShell(servers, query, @"\\a000s-drrpt1\MigrationUploadData\Abud\Taufiq\Publishing tools\publishpss4wwa.ps1");
                //string[] test = {"A000S-ITPSAP02", "A000S-ITPSAP04"};
                //eps.execToPowerShell(test, query, @"\\a000s-drrpt1\MigrationUploadData\Abud\Taufiq\Publishing tools\publishpss4wwa.ps1");
                
            }
            else if (typeStatus == "PSS2W")
            {
                connection = new SqlConnection(constring);
                connection.Open();

                //update webapp
                sql = @"UPDATE DBA.dbo.PublishPath SET LocalPath = '"
                    + ConfigurationManager.AppSettings["localPath_PSS2W"].ToString()
                    + currentDate + @"\APP\PSS"
                    + "' WHERE psstype = 'PSS2W' and publishtype = 'Web Apps'";
                command = new SqlCommand(sql, connection);
                command.ExecuteNonQuery();

                connection.Close();
                //sap.getServer(constring, typeStatus, publishType);
                //string[] servert = {"A000S-ITPSAP15", "A000S-ITPSAP14" };
                string query = "select localpath, prodpath from publishpath where psstype = 'pss2w' and publishtype = 'web apps'";
                eps.execToPowerShell(servers, query, @"\\a000s-drrpt1\MigrationUploadData\Abud\Taufiq\Publishing tools\publishpss4wwa.ps1");

            }
            else if (typeStatus == "TAM")
            {
                connection = new SqlConnection(constring);
                connection.Open();
                currentDate = DateTime.Now.AddDays(-1).ToString("yyMMdd");


                //update webapp
                sql = @"UPDATE DBA.dbo.PublishPath SET LocalPath = '"
                    + ConfigurationManager.AppSettings["localPath_PSS4W"].ToString()
                    + currentDate + @"\Publish Dev"
                    + "' WHERE IsWebApp = 1";
                command = new SqlCommand(sql, connection);
                command.ExecuteNonQuery();

                //update webservice
                sql = @"UPDATE DBA.dbo.PublishPath SET LocalPath = '"
                    + ConfigurationManager.AppSettings["localPath_PSS4W"].ToString()
                    + currentDate + @"\Web Service"
                    + "' WHERE IsWebService = 1";
                command = new SqlCommand(sql, connection);
                command.ExecuteNonQuery();

                connection.Close();

                string query = "select localpath, prodpath from publishpath where psstype = 'tam' and publishtype = 'web apps' and (IsWebApp = 1 or IsWebService = 1)";
                eps.execToPowerShell(servers, query, @"\\a000s-drrpt1\MigrationUploadData\Abud\Taufiq\Publishing tools\publishpss4wwa.ps1");
            }
        }

        private void pwebapp_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressUpdate.Text = "Done";
        }

        private void bapp_DoWork(object sender, DoWorkEventArgs e)
        {
            progressUpdate.Invoke((MethodInvoker)delegate
            {
                progressUpdate.Text = "Running powershell file ...";
            });
            if (typeStatus == "PSS4W")
            {
                string[] sourceFolders = new string[] { "Astra.Pss.MailingDownload", "Astra.Pss.ReportAutoDownload", 
                    "Astra.Pss.RetainPPVUnit", "PSS4W.ClosingApps", "Astra.Pss.PssConsoleApps", "PSS4W.ReClosingApps", 
                    "Astra.Pss.Scheduling.BusinessLib" };
                //string[] sourceServers = new string[] { "ITPSAP02", "ITPSAP03", "ITPSAP04", "ITPSAP05", "PSSTG1" };
                string[] sourceServers = new string[] { "ITPSAP03", "ITPSAP04", "ITPSAP05", "PSSTG1" };
                connection = new SqlConnection(constring);
                connection.Open();
                foreach (string srv in sourceServers)
                {
                    foreach (string src in sourceFolders)
                    {
                        sql = @"UPDATE DBA.dbo.PublishPath SET BackupPath = '\\"
                            + backupServer
                            + ConfigurationManager.AppSettings["backupPath_PSS4W_webApps"].ToString()
                            + currentDate + @"\Apps\" + srv + @"\" + src
                            + "' WHERE ProdPath like '%" + srv + "%' and ProdPath like '%" + src + "'";
                        command = new SqlCommand(sql, connection);
                        command.ExecuteNonQuery();
                    }
                }
                connection.Close();
                //sap.getServer(constring, typeStatus, publishType);
                //string[] servers = sap.Getservers();

                string query = "select prodpath,backuppath from publishpath where psstype = 'pss4w' and publishtype = 'apps'";
                eps.execToPowerShell(query, @"\\a000s-drrpt1\MigrationUploadData\Abud\Taufiq\Publishing tools\backuppss4wapp.ps1");
            }
            else if (typeStatus == "TAM")
            {
                string[] sourceFolders = new string[] { "Astra.Pss.MailingDownload", "Astra.Pss.ReportAutoDownload",
                    "Astra.Pss.RetainPPVUnit", "PSS4W.ClosingApps", "Astra.Pss.PssConsoleApps", "PSS4W.ReClosingApps",
                    "Astra.Pss.Scheduling.BusinessLib" };
                //string[] sourceServers = new string[] { "ITPSAP02", "ITPSAP03", "ITPSAP04", "ITPSAP05", "PSSTG1" };
                string[] sourceServers = new string[] { "PSSAPT1", "PSSAPT3" };
                connection = new SqlConnection(constring);
                connection.Open();
                foreach (string srv in sourceServers)
                {
                    foreach (string src in sourceFolders)
                    {
                        sql = @"UPDATE DBA.dbo.PublishPath SET BackupPath = '"
                             + ConfigurationManager.AppSettings["backupPath_TAM_webApps"].ToString()
                             + currentDate + @"\Apps\" + srv + @"\" + src
                            + "' WHERE ProdPath like '%" + srv + "%' and ProdPath like '%" + src + "'";
                        command = new SqlCommand(sql, connection);
                        command.ExecuteNonQuery();
                    }
                }
                connection.Close();

                string query = "select prodpath,backuppath from publishpath where psstype = 'tam' and publishtype = 'apps'";
                eps.execToPowerShell(query, @"\\a000s-drrpt1\MigrationUploadData\Abud\Taufiq\Publishing tools\backuppss4wapp.ps1");
            }
            
        }

        private void bapp_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressUpdate.Text = "Done";
        }

        private void papp_DoWork(object sender, DoWorkEventArgs e)
        {
            progressUpdate.Invoke((MethodInvoker)delegate
            {
                progressUpdate.Text = "Running powershell file ...";
            });
            if (typeStatus == "PSS4W")
            {
                string[] sourceFolders = new string[] { "Astra.Pss.MailingDownload", "Astra.Pss.ReportAutoDownload",
                    "Astra.Pss.RetainPPVUnit", "PSS4W.ClosingApps", "Astra.Pss.PssConsoleApps", "PSS4W.ReClosingApps",
                    "Astra.Pss.Scheduling.BusinessLib" };
                //string[] sourceServers = new string[] { "ITPSAP02", "ITPSAP03", "ITPSAP04", "ITPSAP05", "PSSTG1" };
                string[] sourceServers = new string[] { "ITPSAP03", "ITPSAP04", "ITPSAP05", "PSSTG1" };
                connection = new SqlConnection(constring);
                connection.Open();
                foreach (string srv in sourceServers)
                {
                    foreach (string src in sourceFolders)
                    {
                        sql = @"UPDATE DBA.dbo.PublishPath SET LocalPath = '"
                             + ConfigurationManager.AppSettings["localPath_PSS4W"].ToString()
                             + currentDate + @"\PSSApps\" + src
                             + "' WHERE ProdPath like '%" + srv + "%' and ProdPath like '%" + src + "'";
                        command = new SqlCommand(sql, connection);
                        command.ExecuteNonQuery();
                    }
                }
                connection.Close();

                string query = "select localpath, prodpath from publishpath where psstype = 'pss4w' and publishtype = 'apps'";
                eps.execToPowerShell(query, @"\\a000s-drrpt1\MigrationUploadData\Abud\Taufiq\Publishing tools\publishpss4wa.ps1");
            }
            else if (typeStatus == "TAM")
            {
                currentDate = DateTime.Now.AddDays(-1).ToString("yyMMdd");
                string[] sourceFolders = new string[] { "Astra.Pss.MailingDownload", "Astra.Pss.ReportAutoDownload",
                    "Astra.Pss.RetainPPVUnit", "PSS4W.ClosingApps", "Astra.Pss.PssConsoleApps", "PSS4W.ReClosingApps",
                    "Astra.Pss.Scheduling.BusinessLib" };
                //string[] sourceServers = new string[] { "ITPSAP02", "ITPSAP03", "ITPSAP04", "ITPSAP05", "PSSTG1" };
                string[] sourceServers = new string[] { "PSSAPT1", "PSSAPT3" };
                connection = new SqlConnection(constring);
                connection.Open();
                foreach (string srv in sourceServers)
                {
                    foreach (string src in sourceFolders)
                    {
                        sql = @"UPDATE DBA.dbo.PublishPath SET LocalPath = '"
                             + ConfigurationManager.AppSettings["localPath_PSS4W"].ToString()
                             + currentDate + @"\PSSApps\" + src
                             + "' WHERE ProdPath like '%" + srv + "%' and ProdPath like '%" + src + "'";
                        command = new SqlCommand(sql, connection);
                        command.ExecuteNonQuery();
                    }
                }
                connection.Close();

                string query = "select localpath, prodpath from publishpath where psstype = 'tam' and publishtype = 'apps'";
                eps.execToPowerShell(query, @"\\a000s-drrpt1\MigrationUploadData\Abud\Taufiq\Publishing tools\publishpss4wa.ps1");
            }
        }

        private void papp_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressUpdate.Text = "Done";
        }

        private void exportToCSV_Click(object sender, System.EventArgs e)
        {
            string publishLogLocation = @"\\a000s-drrpt1\MigrationUploadData\Abud\Taufiq\Publishing tools\PublishLog " +
                DateTime.Now.ToString("yyMMdd HHmm") + ".csv";
            SaveDataGridViewToCSV(dgvPublishLog, publishLogLocation);
        }

        private void exportToCSV2_Click(object sender, System.EventArgs e)
        {
            string publishLogLocation = @"\\a000s-drrpt1\MigrationUploadData\Abud\Taufiq\Publishing tools\ExecScriptLog " +
                DateTime.Now.ToString("yyMMdd HHmm") + ".csv";
            SaveDataGridViewToCSV(dgvDBlog, publishLogLocation);
        }

        public void SaveDataGridViewToCSV(DataGridView dgv, string filename)
        {
            try
            {
                // Save the current state of the clipboard so we can restore it after we are done
                IDataObject objectSave = Clipboard.GetDataObject();

                // Choose whether to write header. Use EnableWithoutHeaderText instead to omit header.
                dgv.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
                // Select all the cells
                dgv.SelectAll();
                // Copy (set clipboard)
                Clipboard.SetDataObject(dgvPublishLog.GetClipboardContent());
                // Paste (get the clipboard and serialize it to a file)
                File.WriteAllText(filename, Clipboard.GetText(TextDataFormat.CommaSeparatedValue));

                // Restore the current state of the clipboard so the effect is seamless
                if (objectSave != null) // If we try to set the Clipboard to an object that is null, it will throw...
                {
                    Clipboard.SetDataObject(objectSave);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex);
            }
        }

        #endregion

        #region databases
        //--------------------------------------------------------------------
        //                             tab Database
        //--------------------------------------------------------------------

        Constring connectsDB;
        Constring connects;
        //FileInfo file;
        public Server cons = new Server();
        public int flag, flagitemchecked;
        Collection<string> collCheckItem = new Collection<string>();
        string selectedOption;
        string selectedServer;
        string grant2Wpath;
        //int lastPoint;
        //int startPoint;
        //int endPoint = 999999;
        //string[] checkedDB;
        ArrayList checkedDB;
        ExecSQLScript ess = new ExecSQLScript();
        string[] exts = {"*.sql","*.txt"};

        private void ListServer()
        {
            //string[] tempListServer = ConfigurationSettings.AppSettings["ListServer"].Split(';');

            connection = new SqlConnection(ConfigurationManager.ConnectionStrings["connString"].ToString());

            connection.Open();
            sql = "select count(*) from ServerCategory G join Server S on G.ID = S.ServerCategoryID where G.ServerCategoryName = 'DB'";
            command = new SqlCommand(sql, connection);
            dataReader = command.ExecuteReader();
            dataReader.Read();
            int s = Convert.ToInt32(dataReader.GetValue(0));
            tempListServer = new string[s];
            dataReader.Close();
            command.Dispose();
            //connection.Close();

            sql = "select S.ServerName from ServerCategory G join Server S on G.ID = S.ServerCategoryID where G.ServerCategoryName = 'DB' order by S.ServerName";

            try
            {
                //connection.Open();
                command = new SqlCommand(sql, connection);
                dataReader = command.ExecuteReader();
                int i = 0;
                while (dataReader.Read())
                {
                    //MessageBox.Show(dataReader.GetValue(0) + " - " + dataReader.GetValue(1) + " - " + dataReader.GetValue(2));
                    tempListServer[i] = Convert.ToString(dataReader.GetValue(0));
                    //testingjerie
                    //tempListServer[i] = "A000S-PSSCAUDIT";
                    //tempListServer[i] = "A000S-ITSQL12";
                    //tempListServer[i] = "A000S-DRPSTES01";
                    i++;
                    //MessageBox.Show(Convert.ToString(dataReader.GetValue(0)));
                }
                dataReader.Close();
                command.Dispose();
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Can not open connection ! " + ex.InnerException.Message);
            }

            if (tempListServer.Length == 0)
            {
                MessageBox.Show("Please configure List server at config first");
                return;
            }

            for (int i = 0; i < tempListServer.Length; i++)
            {
                ddlServer.Items.Add(tempListServer[i]);
            }
        }

        private void ListAuthentication()
        {
            cboAuthentication.Items.Add("Windows Authentication");
            cboAuthentication.Items.Add("SQL Server");
            cboAuthentication.SelectedIndex = 0;
        }

        private void ListDatabase()
        {

            string[] tempListDB = ConfigurationManager.AppSettings["ListDatabase"].Split(';');


            if (tempListDB.Length == 0)
            {
                MessageBox.Show("Please configure List DB at config first");
                return;
            }


            for (int i = 0; i < tempListDB.Length; i++)
            {
                ddlDatabase.Items.Add(tempListDB[i]);
            }
        }

        private void cboAuthentication_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (cboAuthentication.Items[cboAuthentication.SelectedIndex].ToString().Contains("Windows"))
            {
                txtUsername.Enabled = false;
                txtPassword.Enabled = false;
            }
            else
            {
                txtUsername.Enabled = true;
                txtPassword.Enabled = true;
            }
        }

        private bool PageIsValid(ref string message)
        {

            if (string.IsNullOrEmpty(ddlServer.Text))
            {
                message = "Please select the server first";
                return false;
            }


            if (string.IsNullOrEmpty(txtUsername.Text))
            {
                message = "UserName Can't be Empty";
                return false;
            }

            if (string.IsNullOrEmpty(txtPassword.Text))
            {
                message = "Password Can't Be empty";
                return false;
            }
            return true;
        }

        private void ddlServer_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlServer.Text == "Other")
            {
                otherServer.Enabled = true;
            }
            else
            {
                otherServer.Enabled = false;
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            string message = string.Empty;
            string err = string.Empty;

            if (cboAuthentication.Items[cboAuthentication.SelectedIndex].ToString().Contains("Windows"))
            {
                if (string.IsNullOrEmpty(ddlServer.Text) || 
                    (ddlServer.Text == "Other" && string.IsNullOrEmpty(otherServer.Text)))
                {
                    MessageBox.Show("Please select the server first");
                    return;
                }

                if (ddlServer.Text == "Other")
                {
                    selectedServer = otherServer.Text;
                }
                else
                {
                    selectedServer = ddlServer.Text;
                    //connects = new Constring("A000S-ITPSSQ02");
                }

                connects = new Constring(selectedServer);
            }
            else
            {
                if (!PageIsValid(ref message))
                {
                    MessageBox.Show(message);
                    return;
                }

                if (ddlServer.Text == "Other")
                {
                    connects = new Constring(txtUsername.Text, txtPassword.Text, otherServer.Text);
                }
                else
                {
                    connects = new Constring(txtUsername.Text, txtPassword.Text, ddlServer.Text);
                }
            }
            try
            {
                cons = connects.Connect();
                chkDatabase.Items.Clear();
                List<string> collDBScort = connects.GetListDBSort(ref err);
                connectsDB = connects;

                for (int i = 0; i < collDBScort.Count; i++)
                {
                    chkDatabase.Items.Add(collDBScort[i]);
                }

                if (!string.IsNullOrEmpty(err))
                {
                    MessageBox.Show(err);
                    return;
                }
                ddlDatabase.SelectedIndex = 0;
                ddlDatabase.Enabled = true;

            }
            catch (ExecutionFailureException er)
            {
                MessageBox.Show(er.InnerException.Message);
            }

        }

        private void ddlDatabase_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string err = string.Empty;
                List<string> collDBScort = connectsDB.GetListDBSort(ref err, ddlDatabase.SelectedIndex);
                PopulateDB(collDBScort);
                if (!string.IsNullOrEmpty(err))
                {
                    MessageBox.Show(err);
                }
                //BusinessFacade facade = new BusinessFacade();
                //if (!string.IsNullOrEmpty(txtPath.Text))
                //{
                //    string[] filePaths = Directory.GetFiles(txtPath.Text.Trim(), "*.sql", SearchOption.AllDirectories);
                //    List<string> lstFullPath = facade.GetListSortFullPathFiles(filePaths, ddlDatabase.Text);
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error : " + ex.InnerException.Message);
            }

        }

        private void PopulateDB(List<string> lst)
        {
            try
            {
                chkDatabase.Items.Clear();
                for (int i = 0; i < lst.Count; i++)
                {
                    chkDatabase.Items.Add(lst[i]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error : " + ex.InnerException.Message);
            }
        }

        private void selectAll_CheckedChanged(object sender, EventArgs e)
        {
            //if (ddlDatabase.Text.Equals("-Select-") || ddlDatabase.Text.Equals("") && (chkDatabase.SelectedItems.Count == 0))
            ////if (chkDatabase.SelectedItems.Count == 0)
            //{
            //    MessageBox.Show("Please Fill the database first");
            //    return;
            //}

            if (selectAll.Checked)
            {
                for (int i = 0; i < chkDatabase.Items.Count; i++)
                {
                    chkDatabase.SetItemChecked(i, true);
                }
            }
            else if (!selectAll.Checked)
            {
                for (int i = 0; i < chkDatabase.Items.Count; i++)
                {
                    chkDatabase.SetItemChecked(i, false);
                }
            }
        }

        private void btnPopulate_Click(object sender, EventArgs e)
        {
            string path = txtPath.Text;

            if (ddlDatabase.Text.Equals("-Select-") || ddlDatabase.Text.Equals("") && (chkDatabase.SelectedItems.Count == 0))
            //if (chkDatabase.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please Fill the database first");
                return;
            }

            if (!string.IsNullOrEmpty(path))
            {
                try
                {
                    btnGenerateScript.Enabled = true;
                    execOptions.Enabled = true;

                    string[] filePaths;
                    BusinessFacade facade = new BusinessFacade();
                    List<string> lstSortedFiles = new List<string>();
                    //lstDrive.Items.Clear();
                    dgvListProcess.Rows.Clear();
                    foreach (string ext in exts)
                    {
                        filePaths = Directory.GetFiles(path.Trim(), ext, SearchOption.AllDirectories);

                        //string[] filePaths = Directory.GetFiles(path.Trim(), "*.sql", SearchOption.AllDirectories);
                        if (!ddlDatabase.Text.Equals("All"))
                            lstSortedFiles = facade.GetListSortFullPathFiles(filePaths, ddlDatabase.Text);
                        else
                            lstSortedFiles = facade.GetListSortFullPathFiles(filePaths);

                        //for (int i = 0; i < lstSortedFiles.Count; i++)
                        //{
                        //    lstDrive.Items.Add(lstSortedFiles[i]);
                        //}
                        dgvListProcess.Rows.Add(lstSortedFiles.Count);
                        for (int i = 0; i < lstSortedFiles.Count; i++)
                        {
                            //lstDrive.Items.Add(lstSortedFiles[i]);
                            dgvListProcess.Rows[i].Cells[1].Value = lstSortedFiles[i];
                            dgvListProcess.Rows[i].Cells[0].Value = "";
                        }
                    }

                    if (lstSortedFiles.Count == 0)
                    {
                        //filePaths = Directory.GetFiles(path.Trim(), "*.txt", SearchOption.AllDirectories);
                        MessageBox.Show("There is no script suitable.");
                        return;
                    }
                    //Add To datagridview
                    //DataGridViewRow row1 = (DataGridViewRow)dgvListProcess.Rows[0].Clone();
                    
                    flag = dgvListProcess.Rows.Count;
                    //reset lastpoint
                    //lastPoint = 0;
                }
                catch (IOException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void btnPopulateOth_Click(object sender, EventArgs e)
        {
            string path = txtPath.Text;

            if (ddlDatabase.Text.Equals("-Select-") || ddlDatabase.Text.Equals("") && (chkDatabase.SelectedItems.Count == 0))
            //if (chkDatabase.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please Fill the database first");
                return;
            }

            if (!string.IsNullOrEmpty(path))
            {
                try
                {
                    btnGenerateScript.Enabled = true;
                    execOptions.Enabled = true;

                    string[] filePaths;
                    BusinessFacade facade = new BusinessFacade();
                    List<string> lstSortedFiles = new List<string>();
                    //lstDrive.Items.Clear();
                    dgvListProcess.Rows.Clear();
                    foreach (string ext in exts)
                    {
                        filePaths = Directory.GetFiles(path.Trim(), ext, SearchOption.AllDirectories);
                        //string[] filePaths = Directory.GetFiles(path.Trim(), "*.sql", SearchOption.AllDirectories);
                        if (!ddlDatabase.Text.Equals("All"))
                            lstSortedFiles = facade.GetOthListSortFullPathFiles(filePaths, ddlDatabase.Text);
                        else
                            lstSortedFiles = facade.GetListSortFullPathFiles(filePaths);

                        //for (int i = 0; i < lstSortedFiles.Count; i++)
                        //{
                        //    lstDrive.Items.Add(lstSortedFiles[i]);
                        //}
                        dgvListProcess.Rows.Add(lstSortedFiles.Count);
                        for (int i = 0; i < lstSortedFiles.Count; i++)
                        {
                            //lstDrive.Items.Add(lstSortedFiles[i]);
                            dgvListProcess.Rows[i].Cells[1].Value = lstSortedFiles[i];
                            dgvListProcess.Rows[i].Cells[0].Value = "";
                        }
                    }

                    
                    if (lstSortedFiles.Count == 0)
                    {
                        //filePaths = Directory.GetFiles(path.Trim(), "*.txt", SearchOption.AllDirectories);
                        if (lstSortedFiles.Count == 0)
                        {
                            MessageBox.Show("There is no script suitable.");
                            return;
                        }
                    }
                    //Add To datagridview
                    //DataGridViewRow row1 = (DataGridViewRow)dgvListProcess.Rows[0].Clone();

                    flag = dgvListProcess.Rows.Count;
                    //reset lastpoint
                    //lastPoint = 0;
                }
                catch (IOException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

        }

        private void btnPopulateTables_Click(object sender, EventArgs e)
        {
            string path = txtPath.Text;

            if (ddlDatabase.Text.Equals("-Select-") || ddlDatabase.Text.Equals("") && (chkDatabase.SelectedItems.Count == 0))
            //if (chkDatabase.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please Fill the database first");
                return;
            }

            if (!string.IsNullOrEmpty(path))
            {
                try
                {
                    btnGenerateScript.Enabled = true;
                    execOptions.Enabled = true;

                    string[] filePaths;
                    BusinessFacade facade = new BusinessFacade();
                    List<string> lstSortedFiles = new List<string>();
                    //lstDrive.Items.Clear();
                    dgvListProcess.Rows.Clear();
                    foreach (string ext in exts)
                    {
                        filePaths = Directory.GetFiles(path.Trim(), ext, SearchOption.AllDirectories);
                        //string[] filePaths = Directory.GetFiles(path.Trim(), "*.sql", SearchOption.AllDirectories);
                        if (!ddlDatabase.Text.Equals("All"))
                            lstSortedFiles = facade.GetTblListSortFullPathFiles(filePaths, ddlDatabase.Text);
                        else
                            lstSortedFiles = facade.GetListSortFullPathFiles(filePaths);

                        //for (int i = 0; i < lstSortedFiles.Count; i++)
                        //{
                        //    lstDrive.Items.Add(lstSortedFiles[i]);
                        //}
                    
                        dgvListProcess.Rows.Add(lstSortedFiles.Count);
                        for (int i = 0; i < lstSortedFiles.Count; i++)
                        {
                            //lstDrive.Items.Add(lstSortedFiles[i]);
                            dgvListProcess.Rows[i].Cells[1].Value = lstSortedFiles[i];
                            dgvListProcess.Rows[i].Cells[0].Value = "";
                        }
                    }

                    if (lstSortedFiles.Count == 0)
                    {
                        //filePaths = Directory.GetFiles(path.Trim(), "*.txt", SearchOption.AllDirectories);
                        if (lstSortedFiles.Count == 0)
                        {
                            MessageBox.Show("There is no script suitable.");
                            return;
                        }
                    }
                    //Add To datagridview
                    //DataGridViewRow row1 = (DataGridViewRow)dgvListProcess.Rows[0].Clone();
                    
                    flag = dgvListProcess.Rows.Count;
                    //reset lastpoint
                    //lastPoint = 0;
                }
                catch (IOException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void btnGenerateScript_Click(object sender, EventArgs e)
        {
            DialogResult rs1 = MessageBox.Show("yakin?", "yakin?",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);
            if (rs1 == DialogResult.Cancel)
            {
                return;
            }
            if (statusbgwScript.Text == "Executing ...")
            {
                return;
            }

            string pathErrolog, pathHistoryLog;

            try
            {
                pathErrolog = ConfigurationManager.AppSettings["ErrLog"];
                pathHistoryLog = ConfigurationManager.AppSettings["History"];
                if (string.IsNullOrEmpty(pathErrolog) || string.IsNullOrEmpty(pathHistoryLog))
                {
                    MessageBox.Show("Please configure config path Error Log and History Log ");
                    return;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Please configure config path Error Log and History Log First ");
                return;
            }

            if (ddlDatabase.Text.Equals("-Select-"))
            {
                MessageBox.Show("Please select Database first");
                return;
            }
            //if (execOptions.SelectedIndex == -1)
            //{
            //    MessageBox.Show("Please select options below first.");
            //    return;
            //}

            //if (dgvListProcess.SelectedRows.Count > 1)
            //{
            //    MessageBox.Show("Jangan centang lebih dari 1 baris!!!");
            //}
            //else
            //{
            //}
            if (execOptions.SelectedIndex == 0)
            {
                selectedOption = "all";
            }
            else if (execOptions.SelectedIndex == 1)
            {
                selectedOption = "one";
            }
            else if (execOptions.SelectedIndex == 2)
            {
                selectedOption = "from";
            }

            checkedDB = new ArrayList(chkDatabase.CheckedItems.OfType<string>().ToList());

            bgwScript.RunWorkerAsync();
            statusbgwScript.Text = "Executing ...";
        }

        private void bgwScript_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                //string erno = string.Empty;
                ess.ExecuteScripts(selectedOption, dgvListProcess, checkedDB, label9, sender as BackgroundWorker, label10, cons, rcProcess,
                    connects, cboAuthentication, selectedServer, txtUsername, txtPassword, grantCheckBox, constring);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error : " + ex.Message);
            }
        }

        private void bgwScript_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.dgvListProcess.Rows[e.ProgressPercentage].Cells[0].Value = e.UserState;
        }

        private void bgwScript_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            lastActivity.Text = "Executed scripts";
            statusbgwScript.Text = "Ready";
        }

        private void grant2WButton_Click(object sender, System.EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedServer) || string.IsNullOrEmpty(connects.ToString()))
            {
                MessageBox.Show("Pilih dulu servernya!");
                return;
            }
            try
            {
                string erro = null;
                grant2Wpath = @"\\a000s-drrpt1\MigrationUploadData\Abud\Taufiq\Publishing tools\Grant 2W.txt";
                ess.GenerateStoredProc(grant2Wpath, erro, label10, connects.getSqlConnection(), cons, rcProcess, "Grant", selectedServer, constring);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error :" + ex);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            DialogResult rs1 = MessageBox.Show("yakin?", "yakin?",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);
            if (rs1 == DialogResult.Cancel)
            {
                return;
            }
            StreamWriter sw = null;
            string erno = string.Empty;
            string tempFI;
            try
            {
                //ambil path
                string path = txtPath.Text.Trim() + "\\" + label10.Text.Trim();
                FileInfo FI = new FileInfo(path);
                //ambil filenya
                tempFI = FI.Directory.GetFiles(label10.Text.Trim(), SearchOption.AllDirectories)[0].FullName;
                FileInfo fiPos = new FileInfo(tempFI);
                fiPos.Attributes = FileAttributes.Normal;
                //masukin file ke streamwriter sw
                sw = new StreamWriter(tempFI);
                //overwrite files
                sw.WriteLine(rcProcess.Text);
                sw.Flush();
                sw.Close();

                //for (int i = 0; i < chkDatabase.Items.Count; i++)
                //{
                //    if (i < flagitemchecked)
                //        continue;
                //    if (chkDatabase.GetItemChecked(i))
                //    {
                //        label9.Text = chkDatabase.Items[i].ToString();
                //        ExecuteScripts(chkDatabase.Items[i].ToString(), ref erno);
                //        if (!string.IsNullOrEmpty(erno))
                //        {
                //            MessageBox.Show(erno);
                //            return;
                //        }
                //    }
                //}
            }
            catch (Exception errs)
            {
                MessageBox.Show(errs.Message);
                return;
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                rcProcess.Clear();
                //txtPath.Text = string.Empty;
                //lstDrive.Items.Clear();
                //chkDatabase.Items.Clear();
                dgvListProcess.Rows.Clear();
                dgvListProcess.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error :" + ex.InnerException.Message);
            }
        }

        #region ex-grant
        //private void grantButton_Click(object sender, EventArgs e)
        //{

        //    if (string.IsNullOrEmpty(grantScriptBox.Text))
        //    {
        //        MessageBox.Show("Please type the path first.");
        //        return;
        //    }
        //    bgwGrant.RunWorkerAsync();
        //    statusbgwScript.Text = "Granting ...";
        //}

        //private void bgwGrant_DoWork(object sender, DoWorkEventArgs e)
        //{
        //    try
        //    {
        //        ess.ExecuteGrantScript(grantScriptBox, cons, rcProcess, lastActivity);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Error : " + ex.InnerException.Message);
        //    }
        //}

        //private void bgwGrant_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        //{
        //    statusbgwScript.Text = "Ready";
        //    lastActivity.Text = "Executed grant";
        //}
        #endregion
        #endregion

        #region manual tasks
        //--------------------------------------------------------------------
        //                             tab Manual Tasks
        //--------------------------------------------------------------------

        //testing jerie
        //string configServer = @"\\A000S-DRRPT1";
        //XmlDocument xml;
        XMLEditor xe = new XMLEditor();

        private void webAppComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (webAppComboBox.SelectedItem.ToString())
            {
                case "PSS4W":
                    xmlPath.ReadOnly = true;
                    //testing jerie
                    //xmlPath.Text = configServer + @"\MigrationUploadData\Abud\Taufiq\Publishing tools\AppConfig.config";
                    //path = @"\MigrationUploadData\Abud\Taufiq\Publishing tools\testplantation";
                    xmlPath.Text = @"\\" + pathServerBox.Text + ConfigurationManager.AppSettings["xmlSource_4W"].ToString() + @"PSS4W\AppConfig.config";
                    path = ConfigurationManager.AppSettings["xmlDest_4W"].ToString() + @"pss4w\config";
                    break;
                case "pss4wservice":
                    xmlPath.ReadOnly = true;
                    //testing jerie
                    //xmlPath.Text = configServer + @"\MigrationUploadData\Abud\Taufiq\Publishing tools\AppConfig.config";
                    //path = @"\MigrationUploadData\Abud\Taufiq\Publishing tools\testplantation";
                    xmlPath.Text = @"\\" + pathServerBox.Text + ConfigurationManager.AppSettings["xmlSource_4W"].ToString() + @"pss4wservice\AppConfig.config";
                    path = ConfigurationManager.AppSettings["xmlDest_4W"].ToString() + @"pss4wservice\config";
                    break;
                case "MobilePSSWebService":
                    xmlPath.ReadOnly = true;
                    //testing jerie
                    //xmlPath.Text = configServer + @"\MigrationUploadData\Abud\Taufiq\Publishing tools\AppConfig.config";
                    //path = @"\MigrationUploadData\Abud\Taufiq\Publishing tools\testplantation";
                    xmlPath.Text = @"\\" + pathServerBox.Text + ConfigurationManager.AppSettings["xmlSource_4W"].ToString() + @"MobilePSSWebService\AppConfig.config";
                    path = ConfigurationManager.AppSettings["xmlDest_4W"].ToString() + @"MobilePSSWebService\config";
                    break;
                case "TDMS":
                    xmlPath.ReadOnly = true;
                    //testing jerie
                    //xmlPath.Text = configServer + @"\MigrationUploadData\Abud\Taufiq\Publishing tools\AppConfig.config";
                    //path = @"\MigrationUploadData\Abud\Taufiq\Publishing tools\testplantation";
                    xmlPath.Text = @"\\" + pathServerBox.Text + ConfigurationManager.AppSettings["xmlSource_4W"].ToString() + @"TDMS\AppConfig.config";
                    path = ConfigurationManager.AppSettings["xmlDest_4W"].ToString() + @"tdms\config";
                    break;
                case "tdmsservice":
                    xmlPath.ReadOnly = true;
                    //testing jerie
                    //xmlPath.Text = configServer + @"\MigrationUploadData\Abud\Taufiq\Publishing tools\AppConfig.config";
                    //path = @"\MigrationUploadData\Abud\Taufiq\Publishing tools\testplantation";
                    xmlPath.Text = @"\\" + pathServerBox.Text + ConfigurationManager.AppSettings["xmlSource_4W"].ToString() + @"tdmsservice\AppConfig.config";
                    path = ConfigurationManager.AppSettings["xmlDest_4W"].ToString() + @"tdmsservice\config";
                    break;
                case "PSS4W ConnString":
                    xmlPath.ReadOnly = true;
                    //testing jerie
                    //xmlPath.Text = configServer + @"\MigrationUploadData\Abud\Taufiq\Publishing tools\AppConfig.config";
                    //path = @"\MigrationUploadData\Abud\Taufiq\Publishing tools\testplantation";
                    xmlPath.Text = @"\\" + pathServerBox.Text + ConfigurationManager.AppSettings["connStringSource_4W"].ToString() + @"PSS4W\ConnString.config";
                    path = ConfigurationManager.AppSettings["xmlDest_4W"].ToString() + @"pss4w\config";
                    break;
                case "pss4wservice ConnString":
                    xmlPath.ReadOnly = true;
                    //testing jerie
                    //xmlPath.Text = configServer + @"\MigrationUploadData\Abud\Taufiq\Publishing tools\AppConfig.config";
                    //path = @"\MigrationUploadData\Abud\Taufiq\Publishing tools\testplantation";
                    xmlPath.Text = @"\\" + pathServerBox.Text + ConfigurationManager.AppSettings["connStringSource_4W"].ToString() + @"pss4wservice\ConnString.config";
                    path = ConfigurationManager.AppSettings["xmlDest_4W"].ToString() + @"pss4wservice\config";
                    break;
                case "MobilePSSWebService ConnString":
                    xmlPath.ReadOnly = true;
                    //testing jerie
                    //xmlPath.Text = configServer + @"\MigrationUploadData\Abud\Taufiq\Publishing tools\AppConfig.config";
                    //path = @"\MigrationUploadData\Abud\Taufiq\Publishing tools\testplantation";
                    xmlPath.Text = @"\\" + pathServerBox.Text + ConfigurationManager.AppSettings["connStringSource_4W"].ToString() + @"MobilePSSWebService\ConnString.config";
                    path = ConfigurationManager.AppSettings["xmlDest_4W"].ToString() + @"MobilePSSWebService\config";
                    break;
                case "TDMS ConnString":
                    xmlPath.ReadOnly = true;
                    //testing jerie
                    //xmlPath.Text = configServer + @"\MigrationUploadData\Abud\Taufiq\Publishing tools\AppConfig.config";
                    //path = @"\MigrationUploadData\Abud\Taufiq\Publishing tools\testplantation";
                    xmlPath.Text = @"\\" + pathServerBox.Text + ConfigurationManager.AppSettings["connStringSource_4W"].ToString() + @"TDMS\ConnString.config";
                    path = ConfigurationManager.AppSettings["xmlDest_4W"].ToString() + @"tdms\config";
                    break;
                case "tdmsservice ConnString":
                    xmlPath.ReadOnly = true;
                    //testing jerie
                    //xmlPath.Text = configServer + @"\MigrationUploadData\Abud\Taufiq\Publishing tools\AppConfig.config";
                    //path = @"\MigrationUploadData\Abud\Taufiq\Publishing tools\testplantation";
                    xmlPath.Text = @"\\" + pathServerBox.Text + ConfigurationManager.AppSettings["connStringSource_4W"].ToString() + @"tdmsservice\ConnString.config";
                    path = ConfigurationManager.AppSettings["xmlDest_4W"].ToString() + @"tdmsservice\config";
                    break;
                case "PSS2W":
                    xmlPath.ReadOnly = true;
                    xmlPath.Text = @"\\" + pathServerBox.Text + ConfigurationManager.AppSettings["xmlSource_2W"].ToString() + @"PSS2W\Web.config";
                    path = ConfigurationManager.AppSettings["xmlDest_2W"].ToString() + "PSS2W";
                    break;
                case "PSS2W1":
                    xmlPath.ReadOnly = true;
                    xmlPath.Text = @"\\" + pathServerBox.Text + ConfigurationManager.AppSettings["xmlSource_2W"].ToString() + @"PSS2W1\Web.config";
                    path = ConfigurationManager.AppSettings["xmlDest_2W"].ToString() + "PSS2W1";
                    break;
                case "PSS2W2":
                    xmlPath.ReadOnly = true;
                    xmlPath.Text = @"\\" + pathServerBox.Text + ConfigurationManager.AppSettings["xmlSource_2W"].ToString() + @"PSS2W2\Web.config";
                    path = ConfigurationManager.AppSettings["xmlDest_2W"].ToString() + "PSS2W2";
                    break;
                case "PSS2W3":
                    xmlPath.ReadOnly = true;
                    xmlPath.Text = @"\\" + pathServerBox.Text + ConfigurationManager.AppSettings["xmlSource_2W"].ToString() + @"PSS2W3\Web.config";
                    path = ConfigurationManager.AppSettings["xmlDest_2W"].ToString() + "PSS2W3";
                    break;
                case "PSS2W4":
                    xmlPath.ReadOnly = true;
                    xmlPath.Text = @"\\" + pathServerBox.Text + ConfigurationManager.AppSettings["xmlSource_2W"].ToString() + @"PSS2W4\Web.config";
                    path = ConfigurationManager.AppSettings["xmlDest_2W"].ToString() + "PSS2W4";
                    break;
                case "PSS2W5":
                    xmlPath.ReadOnly = true;
                    xmlPath.Text = @"\\" + pathServerBox.Text + ConfigurationManager.AppSettings["xmlSource_2W"].ToString() + @"PSS2W5\Web.config";
                    path = ConfigurationManager.AppSettings["xmlDest_2W"].ToString() + "PSS2W5";
                    break;
                case "PSS2W6":
                    xmlPath.ReadOnly = true;
                    xmlPath.Text = @"\\" + pathServerBox.Text + ConfigurationManager.AppSettings["xmlSource_2W"].ToString() + @"PSS2W6\Web.config";
                    path = ConfigurationManager.AppSettings["xmlDest_2W"].ToString() + "PSS2W6";
                    break;
                case "PSS2W7":
                    xmlPath.ReadOnly = true;
                    xmlPath.Text = @"\\" + pathServerBox.Text + ConfigurationManager.AppSettings["xmlSource_2W"].ToString() + @"PSS2W7\Web.config";
                    path = ConfigurationManager.AppSettings["xmlDest_2W"].ToString() + "PSS2W7";
                    break;
                case "PSS2W8":
                    xmlPath.ReadOnly = true;
                    xmlPath.Text = @"\\" + pathServerBox.Text + ConfigurationManager.AppSettings["xmlSource_2W"].ToString() + @"PSS2W8\Web.config";
                    path = ConfigurationManager.AppSettings["xmlDest_2W"].ToString() + "PSS2W8";
                    break;
                case "PSS2W9":
                    xmlPath.ReadOnly = true;
                    xmlPath.Text = @"\\" + pathServerBox.Text + ConfigurationManager.AppSettings["xmlSource_2W"].ToString() + @"PSS2W9\Web.config";
                    path = ConfigurationManager.AppSettings["xmlDest_2W"].ToString() + "PSS2W9";
                    break;
                case "PSS2W10":
                    xmlPath.ReadOnly = true;
                    xmlPath.Text = @"\\" + pathServerBox.Text + ConfigurationManager.AppSettings["xmlSource_2W"].ToString() + @"PSS2W10\Web.config";
                    path = ConfigurationManager.AppSettings["xmlDest_2W"].ToString() + "PSS2W10";
                    break;
                case "PSS2W11":
                    xmlPath.ReadOnly = true;
                    xmlPath.Text = @"\\" + pathServerBox.Text + ConfigurationManager.AppSettings["xmlSource_2W"].ToString() + @"PSS2W11\Web.config";
                    path = ConfigurationManager.AppSettings["xmlDest_2W"].ToString() + "PSS2W11";
                    break;
                case "PSSHSOM29":
                    xmlPath.ReadOnly = true;
                    xmlPath.Text = @"\\" + pathServerBox.Text + ConfigurationManager.AppSettings["xmlSource_2W"].ToString() + @"PSSHSOM29\Web.config";
                    path = ConfigurationManager.AppSettings["xmlDest_2W"].ToString() + "PSSHSOM29";
                    break;
                case "PSSHSOHO":
                    xmlPath.ReadOnly = true;
                    xmlPath.Text = @"\\" + pathServerBox.Text + ConfigurationManager.AppSettings["xmlSource_2W"].ToString() + @"PSSHSOHO\Web.config";
                    path = ConfigurationManager.AppSettings["xmlDest_2W"].ToString() + "PSSHSOHO";
                    break;
                case "Other":
                    xmlPath.ReadOnly = false;
                    xmlPath.Text = @"\\" + pathServerBox.Text;
                    break;
            }
        }

        private void structureComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (structureComboBox.SelectedItem.ToString() == "other")
            {
                structureBox.Text = String.Empty;
            }
            else
            {
                structureBox.Text = structureComboBox.SelectedItem.ToString();
            }
        }

        #region app config
        private void chooseKeyButton_Click(object sender, EventArgs e)
        {
            if (keyBox.Text.Length > 2)
            {
                updateValueButton.Enabled = false;
                addXMLButton.Enabled = false;
                currentValueText.Text = string.Empty;
                newKeyText.Text = string.Empty;
                newValueText.Text = string.Empty;

                xe.SearchAtt(xmlPath, structureBox, keyBox, currentValueText, dgvConfig, updateValueButton, addXMLButton, newKeyText, 
                    newValueText, "key");
            }
            else
            {
                MessageBox.Show("Ketikkan lebih dari 2 huruf. Terima kasih.");
            }
        }

        private void chooseValueButton_Click(object sender, EventArgs e)
        {
            if (valueBox.Text.Length > 2)
            {
                updateValueButton.Enabled = false;
                addXMLButton.Enabled = false;
                currentValueText.Text = string.Empty;
                newKeyText.Text = string.Empty;
                newValueText.Text = string.Empty;

                xe.SearchAtt(xmlPath, structureBox, valueBox, currentValueText, dgvConfig, updateValueButton, addXMLButton, newKeyText,
                    newValueText, "value");

                ////read xml file
                ////po = XDocument.Load(xmlPath.Text);
                //xml = new XmlDocument();
                ////xml.LoadXml(po.ToString());
                //xml.Load(xmlPath.Text);
                //XmlNodeList nodes = xml.SelectNodes(structureBox.Text);
                //string[] addConfig;

                //foreach (XmlNode node in nodes)
                //{
                //    XmlAttributeCollection nodeAtt = node.Attributes;
                //    if (nodeAtt["value"].Value.ToLower().ToString() == valueBox.Text.ToLower() || nodeAtt["value"].Value.ToLower().ToString().Contains(valueBox.Text.ToLower()))
                //    {
                //        currentValueText.Text = nodeAtt["value"].Value.ToString();
                //        addConfig = new string[] { nodeAtt["key"].Value.ToString(), nodeAtt["value"].Value.ToString() };
                //        dgvConfig.Rows.Add(addConfig);
                //        updateValueButton.Enabled = true;
                //    }
                //}

                //if (string.IsNullOrEmpty(currentValueText.Text))
                //{
                //    currentValueText.Text = "Value belum ada.";
                //    MessageBox.Show("Value belum ada.");
                //    addXMLButton.Enabled = true;
                //    newKeyText.Enabled = true;
                //    newValueText.Enabled = true;
                //}
            }
            else
            {
                MessageBox.Show("Ketikkan lebih dari 2 huruf. Terima kasih.");
            }
        }

        private void updateValueButton_Click(object sender, EventArgs e)
        {
            xe.UpdateXML(dgvConfig, structureBox, xmlPath, lastActivity, "key", "value");
        }

        private void addXMLButton_Click(object sender, EventArgs e)
        {
            xe.AddXML(structureBox, newKeyText, newValueText, xmlPath, lastActivity, newKeyText, "key", "value");
        }

        private void plantButton4w_Click(object sender, EventArgs e)
        {
            DialogResult rs1 = MessageBox.Show("yakin?", "yakin?",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);
            if (rs1 == DialogResult.Cancel)
            {
                return;
            }
            try
            {
                typeStatus = "PSS4W";
                publishType = "Web Apps";
                sap.getServer(ConfigurationManager.ConnectionStrings["connString"].ToString(), typeStatus, publishType);
                servers = sap.Getservers();
                string pathDir = Path.GetDirectoryName(xmlPath.Text);

                //testing jerie
                //string[] servers = {"a000s-drrpt1"};

                foreach (var server in servers)
                {

                    string destination = @"\\" + server + path;

                    if (File.Exists(xmlPath.Text.Replace(pathDir, destination)))
                    {
                        File.Delete(xmlPath.Text.Replace(pathDir, destination));
                    }
                    string tes = xmlPath.Text.Replace(pathDir, destination);
                    File.Copy(xmlPath.Text, xmlPath.Text.Replace(pathDir, destination), true);
                    logData += xmlPath.Text.Replace(pathDir, destination) + "\n";
                }

                plantLogPath = ConfigurationManager.AppSettings["appsLog"].ToString() + "plantation4w.txt";
                log = new Progress(logData, plantLogPath);
                log.Show();
                lastActivity.Text = "Planted AppConfig to " + servers.Length + " server(s)";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error : " + ex.InnerException.Message);
            }
        }

        private void loadList4w_Click(object sender, EventArgs e)
        {
            try
            {
                list4w.Items.Clear();
                typeStatus = "PSS4W";
                publishType = "Web Apps";
                sap.getServer(ConfigurationManager.ConnectionStrings["connString"].ToString(), typeStatus, publishType);
                servers = sap.Getservers();
                string pathDir = Path.GetDirectoryName(xmlPath.Text);

                foreach (var server in servers)
                {
                    string destination = @"\\" + server + path;
                    list4w.Items.Add(destination);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error : " + ex.InnerException.Message);
            }
        }

        private void plantButton2w_Click(object sender, EventArgs e)
        {
            DialogResult rs1 = MessageBox.Show("yakin?", "yakin?",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);
            if (rs1 == DialogResult.Cancel)
            {
                return;
            }
            try
            {
                string pathDir = Path.GetDirectoryName(xmlPath.Text);
                string[] servers = { "A000S-ITPSAP14", "A000S-ITPSAP16" };

                //testing jerie
                //string[] servers = {"a000s-drrpt1"};

                foreach (var server in servers)
                {
                    string destination = @"\\" + server + path;

                    if (File.Exists(xmlPath.Text.Replace(pathDir, destination)))
                    {
                        File.Delete(xmlPath.Text.Replace(pathDir, destination));
                    }
                    File.Copy(xmlPath.Text, xmlPath.Text.Replace(pathDir, destination), true);
                    logData += xmlPath.Text.Replace(pathDir, destination) + "\n";
                }

                plantLogPath = ConfigurationManager.AppSettings["appsLog"].ToString() + "plantation2w.txt";
                log = new Progress(logData, plantLogPath);
                log.Show();
                lastActivity.Text = "Planted AppConfig to " + servers.Length + " server(s)";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error : " + ex.InnerException.Message);
            }
        }

        private void loadList2w_Click(object sender, EventArgs e)
        {
            try
            {
                list2w.Items.Clear();
                string pathDir = Path.GetDirectoryName(xmlPath.Text);
                string[] servers = { "A000S-ITPSAP14", "A000S-ITPSAP15", "A000S-ITPSAP16" };

                foreach (var server in servers)
                {
                    string destination = @"\\" + server + path;
                    list2w.Items.Add(destination);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error : " + ex.InnerException.Message);
            }
        }

        private void clearGridButton_Click(object sender, EventArgs e)
        {
            dgvConfig.Rows.Clear();
            dgvConfig.Refresh();
        }

        private void deleteGridButton_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow item in dgvConfig.SelectedRows)
                {
                    dgvConfig.Rows.RemoveAt(item.Index);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error : " + ex.InnerException.Message);
            }
        }

        private void dgvConfig_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            updateValueButton.Enabled = true;
        }
        #endregion

        #region connstring
        //----connstring--//
        private void chooseNameButton_Click(object sender, EventArgs e)
        {
            if (nameBox.Text.Length > 2)
            {
                newNameText.Text = string.Empty;
                newConnStringText.Text = string.Empty;
                newProvNameText.Text = string.Empty;
                xe.SearchAtt(xmlPath, structureBox, nameBox, currentValueText, dgvConnString, updateConnStringButton, addXMLButton, newNameText,
                    newConnStringText, newProvNameText, "name");
            }
            else
            {
                MessageBox.Show("Ketikkan lebih dari 2 huruf. Terima kasih.");
            }
        }

        private void chooseConnStringButton_Click(object sender, EventArgs e)
        {
            if (connStringBox.Text.Length > 2)
            {
                newNameText.Text = string.Empty;
                newConnStringText.Text = string.Empty;
                newProvNameText.Text = string.Empty;
                xe.SearchAtt(xmlPath, structureBox, connStringBox, currentValueText, dgvConnString, updateConnStringButton, addXMLButton, newNameText,
                    newConnStringText, newProvNameText, "connectionString");
            }
            else
            {
                MessageBox.Show("Ketikkan lebih dari 2 huruf. Terima kasih.");
            }
        }

        private void chooseProvNameButton_Click(object sender, EventArgs e)
        {
            if (provNameBox.Text.Length > 2)
            {
                newNameText.Text = string.Empty;
                newConnStringText.Text = string.Empty;
                newProvNameText.Text = string.Empty;
                xe.SearchAtt(xmlPath, structureBox, provNameBox, currentValueText, dgvConnString, updateConnStringButton, addXMLButton, newNameText,
                    newConnStringText, newProvNameText, "providerName");
            }
            else
            {
                MessageBox.Show("Ketikkan lebih dari 2 huruf. Terima kasih.");
            }
        }

        private void updateConnStringButton_Click(object sender, EventArgs e)
        {
            xe.UpdateXML(dgvConnString, structureBox, xmlPath, lastActivity, "name", "connectionString", "providerName");
        }

        private void addConnStringButton_Click(object sender, EventArgs e)
        {
            xe.AddXML(structureBox, newNameText, newConnStringText, newProvNameText, xmlPath, lastActivity, newNameText, "name", "connectionString", "providerName");
        }

        private void deleteConnButton_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow item in dgvConnString.SelectedRows)
                {
                    dgvConnString.Rows.RemoveAt(item.Index);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error : " + ex.InnerException.Message);
            }
        }

        private void clearConnButton_Click(object sender, EventArgs e)
        {
            dgvConnString.Rows.Clear();
            dgvConnString.Refresh();
        }

        private void plantConnString_Click(object sender, EventArgs e)
        {
            DialogResult rs1 = MessageBox.Show("yakin?", "yakin?",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);
            if (rs1 == DialogResult.Cancel)
            {
                return;
            }
            try
            {
                typeStatus = "PSS4W";
                publishType = "Web Apps";
                sap.getServer(ConfigurationManager.ConnectionStrings["connString"].ToString(), typeStatus, publishType);
                servers = sap.Getservers();
                string pathDir = Path.GetDirectoryName(xmlPath.Text);

                //testing jerie
                //string[] servers = {"a000s-drrpt1"};

                foreach (var server in servers)
                {

                    string destination = @"\\" + server + path;

                    if (File.Exists(xmlPath.Text.Replace(pathDir, destination)))
                    {
                        File.Delete(xmlPath.Text.Replace(pathDir, destination));
                    }
                    string tes = xmlPath.Text.Replace(pathDir, destination);
                    File.Copy(xmlPath.Text, xmlPath.Text.Replace(pathDir, destination), true);
                    logData += xmlPath.Text.Replace(pathDir, destination) + "\n";
                }

                plantLogPath = ConfigurationManager.AppSettings["appsLog"].ToString() + "plantationConnString.txt";
                log = new Progress(logData, plantLogPath);
                log.Show();
                lastActivity.Text = "Planted ConnString to " + servers.Length + " server(s)";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error : " + ex.InnerException.Message);
            }
        }

        private void loadListConnString_Click(object sender, EventArgs e)
        {
            try
            {
                listConnString.Items.Clear();
                typeStatus = "PSS4W";
                publishType = "Web Apps";
                sap.getServer(ConfigurationManager.ConnectionStrings["connString"].ToString(), typeStatus, publishType);
                servers = sap.Getservers();
                string pathDir = Path.GetDirectoryName(xmlPath.Text);

                foreach (var server in servers)
                {
                    string destination = @"\\" + server + path;
                    listConnString.Items.Add(destination);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error : " + ex.InnerException.Message);
            }
        }
        #endregion

        private void publishAppsButton_Click_1(object sender, System.EventArgs e)
        {

        }



       
       
        #endregion

        private void label2_Click(object sender, System.EventArgs e)
        {

        }



        

        






    }
}

