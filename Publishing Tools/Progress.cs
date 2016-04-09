using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using StreamWriter = System.IO.StreamWriter;

namespace Publishing_Tools
{
    public partial class Progress : Form 
    {
        string logData, path;

        public Progress(string logData, string path)
        {
            InitializeComponent();
            this.logData = logData;
            this.path = path;
        }

        private void BackupProgress_Load(object sender, EventArgs e)
        {
            try
            {
                logBox.Text = logData;
                StreamWriter file = new StreamWriter(path);
                file.Write(logData.Replace("\n", "\r\n"));
                file.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error : " + ex.InnerException.Message);
            }
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
