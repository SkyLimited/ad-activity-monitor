using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Threading;

namespace LogViewerService
{
   public class cDomainComputer
    {
        private int m_IdSQL;
        private string m_Name;
        private string m_DN;
        private bool m_Enabled;
        private List<cDomainGroup> m_memberOf;

        public cDomainComputer(int ID, string DN)
        {
            m_IdSQL = ID;
            m_DN = DN;
            m_memberOf = new List<cDomainGroup>();
        }

        public void clearGroups()
        {
            m_memberOf.Clear();
        }


        public cDomainComputer(string Name)
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
                m_memberOf = new List<cDomainGroup>();
                using (SqlCommand cm = new SqlCommand("SELECT ID_Machine FROM Machines WHERE Name=@cmpName", cs))
                {
                    cm.Parameters.AddWithValue("@cmpName", Name);
                    object IdSQL = cm.ExecuteScalar();
                    if (IdSQL != null && IdSQL != DBNull.Value)
                    {
                        m_IdSQL = Convert.ToInt32(IdSQL);

                    }
                    else
                    {

                        using (SqlCommand cm2 = new SqlCommand("INSERT INTO Machines (Name,Bypass) VALUES (@cmpName,0) ", cs))
                        {
                            cm2.Parameters.AddWithValue("@cmpName", Name);
                            cm2.ExecuteNonQuery();
                        }

                        using (SqlCommand cm3 = new SqlCommand("SELECT @@IDENTITY;", cs))
                        {
                            m_IdSQL = Convert.ToInt32(cm.ExecuteScalar());
                        }
                    }
                }
            }
            m_Name = Name;
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

        public string Name
        {
            get
            {
                return m_Name;
            }
            set
            {
                m_Name = value;
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
            using (SqlCommand cmd = new SqlCommand("INSERT INTO Machines_ActivationHistory (ID_Machine,Status,Actts) VALUES (@idMachine,@Status,GETDATE())", cs))
            {
                cmd.Parameters.AddWithValue("@idMachine", m_IdSQL);
                cmd.Parameters.AddWithValue("@Status", this.m_Enabled);
                cmd.ExecuteNonQuery();
            }


        }

        private void addGroups(SqlConnection cs)
        {
            int idGroupHistory;
            using (SqlCommand cmd = new SqlCommand("INSERT INTO Machines_GroupHistory (ID_Machine,Actts) VALUES (@idCmp,GETDATE())", cs))
            {
                cmd.Parameters.AddWithValue("@idCmp", m_IdSQL);
                cmd.ExecuteNonQuery();
            }
            using (SqlCommand cmd = new SqlCommand("SELECT @@IDENTITY;", cs))
            {
                idGroupHistory = Convert.ToInt32(cmd.ExecuteScalar());
            }
            foreach (cDomainGroup gr in m_memberOf)
            {
                using (SqlCommand cmd = new SqlCommand("INSERT INTO Machines_GroupHistoryDetail (ID_MachineGroupHistory,ID_DomainGroup) VALUES (@idMachineGroupHistory,@IdDomainGroup)", cs))
                {
                    cmd.Parameters.AddWithValue("@idMachineGroupHistory", idGroupHistory);
                    cmd.Parameters.AddWithValue("@IdDomainGroup", gr.SQLID);
                    cmd.ExecuteNonQuery();
                }
            }
        }


        private void addDNInfo(SqlConnection cs)
        {
            using (SqlCommand cmd = new SqlCommand("INSERT INTO Machines_DNHistory (ID_Machine,DN,Actts) VALUES (@idMachine,@DN,GETDATE())", cs))
            {
                cmd.Parameters.AddWithValue("@idMachine", m_IdSQL);
                cmd.Parameters.AddWithValue("@DN", this.m_DN);
                cmd.ExecuteNonQuery();
            }
        }


        public void processDBChanges()
        {
            using(SqlConnection cs = new SqlConnection(LogViewerService.sets.SqlServerString))
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
            bool needToAddDNInfo = false;
            using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 * FROM Machines_DNHistory WHERE ID_Machine=@idCmp ORDER BY Actts DESC;", cs))
            {
                cmd.Parameters.AddWithValue("@idCmp", m_IdSQL);
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
            using(SqlCommand cmd = new SqlCommand("SELECT TOP 1 * FROM Machines_ActivationHistory WHERE ID_Machine=@idCmp ORDER BY Actts DESC", cs))
            {
                 cmd.Parameters.AddWithValue("@idCmp", m_IdSQL);
                 using (SqlDataReader rd = cmd.ExecuteReader())
                 {
                     if (!rd.HasRows)
                     {
                         needToAddDisableInfo = true;
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
            bool needAddGroupInfo = false;
            List<int> groupsFrom = new List<int>();
            using(SqlCommand cmd = new SqlCommand("SELECT * FROM Machines_GroupHistoryDetail WHERE ID_MachineGroupHistory IN (SELECT TOP 1 ID FROM Machines_GroupHistory WHERE ID_Machine=@idCmp ORDER BY Actts DESC)", cs))
            {
                cmd.Parameters.AddWithValue("@idCmp", m_IdSQL);
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    if (!rd.HasRows)
                    {
                        needAddGroupInfo = true;
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
                        needAddGroupInfo = true;
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
                        needAddGroupInfo = true;
                        break;
                    }

                }
                if (needAddGroupInfo)
                    addGroups(cs);
            }
        }
    }
}
