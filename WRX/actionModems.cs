using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace TransitServer
{
    public partial class ActionModems : Form
    {
        public ActionModems()
        {
            InitializeComponent();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            ViewModems(SQLite.Instance.GetModems());
            Close();
        }

        private void btnDeleteModem_Click(object sender, EventArgs e)
        {
            SQLite.Instance.DeleteModems(txtId.Text);
            ViewModems(SQLite.Instance.GetModems());
            Close();
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
                form1.dgvModems.Rows[form1.dgvModems.RowCount - 1].Cells[MODEMSCOLLACTIVECONNECTION].Value = rec.activeConnection;
                if ((int)rec.activeConnection == 1)
                    form1.dgvModems.Rows[form1.dgvModems.RowCount - 1].Cells[MODEMSCOLLACTIVECONNECTION].Style.BackColor = System.Drawing.Color.Green;
                else form1.dgvModems.Rows[form1.dgvModems.RowCount - 1].Cells[MODEMSCOLLACTIVECONNECTION].Style.BackColor = System.Drawing.Color.White;
            }
        }

        private void btnSaveChanges_Click(object sender, EventArgs e)
        {
            SQLite.Instance.UpdateNameModemsbyImei(txtImei.Text, txtName.Text);
            MessageBox.Show("Изменения сохранены!");
        }

        private void ActionModems_Load(object sender, EventArgs e)
        {
            btnSaveChanges.Enabled = false;
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            if (txtName.Text != "" && txtName.Text != oldNameModem) btnSaveChanges.Enabled = true;
        }
    }
}
