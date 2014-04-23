using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogViewerClient
{
    public class cReportData
    {
        private string m_User;
        private DateTime m_lastLogin;
        private string m_ARMs;
        private string m_Server;
        private string m_DateD;

        public cReportData(string User, DateTime LastLogin, string ARMs,string Server, string DateD)
        {
            m_ARMs = ARMs;
            m_User = User;
            m_lastLogin = LastLogin;
            m_Server = Server;
            m_DateD = DateD;
        }

        public string User
        {
            get
            {
                return m_User;
            }
        }

        public DateTime lastLogin
        {
            get
            {
                return m_lastLogin;
            }
        }

        public string ARMs
        {
            get
            {
                return m_ARMs;
            }
        }

        public string Server
        {
            get
            {
                return m_Server;
            }
        }

        public string DateD
        {
            get
            {
                return m_DateD;
            }
        }
    }

    public class repData
    {
        private List<cReportData> m_list;

        public repData()
        {
            m_list = new List<cReportData>();
        }
        public void Add(cReportData el)
        {
            m_list.Add(el);
        }

        public List<cReportData> GetData()
        {
            return m_list;
        }
    }

}
