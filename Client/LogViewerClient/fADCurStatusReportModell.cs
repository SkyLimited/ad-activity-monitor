using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogViewerClient
{
    public class fADCurStatusReportModell
    {
        private cADObject m_Object;
        private bool m_status;
        private string m_DN;

        public fADCurStatusReportModell(cADObject obj, bool Enabled, string DN)
        {
            m_Object = obj;
            m_status = Enabled;
            m_DN = DN;
        }

        public string objectType
        {
            get
            {
                return m_Object.objType;
            }

        }

        public string objectName
        {
            get
            {
                return m_Object.Name;
            }
        }

        public string Status
        {
            get
            {
                if (m_status)
                    return "Включен";
                else
                    return "Выключен";
            }
        }

        public string DN
        {
            get
            {
                return fMain.translateDN(m_DN);
            }
        }
    }
}
