namespace TransitServer
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.updateDgvModems = new System.Windows.Forms.Button();
            this.btn_AddModem = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiRedactorModema = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAddGroup = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMailSendCustom = new System.Windows.Forms.ToolStripMenuItem();
            this.panel5 = new System.Windows.Forms.Panel();
            this.tcDown = new System.Windows.Forms.TabControl();
            this.tpEvent = new System.Windows.Forms.TabPage();
            this.panel9 = new System.Windows.Forms.Panel();
            this.dgvEvent = new System.Windows.Forms.DataGridView();
            this.colEventNameModem = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEventDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEventMessage = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEventQuite = new System.Windows.Forms.DataGridViewButtonColumn();
            this.colEventImeiModem = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel8 = new System.Windows.Forms.Panel();
            this.btnGetAllEvents = new System.Windows.Forms.Button();
            this.btnSongMute = new System.Windows.Forms.Button();
            this.tpConsole = new System.Windows.Forms.TabPage();
            this.panel7 = new System.Windows.Forms.Panel();
            this.lbConsole = new System.Windows.Forms.ListBox();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.panel6 = new System.Windows.Forms.Panel();
            this.btnClear = new System.Windows.Forms.Button();
            this.statusString = new System.Windows.Forms.StatusStrip();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.panel11 = new System.Windows.Forms.Panel();
            this.trView1 = new System.Windows.Forms.TreeView();
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.добавитьГруппуToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDelNode = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCustomGroupSendMail = new System.Windows.Forms.ToolStripMenuItem();
            this.label22 = new System.Windows.Forms.Label();
            this.txtFinder = new System.Windows.Forms.TextBox();
            this.label23 = new System.Windows.Forms.Label();
            this.dgvModems = new System.Windows.Forms.DataGridView();
            this.id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.imei = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.port = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lastConnection = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.activeConnection = new System.Windows.Forms.DataGridViewImageColumn();
            this.btChangePort = new System.Windows.Forms.Button();
            this.dataGridViewImageColumn1 = new System.Windows.Forms.DataGridViewImageColumn();
            this.contextMenuStrip1.SuspendLayout();
            this.panel5.SuspendLayout();
            this.tcDown.SuspendLayout();
            this.tpEvent.SuspendLayout();
            this.panel9.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEvent)).BeginInit();
            this.panel8.SuspendLayout();
            this.tpConsole.SuspendLayout();
            this.panel7.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel11.SuspendLayout();
            this.contextMenuStrip2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvModems)).BeginInit();
            this.SuspendLayout();
            // 
            // updateDgvModems
            // 
            this.updateDgvModems.Location = new System.Drawing.Point(482, 428);
            this.updateDgvModems.Name = "updateDgvModems";
            this.updateDgvModems.Size = new System.Drawing.Size(97, 23);
            this.updateDgvModems.TabIndex = 2;
            this.updateDgvModems.Text = "Обновить";
            this.updateDgvModems.UseVisualStyleBackColor = true;
            this.updateDgvModems.Click += new System.EventHandler(this.UpdateDgvModems_Click);
            // 
            // btn_AddModem
            // 
            this.btn_AddModem.Location = new System.Drawing.Point(350, 428);
            this.btn_AddModem.Name = "btn_AddModem";
            this.btn_AddModem.Size = new System.Drawing.Size(120, 23);
            this.btn_AddModem.TabIndex = 1;
            this.btn_AddModem.Text = "Добавить модем";
            this.btn_AddModem.UseVisualStyleBackColor = true;
            this.btn_AddModem.Click += new System.EventHandler(this.Btn_AddModem_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiRedactorModema,
            this.tsmiAddGroup,
            this.tsmiMailSendCustom});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(239, 70);
            // 
            // tsmiRedactorModema
            // 
            this.tsmiRedactorModema.Name = "tsmiRedactorModema";
            this.tsmiRedactorModema.Size = new System.Drawing.Size(238, 22);
            this.tsmiRedactorModema.Text = "Свойства модема";
            this.tsmiRedactorModema.Click += new System.EventHandler(this.TsmiRedactorModema_Click);
            // 
            // tsmiAddGroup
            // 
            this.tsmiAddGroup.Name = "tsmiAddGroup";
            this.tsmiAddGroup.Size = new System.Drawing.Size(238, 22);
            this.tsmiAddGroup.Text = "Добавить в группу";
            this.tsmiAddGroup.Click += new System.EventHandler(this.TsmiAddGroup_Click);
            // 
            // tsmiMailSendCustom
            // 
            this.tsmiMailSendCustom.Name = "tsmiMailSendCustom";
            this.tsmiMailSendCustom.Size = new System.Drawing.Size(238, 22);
            this.tsmiMailSendCustom.Text = "Настройка отправки событий";
            this.tsmiMailSendCustom.Click += new System.EventHandler(this.TsmiMailSendCustom_Click);
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.tcDown);
            this.panel5.Location = new System.Drawing.Point(6, 504);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(993, 199);
            this.panel5.TabIndex = 2;
            // 
            // tcDown
            // 
            this.tcDown.Controls.Add(this.tpEvent);
            this.tcDown.Controls.Add(this.tpConsole);
            this.tcDown.Location = new System.Drawing.Point(7, 4);
            this.tcDown.Name = "tcDown";
            this.tcDown.SelectedIndex = 0;
            this.tcDown.Size = new System.Drawing.Size(986, 188);
            this.tcDown.TabIndex = 0;
            // 
            // tpEvent
            // 
            this.tpEvent.Controls.Add(this.panel9);
            this.tpEvent.Controls.Add(this.panel8);
            this.tpEvent.Location = new System.Drawing.Point(4, 22);
            this.tpEvent.Name = "tpEvent";
            this.tpEvent.Size = new System.Drawing.Size(978, 162);
            this.tpEvent.TabIndex = 2;
            this.tpEvent.Text = "События";
            this.tpEvent.UseVisualStyleBackColor = true;
            // 
            // panel9
            // 
            this.panel9.Controls.Add(this.dgvEvent);
            this.panel9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel9.Location = new System.Drawing.Point(0, 0);
            this.panel9.Name = "panel9";
            this.panel9.Size = new System.Drawing.Size(941, 162);
            this.panel9.TabIndex = 2;
            // 
            // dgvEvent
            // 
            this.dgvEvent.AllowUserToAddRows = false;
            this.dgvEvent.BackgroundColor = System.Drawing.SystemColors.ActiveCaption;
            this.dgvEvent.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvEvent.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colEventNameModem,
            this.colEventDate,
            this.colEventMessage,
            this.colEventQuite,
            this.colEventImeiModem});
            this.dgvEvent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvEvent.Location = new System.Drawing.Point(0, 0);
            this.dgvEvent.Name = "dgvEvent";
            this.dgvEvent.RowHeadersWidth = 20;
            this.dgvEvent.Size = new System.Drawing.Size(941, 162);
            this.dgvEvent.TabIndex = 0;
            this.dgvEvent.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgvEvent1_CellContentClick);
            // 
            // colEventNameModem
            // 
            this.colEventNameModem.Frozen = true;
            this.colEventNameModem.HeaderText = "name";
            this.colEventNameModem.Name = "colEventNameModem";
            this.colEventNameModem.Width = 150;
            // 
            // colEventDate
            // 
            this.colEventDate.FillWeight = 200F;
            this.colEventDate.Frozen = true;
            this.colEventDate.HeaderText = "date";
            this.colEventDate.Name = "colEventDate";
            this.colEventDate.ReadOnly = true;
            this.colEventDate.Width = 180;
            // 
            // colEventMessage
            // 
            this.colEventMessage.FillWeight = 300F;
            this.colEventMessage.Frozen = true;
            this.colEventMessage.HeaderText = "message";
            this.colEventMessage.Name = "colEventMessage";
            this.colEventMessage.ReadOnly = true;
            this.colEventMessage.Width = 300;
            // 
            // colEventQuite
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.NullValue = "квитировать";
            this.colEventQuite.DefaultCellStyle = dataGridViewCellStyle1;
            this.colEventQuite.FillWeight = 80F;
            this.colEventQuite.Frozen = true;
            this.colEventQuite.HeaderText = "quite";
            this.colEventQuite.Name = "colEventQuite";
            this.colEventQuite.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colEventQuite.Width = 120;
            // 
            // colEventImeiModem
            // 
            this.colEventImeiModem.Frozen = true;
            this.colEventImeiModem.HeaderText = "imei";
            this.colEventImeiModem.Name = "colEventImeiModem";
            this.colEventImeiModem.Width = 170;
            // 
            // panel8
            // 
            this.panel8.Controls.Add(this.btnGetAllEvents);
            this.panel8.Controls.Add(this.btnSongMute);
            this.panel8.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel8.Location = new System.Drawing.Point(941, 0);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(37, 162);
            this.panel8.TabIndex = 1;
            // 
            // btnGetAllEvents
            // 
            this.btnGetAllEvents.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnGetAllEvents.Location = new System.Drawing.Point(0, 27);
            this.btnGetAllEvents.Name = "btnGetAllEvents";
            this.btnGetAllEvents.Size = new System.Drawing.Size(37, 27);
            this.btnGetAllEvents.TabIndex = 2;
            this.btnGetAllEvents.Text = "all";
            this.btnGetAllEvents.UseVisualStyleBackColor = true;
            this.btnGetAllEvents.Click += new System.EventHandler(this.BtnGetAllEvents_Click);
            // 
            // btnSongMute
            // 
            this.btnSongMute.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnSongMute.Image = global::TransitServer.Properties.Resources.sound1;
            this.btnSongMute.Location = new System.Drawing.Point(0, 0);
            this.btnSongMute.Name = "btnSongMute";
            this.btnSongMute.Size = new System.Drawing.Size(37, 27);
            this.btnSongMute.TabIndex = 1;
            this.btnSongMute.UseVisualStyleBackColor = true;
            this.btnSongMute.Click += new System.EventHandler(this.BtnSongMute_Click);
            // 
            // tpConsole
            // 
            this.tpConsole.Controls.Add(this.panel7);
            this.tpConsole.Controls.Add(this.splitter1);
            this.tpConsole.Controls.Add(this.panel6);
            this.tpConsole.Location = new System.Drawing.Point(4, 22);
            this.tpConsole.Name = "tpConsole";
            this.tpConsole.Size = new System.Drawing.Size(978, 162);
            this.tpConsole.TabIndex = 0;
            this.tpConsole.Text = "Консоль";
            this.tpConsole.UseVisualStyleBackColor = true;
            // 
            // panel7
            // 
            this.panel7.Controls.Add(this.lbConsole);
            this.panel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel7.Location = new System.Drawing.Point(0, 0);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(950, 162);
            this.panel7.TabIndex = 3;
            // 
            // lbConsole
            // 
            this.lbConsole.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbConsole.FormattingEnabled = true;
            this.lbConsole.Location = new System.Drawing.Point(0, 0);
            this.lbConsole.Name = "lbConsole";
            this.lbConsole.Size = new System.Drawing.Size(950, 162);
            this.lbConsole.TabIndex = 0;
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitter1.Location = new System.Drawing.Point(950, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 162);
            this.splitter1.TabIndex = 2;
            this.splitter1.TabStop = false;
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.btnClear);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel6.Location = new System.Drawing.Point(953, 0);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(25, 162);
            this.panel6.TabIndex = 1;
            // 
            // btnClear
            // 
            this.btnClear.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnClear.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnClear.Image = global::TransitServer.Properties.Resources.delete;
            this.btnClear.Location = new System.Drawing.Point(0, 0);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(25, 23);
            this.btnClear.TabIndex = 15;
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.BtnClear_Click);
            // 
            // statusString
            // 
            this.statusString.Location = new System.Drawing.Point(0, 706);
            this.statusString.Name = "statusString";
            this.statusString.Size = new System.Drawing.Size(1022, 22);
            this.statusString.TabIndex = 3;
            this.statusString.Text = "statusStrip1";
            this.statusString.MouseMove += new System.Windows.Forms.MouseEventHandler(this.StatusString_MouseMove);
            // 
            // panel11
            // 
            this.panel11.BackColor = System.Drawing.Color.White;
            this.panel11.Controls.Add(this.trView1);
            this.panel11.Controls.Add(this.label22);
            this.panel11.Location = new System.Drawing.Point(6, 12);
            this.panel11.Name = "panel11";
            this.panel11.Size = new System.Drawing.Size(330, 460);
            this.panel11.TabIndex = 4;
            // 
            // trView1
            // 
            this.trView1.ContextMenuStrip = this.contextMenuStrip2;
            this.trView1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.trView1.Location = new System.Drawing.Point(6, 36);
            this.trView1.Name = "trView1";
            this.trView1.Size = new System.Drawing.Size(321, 421);
            this.trView1.TabIndex = 1;
            this.trView1.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.TrView1_NodeMouseClick);
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.добавитьГруппуToolStripMenuItem,
            this.tsmiDelNode,
            this.tsmiCustomGroupSendMail});
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            this.contextMenuStrip2.Size = new System.Drawing.Size(236, 70);
            // 
            // добавитьГруппуToolStripMenuItem
            // 
            this.добавитьГруппуToolStripMenuItem.Name = "добавитьГруппуToolStripMenuItem";
            this.добавитьГруппуToolStripMenuItem.Size = new System.Drawing.Size(235, 22);
            this.добавитьГруппуToolStripMenuItem.Text = "Добавить группу";
            this.добавитьГруппуToolStripMenuItem.Click += new System.EventHandler(this.ToolStripMenuItem_Click);
            // 
            // tsmiDelNode
            // 
            this.tsmiDelNode.Name = "tsmiDelNode";
            this.tsmiDelNode.Size = new System.Drawing.Size(235, 22);
            this.tsmiDelNode.Text = "Удалить группу";
            this.tsmiDelNode.Click += new System.EventHandler(this.DelToolStripMenuItem_Click);
            // 
            // tsmiCustomGroupSendMail
            // 
            this.tsmiCustomGroupSendMail.Name = "tsmiCustomGroupSendMail";
            this.tsmiCustomGroupSendMail.Size = new System.Drawing.Size(235, 22);
            this.tsmiCustomGroupSendMail.Text = "Настроить отправку события";
            this.tsmiCustomGroupSendMail.Click += new System.EventHandler(this.TsmiCustomGroupSendMail_Click);
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label22.Location = new System.Drawing.Point(7, 9);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(158, 24);
            this.label22.TabIndex = 0;
            this.label22.Text = "Древо объектов";
            // 
            // txtFinder
            // 
            this.txtFinder.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txtFinder.Location = new System.Drawing.Point(406, 12);
            this.txtFinder.Name = "txtFinder";
            this.txtFinder.Size = new System.Drawing.Size(173, 26);
            this.txtFinder.TabIndex = 5;
            this.txtFinder.TextChanged += new System.EventHandler(this.TxtFinder_TextChanged);
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label23.Location = new System.Drawing.Point(346, 15);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(54, 19);
            this.label23.TabIndex = 6;
            this.label23.Text = "Поиск:";
            // 
            // dgvModems
            // 
            this.dgvModems.AllowUserToAddRows = false;
            this.dgvModems.BackgroundColor = System.Drawing.SystemColors.ActiveCaption;
            this.dgvModems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvModems.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.id,
            this.imei,
            this.port,
            this.name,
            this.lastConnection,
            this.activeConnection});
            this.dgvModems.ContextMenuStrip = this.contextMenuStrip1;
            this.dgvModems.GridColor = System.Drawing.SystemColors.Control;
            this.dgvModems.Location = new System.Drawing.Point(350, 62);
            this.dgvModems.Name = "dgvModems";
            this.dgvModems.Size = new System.Drawing.Size(649, 360);
            this.dgvModems.TabIndex = 7;
            this.dgvModems.CellMouseEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvModems_CellMouseEnter);
            // 
            // id
            // 
            this.id.HeaderText = "Номер";
            this.id.Name = "id";
            this.id.Visible = false;
            this.id.Width = 50;
            // 
            // imei
            // 
            this.imei.HeaderText = "Imei";
            this.imei.Name = "imei";
            this.imei.Width = 200;
            // 
            // port
            // 
            this.port.HeaderText = "Порт";
            this.port.Name = "port";
            // 
            // name
            // 
            this.name.HeaderText = "Имя объекта";
            this.name.Name = "name";
            this.name.Width = 150;
            // 
            // lastConnection
            // 
            this.lastConnection.HeaderText = "Последнее подключение";
            this.lastConnection.Name = "lastConnection";
            this.lastConnection.Width = 120;
            // 
            // activeConnection
            // 
            this.activeConnection.HeaderText = "";
            this.activeConnection.Name = "activeConnection";
            this.activeConnection.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.activeConnection.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.activeConnection.ToolTipText = "Статус подключения модема";
            this.activeConnection.Width = 35;
            // 
            // btChangePort
            // 
            this.btChangePort.BackColor = System.Drawing.Color.Aquamarine;
            this.btChangePort.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btChangePort.BackgroundImage")));
            this.btChangePort.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btChangePort.Location = new System.Drawing.Point(953, 12);
            this.btChangePort.Name = "btChangePort";
            this.btChangePort.Size = new System.Drawing.Size(46, 42);
            this.btChangePort.TabIndex = 8;
            this.btChangePort.UseVisualStyleBackColor = false;
            this.btChangePort.Click += new System.EventHandler(this.BtChangePort_Click);
            // 
            // dataGridViewImageColumn1
            // 
            this.dataGridViewImageColumn1.HeaderText = " ";
            this.dataGridViewImageColumn1.Image = ((System.Drawing.Image)(resources.GetObject("dataGridViewImageColumn1.Image")));
            this.dataGridViewImageColumn1.Name = "dataGridViewImageColumn1";
            this.dataGridViewImageColumn1.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewImageColumn1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.dataGridViewImageColumn1.Width = 20;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Azure;
            this.ClientSize = new System.Drawing.Size(1022, 728);
            this.Controls.Add(this.btChangePort);
            this.Controls.Add(this.dgvModems);
            this.Controls.Add(this.btn_AddModem);
            this.Controls.Add(this.updateDgvModems);
            this.Controls.Add(this.label23);
            this.Controls.Add(this.txtFinder);
            this.Controls.Add(this.panel11);
            this.Controls.Add(this.statusString);
            this.Controls.Add(this.panel5);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Server";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.tcDown.ResumeLayout(false);
            this.tpEvent.ResumeLayout(false);
            this.panel9.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvEvent)).EndInit();
            this.panel8.ResumeLayout(false);
            this.tpConsole.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.panel11.ResumeLayout(false);
            this.panel11.PerformLayout();
            this.contextMenuStrip2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvModems)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.TabControl tcDown;
        private System.Windows.Forms.TabPage tpConsole;
        private System.Windows.Forms.StatusStrip statusString;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.TabPage tpEvent;
        private System.Windows.Forms.DataGridView dgvEvent;
        private System.Windows.Forms.ListBox lbConsole;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Panel panel9;
        private System.Windows.Forms.Panel panel8;
        private System.Windows.Forms.Button btnSongMute;
        private System.Windows.Forms.Button btnGetAllEvents;
        private System.Windows.Forms.Button btn_AddModem;
        private System.Windows.Forms.Panel panel11;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Button updateDgvModems;
        private System.Windows.Forms.TextBox txtFinder;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tsmiRedactorModema;
        private System.Windows.Forms.DataGridViewImageColumn dataGridViewImageColumn1;
        public System.Windows.Forms.DataGridView dgvModems;
        private System.Windows.Forms.TreeView trView1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
        private System.Windows.Forms.ToolStripMenuItem добавитьГруппуToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsmiDelNode;
        private System.Windows.Forms.ToolStripMenuItem tsmiAddGroup;
        private System.Windows.Forms.Button btChangePort;
        private System.Windows.Forms.DataGridViewTextBoxColumn id;
        private System.Windows.Forms.DataGridViewTextBoxColumn imei;
        private System.Windows.Forms.DataGridViewTextBoxColumn port;
        private System.Windows.Forms.DataGridViewTextBoxColumn name;
        private System.Windows.Forms.DataGridViewTextBoxColumn lastConnection;
        private System.Windows.Forms.DataGridViewImageColumn activeConnection;
        private System.Windows.Forms.ToolStripMenuItem tsmiMailSendCustom;
        private System.Windows.Forms.ToolStripMenuItem tsmiCustomGroupSendMail;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEventNameModem;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEventDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEventMessage;
        private System.Windows.Forms.DataGridViewButtonColumn colEventQuite;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEventImeiModem;
    }
}

