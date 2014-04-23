using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Threading;

namespace LogViewerService
{
    public class cDomainGroup
    {
        private string m_GroupDN;
        private int m_SQLID;


        public cDomainGroup(int SQLID, string DN)
        {
            m_SQLID = SQLID;
            m_GroupDN = DN;
        }
        
        public cDomainGroup(string DN)
        {
            m_GroupDN = DN;
            using (SqlConnection cs = new SqlConnection(LogViewerService.sets.SqlServerString))
            {
                while (cs.State != System.Data.ConnectionState.Open)
                {
                    try
                    {
                        cs.Open();
                    }
                    catch (Exception e)
                    {
                        LogViewerService.Log("Unable to set connection: " + e.Message + "\n" + e.StackTrace);
                    }
                    Thread.Sleep(1000);

                }
                using (SqlCommand cm = new SqlCommand("SELECT ID FROM DomainGroups WHERE DNGroupName=@dnGroupName", cs))
                {
                    cm.Parameters.AddWithValue("@dnGroupName", DN);
                    object SQLID = cm.ExecuteScalar();
                    if (SQLID != null && SQLID != DBNull.Value)
                    {
                        m_SQLID = Convert.ToInt32(SQLID);
                    }
                    else
                    {
                        using (SqlCommand cm2 = new SqlCommand("INSERT INTO DomainGroups (DNGroupName) VALUES (@dnGroupName)", cs))
                        {
                            cm2.Parameters.AddWithValue("@dnGroupName", DN);
                            cm2.ExecuteNonQuery();
                        }
                        using (SqlCommand cm3 = new SqlCommand("SELECT @@IDENTITY;", cs))
                        {
                            this.m_SQLID = Convert.ToInt32(cm3.ExecuteScalar());
                        }


                    }
                }

            }
        }

        public int SQLID
        {
            get
            {
                return m_SQLID;
            }
        }

        public string DN
        {
            get
            {
                return m_GroupDN;
            }

        }
    }


    public class cDomainUser
    {
        private int m_IdSQL;
        private string m_UserName;
        private string m_SamAccount;
        private string m_DN;
        private bool m_Enabled;
        private List<cDomainGroup> m_memberOf;

        public cDomainUser(int ID, string SAMAccount)
        {
            m_memberOf = new List<cDomainGroup>();
            m_SamAccount = SAMAccount;
            m_IdSQL = ID;

        }

        public void clearGroups()
        {
            m_memberOf.Clear();

        }
        
        public cDomainUser(string SAMAccount)
        {
            using (SqlConnection cs = new SqlConnection(LogViewerService.sets.SqlServerString))
            {
                // cs.ConnectionTimeout = 0;
                //            @todo  Реализовать проверку подключения.

                while (cs.State != System.Data.ConnectionState.Open)
                {
                    try
                    {
                        cs.Open();
                    }
                    catch (Exception e)
                    {
                        LogViewerService.Log("Unable to set connection: " + e.Message + "\n" + e.StackTrace);
                    }
                    Thread.Sleep(1000);

                }
                m_memberOf = new List<cDomainGroup>();
                using (SqlCommand cm = new SqlCommand("SELECT ID_User FROM Users WHERE NTLogin=@ntLogin", cs))
                {
                    cm.Parameters.AddWithValue("@ntLogin", SAMAccount);
                    object IdSQL = cm.ExecuteScalar();
                    if (IdSQL != null && IdSQL != DBNull.Value)
                    {
                        m_IdSQL = Convert.ToInt32(IdSQL);
                    }
                    else
                    {
                        using (SqlCommand cm2 = new SqlCommand("INSERT INTO Users (NTLogin,Bypass) VALUES (@ntLogin,0) ", cs))
                        {
                            cm2.Parameters.AddWithValue("@ntLogin", SAMAccount);
                            cm2.ExecuteNonQuery();
                        }
                        using (SqlCommand cm2 = new SqlCommand("SELECT @@IDENTITY;", cs))
                        {
                            m_IdSQL = Convert.ToInt32(cm2.ExecuteScalar());
                        }
                    }
                }
            }
            m_SamAccount = SAMAccount;
        }

        public bool Enabled
        {
            get
            {
                return m_Enabled;
            }
            set
            {
                m_Enabled = value;
            }
        }

        public string DN
        {
            get
            {
                return m_DN;
            }
            set
            {
                m_DN = value;
            }
        }

        public string UserName
        {
            get
            {
                return m_UserName;
            }
            set
            {
                m_UserName = value;
            }

        }

        public List<cDomainGroup> memberOf
        {
            get
            {
                return m_memberOf;
            }
        }


        public void addDomainGroup(cDomainGroup domainGroup)
        {
            m_memberOf.Add(domainGroup);
        }

        private void addDisableInfo(SqlConnection cs)
        {
            
                using (SqlCommand cmd = new SqlCommand("INSERT INTO Users_ActivationHistory (ID_User,Status,Actts) VALUES (@idUser,@Status,GETDATE())", cs))
                {
                    cmd.Parameters.AddWithValue("@idUser", m_IdSQL);
                    cmd.Parameters.AddWithValue("@Status", this.m_Enabled);
                    cmd.ExecuteNonQuery();
                }
           

        }

