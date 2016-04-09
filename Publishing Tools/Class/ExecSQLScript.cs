using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GenerateScripts;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace Publishing_Tools.Class
{
    class ExecSQLScript
    {
        int startPoint, endPoint = 999999;
        FileInfo file;
        SqlConnection connection;
        string sql;
        SqlCommand command;
        //string[] scriptGrant2W;
        List<string> scriptGrant2W = new List<string>();
        //SqlDataReader dataReader;
        
        public void ExecuteScripts(string selectedOption, DataGridView dgvListProcess, ArrayList checkedDB, Label label9,
            BackgroundWorker bgwScript, Label label10, Server cons, RichTextBox rcProcess, Constring connects, ComboBox cboAuthentication,
            string selectedServer, TextBox txtUsername, TextBox txtPassword, CheckBox grantCheckBox, string constring)
        {
            BusinessFacade facade = new BusinessFacade();

            string erro = string.Empty;

            //if (flag != filePaths.Length)
            //{
            //    //ubah lstdrive ke dgvListProcess
            //    filePaths = new string[dgvListProcess.RowCount];
            //    //filePaths = new string[lstDrive.Items.Count];

            //    for (int j = 0; j < dgvListProcess.RowCount; j++)
            //    {
            //        //filePaths[j] = lstDrive.Items[j].ToString();
            //        filePaths[j] = dgvListProcess.Rows[j].ToString();
            //    }
            //}
            int point;
            switch (selectedOption)
            {
                case "all":
                    startPoint = 0;
                    break;
                case "one":
                    point = dgvListProcess.CurrentRow.Index;
                    startPoint = point;
                    endPoint = point;
                    break;
                case "from":
                    point = dgvListProcess.CurrentRow.Index;
                    startPoint = point;
                    break;
            }

            //else if (execOptions.SelectedIndex == null)
            //{
            //    MessageBox.Show("Please select options below first.");
            //    return;
            //}

            ////get checked dgv
            //if (dgvListProcess.SelectedRows.Count == 0)
            //{
            //    startPoint = 0;
            //}
            //else
            //{
            //    int point = dgvListProcess.CurrentRow.Index;
            //    startPoint = point;
            //}

            //initiate connection
            connection = new SqlConnection(constring);
            connection.Open();

            int i;
            for (i = startPoint; i < dgvListProcess.Rows.Count - 1; i++)
            {
                //if (i >= filePaths.Length)
                //{
                //    break;
                //}
                //Constring connects;
                foreach (string DB in checkedDB)
                {
                    label9.Invoke((MethodInvoker)delegate
                    {
                        label9.Text = DB;
                    });
                    //connect to db
                    cboAuthentication.Invoke((MethodInvoker)delegate
                    {
                        if (cboAuthentication.Items[cboAuthentication.SelectedIndex].ToString().Contains("Windows"))
                        {
                            //connects = new Constring(this.ddlServer.Text, DB);
                            connects = new Constring(selectedServer, DB);
                        }
                        else
                        {
                            //connects = new Constring(this.txtUsername.Text, this.txtPassword.Text, this.ddlServer.Text, DB);
                            connects = new Constring(txtUsername.Text, txtPassword.Text, selectedServer, DB);
                        }

                        cons = connects.Connect();
                    });

                    //facade.InsertAppLog(filePaths[i], DB);
                    facade.InsertAppLog(dgvListProcess.Rows[i].Cells[1].Value.ToString(), DB);
                    //exec script
                    this.GenerateStoredProc(dgvListProcess.Rows[i].Cells[1].Value.ToString(), ref erro, label10, cons, rcProcess, DB, selectedServer);
                    if (grantCheckBox.Checked == false)
                    {
                        //if execution fails
                        if (!string.IsNullOrEmpty(erro))
                        {
                            facade.InsertAppLog(dgvListProcess.Rows[i].Cells[1].Value.ToString(), DB, erro, grantCheckBox);
                            bgwScript.ReportProgress(i, "Error");
                            MessageBox.Show(erro);
                            return;
                        }
                    }
                    else
                    {
                        //if execution fails
                        if (!string.IsNullOrEmpty(erro))
                        {
                            facade.InsertAppLog(dgvListProcess.Rows[i].Cells[1].Value.ToString(), DB, erro, grantCheckBox);
                            bgwScript.ReportProgress(i, "Error");
                            MessageBox.Show(erro);
                            //ga berhenti jika ada error
                        }
                    }
                    

                    //this.lstDrive.Items.RemoveAt(this.lstDrive.FindString(filePaths[i]));
                    //bgwScript.ReportProgress(i, "Success");
                    //this.dgvListProcess.Rows[i].Cells[0].Value = "Success";
                }
                bgwScript.ReportProgress(i, "Success");
                //this.flag--;
                if (i == endPoint)
                {
                    endPoint = 999999;
                    //MessageBox.Show("scripts on DB " + label9.Text + " Successfully execute");
                    return;
                }
            }
            //close connection
            connection.Close();
        }

        //method exec query
        public void GenerateStoredProc(string fullpath, ref string erro, Label label10, Server cons, RichTextBox rcProcess, string DB, string selectedServer)
        {
            string[] tempText = fullpath.Split('\\');
            file = new FileInfo(fullpath);
            file.Attributes = FileAttributes.Normal;

            //StreamReader sr = new StreamReader(fullpath);
            //string script = sr.ReadToEnd();
            string script = File.ReadAllText(fullpath);
            label10.Invoke((MethodInvoker)delegate
            {
                label10.Text = tempText[tempText.Length - 1];
            });
            try
            {
                int a = cons.ConnectionContext.ExecuteNonQuery(script);
                //cons.ConnectionContext.ExecuteNonQuery(script);
                //cons.ConnectionContext.Disconnect();
            }
            catch (ExecutionFailureException er)
            {
                erro += er.InnerException.Message;
            }
            catch (SqlException se)
            {
                erro += se.InnerException.Message;
            }
            finally
            {
                //tampilin script di rcProcess
                rcProcess.Invoke((MethodInvoker)delegate
                {
                    rcProcess.Text = script;
                });
                if (string.IsNullOrEmpty(erro))
                {
                    sql = "insert into execscriptlog (servername, databasename, query, querypath) values ('" + selectedServer + "', '" + DB + "', '" + script.Replace("'", "''") + "', '" + fullpath + "')";
                    command = new SqlCommand(sql, connection);
                    command.ExecuteNonQuery();
                    command.Dispose();
                }
            }
            //sr.Close();
        }

        //method exec query grant PSS2W
        public void GenerateStoredProc(string fullpath, string erro, Label label10, SqlConnection connects, Server cons, RichTextBox rcProcess, string DB, 
            string selectedServer, string constring)
        {
            //initiate connection
            connection = new SqlConnection(constring);
            connection.Open();

            string[] tempText = fullpath.Split('\\');
            file = new FileInfo(fullpath);
            file.Attributes = FileAttributes.Normal;

            //StreamReader sr = new StreamReader(fullpath);
            //string script = sr.ReadToEnd();
            string script = File.ReadAllText(fullpath);
            label10.Invoke((MethodInvoker)delegate
            {
                label10.Text = tempText[tempText.Length - 1];
            });
            try
            {
                //generate scripts
                //script = "select 'use '+ name + CHAR(10)+'exec spGrantExectoAllStoredProcs psshso'+CHAR(10) from sys.databases where name like 'PSSH%' and name not like '%sync' union all select 'use '+ name + CHAR(10)+'exec spGrantExectoAllStoredProcs psshso_sco'+CHAR(10) from sys.databases where name like 'PSSH%' and name not like '%sync' ";
                SqlDataReader dataReader = cons.ConnectionContext.ExecuteReader(script);

                //command = new SqlCommand(script, connects);
                //dataReader = command.ExecuteReader();
                int i = 0;
                while (dataReader.Read())
                {
                    scriptGrant2W.Add(dataReader.GetString(0));
                    //MessageBox.Show(dataReader.GetValue(0) + " - " + dataReader.GetValue(1) + " - " + dataReader.GetValue(2));
                    //scriptGrant2W[i] = Convert.ToString(dataReader.GetValue(0));
                    //MessageBox.Show(dataReader.GetString(0));
                    i++;
                }
                //command.Dispose();
                dataReader.Close();

                //exec generated script
                foreach (string row in scriptGrant2W)
                {
                    int a = cons.ConnectionContext.ExecuteNonQuery(row);
                }
            }
            catch (ExecutionFailureException er)
            {
                erro += er.InnerException.Message;
            }
            catch (SqlException se)
            {
                erro += se.InnerException.Message;
            }
            finally
            {
                //tampilin script di rcProcess
                rcProcess.Invoke((MethodInvoker)delegate
                {
                    rcProcess.Text = script;
                });
                if (string.IsNullOrEmpty(erro))
                {
                    sql = "insert into execscriptlog (servername, databasename, query, querypath) values ('" + selectedServer + "', '" + DB + "', '" + script.Replace("'", "''") + "', '" + fullpath + "')";
                    command = new SqlCommand(sql, connection);
                    command.ExecuteNonQuery();
                    command.Dispose();

                    connection.Close();
                }
            }
            //sr.Close();
        }

        #region exgrant
        //public void ExecuteGrantScript(TextBox grantScriptBox, Server cons, RichTextBox rcProcess, ToolStripStatusLabel lastActivity)
        //{
        //    file = new FileInfo(grantScriptBox.Text);
        //    file.Attributes = FileAttributes.Normal;
        //    //StreamReader sr = new StreamReader(grantScriptBox.Text);
        //    //string script = sr.ReadToEnd();
        //    string script = File.ReadAllText(grantScriptBox.Text);
        //    try
        //    {
        //        int a = cons.ConnectionContext.ExecuteNonQuery(script);
        //        //cons.ConnectionContext.ExecuteNonQuery(script);
        //        //cons.ConnectionContext.Disconnect();
        //    }
        //    catch (ExecutionFailureException er)
        //    {
        //        MessageBox.Show(er.InnerException.Message);
        //    }
        //    catch (SqlException se)
        //    {
        //        MessageBox.Show(se.ToString());
        //    }
        //    rcProcess.Invoke((MethodInvoker)delegate
        //    {
        //        //tampilin script di rcProcess
        //        rcProcess.Text = script;
        //    });
        //    //sr.Close();
        //}
        #endregion
    }
}
