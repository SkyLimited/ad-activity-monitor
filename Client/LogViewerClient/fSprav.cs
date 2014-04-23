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
    public partial class fSprav : Form
    {
        private SqlDataAdapter dataAdapter;
        private SqlCommandBuilder cb;
        private string Command;
        private Settings s;
        private bool changd;
        
        public fSprav(string Command,Settings s)
        {
            InitializeComponent();
            this.s = s;
            this.Command = Command;
            changd = false;
            getData();
        }

        private void getData()
       {
           string selectCommand = "SELECT * FROM ["+Command+"]" ;
            dataAdapter = new SqlDataAdapter(selectCommand, s.SqlServerString);
            cb = new SqlCommandBuilder(dataAdapter);

            DataTable table = new DataTable();
            table.Locale = System.Globalization.CultureInfo.InvariantCulture;
            dataAdapter.Fill(table);
            bindingSource1.DataSource = table;
            
            dataGridView1.DataSource = bindingSource1;
            dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader);
            if(dataGridView1.Columns.Contains("ByPass"))
                dataGridView1.Columns["ByPass"].HeaderText = "Пропускать";
            if (dataGridView1.Columns.Contains("ServerName"))
                dataGridView1.Columns["ServerName"].HeaderText = "Сервер";
            if (dataGridView1.Columns.Contains("ID_Server"))
                dataGridView1.Columns["ID_Server"].HeaderText = "ID";
            if (dataGridView1.Columns.Contains("Name"))
                dataGridView1.Columns["Name"].HeaderText = "Наименование";
            if (dataGridView1.Columns.Contains("ID_Machine"))
                dataGridView1.Columns["ID_Machine"].HeaderText = "ID";
            if (dataGridView1.Columns.Contains("DomainName"))
                dataGridView1.Columns["DomainName"].HeaderText = "Домен";
            if (dataGridView1.Columns.Contains("ID_Domain"))
                dataGridView1.Columns["ID_Domain"].HeaderText = "ID";
            if (dataGridView1.Columns.Contains("NTLogin"))
                dataGridView1.Columns["NTLogin"].HeaderText = "Учетная запись";
            if (dataGridView1.Columns.Contains("ID_User"))
                dataGridView1.Columns["ID_User"].HeaderText = "ID";   
       
       }


        private void toolStripButton1_Click_1(object sender, EventArgs e)
        {
            dataAdapter.Update((DataTable)bindingSource1.DataSource);
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            getData();
        }

        private void fSprav_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (changd)
            {
                DialogResult a = MessageBox.Show("Сохранить внесенные изменения?", "", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (a == DialogResult.Yes)
                {
                    dataAdapter.Update((DataTable)bindingSource1.DataSource);
                    e.Cancel = false;
                }
                if (a == DialogResult.No)
                {
                    e.Cancel = false;
                }
                if (a == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
            }

        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            changd = true;
        }
    }
}
