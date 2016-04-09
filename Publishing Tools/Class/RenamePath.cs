using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Publishing_Tools.Class
{
    class RenamePath
    {
        DirectoryInfo di, diApps;
        string newPath, spPSS, spWebApps, spWebService, spClosingApps, spRetainPPVUnit;

        public void renamePathRule(string localPath)
        {
            di = new DirectoryInfo(localPath);
            diApps = new DirectoryInfo(localPath + @"\PSSApps");

            spPSS = "PSSApps*";
            spWebApps = "Publish Dev*";
            spWebService = "Web Service*";
            spClosingApps = "Astra.Pss.MonthlyClosing";
            spRetainPPVUnit = "Astra.Pss.ReverseRetainPPVUnit";


            newPath = localPath + @"\PSSApps";
            renamePath(di, spPSS, newPath);

            newPath = localPath + @"\Publish Dev";
            renamePath(di, spWebApps, newPath);

            newPath = localPath + @"\Web Service";
            renamePath(di, spWebService, newPath);

            newPath = localPath + @"\PSSApps" + @"\PSS4W.ClosingApps";
            renamePath(diApps, spClosingApps, newPath);

            newPath = localPath + @"\PSSApps" + @"\Astra.Pss.RetainPPVUnit";
            renamePath(diApps, spRetainPPVUnit, newPath);

        }

        public void renamePath(DirectoryInfo d, string oldPath, string newPath)
        {
            DirectoryInfo[] diOldPath = d.GetDirectories(oldPath);
            try
            {
                foreach (DirectoryInfo oP in diOldPath)
                {
                    Directory.Move(oP.FullName.ToString(), newPath);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Error = " + e);
            }
        }
    }
}
