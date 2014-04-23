using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using Microsoft.Office.Interop.Excel;
using System.Reflection;

namespace LogViewerClient
{
    public partial class fMain : Form
    {
        private Settings s;

        private SqlDataReader dr;
        public repData DataForReport;
        private string orderBy;
        private int sortColIndex;
        private bool sortAsc;
        public List<cADLogRecord> adLogs;

        private bool _isSortAscending;
        private DataGridViewColumn _sortColumn;

        private Control lastFocused;

        public fMain(Settings s)
        {

            InitializeComponent();
            //   orderBy = "6 DESC";
            this.s = s;
            dataGridView1.DataSource = bindingSource1;
            dtFrom.Value = DateTime.Now.AddDays(-7);
            sortColIndex = -1;
            sortAsc = false;
            getData();
        }

        private void getData()
        {

            tbTextFilter.Enabled = false;
            cbGroupByARM.Enabled = false;
            cbUndefinedArms.Enabled = false;
            cbSystemAccount.Enabled = false;
            cbDomainARMs.Enabled = false;
            dtFrom.Enabled = false;
            dtTo.Enabled = false;
            cbNotDomainARMs.Enabled = false;
            toolStripProgressBar1.ProgressBar.Style = ProgressBarStyle.Marquee;
            if (!cbUndefinedArms.Checked && !cbDomainARMs.Checked && !cbNotDomainARMs.Checked && !cbSystemAccount.Checked)
            {
                dataGridView1.DataSource = null;

                bindingSource1.DataSource = null;
                bindingSource1.Clear();
                dataGridView1.Refresh();
                tbTextFilter.Enabled = true;
                cbGroupByARM.Enabled = true;
                cbUndefinedArms.Enabled = true;
                cbSystemAccount.Enabled = true;
                cbDomainARMs.Enabled = true;
                dtFrom.Enabled = true;
                dtTo.Enabled = true;
                cbNotDomainARMs.Enabled = true;
                toolStripProgressBar1.ProgressBar.Style = ProgressBarStyle.Continuous;
                return;
            }


            if (!backgroundWorker1.IsBusy)
                backgroundWorker1.RunWorkerAsync();

        }

        private void getADdata()
        {
            lastFocused = this.ActiveControl;
            tbADFilterField.Enabled = false;
            dtADFrom.Enabled = false;
            dtADto.Enabled = false;
            cbADShowArms.Enabled = false;
            cbADShowEnableds.Enabled = false;
            cbADShowGroups.Enabled = false;
            cbADShowUnits.Enabled = false;
            cbADShowUsers.Enabled = false;
      
            
            if (!backgroundWorker2.IsBusy)
                backgroundWorker2.RunWorkerAsync();

        }

        private void fMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            try { Environment.Exit(0); }
            catch (Exception) { };
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            getData();
        }

        private void checkBox2_Click(object sender, EventArgs e)
        {
            getData();
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            getData();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            getData();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            getData();
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            getData();
        }

