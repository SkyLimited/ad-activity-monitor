using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;

namespace LogViewerService
{
    public class Settings
    {
        public int WaitTime;
        public string SqlServerString;
        public int ADWaitTime;

        
        public  Settings(string SettingsXMLFile)
        {
            using (System.Data.DataSet ds = new System.Data.DataSet())
            {

                ds.ReadXml(SettingsXMLFile, XmlReadMode.Auto);

                System.Data.DataTable table = ds.Tables["settings"];
                System.Data.DataRow row = table.Rows[0];

                WaitTime = Convert.ToInt32(row["WaitTime"]);
                ADWaitTime = Convert.ToInt32(row["ADWaitTime"]);
                SqlServerString = Convert.ToString(row["SqlServer"]);
            }
        }
       
    }

    
    }

