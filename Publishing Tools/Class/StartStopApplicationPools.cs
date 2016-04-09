using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Windows.Forms;

namespace Publishing_Tools.Class
{
    class StartStopApplicationPools
    {
        string logData;
        string[] servers, pools;
 
        public string GetLogData()
        {
            return logData;
        }

        public void startStopPool(ServerAndPool sap, string connectionString, string typeStatus, string publishType, string path, string type)
        {
            sap.getServer(connectionString, typeStatus, publishType);
            servers = sap.Getservers();
            sap.getPool(connectionString, typeStatus);
            pools = sap.Getpools();

            //testingjerie
            //string[] _servers = { "Z000S-ITPSAP25" };
            //string[] _pools = { "test" };

            try
            {
                RunspaceConfiguration runspaceConfiguration = RunspaceConfiguration.Create();
                Runspace runspace = RunspaceFactory.CreateRunspace(runspaceConfiguration);
                runspace.Open();
                RunspaceInvoke scriptInvoker = new RunspaceInvoke(runspace);
                Pipeline pipeline = runspace.CreatePipeline();

                //Here's how you add a new script with arguments
                Command myCommand = new Command(path);
                CommandParameter a = new CommandParameter("server", servers);
                CommandParameter b = new CommandParameter("appPool", pools);
                //testingjerie
                //CommandParameter a = new CommandParameter("server", _servers);
                //CommandParameter b = new CommandParameter("appPool", _pools);
                CommandParameter c = new CommandParameter("type", type);
                myCommand.Parameters.Add(a);
                myCommand.Parameters.Add(b);
                myCommand.Parameters.Add(c);
                pipeline.Commands.Add(myCommand);

                // Execute PowerShell script
                Collection<PSObject> results = pipeline.Invoke();
                //logData += string.Join(", ", _pools);
                logData += string.Join(", ", pools);

                //foreach (var svr in _servers)
                foreach (var svr in servers)
	            {
                    //foreach (var pool in _pools)
                    foreach (var pool in pools)
	                {
		                //write to event viewer
                        EventLog appLog = new EventLog("");
                        appLog.Source = "Application Pool";
                        if (type == "stop")
                        {
                            appLog.WriteEntry("The IIS 7 Application Pool named " + pool + " on " + svr +
                                ".corp.ai.astra.co.id is unavailable as the Application Pool has been stopped.", EventLogEntryType.Information);
                        }
                        else if (type == "start")
                        {
                            appLog.WriteEntry("The IIS 7 Application Pool named " + pool + " on " + svr +
                                ".corp.ai.astra.co.id is available as the Application Pool has been started.", EventLogEntryType.Information);
                        }
	                }
	            }
                MessageBox.Show(type + " sudah selesai.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error : " + ex);
            }
        }

    }
}
