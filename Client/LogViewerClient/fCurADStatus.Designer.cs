namespace LogViewerClient
{
    partial class fCurADStatus
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(fCurADStatus));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbShowDisabled = new System.Windows.Forms.CheckBox();
            this.cbShowEnabled = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbFilter = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cbShowARM = new System.Windows.Forms.CheckBox();
            this.cbShowUser = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnCancel);
            this.groupBox1.Controls.Add(this.btnOK);
            this.groupBox1.Controls.Add(this.cbShowDisabled);
            this.groupBox1.Controls.Add(this.cbShowEnabled);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.tbFilter);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.cbShowARM);
            this.groupBox1.Controls.Add(this.cbShowUser);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(582, 173);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Параметры";
            // 
            // cbShowDisabled
            // 
            this.cbShowDisabled.AutoSize = true;
            this.cbShowDisabled.Checked = true;
            this.cbShowDisabled.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbShowDisabled.Location = new System.Drawing.Point(236, 89);
            this.cbShowDisabled.Name = "cbShowDisabled";
            this.cbShowDisabled.Size = new System.Drawing.Size(95, 21);
            this.cbShowDisabled.TabIndex = 7;
            this.cbShowDisabled.Text = "Выключен";
            this.cbShowDisabled.UseVisualStyleBackColor = true;
            // 
            // cbShowEnabled
            // 
            this.cbShowEnabled.AutoSize = true;
            this.cbShowEnabled.Checked = true;
            this.cbShowEnabled.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbShowEnabled.Location = new System.Drawing.Point(110, 88);
            this.cbShowEnabled.Name = "cbShowEnabled";
            this.cbShowEnabled.Size = new System.Drawing.Size(85, 21);
            this.cbShowEnabled.TabIndex = 6;
            this.cbShowEnabled.Text = "Включен";
            this.cbShowEnabled.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(23, 89);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 17);
            this.label3.TabIndex = 5;
            this.label3.Text = "Статус:";
            // 
            // tbFilter
            // 
            this.tbFilter.Location = new System.Drawing.Point(110, 53);
            this.tbFilter.Name = "tbFilter";
            this.tbFilter.Size = new System.Drawing.Size(322, 23);
            this.tbFilter.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 17);
            this.label2.TabIndex = 3;
            this.label2.Text = "Фильтр:";
            // 
            // cbShowARM
            // 
            this.cbShowARM.AutoSize = true;
            this.cbShowARM.Checked = true;
            this.cbShowARM.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbShowARM.Location = new System.Drawing.Point(236, 30);
            this.cbShowARM.Name = "cbShowARM";
            this.cbShowARM.Size = new System.Drawing.Size(56, 21);
            this.cbShowARM.TabIndex = 2;
            this.cbShowARM.Text = "АРМ";
            this.cbShowARM.UseVisualStyleBackColor = true;
            // 
            // cbShowUser
            // 
            this.cbShowUser.AutoSize = true;
            this.cbShowUser.Checked = true;
            this.cbShowUser.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbShowUser.Location = new System.Drawing.Point(110, 30);
            this.cbShowUser.Name = "cbShowUser";
            this.cbShowUser.Size = new System.Drawing.Size(120, 21);
            this.cbShowUser.TabIndex = 1;
            this.cbShowUser.Text = "Пользователь";
            this.cbShowUser.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(43, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Тип:";
            // 
            // btnCancel
            // 
            this.btnCancel.Image = global::LogViewerClient.Properties.Resources.dialog_cancel_4;
            this.btnCancel.Location = new System.Drawing.Point(470, 132);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 35);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "Отмена";
            this.btnCancel.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Image = global::LogViewerClient.Properties.Resources.unsortedlist;
            this.btnOK.Location = new System.Drawing.Point(20, 132);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(81, 35);
            this.btnOK.TabIndex = 8;
            this.btnOK.Text = "ОК";
            this.btnOK.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // fCurADStatus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(582, 173);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "fCurADStatus";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Параметры отчета \"Текущее состояние AD\"";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.CheckBox cbShowDisabled;
        private System.Windows.Forms.CheckBox cbShowEnabled;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbFilter;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox cbShowARM;
        private System.Windows.Forms.CheckBox cbShowUser;
        private System.Windows.Forms.Label label1;
    }
}