using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;

namespace LogViewerClient
{
    public partial class fReportPreview : Form
    {
        private repData  data;
        
        public fReportPreview(repData data, int Type)
        {
            InitializeComponent();
            this.data = data;
            if(Type == 1)
                reportViewer1.LocalReport.ReportEmbeddedResource = "LogViewerClient.Report1.rdlc";
            else
                reportViewer1.LocalReport.ReportEmbeddedResource = "LogViewerClient.Report2.rdlc";
        }

        private void fReportPreview_Load(object sender, EventArgs e)
        {
            
            List<ReportParameter> lp = new List<ReportParameter>();
           // lp.Add(new ReportParameter("@ServerName",Server));
           // lp.Add(new ReportParameter("@dateD",dateS));
    //        this.reportViewer1.LocalReport. SetParameters(lp);
            cReportDataBindingSource.DataSource = data.GetData();
            
            this.reportViewer1.RefreshReport();
        }
    }
}
