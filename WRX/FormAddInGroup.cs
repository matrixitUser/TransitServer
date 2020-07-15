using System;
using System.Windows.Forms;

namespace TransitServer
{
    public partial class FormAddInGroup : Form
    {
        public FormAddInGroup()
        {
            InitializeComponent();
        }
        public TreeNodeMouseClickEventArgs nodeAddLocation;
        public string textNode = "";
        public string nameNode = "";
        private void treeViewAdd_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            textNode = e.Node.Name;
            nameNode = e.Node.Text;
        }

        private void btAdd_Click(object sender, EventArgs e)
        {
            SQLite.Instance.UpdateGroupModems(labImei.Text, textNode);
            DialogResult = DialogResult.OK;
        }
    }
}
