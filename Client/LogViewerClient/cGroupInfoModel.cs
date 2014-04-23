using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogViewerClient
{
    public class cGroupInfoModel : fADCurStatusReportModell
    {
        private LogViewerService.cDomainGroup m_domainGroup;

        public cGroupInfoModel(cADObject obj, bool Enabled, LogViewerService.cDomainGroup dGroup ) : base(obj, Enabled, "")
        {
            m_domainGroup = dGroup;
        }

        public string dGroupName
        {
            get
            {
                return fMain.translateDN(m_domainGroup.DN);
            }
        }
    }
}
