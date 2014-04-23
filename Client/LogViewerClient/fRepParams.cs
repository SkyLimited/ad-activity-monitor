using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace LogViewerClient
{
    public partial class fRepParams : Form
    {
        private SqlDataAdapter dataAdapter;
        private DataTable dt;
        private Settings sets;
        private int Type;
        
        public fRepParams(Settings s, int Type)
        {
            InitializeComponent();
            string selectCommand = "SELECT * FROM Servers WHERE ByPass=0";
            dataAdapter = new SqlDataAdapter(selectCommand, s.SqlServerString);
            dt = new DataTable();
            dataAdapter.Fill(dt);
            cbServersList.DisplayMember = "ServerName";
            cbServersList.ValueMember = "ID_Server";
            cbServersList.DataSource = dt;            
            cbDateFilter_CheckedChanged(null, null);
            dataAdapter.Dispose();
            sets = s;
            this.Type = Type;

        }

        private void cbDateFilter_CheckedChanged(object sender, EventArgs e)
        {
            dtFrom.Enabled = cbDateFilter.Checked;
            dtTo.Enabled = cbDateFilter.Checked;
        }

        private void btnMake_Click(object sender, EventArgs e)
        {
            SqlConnection cs = new SqlConnection(sets.SqlServerString);
            cs.Open();
            SqlDataReader rd= null;
            repData repD =  new repData(); 
            if (Type == 1)
            {
                SqlCommand cmd = new SqlCommand("EXEC dbo.spRep" + this.Type.ToString() + " @IdServ,@dFrom,@dTo ", cs);
                cmd.Parameters.AddWithValue("@IdServ", cbServersList.SelectedValue);
                string dateS;
                if (!cbDateFilter.Checked)
                {
                    cmd.Parameters.AddWithValue("@dFrom", DBNull.Value);
                    cmd.Parameters.AddWithValue("@dTo", DBNull.Value);
                    dateS = " - ";
                }
                else
                {
                    cmd.Parameters.AddWithValue("@dFrom", dtFrom.Value);
                    cmd.Parameters.AddWithValue("@dTo", dtTo.Value);
                    dateS = dtFrom.Value.ToShortDateString() + " - " + dtTo.Value.ToShortDateString();
                }
                cmd.CommandTimeout = 0;
                rd = cmd.ExecuteReader();

                if (!rd.HasRows)
                {
                    MessageBox.Show("Данных для отчета нет", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                

                while (rd.Read())
                {
                    string User = Convert.ToString(rd["UserName"]);
                    DateTime lastAct = Convert.ToDateTime(rd["LastAct"]);
                    string Machines = Convert.ToString(rd["MachineList"]);

                    cReportData el = new cReportData(User, lastAct, Machines, cbServersList.Text, dateS);
                    repD.Add(el);
                }
            }
            else
            {
                SqlCommand cmd;
                string dateS;
                if (cbDateFilter.Checked)
                {
                    cmd = new SqlCommand("SELECT Machines.Name,MAX(Actions.actts) as actts,[Users].NTLogin FROM Actions INNER JOIN Machines ON Machines.ID_Machine = Actions.ID_Machine INNER JOIN Users ON Users.ID_User=Actions.ID_User WHERE Actions.ID_Server=@IdServ AND CAST (Actions.actts AS DATE) BETWEEN @dFrom AND @dTo AND Actions.ID_Machine!=80 AND Actions.ID_User!=388 AND RIGHT(Users.NTLogin,1)<>'$' GROUP BY Machines.Name,[Users].NTLogin ORDER BY Machines.Name ", cs);
                    cmd.Parameters.AddWithValue("@dFrom", dtFrom.Value);
                    cmd.Parameters.AddWithValue("@dTo", dtTo.Value);
                    dateS = dtFrom.Value.ToShortDateString() + " - " + dtTo.Value.ToShortDateString();
                }
                else
                {
                    cmd = new SqlCommand("SELECT Machines.Name,MAX(Actions.actts) as actts,[Users].NTLogin FROM Actions INNER JOIN Machines ON Machines.ID_Machine = Actions.ID_Machine INNER JOIN Users ON Users.ID_User=Actions.ID_User WHERE Actions.ID_Server=@IdServ AND Actions.ID_Machine!=80 AND RIGHT(Users.NTLogin,1)<>'$' AND Actions.ID_User!=388 GROUP BY Machines.Name,[Users].NTLogin ORDER BY Machines.Name", cs);
                     dateS = " - ";
                }
                cmd.Parameters.AddWithValue("@IdServ", cbServersList.SelectedValue);
                cmd.CommandTimeout = 0;
                rd = cmd.ExecuteReader();
                string MachineName = "", LastMachine = "";
                DateTime MaxTS = DateTime.Now;
                bool tsSet=false;
                string UserList = "";
                while (rd.Read())
                {
                    MachineName = Convert.ToString(rd["Name"]);                  
                    
                    
                    if (LastMachine != MachineName && LastMachine != "" && UserList!="")
                    {
                        cReportData el = new cReportData(LastMachine, MaxTS, UserList, cbServersList.Text, dateS);                       
                        repD.Add(el);
                        UserList = "";
                        tsSet = false;
                    }
                    if (!tsSet)
                    {
                        MaxTS = Convert.ToDateTime(rd["actts"]);
                        tsSet = true;
                    }
                    else
                    {
                        if(Convert.ToDateTime(rd["actts"]).CompareTo(MaxTS)>0)
                            MaxTS = Convert.ToDateTime(rd["actts"]);
                    }

                    UserList = UserList + Convert.ToString(rd["NTLogin"]) + ", ";
                    LastMachine = MachineName;
                }

            }
            rd.Close();
            cs.Close();
            fReportPreview previewForm = new fReportPreview(repD, this.Type );
            previewForm.ShowDialog();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
