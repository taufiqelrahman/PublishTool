using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Publishing_Tools.Class
{
    class ServerAndPool
    {
        string[] servers, pools;
        SqlConnection connection;
        string sql;
        SqlCommand command;
        SqlDataReader dataReader;

        public string[] Getservers()
        {
            return servers;
        }

        public string[] Getpools()
        {
            return pools;
        }

        public void getServer(string connectionString, string typeStatus, string publishType)
        {
            connection = new SqlConnection(connectionString);

            connection.Open();
            sql = "select count(*) from Publish where Type = '" + typeStatus + "' and PublishType = '" + publishType + "'";
            command = new SqlCommand(sql, connection);
            dataReader = command.ExecuteReader();
            dataReader.Read();
            int s = Convert.ToInt32(dataReader.GetValue(0));
            servers = new string[s];
            dataReader.Close();
            command.Dispose();
            //connection.Close();

            sql = "select S.ServerName from Publish P join Server S on P.ServerID = S.ID where P.PublishType = '" + publishType + "' and P.Type = '" + typeStatus + "'  and rowstatus = 0 order by s.servername";

            try
            {
                //connection.Open();
                command = new SqlCommand(sql, connection);
                dataReader = command.ExecuteReader();
                int i = 0;
                while (dataReader.Read())
                {
                    //MessageBox.Show(dataReader.GetValue(0) + " - " + dataReader.GetValue(1) + " - " + dataReader.GetValue(2));
                    servers[i] = Convert.ToString(dataReader.GetValue(0));
                    //testingjerie
                    //servers[i] = "z000s-itpsap25";
                    //servers[i] = "a000s-drtc02";
                    i++;
                    //MessageBox.Show(Convert.ToString(dataReader.GetValue(0)));
                }
                dataReader.Close();
                command.Dispose();
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Can not open connection ! " + ex);
            }
        }

        public void getPool(string connectionString, string typeStatus)
        {
            connection = new SqlConnection(connectionString);
            connection.Open();
            sql = "select count(distinct A.name) from AppPool A join Publish P  on A.ServerID = P.ServerID where P.Type = '" + typeStatus + "'";
            command = new SqlCommand(sql, connection);
            dataReader = command.ExecuteReader();
            dataReader.Read();
            int s = Convert.ToInt32(dataReader.GetValue(0));
            pools = new string[s];
            dataReader.Close();
            command.Dispose();
            //connection.Close();

            sql = "select distinct A.name from AppPool A join Publish P  on A.ServerID = P.ServerID where P.Type = '" + typeStatus + "'";
            try
            {
                //connection.Open();
                command = new SqlCommand(sql, connection);
                dataReader = command.ExecuteReader();
                int i = 0;
                while (dataReader.Read())
                {
                    //MessageBox.Show(dataReader.GetValue(0) + " - " + dataReader.GetValue(1) + " - " + dataReader.GetValue(2));
                    pools[i] = Convert.ToString(dataReader.GetValue(0));
                    ////testing jerie
                    //pools[i] = "testingPublishTool";
                    i++;
                    //MessageBox.Show(Convert.ToString(dataReader.GetValue(0)));
                }
                dataReader.Close();
                command.Dispose();
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Can not open connection ! " + ex);
            }
        }

    }
}
