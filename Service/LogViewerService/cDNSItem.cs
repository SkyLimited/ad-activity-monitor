using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Management;
namespace LogViewerService
{
    class cDNSItem
    {
        private string m_HostName;
        private IPAddress[] m_IP;

        public cDNSItem(string HostName)
        {

            m_HostName = HostName;
            try
            {
                IPHostEntry entr = Dns.GetHostEntry(HostName);
                m_IP = entr.AddressList;
            }
            catch (Exception)
            {
                m_IP = new IPAddress[1];
                m_IP[0] = new IPAddress(0);
            }

        }

        public bool belongToIP(string IP)
        {
            if (m_IP.Any(x => x.ToString() == IP))
                return true;
            return false;
        }

        public string HostName
        {
            get
            {
                return m_HostName;
            }
        }

        public string IP
        {
            get
            {
                string res= "";
                foreach (IPAddress t in m_IP)
                {
                    res += t.ToString() + ";";
                }
                    
                    return res;
            }

        }
    }
}
