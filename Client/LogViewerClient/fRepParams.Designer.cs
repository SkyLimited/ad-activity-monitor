namespace LogViewerClient
{
    partial class fRepParams
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cbServersList = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cbDateFilter = new System.Windows.Forms.CheckBox();
            this.dtFrom = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.dtTo = new System.Windows.Forms.DateTimePicker();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnMake = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cbServersList
            // 
            this.cbServersList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbServersList.FormattingEnabled = true;
            this.cbServersList.Location = new System.Drawing.Point(98, 12);
            this.cbServersList.Name = "cbServersList";
            this.cbServersList.Size = new System.Drawing.Size(300, 21);
            this.cbServersList.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(45, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Сервер:";
            // 
            // cbDateFilter
            // 
            this.cbDateFilter.AutoSize = true;
            this.cbDateFilter.Location = new System.Drawing.Point(98, 39);
            this.cbDateFilter.Name = "cbDateFilter";
            this.cbDateFilter.Size = new System.Drawing.Size(107, 17);
            this.cbDateFilter.TabIndex = 2;
            this.cbDateFilter.Text = "Фильтр по дате";
            this.cbDateFilter.UseVisualStyleBackColor = true;
            this.cbDateFilter.CheckedChanged += new System.EventHandler(this.cbDateFilter_CheckedChanged);
            // 
            // dtFrom
            // 
            this.dtFrom.Location = new System.Drawing.Point(98, 62);
            this.dtFrom.Name = "dtFrom";
            this.dtFrom.Size = new System.Drawing.Size(139, 20);
            this.dtFrom.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(243, 67);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(10, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "-";
            // 
            // dtTo
            // 
            this.dtTo.Location = new System.Drawing.Point(259, 62);
            this.dtTo.Name = "dtTo";
            this.dtTo.Size = new System.Drawing.Size(139, 20);
            this.dtTo.TabIndex = 5;
            // 
            // btnClose
            // 
            this.btnClose.Image = global::LogViewerClient.Properties.Resources.dialog_cancel_4;
            this.btnClose.Location = new System.Drawing.Point(301, 110);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(97, 29);
            this.btnClose.TabIndex = 7;
            this.btnClose.Text = "Отмена";
            this.btnClose.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnMake
            // 
            this.btnMake.Image = global::LogViewerClient.Properties.Resources.unsortedlist;
            this.btnMake.Location = new System.Drawing.Point(24, 110);
            this.btnMake.Name = "btnMake";
            this.btnMake.Size = new System.Drawing.Size(90, 29);
            this.btnMake.TabIndex = 6;
            this.btnMake.Text = "ОК";
            this.btnMake.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnMake.UseVisualStyleBackColor = true;
            this.btnMake.Click += new System.EventHandler(this.btnMake_Click);
            // 
            // fRepParams
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(406, 141);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnMake);
            this.Controls.Add(this.dtTo);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dtFrom);
            this.Controls.Add(this.cbDateFilter);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbServersList);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "fRepParams";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Параметры отчета";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbServersList;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox cbDateFilter;
        private System.Windows.Forms.DateTimePicker dtFrom;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dtTo;
        private System.Windows.Forms.Button btnMake;
        private System.Windows.Forms.Button btnClose;
    }
}