using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using System.Windows.Forms;

namespace LogViewerClient
{
    public class Settings
    {
        public string SqlServerString;


        public Settings()
        {
            System.Data.DataSet ds = new System.Data.DataSet();
            try
            {
                ds.ReadXml(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),@"SkyLimited LLC\Log Viewer Client\Settings.xml"), XmlReadMode.Auto);

                System.Data.DataTable table = ds.Tables["settings"];
                System.Data.DataRow row = table.Rows[0];

                SqlServerString = (string)row["SqlServer"];

            }
            catch (Exception)
            {
                //Trace.WriteLine("Unable to load Settings.xml due to " + ex.Message);
                MessageBox.Show("Не удалось загрузить настройки. Проверьте, что файл Settings.xml не поврежден.", "Ошибка инициализации", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(-1);

            }
        }
    }
}
