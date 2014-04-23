using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace LogViewerClient
{
    public partial class fCurADStatus : Form
    {
        private string m_connStr;
        private List<fADCurStatusReportModell> reportDataList;
        
        public fCurADStatus(string connStr)
        {
            InitializeComponent();
            m_connStr = connStr;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if(!cbShowARM.Checked && !cbShowUser.Checked)
            {
                MessageBox.Show("Должно быть выбрано отображение пользователей/АРМ!","Ошибка входных данных",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
                return;
            }
             if(!cbShowEnabled.Checked && !cbShowDisabled.Checked)
            {
                MessageBox.Show("Должно быть выбрано отображение Включенных/Выключенных!","Ошибка входных данных",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
                return;
            }


            using (SqlConnection cs = new SqlConnection(m_connStr))
            {
                cs.Open();
                reportDataList = new List<fADCurStatusReportModell>();
                if (cbShowARM.Checked)
                {
                    string sql = "SELECT Machines.ID_Machine,Machines.Name,Machines_ActivationHistory.Status,Machines_DNHistory.DN FROM Machines LEFT JOIN " +
                        " (SELECT ID_Machine,MAX(Actts) as MaxActTS FROM Machines_ActivationHistory GROUP BY ID_Machine) LastactHist ON " +
                        " Machines.ID_Machine = LastactHist.ID_Machine " +
                        " LEFT JOIN Machines_ActivationHistory ON  LastactHist.MaxActTS = Machines_ActivationHistory.ActTS and  " +
                        " Machines_ActivationHistory.ID_Machine = LastactHist.ID_Machine " +
                        " LEFT JOIN " +
                        "(SELECT ID_Machine,MAX(Actts) as MaxActTS FROM Machines_DNHistory GROUP BY ID_Machine) LastDNHist ON " +
                        " LastDNHist.ID_Machine = Machines.ID_machine " +
                        " LEFT JOIN Machines_DNHistory ON Machines_DNHistory.ID_Machine = Machines.ID_machine and Machines_DNHistory.ActTs = LastDNHist.MaxActTS " +
                        " WHERE  Machines.Name LIKE @tf ";
                    if(!cbShowEnabled.Checked || !cbShowDisabled.Checked)
                    {
                        if (cbShowDisabled.Checked)
                            sql += " and Machines_ActivationHistory.Status = 0";
                        else
                            sql += " and Machines_ActivationHistory.Status = 1";
                    }
                   
                    using (SqlCommand cmd = new SqlCommand(sql, cs))
                    {
                        cmd.Parameters.AddWithValue("@tf", "%" + tbFilter.Text + "%");
                        using (SqlDataReader rd = cmd.ExecuteReader())
                        {
                            while (rd.Read())
                            {                                
                                cADObject obj = new cADObject(Convert.ToInt32(rd["ID_Machine"]), Convert.ToString(rd["Name"]), cObjectTypes.Computer);
                                bool RDStatus = true;
                                if(rd["Status"]!=DBNull.Value)
                                    RDStatus = Convert.ToBoolean(rd["Status"]);
                                string DN = "";
                                if(rd["DN"]!=DBNull.Value)
                                    DN = Convert.ToString(rd["DN"]);
                                fADCurStatusReportModell dataModell = new fADCurStatusReportModell(obj, RDStatus, DN);
                                reportDataList.Add(dataModell);
                            }                        
                        }
                    }
                }
                if (cbShowUser.Checked)
                {
                    string sql = "SELECT Users.ID_User,Users.NTLogin,Users_ActivationHistory.Status,Users_DNHistory.DN FROM Users LEFT JOIN " +
                       " (SELECT ID_User,MAX(Actts) as MaxActTS FROM Users_ActivationHistory GROUP BY ID_User) LastactHist ON " +
                       " Users.ID_User = LastactHist.ID_User " +
                       " LEFT JOIN Users_ActivationHistory ON  LastactHist.MaxActTS = Users_ActivationHistory.ActTS and  " +
                       " Users_ActivationHistory.ID_User = LastactHist.ID_User " +
                       " LEFT JOIN " +
                       "(SELECT ID_User,MAX(Actts) as MaxActTS FROM Users_DNHistory GROUP BY ID_User) LastDNHist ON " +
                       " LastDNHist.ID_User = Users.ID_User " +
                       " LEFT JOIN Users_DNHistory ON Users_DNHistory.ID_User = Users.ID_User and Users_DNHistory.ActTs = LastDNHist.MaxActTS " +
                       " WHERE  Users.NTLogin LIKE @tf ";
                    if (!cbShowEnabled.Checked || !cbShowDisabled.Checked)
                    {
                        if (cbShowDisabled.Checked)
                            sql += " and Users_ActivationHistory.Status = 0";
                        else
                            sql += " and Users_ActivationHistory.Status = 1";
                    }

                    using (SqlCommand cmd = new SqlCommand(sql, cs))
                    {
                        cmd.Parameters.AddWithValue("@tf", "%" + tbFilter.Text + "%");
                        using (SqlDataReader rd = cmd.ExecuteReader())
                        {
                            while (rd.Read())
                            {
                                cADObject obj = new cADObject(Convert.ToInt32(rd["ID_User"]), Convert.ToString(rd["NTLogin"]), cObjectTypes.User);
                                bool Status=true;
                                string DN = "Неизвестно";
                                if (!(rd["Status"] is DBNull))
                                {
                                    Status = Convert.ToBoolean(rd["Status"]);
                                }
                                if (!(rd["DN"] is DBNull))
                                {
                                    DN = Convert.ToString(rd["DN"]);
                                }
                                fADCurStatusReportModell dataModell = new fADCurStatusReportModell(obj, Status, DN) ;
                                reportDataList.Add(dataModell);
                            }
                        }
                    }
                }
                
            }
            fCurADStatusReport f = new fCurADStatusReport(reportDataList);
            f.ShowDialog();
        }
    }
}
