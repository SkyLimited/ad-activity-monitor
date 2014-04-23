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
    public partial class fGroupInfoReportPreview : Form
    {
        private List<cGroupInfoModel> m_repData;
        
        public fGroupInfoReportPreview(List<cGroupInfoModel> repData)
        {
            InitializeComponent();
            m_repData = repData;
        }

        private void fGroupInfoReportPreview_Load(object sender, EventArgs e)
        {
            cGroupInfoModelBindingSource.DataSource = m_repData.OrderBy(x => x.objectName);
            this.reportViewer1.RefreshReport();
        }
    }
}