        private void addGroups(SqlConnection cs)
        {
            int idGroupHistory;
            using (SqlCommand cmd = new SqlCommand("INSERT INTO Users_GroupHistory (ID_User,Actts) VALUES (@idUser,GETDATE())", cs))
            {
                cmd.Parameters.AddWithValue("@idUser", m_IdSQL);
                cmd.ExecuteNonQuery();
            }
            using (SqlCommand cmd = new SqlCommand("SELECT @@IDENTITY;", cs))
            {
                idGroupHistory = Convert.ToInt32(cmd.ExecuteScalar());
            }
            foreach (cDomainGroup gr in m_memberOf)
            {
                using (SqlCommand cmd = new SqlCommand("INSERT INTO Users_GroupHistoryDetail (ID_UserGroupHistory,ID_DomainGroup) VALUES (@idUserGroupHistory,@IdDomainGroup)", cs))
                {
                    cmd.Parameters.AddWithValue("@idUserGroupHistory", idGroupHistory);
                    cmd.Parameters.AddWithValue("@IdDomainGroup", gr.SQLID);
                    cmd.ExecuteNonQuery();
                }
            }
        }


        private void addDNInfo(SqlConnection cs)
        {
            using (SqlCommand cmd = new SqlCommand("INSERT INTO Users_DNHistory (ID_User,DN,Actts) VALUES (@idUser,@DN,GETDATE())", cs))
            {
                cmd.Parameters.AddWithValue("@idUser", m_IdSQL);
                cmd.Parameters.AddWithValue("@DN", this.m_DN);
                cmd.ExecuteNonQuery();
            }
        }


        public void processDBChanges()
        {
            using (SqlConnection cs = new SqlConnection(LogViewerService.sets.SqlServerString))
            {
                while (cs.State != System.Data.ConnectionState.Open)
                {
                    try
                    {
                        cs.Open();
                    }
                    catch (Exception e)
                    {
                        LogViewerService.Log("Unable to set connection: " + e.Message + "\n" + e.StackTrace);
                    }
                    Thread.Sleep(1000);

                }

                using (SqlCommand cmd = new SqlCommand("UPDATE Users SET [UserName]=@uName WHERE ID_User=@idU", cs))
                {
                    cmd.Parameters.AddWithValue("@uName", this.m_UserName);
                    cmd.Parameters.AddWithValue("@idU", this.m_IdSQL);
                    cmd.ExecuteNonQuery();
                }
                bool needToAddDNInfo = false;
                using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 * FROM Users_DNHistory WHERE ID_User=@idUser ORDER BY Actts DESC;", cs))
                {
                    cmd.Parameters.AddWithValue("@idUser", m_IdSQL);
                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        if (!rd.HasRows)
                        {
                            needToAddDNInfo = true;
                        }
                        else
                        {
                            rd.Read();
                            if ((rd["DN"] == null || rd["DN"] == DBNull.Value) && !String.IsNullOrWhiteSpace(m_DN))
                            {
                                needToAddDNInfo = true;
                            }
                            else
                                if (Convert.ToString(rd["DN"]) != m_DN)
                                {
                                    needToAddDNInfo = true;
                                }
                        }
                    }
                }
                if (needToAddDNInfo)
                    addDNInfo(cs);
                bool needToAddDisableInfo = false;
                using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 * FROM Users_ActivationHistory WHERE ID_User=@idUser ORDER BY Actts DESC", cs))
                {
                    cmd.Parameters.AddWithValue("@idUser", m_IdSQL);
                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        if (!rd.HasRows)
                        {

                        }
                        else
                        {
                            rd.Read();
                            if ((rd["Status"] == null || rd["Status"] == DBNull.Value))
                                needToAddDisableInfo = true;
                            else
                                if (Convert.ToBoolean(rd["Status"]) != m_Enabled)
                                    needToAddDisableInfo = true;
                        }
                    }
                }
                if (needToAddDisableInfo)
                    addDisableInfo(cs);
                bool needToAddGroups = false;
                List<int> groupsFrom = new List<int>();
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Users_GroupHistoryDetail WHERE ID_UserGroupHistory IN (SELECT TOP 1 ID FROM Users_GroupHistory WHERE ID_User=@idUser ORDER BY Actts DESC)", cs))
                {
                    cmd.Parameters.AddWithValue("@idUser", m_IdSQL);

                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        if (!rd.HasRows)
                        {
                            needToAddGroups = true;
                        }
                        else
                        {
                            while (rd.Read())
                            {
                                groupsFrom.Add(Convert.ToInt32(rd["ID_DomainGroup"]));
                            }
                        }
                    }
                }
                foreach (int gCode in groupsFrom)
                {
                    bool Found = false;
                    foreach (cDomainGroup group in this.m_memberOf)
                    {
                        if (group.SQLID == gCode)
                        {
                            Found = true;
                            break;
                        }
                    }
                    if (!Found)
                    {
                        needToAddGroups = true;
                        break;
                    }
                }
                foreach (cDomainGroup group in this.m_memberOf)
                {
                    bool Found = false;
                    foreach (int gCode in groupsFrom)
                    {
                        if (group.SQLID == gCode)
                        {
                            Found = true;
                            break;
                        }
                    }
                    if (!Found)
                    {
                        needToAddGroups = true;
                        break;
                    }

                }
                if (needToAddGroups)
                    addGroups(cs);


            }
        }

        public string SAM
        {
            get
            {
                return m_SamAccount;
            }

        }
    }
}
