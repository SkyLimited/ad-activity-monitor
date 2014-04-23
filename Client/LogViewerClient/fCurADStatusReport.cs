using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LogViewerClient
{
    public partial class fCurADStatusReport : Form
    {
        private List<fADCurStatusReportModell> m_reportDataList;
        public fCurADStatusReport(List<fADCurStatusReportModell> reportDataList)
        {
            InitializeComponent();
            m_reportDataList = reportDataList;
        }

        private void fCurADStatusReport_Load(object sender, EventArgs e)
        {
            this.fADCurStatusReportModellBindingSource.DataSource = m_reportDataList.OrderBy(x => x.objectName );
            this.reportViewer1.RefreshReport();
        }
    }
}
