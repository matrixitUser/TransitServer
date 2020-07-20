using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TransitServer
{
    public partial class FormRedactorModems : Form
    {
        public string idModem;
        public tsConfig config;
        public FormRedactorModems()
        {
            InitializeComponent();
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            ViewModems(SQLite.Instance.GetModems());
            Close();
        }

        private void BtnDeleteModem_Click(object sender, EventArgs e)
        {
            DialogFormOk dialogFormOk = new DialogFormOk();
            if(dialogFormOk.ShowDialog() == DialogResult.OK)
            {
                SQLite.Instance.DeleteModems(idModem);
                ViewModems(SQLite.Instance.GetModems());
                Close();
                MessageBox.Show("Объект удален!");
            }
        }

        const string MODEMSCOLID = "id";
        const string MODEMSCOLIMEI = "imei";
        const string MODEMSCOLPORT = "port";
        const string MODEMSCOLNAME = "name";
        const string MODEMSCOLLASTCONNECTION = "lastConnection";
        const string MODEMSCOLLACTIVECONNECTION = "activeConnection";
        public string oldNameModem;
        public void ViewModems(List<dynamic> records)
        {
            Form1 form1 = this.Owner as Form1;
            form1.dgvModems.Rows.Clear();
            if (!records.Any())
            {
                return;
            }
            foreach (var rec in records)
            {
                form1.dgvModems.Rows.Add();
                form1.dgvModems.Rows[form1.dgvModems.RowCount - 1].Cells[MODEMSCOLID].Value = rec.id;
                form1.dgvModems.Rows[form1.dgvModems.RowCount - 1].Cells[MODEMSCOLIMEI].Value = rec.imei;
                form1.dgvModems.Rows[form1.dgvModems.RowCount - 1].Cells[MODEMSCOLPORT].Value = rec.port;
                form1.dgvModems.Rows[form1.dgvModems.RowCount - 1].Cells[MODEMSCOLNAME].Value = rec.name;
                form1.dgvModems.Rows[form1.dgvModems.RowCount - 1].Cells[MODEMSCOLLASTCONNECTION].Value = rec.lastConnection;
                if ((int)rec.activeConnection == 1)
                    form1.dgvModems.Rows[form1.dgvModems.RowCount - 1].Cells[MODEMSCOLLACTIVECONNECTION].Value = Properties.Resources.tick;
                else form1.dgvModems.Rows[form1.dgvModems.RowCount - 1].Cells[MODEMSCOLLACTIVECONNECTION].Value = Properties.Resources.cross;
            }
        }

        private void BtnSaveChanges_Click(object sender, EventArgs e)
        {
            SQLite.Instance.UpdateNameModemsbyImei(txtImei.Text, txtNameModem.Text);
            MessageBox.Show("Изменения сохранены!");
            //MessageBox.Show("Кнопка в разработке!");
        }

        private void FormRedactorModems_Load(object sender, EventArgs e)
        {
            txtNetworkAdres.Text = config.u8NetworkAddress.ToString();
            txtMode.Text = config.u8Mode.ToString();
            txtRelease.Text = config.u32ReleaseTs.ToString();
            txtFlashVer.Text = config.u16FlashVer.ToString();
            cbBaudRate1.SelectedItem = (UInt32)config.sUart1.u32BaudRate;
            cbBaudRate2.SelectedItem = (UInt32)config.sUart2.u32BaudRate;
            cbBaudRate3.SelectedItem = (UInt32)config.sUart3.u32BaudRate;
            txtWordLen1.Text = config.sUart1.u8WordLen.ToString();
            txtWordLen2.Text = config.sUart2.u8WordLen.ToString();
            txtWordLen3.Text = config.sUart3.u8WordLen.ToString();
            txtStopBits1.Text = config.sUart1.u8StopBits.ToString();
            txtStopBits2.Text = config.sUart2.u8StopBits.ToString();
            txtStopBits3.Text = config.sUart3.u8StopBits.ToString();
            txtParity1.Text = config.sUart1.u8Parity.ToString();
            txtParity2.Text = config.sUart2.u8Parity.ToString();
            txtParity3.Text = config.sUart3.u8Parity.ToString();
            btnSaveChanges.Enabled = false;
            cbBaudRate1.Items.AddRange(new object[] {600,1200,2400,4800,9600,14400,19200,28800,38400,56000});
            cbBaudRate1.SelectedItem = 4800;
            cbBaudRate2.Items.AddRange(new object[] { 600, 1200, 2400, 4800, 9600, 14400, 19200, 28800, 38400, 56000 });
            cbBaudRate2.SelectedItem = 4800;
            cbBaudRate3.Items.AddRange(new object[] { 600, 1200, 2400, 4800, 9600, 14400, 19200, 28800, 38400, 56000 });
            cbBaudRate3.SelectedItem = 4800;
        }

        private void TxtName_TextChanged(object sender, EventArgs e)
        {
            if (txtNameModem.Text != "" && txtNameModem.Text != oldNameModem) btnSaveChanges.Enabled = true;
        }
    }
}
