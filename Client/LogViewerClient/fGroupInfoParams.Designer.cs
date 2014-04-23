namespace LogViewerClient
{
    partial class fGroupInfoParams
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(fGroupInfoParams));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.clbGroups = new System.Windows.Forms.CheckedListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cbShowARM = new System.Windows.Forms.CheckBox();
            this.cbShowUser = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cbStatusEnabled = new System.Windows.Forms.CheckBox();
            this.cbStatusDisabled = new System.Windows.Forms.CheckBox();
            this.btnCheckAll = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnCheckAll);
            this.groupBox1.Controls.Add(this.btnCancel);
            this.groupBox1.Controls.Add(this.btnOK);
            this.groupBox1.Controls.Add(this.cbStatusDisabled);
            this.groupBox1.Controls.Add(this.cbStatusEnabled);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.cbShowUser);
            this.groupBox1.Controls.Add(this.cbShowARM);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.clbGroups);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(662, 294);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Параметры";
            // 
            // clbGroups
            // 
            this.clbGroups.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.clbGroups.FormattingEnabled = true;
            this.clbGroups.Location = new System.Drawing.Point(100, 39);
            this.clbGroups.Name = "clbGroups";
            this.clbGroups.Size = new System.Drawing.Size(553, 137);
            this.clbGroups.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(31, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 18);
            this.label1.TabIndex = 1;
            this.label1.Text = "Группы:";
            // 
            // cbShowARM
            // 
            this.cbShowARM.AutoSize = true;
            this.cbShowARM.Checked = true;
            this.cbShowARM.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbShowARM.Location = new System.Drawing.Point(100, 182);
            this.cbShowARM.Name = "cbShowARM";
            this.cbShowARM.Size = new System.Drawing.Size(59, 22);
            this.cbShowARM.TabIndex = 2;
            this.cbShowARM.Text = "АРМ";
            this.cbShowARM.UseVisualStyleBackColor = true;
            // 
            // cbShowUser
            // 
            this.cbShowUser.AutoSize = true;
            this.cbShowUser.Checked = true;
            this.cbShowUser.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbShowUser.Location = new System.Drawing.Point(195, 181);
            this.cbShowUser.Name = "cbShowUser";
            this.cbShowUser.Size = new System.Drawing.Size(129, 22);
            this.cbShowUser.TabIndex = 3;
            this.cbShowUser.Text = "Пользователь";
            this.cbShowUser.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(57, 182);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 18);
            this.label2.TabIndex = 4;
            this.label2.Text = "Тип:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(34, 212);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 18);
            this.label3.TabIndex = 5;
            this.label3.Text = "Статус:";
            // 
            // cbStatusEnabled
            // 
            this.cbStatusEnabled.AutoSize = true;
            this.cbStatusEnabled.Checked = true;
            this.cbStatusEnabled.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbStatusEnabled.Location = new System.Drawing.Point(100, 212);
            this.cbStatusEnabled.Name = "cbStatusEnabled";
            this.cbStatusEnabled.Size = new System.Drawing.Size(89, 22);
            this.cbStatusEnabled.TabIndex = 6;
            this.cbStatusEnabled.Text = "Включен";
            this.cbStatusEnabled.UseVisualStyleBackColor = true;
            // 
            // cbStatusDisabled
            // 
            this.cbStatusDisabled.AutoSize = true;
            this.cbStatusDisabled.Checked = true;
            this.cbStatusDisabled.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbStatusDisabled.Location = new System.Drawing.Point(195, 212);
            this.cbStatusDisabled.Name = "cbStatusDisabled";
            this.cbStatusDisabled.Size = new System.Drawing.Size(100, 22);
            this.cbStatusDisabled.TabIndex = 7;
            this.cbStatusDisabled.Text = "Выключен";
            this.cbStatusDisabled.UseVisualStyleBackColor = true;
            // 
            // btnCheckAll
            // 
            this.btnCheckAll.Image = global::LogViewerClient.Properties.Resources.checkbox_2;
            this.btnCheckAll.Location = new System.Drawing.Point(100, 12);
            this.btnCheckAll.Name = "btnCheckAll";
            this.btnCheckAll.Size = new System.Drawing.Size(27, 23);
            this.btnCheckAll.TabIndex = 12;
            this.btnCheckAll.UseVisualStyleBackColor = true;
            this.btnCheckAll.Click += new System.EventHandler(this.btnCheckAll_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Image = global::LogViewerClient.Properties.Resources.dialog_cancel_4;
            this.btnCancel.Location = new System.Drawing.Point(550, 254);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 35);
            this.btnCancel.TabIndex = 11;
            this.btnCancel.Text = "Отмена";
            this.btnCancel.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Image = global::LogViewerClient.Properties.Resources.unsortedlist;
            this.btnOK.Location = new System.Drawing.Point(13, 254);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(81, 35);
            this.btnOK.TabIndex = 10;
            this.btnOK.Text = "ОК";
            this.btnOK.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // fGroupInfoParams
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(662, 294);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "fGroupInfoParams";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Параметры отчета \"Текущее состояние Группы AD\"";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckedListBox clbGroups;
        private System.Windows.Forms.CheckBox cbStatusDisabled;
        private System.Windows.Forms.CheckBox cbStatusEnabled;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox cbShowUser;
        private System.Windows.Forms.CheckBox cbShowARM;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCheckAll;
    }
}