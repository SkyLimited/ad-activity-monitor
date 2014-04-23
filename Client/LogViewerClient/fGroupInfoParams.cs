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
    public partial class fGroupInfoParams : Form
    {
        private string m_connStr;
        private bool m_Dir;
        
        public fGroupInfoParams(string connStr)
        {
            InitializeComponent();
            m_Dir = true;
            using (SqlConnection cs = new SqlConnection(connStr))
            {
                cs.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM DOmainGroups ORDER BY DNGroupName", cs))
                {
                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        List<LogViewerService.cDomainGroup> groupsList = new List<LogViewerService.cDomainGroup>();
                        while (rd.Read())
                        {
                           LogViewerService.cDomainGroup dGroup =new LogViewerService.cDomainGroup(Convert.ToInt32(rd["id"]),fMain.translateDN(Convert.ToString(rd["DNGroupName"])));
                           groupsList.Add(dGroup);                            
                        }
                      
                  
                       ((ListBox)clbGroups).DataSource = groupsList ;
                       ((ListBox)clbGroups).ValueMember = "SQLID";
                       ((ListBox)clbGroups).DisplayMember = "DN";
                      
                    }
                }
            }
            m_connStr = connStr;
        }

        private void btnCheckAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < clbGroups.Items.Count; i++)
                clbGroups.SetItemChecked(i, m_Dir);
            m_Dir = !m_Dir;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!cbStatusDisabled.Checked && cbStatusEnabled.Checked)
            {
                MessageBox.Show("Необходимо выбрать включенные/отклченные аккаунты.", "Ошибка входных данных", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (!cbShowARM.Checked && !cbShowUser.Checked)
            {
                MessageBox.Show("Необходимо выбрать аккаунты пользователей/АРМ.", "Ошибка входных данных", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            List<cGroupInfoModel> repDataList = new List<cGroupInfoModel>();
            
            using (SqlConnection cs = new SqlConnection(m_connStr))
            {
                cs.Open();
                string where = "(1=0 ";
                foreach (LogViewerService.cDomainGroup o in clbGroups.CheckedItems)
                {
                    where += " OR DomainGroups.ID = " + o.SQLID;

                }
                where += " )";

                if (cbShowUser.Checked)
                {
                    string sql = "SELECT DomainGroups.ID,DomainGroups.DNGroupName,Users.ID_User,Users.NTLogin,Users_ActivationHistory.Status,Users_DNHistory.DN FROM Users " +
                                 " INNER JOIN (SELECT ID_User,Max(ActTS) as MaxTS FROM  Users_GroupHistory GROUP BY ID_User) MaxUGH " +
                                 " ON MaxUGH.ID_User =  Users.ID_User " +
                                 " INNER JOIN Users_GroupHistory ON MaxUGH.ID_User = Users_GroupHistory.ID_User " +
                                 " and MaxUGH.MaxTS = Users_GroupHistory.Actts " +
                                 " INNER JOIN Users_GroupHistoryDetail ON Users_GroupHistoryDetail.ID_UserGroupHistory = Users_GroupHistory.ID " +
                                 " INNER JOIN DomainGroups ON DomainGroups.ID= Users_GroupHistoryDetail.ID_DomainGroup " +
                                 " INNER JOIN " +
                                 " (SELECT ID_User,MAX(Actts) as MaxActTS FROM Users_ActivationHistory GROUP BY ID_User) LastactHist ON " +
                                 " Users.ID_User = LastactHist.ID_User " +
                                 " INNER JOIN Users_ActivationHistory ON  LastactHist.MaxActTS = Users_ActivationHistory.ActTS and  " +
                                 " Users_ActivationHistory.ID_User = LastactHist.ID_User " +
                                 " INNER JOIN " +
                                 "(SELECT ID_User,MAX(Actts) as MaxActTS FROM Users_DNHistory GROUP BY ID_User) LastDNHist ON " +
                                 " LastDNHist.ID_User = Users.ID_User " +
                                 " INNER JOIN Users_DNHistory ON Users_DNHistory.ID_User = Users.ID_User and Users_DNHistory.ActTs = LastDNHist.MaxActTS " +
                               "";//  " WHERE  Users.NTLogin LIKE @tf ";
                    if (!cbStatusEnabled.Checked || !cbStatusDisabled.Checked)
                    {
                        if (cbStatusDisabled.Checked)
                            sql += " and Users_ActivationHistory.Status = 0";
                        else
                            sql += " and Users_ActivationHistory.Status = 1";
                    }

                    sql += "WHERE " + where;



                    using (SqlCommand cmd = new SqlCommand(sql, cs))
                    {
                     //   cmd.Parameters.AddWithValue("@tf",tb
                        using (SqlDataReader rd = cmd.ExecuteReader())
                        {
                           while(rd.Read())
                           {
                               cADObject obj = new cADObject(Convert.ToInt32(rd["ID_User"]), Convert.ToString(rd["NTLogin"]), cObjectTypes.User);
                               LogViewerService.cDomainGroup dGroup = new LogViewerService.cDomainGroup(Convert.ToInt32(rd["ID"]), Convert.ToString(rd["DNGroupName"]));
                               cGroupInfoModel mdl = new cGroupInfoModel(obj, Convert.ToBoolean(rd["Status"]), dGroup);
                               repDataList.Add(mdl);
                           }
                        }
                    }

                }
                if (cbShowARM.Checked)
                {
                    string sql = "SELECT DomainGroups.ID,DomainGroups.DNGroupName,Machines.ID_Machine,Machines.Name,Machines_ActivationHistory.Status,Machines_DNHistory.DN FROM Machines " +
                                 " INNER JOIN (SELECT ID_Machine,Max(ActTS) as MaxTS FROM  Machines_GroupHistory GROUP BY ID_Machine) MaxUGH " +
                                 " ON MaxUGH.ID_Machine =  Machines.ID_Machine " +
                                 " INNER JOIN Machines_GroupHistory ON MaxUGH.ID_Machine = Machines_GroupHistory.ID_Machine " +
                                 " and MaxUGH.MaxTS = Machines_GroupHistory.Actts " +
                                 " INNER JOIN Machines_GroupHistoryDetail ON Machines_GroupHistoryDetail.ID_MachineGroupHistory = Machines_GroupHistory.ID " +
                                 " INNER JOIN DomainGroups ON DomainGroups.ID= Machines_GroupHistoryDetail.ID_DomainGroup " +
                                 " INNER JOIN " +
                                 " (SELECT ID_Machine,MAX(Actts) as MaxActTS FROM Machines_ActivationHistory GROUP BY ID_Machine) LastactHist ON " +
                                 " Machines.ID_Machine = LastactHist.ID_Machine " +
                                 " INNER JOIN Machines_ActivationHistory ON  LastactHist.MaxActTS = Machines_ActivationHistory.ActTS and  " +
                                 " Machines_ActivationHistory.ID_Machine = LastactHist.ID_Machine " +
                                 " INNER JOIN " +
                                 "(SELECT ID_Machine,MAX(Actts) as MaxActTS FROM Machines_DNHistory GROUP BY ID_Machine) LastDNHist ON " +
                                 " LastDNHist.ID_Machine = Machines.ID_Machine " +
                                 " INNER JOIN Machines_DNHistory ON Machines_DNHistory.ID_Machine = Machines.ID_Machine and Machines_DNHistory.ActTs = LastDNHist.MaxActTS " +
                             "";
                    if (!cbStatusEnabled.Checked || !cbStatusDisabled.Checked)
                    {
                        if (cbStatusDisabled.Checked)
                            sql += " and Machines_ActivationHistory.Status = 0";
                        else
                            sql += " and Machines_ActivationHistory.Status = 1";
                    }

                    sql += "WHERE " + where;



                    using (SqlCommand cmd = new SqlCommand(sql, cs))
                    {
                        using (SqlDataReader rd = cmd.ExecuteReader())
                        {
                            while (rd.Read())
                            {
                                cADObject obj = new cADObject(Convert.ToInt32(rd["ID_machine"]), Convert.ToString(rd["Name"]), cObjectTypes.Computer);
                                LogViewerService.cDomainGroup dGroup = new LogViewerService.cDomainGroup(Convert.ToInt32(rd["ID"]), Convert.ToString(rd["DNGroupName"]));
                                cGroupInfoModel mdl = new cGroupInfoModel(obj, Convert.ToBoolean(rd["Status"]), dGroup);
                                
                                repDataList.Add(mdl);
                            }
                        }
                    }



                }
            }

            fGroupInfoReportPreview f = new fGroupInfoReportPreview(repDataList);
            f.ShowDialog();
        }
    }
}
