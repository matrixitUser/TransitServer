using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace TransitServer
{
    public partial class FormRedactorModems : Form
    {
        public string idModem;
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
