using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace LogViewerService
{
    public class cAvailCheckDate
    {
        public string IP {get; set;}
        public DateTime CheckDate {get; set;}
    }


    public class cDataPrecache
    {
        private List<cDomainUser> m_UsersList;
        private List<cDomainGroup> m_DomainGroups;
        private List<cDomainComputer> m_MachinesList;



        public List<cAvailCheckDate> AvailabilityChecks;



        public cDataPrecache(Settings sets)
        {
            m_UsersList = new List<cDomainUser>();
            m_DomainGroups = new List<cDomainGroup>();
            m_MachinesList = new List<cDomainComputer>();
            AvailabilityChecks = new List<cAvailCheckDate>();
            using (SqlConnection cs = new SqlConnection(sets.SqlServerString))
            {
                cs.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM DomainGroups", cs))
                {
                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            int id = Convert.ToInt32(rd["ID"]);
                            string dnName = Convert.ToString(rd["DNGroupName"]);
                            cDomainGroup dg = new cDomainGroup(id, dnName);
                            m_DomainGroups.Add(dg);
                        }
                    }
                }
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Machines", cs))
                {
                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            int id = Convert.ToInt32(rd["ID_Machine"]);
                            string dnName = Convert.ToString(rd["Name"]);
                            cDomainComputer cmp = new cDomainComputer(id, dnName);
                            m_MachinesList.Add(cmp);
                        }
                    }
                }
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Users", cs))
                {
                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            int id = Convert.ToInt32(rd["ID_User"]);
                            string dnName = Convert.ToString(rd["NTLogin"]);
                            cDomainUser us = new cDomainUser(id, dnName);
                            m_UsersList.Add(us);
                        }
                    }
                }
            
            }
        }

        public cDomainGroup findGroupByName(string name)
        {
            return m_DomainGroups.FirstOrDefault(x => x.DN == name);
        }

        public cDomainComputer findCompByDN(string DN)
        {
            return m_MachinesList.FirstOrDefault(x => x.DN == DN);
        }

        public cDomainUser findUserBySamAccount(string SamAccount)
        {
            return m_UsersList.FirstOrDefault(x => x.SAM == SamAccount);
        }

        public void addGroup(cDomainGroup dg)
        {
            m_DomainGroups.Add(dg);
        }

        public void addComp(cDomainComputer dcmp)
        {
            m_MachinesList.Add(dcmp);
        }

        public void addUser(cDomainUser dUs)
        {
            m_UsersList.Add(dUs);
        }
    }
}
