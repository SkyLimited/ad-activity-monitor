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
    public partial class fWelcome : Form
    {
        public fWelcome()
        {
            InitializeComponent();
        }

        private void clDispTimer_Tick(object sender, EventArgs e)
        {
            Settings s = new Settings();            
            fMain f = new fMain(s);
            f.Show();
            this.Visible = false;
            clDispTimer.Enabled = false;
        }
    }
}
