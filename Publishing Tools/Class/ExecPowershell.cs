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
    class ExecPowershell
    {
        Command myCommand;

        public void execToPowerShell(string server, string query, string ps1path)
        {
            try
            {
                //exec script
                RunspaceConfiguration runspaceConfiguration = RunspaceConfiguration.Create();
                Runspace runspace = RunspaceFactory.CreateRunspace(runspaceConfiguration);

                runspace.Open();
                RunspaceInvoke scriptInvoker = new RunspaceInvoke(runspace);
                Pipeline pipeline = runspace.CreatePipeline();
                Command myCommand = new Command(ps1path);
                CommandParameter a = new CommandParameter("server", server);
                CommandParameter b = new CommandParameter("query", query);
                myCommand.Parameters.Add(a);
                myCommand.Parameters.Add(b);
                pipeline.Commands.Add(myCommand);

                // Execute PowerShell script
                Collection<PSObject> results = pipeline.Invoke();

                //foreach (PSObject s in results)
                //{
                //    //Process p = s.BaseObject as Process;
                //    string e = s.Members["PercentComplete"].Value.ToString();
                //    MessageBox.Show(e);

                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error : " + ex.InnerException.Message);
            }
        }

        public void execToPowerShell(string query, string ps1path)
        {
            try
            {
                //exec script
                RunspaceConfiguration runspaceConfiguration = RunspaceConfiguration.Create();
                Runspace runspace = RunspaceFactory.CreateRunspace(runspaceConfiguration);

                runspace.Open();
                RunspaceInvoke scriptInvoker = new RunspaceInvoke(runspace);
                Pipeline pipeline = runspace.CreatePipeline();
                Command myCommand = new Command(ps1path);
                //CommandParameter a = new CommandParameter("server", server);
                CommandParameter b = new CommandParameter("query", query);
                //myCommand.Parameters.Add(a);
                myCommand.Parameters.Add(b);
                pipeline.Commands.Add(myCommand);

                // Execute PowerShell script
                Collection<PSObject> results = pipeline.Invoke();

                //foreach (PSObject s in results)
                //{
                //    //Process p = s.BaseObject as Process;
                //    string e = s.Members["PercentComplete"].Value.ToString();
                //    MessageBox.Show(e);

                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error : " + ex.InnerException.Message);
            }
        }

        public void execToPowerShell(string[] server, string query, string ps1path)
        {
            try
            {
                //exec script
                RunspaceConfiguration runspaceConfiguration = RunspaceConfiguration.Create();
                Runspace runspace = RunspaceFactory.CreateRunspace(runspaceConfiguration);

                runspace.Open();
                RunspaceInvoke scriptInvoker = new RunspaceInvoke(runspace);
                Pipeline pipeline = runspace.CreatePipeline();
                Command myCommand = new Command(ps1path);
                CommandParameter a = new CommandParameter("server", server);
                CommandParameter b = new CommandParameter("query", query);
                myCommand.Parameters.Add(a);
                myCommand.Parameters.Add(b);
                pipeline.Commands.Add(myCommand);

                // Execute PowerShell script
                Collection<PSObject> results = pipeline.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error : " + ex.InnerException.Message);
            }
        }

        //public void execToPowerShell(string server,string date, string ps1path)
        //{
        //    try
        //    {
        //        //exec script
        //        RunspaceConfiguration runspaceConfiguration = RunspaceConfiguration.Create();
        //        Runspace runspace = RunspaceFactory.CreateRunspace(runspaceConfiguration);

        //        runspace.Open();
        //        RunspaceInvoke scriptInvoker = new RunspaceInvoke(runspace);
        //        Pipeline pipeline = runspace.CreatePipeline();
        //        myCommand = new Command(ps1path);
        //        myCommand.Parameters.Add("server", server);
        //        myCommand.Parameters.Add("date", date);
        //        pipeline.Commands.Add(myCommand);

        //        // Execute PowerShell script
        //        Collection<PSObject> results = pipeline.Invoke();
        //    }
        //    catch (Exception ex)

        //    {
        //        MessageBox.Show("Error : " + ex.InnerException.Message);
        //    }
        //}

        //public void execToPowerShell(string[] server, string date, string ps1path)
        //{
        //    try
        //    {
        //        //exec script
        //        RunspaceConfiguration runspaceConfiguration = RunspaceConfiguration.Create();
        //        Runspace runspace = RunspaceFactory.CreateRunspace(runspaceConfiguration);

        //        runspace.Open();
        //        RunspaceInvoke scriptInvoker = new RunspaceInvoke(runspace);
        //        Pipeline pipeline = runspace.CreatePipeline();
        //        myCommand = new Command(ps1path);
        //        myCommand.Parameters.Add("server", server);
        //        myCommand.Parameters.Add("date", date);
        //        pipeline.Commands.Add(myCommand);

        //        // Execute PowerShell script
        //        Collection<PSObject> results = pipeline.Invoke();
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Error : " + ex.InnerException.Message);
        //    }
        //}

        public void execToPowerShell(string[] server, string date, string[] app, string ps1path)
        {
            try
            {
                //exec script
                RunspaceConfiguration runspaceConfiguration = RunspaceConfiguration.Create();
                Runspace runspace = RunspaceFactory.CreateRunspace(runspaceConfiguration);

                runspace.Open();
                RunspaceInvoke scriptInvoker = new RunspaceInvoke(runspace);
                Pipeline pipeline = runspace.CreatePipeline();
                myCommand = new Command(ps1path);

                myCommand.Parameters.Add("server", server);
                myCommand.Parameters.Add("date", date);
                myCommand.Parameters.Add("app", app);
                pipeline.Commands.Add(myCommand);

                // Execute PowerShell script
                Collection<PSObject> results = pipeline.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error : " + ex.InnerException.Message);
            }
        }

    }
}