        private void доменыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fSprav f = new fSprav("Domains", s);
            f.ShowDialog();
        }

        private void компьютерыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fSprav f = new fSprav("Machines", s);
            f.ShowDialog();
        }

        private void пользователиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fSprav f = new fSprav("Users", s);
            f.ShowDialog();
        }

        private void серверыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fSprav f = new fSprav("Servers", s);
            f.ShowDialog();
        }

        private void сохранитьКакToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult sd = saveFileDialog1.ShowDialog();
            if (sd == DialogResult.OK)
            {
                Microsoft.Office.Interop.Excel.Application ExcelApp = new Microsoft.Office.Interop.Excel.Application();
                Workbook ExcelWorkBook;
                Worksheet ExcelWorkSheet;

                ExcelWorkBook = ExcelApp.Workbooks.Add(Missing.Value);
                ExcelWorkSheet = (Worksheet)ExcelWorkBook.Worksheets.get_Item(1);
                if (tabControl1.SelectedTab == tabPage1)
                {
                    for (int i = 0; i < dataGridView1.Rows.Count; i++)
                        for (int j = 0; j < dataGridView1.ColumnCount; j++)
                            ExcelWorkSheet.Cells[i + 1, j + 1] = dataGridView1.Rows[i].Cells[j].Value;
                }
                else
                    for (int i = 0; i < dataGridView2.Rows.Count; i++)
                        for (int j = 0; j < dataGridView2.ColumnCount; j++)
                            ExcelWorkSheet.Cells[i + 1, j + 1] = dataGridView2.Rows[i].Cells[j].Value;
                ExcelWorkBook.SaveAs(saveFileDialog1.FileName);
                ExcelApp.Quit();
            }

        }

        private void отчетПоИспользовавшимсяАРМToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fRepParams fRp = new fRepParams(s, 1);
            fRp.ShowDialog();

        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fAboutBox fc = new fAboutBox();
            fc.ShowDialog();

        }

        private void cbGroupByARM_Click(object sender, EventArgs e)
        {
            getData();
        }

        private void cbSystemAccount_Click(object sender, EventArgs e)
        {
            getData();
        }


        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            if (dataGridView1.Columns.Contains("ID_RecordType"))
                dataGridView1.Columns["ID_RecordType"].Visible = false;
            // dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);

            tbTextFilter.Enabled = true;
            cbGroupByARM.Enabled = true;
            cbUndefinedArms.Enabled = true;
            cbSystemAccount.Enabled = true;
            cbDomainARMs.Enabled = true;
            dtFrom.Enabled = true;
            dtTo.Enabled = true;
            cbNotDomainARMs.Enabled = true;
            toolStripProgressBar1.ProgressBar.Style = ProgressBarStyle.Continuous;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            string selectCommand;
            if (!cbGroupByARM.Checked)
                selectCommand = "SELECT  * from dbo.AllActions WHERE ";
            else
                selectCommand = "SELECT * from dbo.GroupedActions WHERE ";
            string clause = " CAST([Дата/Время] as Date) BETWEEN @p1 and @p2 ";



            if (cbUndefinedArms.Checked || cbDomainARMs.Checked || cbNotDomainARMs.Checked || cbSystemAccount.Checked)
            {
                bool first = false;
                clause += " AND ID_RecordType IN (";
                if (cbUndefinedArms.Checked)
                {
                    clause += "2";
                    first = true;
                }
                if (cbDomainARMs.Checked)
                {
                    if (first)
                        clause += ",";
                    clause += "0";
                    first = true;
                }
                if (cbNotDomainARMs.Checked)
                {
                    if (first)
                        clause += ",";
                    clause += "1";
                    first = true;
                }
                if (cbSystemAccount.Checked)
                {
                    if (first)
                        clause += ",";
                    clause += "3";
                    first = true;
                }

                clause += ")";

            }
            if (!String.IsNullOrWhiteSpace(tbTextFilter.Text))
            {
                clause += "  AND [Пользователь] LIKE @tf ";
            }
            selectCommand = selectCommand + clause + this.orderBy;
            /* dataAdapter = new SqlDataAdapter(selectCommand, s.ConnectionString);*/
            using (SqlConnection cs = new SqlConnection(s.SqlServerString))
            {
                cs.Open();
                using (SqlCommand cmd = new SqlCommand(selectCommand, cs))
                {

                    cmd.Parameters.AddWithValue("p1", dtFrom.Value);
                    cmd.Parameters.AddWithValue("p2", dtTo.Value);
                    if (!String.IsNullOrWhiteSpace(tbTextFilter.Text))
                        cmd.Parameters.AddWithValue("tf", "%" + tbTextFilter.Text + "%");
                    try
                    {
                        dr = cmd.ExecuteReader();
                    }
                    catch (Exception) { }
                    lock (dataGridView1)
                    {
                        if (this.dataGridView1.InvokeRequired)
                        {
                            this.dataGridView1.Invoke((MethodInvoker)delegate
                           {

                               if (dr.HasRows)
                               {
                                   bindingSource1.DataSource = dr;
                                   dataGridView1.DataSource = bindingSource1;
                               }
                               else
                               {

                                   bindingSource1.DataSource = null;
                                   dataGridView1.DataSource = bindingSource1;
                               }

                               if (sortColIndex != -1)
                               {
                                   if (sortAsc)
                                       dataGridView1.Columns[sortColIndex].HeaderCell.SortGlyphDirection = System.Windows.Forms.SortOrder.Ascending;
                                   else
                                       dataGridView1.Columns[sortColIndex].HeaderCell.SortGlyphDirection = System.Windows.Forms.SortOrder.Descending;
                               }

                           });
                        }
                        else
                            bindingSource1.DataSource = dr;
                    }
                }
            }
        }

        private void dataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {


            int cIndex = e.ColumnIndex + 1;


            if (orderBy == " ORDER BY " + cIndex.ToString() + " DESC" || orderBy != " ORDER BY " + cIndex.ToString())
            {
                orderBy = " ORDER BY " + cIndex.ToString();
                dataGridView1.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection = System.Windows.Forms.SortOrder.Ascending;
                sortAsc = true;
            }
            else
            {
                orderBy = " ORDER BY " + cIndex.ToString() + " DESC";
                dataGridView1.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection = System.Windows.Forms.SortOrder.Descending;
                sortAsc = false;
            }
            getData();
            sortColIndex = e.ColumnIndex;
        }

        public static string translateDN(string DN)
        {
            string CN = DN.Substring(DN.IndexOf("CN=") + 3, DN.IndexOf(",") - 3);
            return string.Join("/", DN.Split(',').Where(sig => sig.StartsWith("OU")).Reverse().ToArray()).Replace("OU=", "") + "/" + CN;
        }

        private void отчетПоПодключениямАРМToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fRepParams fRp = new fRepParams(s, 2);
            fRp.ShowDialog();
        }

        private void cbNotDomainARMs_Click(object sender, EventArgs e)
        {
            getData();
        }

        private void cbSystemAccount_Click_1(object sender, EventArgs e)
        {
            getData();
        }

        private void cbUndefinedArms_Click(object sender, EventArgs e)
        {
            getData();
        }

        private void cbDomainARMs_Click(object sender, EventArgs e)
        {
            getData();
        }

        private void cbGroupByARM_Click_1(object sender, EventArgs e)
        {
            getData();
        }

        private void dtFrom_ValueChanged(object sender, EventArgs e)
        {
            getData();
        }

        private void dtTo_ValueChanged(object sender, EventArgs e)
        {
            getData();
        }

        private void fillMachinesActivationHistory(ref List<cADLogRecord> adLogs)
        {
            using (SqlConnection cs = new SqlConnection(s.SqlServerString))
            {
                cs.Open();
                using (SqlCommand cm = new SqlCommand("SELECT Machines_ActivationHistory.ID_Machine,Machines.Name,Machines_ActivationHistory.ActTs,Machines_ActivationHistory.Status FROM Machines_ActivationHistory INNER JOIN Machines ON Machines.ID_Machine=Machines_ActivationHistory.ID_Machine  WHERE Machines.Name LIKE @mname and cast(Actts as date) between @dFrom and @dTo ORDER BY Actts DESC", cs))
                {
                    cm.Parameters.AddWithValue("@mname", "%" + tbADFilterField.Text + "%");
                    cm.Parameters.AddWithValue("@dFrom", dtADFrom.Value.Subtract(dtADFrom.Value.TimeOfDay));
                    cm.Parameters.AddWithValue("@dTo", dtADto.Value.Subtract(dtADto.Value.TimeOfDay));
                    using (SqlDataReader rd = cm.ExecuteReader())
                    {
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                cADObject obj = new cADObject(Convert.ToInt32(rd["ID_Machine"]), Convert.ToString(rd["Name"]), cObjectTypes.Computer);
                                cEventTypes evtType;
                                if (Convert.ToInt32(rd["Status"]) == 1)
                                    evtType = cEventTypes.Enabled;
                                else
                                    evtType = cEventTypes.Disabled;
                                cADLogRecord rec = new cADLogRecord(obj, Convert.ToDateTime(rd["ActTs"]), "", evtType);
                                adLogs.Add(rec);
                            }
                        }
                    }
                }
            }

        }

        private void fillMachinesDNHistory(ref List<cADLogRecord> adLogs)
        {
            using (SqlConnection cs = new SqlConnection(s.SqlServerString))
            {
                cs.Open();
                using (SqlCommand cm = new SqlCommand("SELECT Machines_DNHistory.ID_Machine,Machines.Name,Machines_DNHistory.ActTs,Machines_DNHistory.DN FROM Machines_DNHistory INNER JOIN Machines ON Machines.ID_Machine=Machines_DNHistory.ID_Machine  WHERE Machines.Name LIKE @mname and cast(Actts as date) between @dFrom and @dTo ORDER BY Actts DESC", cs))
                {
                    cm.Parameters.AddWithValue("@mname", "%" + tbADFilterField.Text + "%");
                    cm.Parameters.AddWithValue("@dFrom", dtADFrom.Value.Subtract(dtADFrom.Value.TimeOfDay));
                    cm.Parameters.AddWithValue("@dTo", dtADto.Value.Subtract(dtADto.Value.TimeOfDay));
                    using (SqlDataReader rd = cm.ExecuteReader())
                    {
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                cADObject obj = new cADObject(Convert.ToInt32(rd["ID_Machine"]), Convert.ToString(rd["Name"]), cObjectTypes.Computer);
                                string DN = fMain.translateDN(Convert.ToString(rd["DN"]));
                                cADLogRecord rec = new cADLogRecord(obj, Convert.ToDateTime(rd["ActTs"]), DN, cEventTypes.New);
                                TimeSpan tDif = TimeSpan.MaxValue;
                                cADLogRecord foundObj = null;
                                foreach (cADLogRecord recZ in adLogs)
                                {
                                    if (recZ.eventTime <= rec.eventTime || recZ.eventTypeValue != cEventTypes.New) continue;
                                    if (cADObject.compareObjects(rec.Object, recZ.Object))
                                    {
                                        if (tDif == TimeSpan.MaxValue)
                                        {
                                            tDif = recZ.eventTime - rec.eventTime;
                                            foundObj = recZ;
                                        }
                                        else
                                        {
                                            if (recZ.eventTime - rec.eventTime < tDif)
                                            {
                                                tDif = recZ.eventTime - rec.eventTime;
                                                foundObj = recZ;
                                            }
                                        }

                                    }

                                }
                                if (foundObj != null)
                                {
                                    foundObj.oldValue = DN;

                                }
                                adLogs.Add(rec);
                            }
                        }
                    }
                }

            }
        }

        private void fillMachinesGChangeHistory(ref List<cADLogRecord> adLogs)
        {
            using (SqlConnection cs = new SqlConnection(s.SqlServerString))
            {
                cs.Open();
                LogViewerService.LogViewerService.sets = s;
                TimeSpan tDif2 = TimeSpan.MaxValue;
                using (SqlCommand cmd = new SqlCommand("SELECT Machines.ID_Machine,Machines.Name,Machines_GroupHistory.Actts,Machines_GroupHistoryDetail.ID_DomainGroup,DomainGroups.DNGroupName FROM Machines_GroupHistoryDetail INNER JOIN  Machines_GroupHistory ON Machines_GroupHistory.ID=Machines_GroupHistoryDetail.ID_MachineGroupHistory INNER JOIN Machines ON Machines.ID_Machine = Machines_GroupHistory.ID_Machine INNER JOIN DomainGroups ON DomainGroups.ID= Machines_GroupHistoryDetail.ID_DomainGroup WHERE CAST(Machines_GroupHistory.Actts AS DATE) <= @d2 and Machines.Name like @tf ORDER BY Machines.ID_Machine,Machines_GroupHistory.Actts DESC", cs))
                {

                //    cmd.Parameters.AddWithValue("@d1", dtADFrom.Value.Subtract(dtADFrom.Value.TimeOfDay));
                    cmd.Parameters.AddWithValue("@d2", dtADto.Value.Subtract(dtADto.Value.TimeOfDay));
                    cmd.Parameters.AddWithValue("@tf", "%" + tbADFilterField.Text + "%");

                    DateTime lastAct = DateTime.MinValue;
                    cADLogRecord cRec = null;
                    cADLogRecord foundObj = null;
                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            int cMachineID = Convert.ToInt32(rd["ID_Machine"]);
                            DateTime cDateTime = Convert.ToDateTime(rd["Actts"]);
                            cADObject obj = new cADObject(cMachineID, Convert.ToString(rd["Name"]), cObjectTypes.Computer);
                            LogViewerService.cDomainGroup dGroup = new LogViewerService.cDomainGroup(Convert.ToInt32(rd["ID_DomainGroup"]), Convert.ToString(rd["DNGroupName"]));
                            if (lastAct != cDateTime)
                            {
                                //processing Changes
                                if (lastAct != DateTime.MinValue)
                                {
                                    if(cRec.eventTime.Date >= dtADFrom.Value.Date)
                                        adLogs.Add(cRec); //not first time
                               
                                    lastAct = cDateTime;
                                    if (foundObj != null)
                                    {
                                        foreach (LogViewerService.cDomainGroup dGrp2 in cRec.domainGroups)
                                        {
                                            foundObj.addPreviousGroup(dGrp2);
                                        }
                                    }
                                    cRec = new cADLogRecord(obj, cDateTime, "", cEventTypes.GroupChanged);
                                    foundObj = null;
                                    tDif2 = TimeSpan.MaxValue;
                                    foreach (cADLogRecord recZ in adLogs)
                                    {
                                        if (recZ.eventTime <= cRec.eventTime || recZ.eventTypeValue != cEventTypes.GroupChanged || recZ.HavePrevGroups) continue;
                                        if (cADObject.compareObjects(cRec.Object, recZ.Object))
                                        {
                                            if (tDif2 == TimeSpan.MaxValue)
                                            {
                                                tDif2 = recZ.eventTime - cRec.eventTime;
                                                foundObj = recZ;
                                            }
                                            else
                                            {
                                                if (recZ.eventTime - cRec.eventTime < tDif2)
                                                {
                                                    tDif2 = recZ.eventTime - cRec.eventTime;
                                                    foundObj = recZ;
                                                }
                                            }

                                        }

                                    }                                


                                }
                                else
                                {
                                    cRec = new cADLogRecord(obj, cDateTime, "", cEventTypes.GroupChanged);
                                    lastAct = cDateTime;

                                }

                            }
                            cRec.addCurrentGroup(dGroup);
                        }
                        
                            if (foundObj != null)
                            {
                                foreach (LogViewerService.cDomainGroup dGrp2 in cRec.domainGroups)
                                {
                                    foundObj.addPreviousGroup(dGrp2);
                                }
                            }
                         if(cRec != null)
                           if (cRec.eventTime.Date >= dtADFrom.Value.Date)
                             adLogs.Add(cRec);
                        
                    }
               
                }
            }
        }

        private void fillUsersActivationHistory(ref List<cADLogRecord> adLogs)
        {
            using (SqlConnection cs = new SqlConnection(s.SqlServerString))
            {
                cs.Open();
                using (SqlCommand cm = new SqlCommand("SELECT Users_ActivationHistory.ID_User,Users.NTLogin,Users_ActivationHistory.ActTs,Users_ActivationHistory.Status FROM Users_ActivationHistory INNER JOIN Users ON Users.ID_User=Users_ActivationHistory.ID_User  WHERE Users.NTLogin LIKE @mname and cast(Actts as date) between @dFrom and @dTo ORDER BY Actts DESC", cs))
                {
                    cm.Parameters.AddWithValue("@mname", "%" + tbADFilterField.Text + "%");
                    cm.Parameters.AddWithValue("@dFrom", dtADFrom.Value.Subtract(dtADFrom.Value.TimeOfDay));
                    cm.Parameters.AddWithValue("@dTo", dtADto.Value.Subtract(dtADto.Value.TimeOfDay));
                    using (SqlDataReader rd = cm.ExecuteReader())
                    {
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                cADObject obj = new cADObject(Convert.ToInt32(rd["ID_User"]), Convert.ToString(rd["NTLogin"]), cObjectTypes.User);
                                cEventTypes evtType;
                                if (Convert.ToInt32(rd["Status"]) == 1)
                                    evtType = cEventTypes.Enabled;
                                else
                                    evtType = cEventTypes.Disabled;
                                cADLogRecord rec = new cADLogRecord(obj, Convert.ToDateTime(rd["ActTs"]), "", evtType);
                                adLogs.Add(rec);
                            }
                        }
                    }
                }
            }
        }

        private void fillUsersDNHistory(ref List<cADLogRecord> adLogs)
        {
            using (SqlConnection cs = new SqlConnection(s.SqlServerString))
            {
                cs.Open();
                string fText = tbADFilterField.Text;
                using (SqlCommand cm = new SqlCommand("SELECT Users_DNHistory.ID_User,Users.NTLogin,Users_DNHistory.ActTs,Users_DNHistory.DN FROM Users_DNHistory INNER JOIN Users ON Users.ID_User=Users_DNHistory.ID_User  WHERE Users.NTLogin LIKE @mname and cast(Actts as date) between @dFrom and @dTo ORDER BY Actts DESC", cs))
                {
                    cm.Parameters.AddWithValue("@mname", "%" + fText + "%");
                    cm.Parameters.AddWithValue("@dFrom", dtADFrom.Value.Subtract(dtADFrom.Value.TimeOfDay));
                    cm.Parameters.AddWithValue("@dTo", dtADto.Value.Subtract(dtADto.Value.TimeOfDay));
                    using (SqlDataReader rd = cm.ExecuteReader())
                    {
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                cADObject obj = new cADObject(Convert.ToInt32(rd["ID_User"]), Convert.ToString(rd["NTLogin"]), cObjectTypes.User);
                                string DN = fMain.translateDN(Convert.ToString(rd["DN"]));
                                cADLogRecord rec = new cADLogRecord(obj, Convert.ToDateTime(rd["ActTs"]), DN, cEventTypes.New);
                                TimeSpan tDif = TimeSpan.MaxValue;
                                cADLogRecord foundObj = null;
                                foreach (cADLogRecord recZ in adLogs)
                                {
                                    if (recZ.eventTime <= rec.eventTime || recZ.eventTypeValue != cEventTypes.New) continue;
                                    if (cADObject.compareObjects(rec.Object, recZ.Object))
                                    {
                                        if (tDif == TimeSpan.MaxValue)
                                        {
                                            tDif = recZ.eventTime - rec.eventTime;
                                            foundObj = recZ;
                                        }
                                        else
                                        {
                                            if (recZ.eventTime - rec.eventTime < tDif)
                                            {
                                                tDif = recZ.eventTime - rec.eventTime;
                                                foundObj = recZ;
                                            }
                                        }

                                    }

                                }
                                if (foundObj != null)
                                {
                                    foundObj.oldValue = DN;

                                }
                                adLogs.Add(rec);
                            }
                        }
                    }
                }

            }
        }
        private void fillUsersGCHistory(ref List<cADLogRecord> adLogs)
        {
            using (SqlConnection cs = new SqlConnection(s.SqlServerString))
            {
                cs.Open();
                LogViewerService.LogViewerService.sets = s;
                TimeSpan tDif2 = TimeSpan.MaxValue;
                using (SqlCommand cmd = new SqlCommand("SELECT Users.ID_User,Users.NTLogin,Users_GroupHistory.Actts,Users_GroupHistoryDetail.ID_DomainGroup,DomainGroups.DNGroupName FROM Users_GroupHistoryDetail INNER JOIN  Users_GroupHistory ON Users_GroupHistory.ID=Users_GroupHistoryDetail.ID_UserGroupHistory INNER JOIN Users ON Users.ID_User = Users_GroupHistory.ID_User INNER JOIN DomainGroups ON DomainGroups.ID= Users_GroupHistoryDetail.ID_DomainGroup WHERE CAST(Users_GroupHistory.Actts AS DATE) <= @d2 and Users.NTLogin like @tf  ORDER BY Users.ID_User,Users_GroupHistory.Actts DESC", cs))
                {

                //    cmd.Parameters.AddWithValue("@d1", dtADFrom.Value.Subtract(dtADFrom.Value.TimeOfDay));
                    cmd.Parameters.AddWithValue("@d2", dtADto.Value.Subtract(dtADto.Value.TimeOfDay));
                    cmd.Parameters.AddWithValue("@tf", "%" + tbADFilterField.Text + "%");

                    DateTime lastAct = DateTime.MinValue;
                    cADLogRecord cRec = null;
                    cADLogRecord foundObj=null;
                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            int cMachineID = Convert.ToInt32(rd["ID_User"]);
                            DateTime cDateTime = Convert.ToDateTime(rd["Actts"]);
                            cADObject obj = new cADObject(cMachineID, Convert.ToString(rd["NTLogin"]), cObjectTypes.User);
                            LogViewerService.cDomainGroup dGroup = new LogViewerService.cDomainGroup(Convert.ToInt32(rd["ID_DomainGroup"]), Convert.ToString(rd["DNGroupName"]));
                            if (lastAct != cDateTime)
                            {
                                //processing Changes
                                if (lastAct != DateTime.MinValue)
                                {
                                    if(cRec.eventTime.Date>=dtADFrom.Value.Date)
                                        adLogs.Add(cRec); //not first time
                               
                                    lastAct = cDateTime;
                                    if (foundObj != null)
                                    {
                                        foreach (LogViewerService.cDomainGroup dGrp2 in cRec.domainGroups)
                                        {
                                            foundObj.addPreviousGroup(dGrp2);
                                        }
                                    }
                                    cRec = new cADLogRecord(obj, cDateTime, "", cEventTypes.GroupChanged);
                                    foundObj = null;
                                    tDif2 = TimeSpan.MaxValue;
                                    foreach (cADLogRecord recZ in adLogs)
                                    {
                                        if (recZ.eventTime <= cRec.eventTime || recZ.eventTypeValue != cEventTypes.GroupChanged || recZ.HavePrevGroups) continue;
                                        
                                        if (cADObject.compareObjects(cRec.Object, recZ.Object))
                                        {
                                            if (tDif2 == TimeSpan.MaxValue)
                                            {
                                                tDif2 = recZ.eventTime - cRec.eventTime;
                                                foundObj = recZ;
                                            }
                                            else
                                            {
                                                if (recZ.eventTime - cRec.eventTime < tDif2)
                                                {
                                                    tDif2 = recZ.eventTime - cRec.eventTime;
                                                    foundObj = recZ;
                                                }
                                            }

                                        }

                                    }                                


                                }
                                else
                                {
                                    cRec = new cADLogRecord(obj, cDateTime, "", cEventTypes.GroupChanged);
                                    lastAct = cDateTime;

                                }

                            }
                            cRec.addCurrentGroup(dGroup);
                        }
                        
                            if (foundObj != null)
                            {
                                foreach (LogViewerService.cDomainGroup dGrp2 in cRec.domainGroups)
                                {
                                    foundObj.addPreviousGroup(dGrp2);
                                }
                            }
                         if(cRec != null)
                            if(cRec.eventTime.Date>=dtADFrom.Value.Date)
                                adLogs.Add(cRec);
                        
                    }
                }
            }
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                if (adLogs != null)
                {
                    lock (adLogs)
                    {
                        adLogs = new List<cADLogRecord>();
                    }
                }
                else
                    adLogs = new List<cADLogRecord>();
                if (cbADShowArms.Checked)
                {
                    if (cbADShowEnableds.Checked)
                        fillMachinesActivationHistory(ref adLogs);
                    if (cbADShowUnits.Checked)
                        fillMachinesDNHistory(ref adLogs);
                    if (cbADShowGroups.Checked)
                        fillMachinesGChangeHistory(ref adLogs);
                }
                if (cbADShowUsers.Checked)
                {
                    if (cbADShowEnableds.Checked)
                        fillUsersActivationHistory(ref adLogs);
                    if (cbADShowUnits.Checked)
                        fillUsersDNHistory(ref adLogs);
                    if (cbADShowGroups.Checked)
                        fillUsersGCHistory(ref adLogs);
                }


                if (this.dataGridView2.InvokeRequired)
                    this.dataGridView2.Invoke((MethodInvoker)delegate
                    {
                        bindingSource2.DataSource = adLogs;
                        dataGridView2.DataSource = bindingSource2;
                        dataGridView2.Columns[4].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                        dataGridView2.Columns[5].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                        dataGridView2.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                        dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader;

                    });
                else
                {
                    bindingSource2.DataSource = adLogs;
                    dataGridView2.DataSource = bindingSource2;
                    dataGridView2.Columns[4].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                    dataGridView2.Columns[5].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                    dataGridView2.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                    dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader;
                }


            }
            catch (Exception e6)
            {
                string h = e6.Message;
            }
        }

        private void dtADFrom_ValueChanged(object sender, EventArgs e)
        {
            getADdata();
        }

        private void dtADto_ValueChanged(object sender, EventArgs e)
        {
            getADdata();
        }

        private void tbADFilterField_TextChanged(object sender, EventArgs e)
        {
            if (tbADFilterField.Text.Length < 3) return;
            getADdata();
        }

        private void backgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            tbADFilterField.Enabled = true;
            dtADFrom.Enabled = true;
            dtADto.Enabled = true;

            cbADShowArms.Enabled = true;
            cbADShowEnableds.Enabled = true;
            cbADShowGroups.Enabled = true;
            cbADShowUnits.Enabled = true;
            cbADShowUsers.Enabled = true;

            this.ActiveControl = lastFocused;
        }

        private void cbADShowUsers_CheckedChanged(object sender, EventArgs e)
        {
            getADdata();
        }

        private void cbADShowArms_CheckedChanged(object sender, EventArgs e)
        {
            getADdata();
        }

        private void cbADShowEnableds_CheckedChanged(object sender, EventArgs e)
        {
            getADdata();
        }

        private void cbADShowUnits_CheckedChanged(object sender, EventArgs e)
        {
            getADdata();
        }

        private void cbADShowGroups_CheckedChanged(object sender, EventArgs e)
        {
            getADdata();
        }

        private void dataGridView2_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {


        }

        private void dataGridView2_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            DataGridViewColumn column = dataGridView2.Columns[e.ColumnIndex];

            _isSortAscending = (_sortColumn == null || _isSortAscending == false);

            string direction = _isSortAscending ? "ASC" : "DESC";

            if (_isSortAscending)
            {
                if (e.ColumnIndex == 0)
                    bindingSource2.DataSource = adLogs.OrderBy(x => x.Object.ToString()).ToList();
                if (e.ColumnIndex == 1)
                    bindingSource2.DataSource = adLogs.OrderBy(x => x.Object.objType).ToList();
                if (e.ColumnIndex == 2)
                    bindingSource2.DataSource = adLogs.OrderBy(x => x.eventType).ToList();
                if (e.ColumnIndex == 3)
                    bindingSource2.DataSource = adLogs.OrderBy(x => x.eventTime).ToList();
                if (e.ColumnIndex == 4)
                    bindingSource2.DataSource = adLogs.OrderBy(x => x.currentValue).ToList();
                if (e.ColumnIndex == 5)
                    bindingSource2.DataSource = adLogs.OrderBy(x => x.oldValue).ToList();
            }
            else
            {
                if (e.ColumnIndex == 0)
                    bindingSource2.DataSource = adLogs.OrderByDescending(x => x.Object.ToString()).ToList();
                if (e.ColumnIndex == 1)
                    bindingSource2.DataSource = adLogs.OrderByDescending(x => x.Object.objType).ToList();
                if (e.ColumnIndex == 2)
                    bindingSource2.DataSource = adLogs.OrderByDescending(x => x.eventType).ToList();
                if (e.ColumnIndex == 3)
                    bindingSource2.DataSource = adLogs.OrderByDescending(x => x.eventTime).ToList();
                if (e.ColumnIndex == 4)
                    bindingSource2.DataSource = adLogs.OrderByDescending(x => x.currentValue).ToList();
                if (e.ColumnIndex == 5)
                    bindingSource2.DataSource = adLogs.OrderByDescending(x => x.oldValue).ToList();
            }

            if (_sortColumn != null) _sortColumn.HeaderCell.SortGlyphDirection = System.Windows.Forms.SortOrder.None;
            column.HeaderCell.SortGlyphDirection = _isSortAscending ? System.Windows.Forms.SortOrder.Ascending : System.Windows.Forms.SortOrder.Descending;
            _sortColumn = column;
        }

        private void текущееСостояниеADToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fCurADStatus f = new fCurADStatus(s.SqlServerString);
            f.ShowDialog();
        }

        private void текущееСостояниеГруппыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fGroupInfoParams f = new fGroupInfoParams(s.SqlServerString);
            f.ShowDialog();
        }
    }
}
