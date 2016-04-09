using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Server;
using Microsoft.SqlServer.Management.Common;
using System.Collections.ObjectModel;


namespace GenerateScripts
{
public class Constring
    {

    private string _userName = string.Empty;
    private string _password = string.Empty;
    private string _serverName = string.Empty;
    private string _dbName = string.Empty;
    private string consTring = string.Empty;
    SqlConnection con;

    public string UserName
    {
        set {
            _userName = value;
        }
        get
        {
         return _userName;
        }
    }

    public string Password
    {
        set
        {
            _password = value;
        }
        get
        {
            return _password;
        }
    }

    public string ServerName
    {
        set
        {
            _serverName = value;
        }
        get
        {
            return _serverName;
        }
    }

    public string DBName
    {
        set
        {
            _dbName = value;
        }
        get
        {
            return _dbName;
        }
    }


    public Constring(string username, string password,string serverName,string databaseName)
    {
        _password = password;
        _userName = username;
        _serverName = serverName;
        _dbName = databaseName;
        consTring = "Database=" + databaseName +  ";Server=" + ServerName + ";Connect Timeout=60000;UID=" + UserName + ";Password=" + Password + ";";

    }

    public Constring(string username, string password, string serverName)
    {
        _password = password;
        _userName = username;
        _serverName = serverName;
        consTring = "Server=" + ServerName + ";Connect Timeout=60000;UID=" + UserName + ";Password=" + Password + ";";

    }



    public Constring( string serverName)
    {
        _serverName = serverName;
        consTring = "Server=" + ServerName + ";Integrated Security=True";

    }

    public Constring(string serverName,string databaseName)
    {
        _serverName = serverName;
        consTring = "Database=" + databaseName +  ";Server=" + ServerName + ";Integrated Security=True";

    }

    public Server Connect()
    {
        con = new SqlConnection(consTring);
        Server cons = new Server();
        cons = new Server(new ServerConnection(con));
        return cons;
    }

    public SqlConnection getSqlConnection()
    {
        return con;
    }

    public Collection<string> GetListDB()
    {

        Collection<string> listDB = new Collection<string>();          
        using (SqlConnection sqlConx = new SqlConnection(consTring))
        {
            sqlConx.Open();
            DataTable tblDatabases = sqlConx.GetSchema("Databases");
            sqlConx.Close();

            foreach (DataRow row in tblDatabases.Rows)
            {
                listDB.Add(row["database_name"].ToString());
                
            }
         
        }
        
        return listDB;
    }

    public List<string> GetListDBSort(ref string err)
    {
        List<string> listDB = new List<string>();
        try
        {
           
            using (SqlConnection sqlConx = new SqlConnection(consTring))
            {
                sqlConx.Open();
                DataTable tblDatabases = sqlConx.GetSchema("Databases");
                sqlConx.Close();

                foreach (DataRow row in tblDatabases.Rows)
                {
                    //if (row["database_name"].ToString().Contains("PSS"))
                        listDB.Add(row["database_name"].ToString());


                }
                listDB.Sort();
            }
        }
        catch (SqlException er)
        {
            err = er.Message;
        }

        return listDB;
    }


  
    public List<string> GetListDBSort(ref string err,int index)
    {
        List<string> listDB = new List<string>();
        try
        {

            using (SqlConnection sqlConx = new SqlConnection(consTring))
            {
                sqlConx.Open();
                DataTable tblDatabases = sqlConx.GetSchema("Databases");
                sqlConx.Close();

                foreach (DataRow row in tblDatabases.Rows)
                {
                    if (index == 0) // All
                    {
                        //if (row["database_name"].ToString().Contains("PSS"))
                            listDB.Add(row["database_name"].ToString());

                    }
                    else if (index == 1) // Master
                    {
                        if (row["database_name"].ToString().Contains("PSS4WMASTER"))
                            listDB.Add(row["database_name"].ToString());

                    }
                    else if (index == 2) // Transaction
                    {

                        if (!row["database_name"].ToString().Contains("PSS4WMASTER") && row["database_name"].ToString().ToUpper().Contains("PSS4W") && !row["database_name"].ToString().Contains("LOG") && !row["database_name"].ToString().Contains("BUG") && !row["database_name"].ToString().Contains("STAGING"))
                            listDB.Add(row["database_name"].ToString());

                    }
                    else if (index == 3) // Transaction 2W
                    {

                        if (row["database_name"].ToString().Contains("PSSH") || (row["database_name"].ToString().Contains("2W"))
                            || (row["database_name"].ToString().ToLower().Contains("usermanagement")) && !row["database_name"].ToString().ToLower().Contains("sync"))
                            listDB.Add(row["database_name"].ToString());

                    }

                    else if (index == 4) // Staging
                    {
                        if (row["database_name"].ToString().Contains("STAGING"))
                            listDB.Add(row["database_name"].ToString());

                    }

                    else if (index == 5) // all
                    {
                        //if (row["database_name"].ToString().Contains("master"))
                            listDB.Add(row["database_name"].ToString());

                    }

                    else if (index == 7) // Grant
                    {
                        if (row["database_name"].ToString().Contains("master"))
                            listDB.Add(row["database_name"].ToString());

                    }

                    else  // SandBox
                    {
                        if (row["database_name"].ToString().ToUpper().Contains("DBA"))
                        //if (row["database_name"].ToString().ToUpper().Contains("SANDBOX"))
                            listDB.Add(row["database_name"].ToString());

                    }


                }
                listDB.Sort();
            }
        }
        catch (SqlException er)
        {
            err = er.Message;
        }

        return listDB;
    }




    }
}

