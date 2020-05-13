using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TransitServer
{
    public partial class FormAddModem : Form
    {
        public String imei { get; set; }
        public String port { get; set; }
        public String name { get; set; }
        public String lastCon { get; set; }
        public FormAddModem()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            imei = txtImei.Text;
            port = txtPort.Text;
            name = txtName.Text;
            lastCon = txtLastCon.Text;
            
            DialogResult = DialogResult.OK;
        }
    }
}
