namespace TransitServer
{
    partial class FormChangeSenderMails
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
            this.labImei = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.labName = new System.Windows.Forms.Label();
            this.labelNameModem = new System.Windows.Forms.Label();
            this.labelImeiModem = new System.Windows.Forms.Label();
            this.txtSenderMail = new System.Windows.Forms.TextBox();
            this.txtNameSenderMail = new System.Windows.Forms.TextBox();
            this.txtRecieverMail = new System.Windows.Forms.TextBox();
            this.txtSubjectMail = new System.Windows.Forms.TextBox();
            this.txtSmtpClient = new System.Windows.Forms.TextBox();
            this.txtSmtpPort = new System.Windows.Forms.TextBox();
            this.txtSenderPassword = new System.Windows.Forms.TextBox();
            this.buttonSave = new System.Windows.Forms.Button();
            this.labGroup = new System.Windows.Forms.Label();
            this.txtGroup = new System.Windows.Forms.Label();
            this.labelInfoText = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labImei
            // 
            this.labImei.AutoSize = true;
            this.labImei.Location = new System.Drawing.Point(13, 43);
            this.labImei.Name = "labImei";
            this.labImei.Size = new System.Drawing.Size(32, 13);
            this.labImei.TabIndex = 0;
            this.labImei.Text = "IMEI:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 98);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Отправитель:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 124);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(99, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Имя отправителя:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 149);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(69, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Получатель:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 174);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(96, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Тема эл. письма:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(11, 198);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(71, 13);
            this.label6.TabIndex = 5;
            this.label6.Text = "smtp Клиент:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(11, 224);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(60, 13);
            this.label7.TabIndex = 6;
            this.label7.Text = "smtp Порт:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(11, 247);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(95, 13);
            this.label8.TabIndex = 7;
            this.label8.Text = "Пароль от почты:";
            // 
            // labName
            // 
            this.labName.AutoSize = true;
            this.labName.Location = new System.Drawing.Point(12, 20);
            this.labName.Name = "labName";
            this.labName.Size = new System.Drawing.Size(77, 13);
            this.labName.TabIndex = 8;
            this.labName.Text = "Имя объекта:";
            // 
            // labelNameModem
            // 
            this.labelNameModem.AutoSize = true;
            this.labelNameModem.Location = new System.Drawing.Point(129, 19);
            this.labelNameModem.Name = "labelNameModem";
            this.labelNameModem.Size = new System.Drawing.Size(74, 13);
            this.labelNameModem.TabIndex = 9;
            this.labelNameModem.Text = "Имя объекта";
            // 
            // labelImeiModem
            // 
            this.labelImeiModem.AutoSize = true;
            this.labelImeiModem.Location = new System.Drawing.Point(129, 43);
            this.labelImeiModem.Name = "labelImeiModem";
            this.labelImeiModem.Size = new System.Drawing.Size(25, 13);
            this.labelImeiModem.TabIndex = 10;
            this.labelImeiModem.Text = "imei";
            // 
            // txtSenderMail
            // 
            this.txtSenderMail.Location = new System.Drawing.Point(128, 94);
            this.txtSenderMail.Name = "txtSenderMail";
            this.txtSenderMail.Size = new System.Drawing.Size(169, 20);
            this.txtSenderMail.TabIndex = 11;
            this.txtSenderMail.TextChanged += new System.EventHandler(this.TxtSenderMail_TextChanged);
            // 
            // txtNameSenderMail
            // 
            this.txtNameSenderMail.Location = new System.Drawing.Point(128, 120);
            this.txtNameSenderMail.Name = "txtNameSenderMail";
            this.txtNameSenderMail.Size = new System.Drawing.Size(169, 20);
            this.txtNameSenderMail.TabIndex = 12;
            this.txtNameSenderMail.TextChanged += new System.EventHandler(this.TxtNameSenderMail_TextChanged);
            // 
            // txtRecieverMail
            // 
            this.txtRecieverMail.Location = new System.Drawing.Point(128, 146);
            this.txtRecieverMail.Name = "txtRecieverMail";
            this.txtRecieverMail.Size = new System.Drawing.Size(169, 20);
            this.txtRecieverMail.TabIndex = 13;
            this.txtRecieverMail.TextChanged += new System.EventHandler(this.TxtRecieverMail_TextChanged);
            // 
            // txtSubjectMail
            // 
            this.txtSubjectMail.Location = new System.Drawing.Point(128, 171);
            this.txtSubjectMail.Name = "txtSubjectMail";
            this.txtSubjectMail.Size = new System.Drawing.Size(169, 20);
            this.txtSubjectMail.TabIndex = 14;
            this.txtSubjectMail.TextChanged += new System.EventHandler(this.TxtSubjectMail_TextChanged);
            // 
            // txtSmtpClient
            // 
            this.txtSmtpClient.Location = new System.Drawing.Point(128, 195);
            this.txtSmtpClient.Name = "txtSmtpClient";
            this.txtSmtpClient.Size = new System.Drawing.Size(169, 20);
            this.txtSmtpClient.TabIndex = 15;
            this.txtSmtpClient.TextChanged += new System.EventHandler(this.TxtSmtpClient_TextChanged);
            // 
            // txtSmtpPort
            // 
            this.txtSmtpPort.Location = new System.Drawing.Point(128, 220);
            this.txtSmtpPort.Name = "txtSmtpPort";
            this.txtSmtpPort.Size = new System.Drawing.Size(169, 20);
            this.txtSmtpPort.TabIndex = 16;
            this.txtSmtpPort.TextChanged += new System.EventHandler(this.TxtSmtpPort_TextChanged);
            // 
            // txtSenderPassword
            // 
            this.txtSenderPassword.Location = new System.Drawing.Point(128, 244);
            this.txtSenderPassword.Name = "txtSenderPassword";
            this.txtSenderPassword.Size = new System.Drawing.Size(169, 20);
            this.txtSenderPassword.TabIndex = 17;
            this.txtSenderPassword.UseSystemPasswordChar = true;
            this.txtSenderPassword.TextChanged += new System.EventHandler(this.TxtSenderPassword_TextChanged);
            // 
            // buttonSave
            // 
            this.buttonSave.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.buttonSave.Location = new System.Drawing.Point(373, 237);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(75, 23);
            this.buttonSave.TabIndex = 18;
            this.buttonSave.Text = "Сохранить";
            this.buttonSave.UseVisualStyleBackColor = false;
            this.buttonSave.Click += new System.EventHandler(this.ButtonSave_Click);
            // 
            // labGroup
            // 
            this.labGroup.AutoSize = true;
            this.labGroup.Location = new System.Drawing.Point(12, 72);
            this.labGroup.Name = "labGroup";
            this.labGroup.Size = new System.Drawing.Size(45, 13);
            this.labGroup.TabIndex = 19;
            this.labGroup.Text = "Группа:";
            // 
            // txtGroup
            // 
            this.txtGroup.AutoSize = true;
            this.txtGroup.Location = new System.Drawing.Point(129, 72);
            this.txtGroup.Name = "txtGroup";
            this.txtGroup.Size = new System.Drawing.Size(34, 13);
            this.txtGroup.TabIndex = 21;
            this.txtGroup.Text = "group";
            // 
            // labelInfoText
            // 
            this.labelInfoText.AutoSize = true;
            this.labelInfoText.Location = new System.Drawing.Point(312, 20);
            this.labelInfoText.Name = "labelInfoText";
            this.labelInfoText.Size = new System.Drawing.Size(68, 13);
            this.labelInfoText.TabIndex = 22;
            this.labelInfoText.Text = "labelInfoText";
            this.labelInfoText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FormChangeSenderMails
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(473, 272);
            this.Controls.Add(this.labelInfoText);
            this.Controls.Add(this.txtGroup);
            this.Controls.Add(this.labGroup);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.txtSenderPassword);
            this.Controls.Add(this.txtSmtpPort);
            this.Controls.Add(this.txtSmtpClient);
            this.Controls.Add(this.txtSubjectMail);
            this.Controls.Add(this.txtRecieverMail);
            this.Controls.Add(this.txtNameSenderMail);
            this.Controls.Add(this.txtSenderMail);
            this.Controls.Add(this.labelImeiModem);
            this.Controls.Add(this.labelNameModem);
            this.Controls.Add(this.labName);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.labImei);
            this.Name = "FormChangeSenderMails";
            this.Text = "Настройка отправки событий через электронную почту";
            this.Load += new System.EventHandler(this.FormChangeSenderMails_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Label labImei;
        public System.Windows.Forms.Label label2;
        public System.Windows.Forms.Label label3;
        public System.Windows.Forms.Label label4;
        public System.Windows.Forms.Label label5;
        public System.Windows.Forms.Label label6;
        public System.Windows.Forms.Label label7;
        public System.Windows.Forms.Label label8;
        public System.Windows.Forms.Label labName;
        public System.Windows.Forms.Label labelNameModem;
        public System.Windows.Forms.Label labelImeiModem;
        public System.Windows.Forms.TextBox txtSenderMail;
        public System.Windows.Forms.TextBox txtNameSenderMail;
        public System.Windows.Forms.TextBox txtRecieverMail;
        public System.Windows.Forms.TextBox txtSubjectMail;
        public System.Windows.Forms.TextBox txtSmtpClient;
        public System.Windows.Forms.TextBox txtSmtpPort;
        public System.Windows.Forms.TextBox txtSenderPassword;
        public System.Windows.Forms.Button buttonSave;
        public System.Windows.Forms.Label labGroup;
        public System.Windows.Forms.Label txtGroup;
        public System.Windows.Forms.Label labelInfoText;
    }
}