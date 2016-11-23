using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MDStudio
{
    public partial class ConfigForm : Form
    {
        public ConfigForm()
        {
            InitializeComponent();

            AcceptButton = okBtn;
            CancelButton = cancelBtn;
        }

        private void ConfigForm_Load(object sender, EventArgs e)
        {

        }

        private void pathButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog pathSelect = new OpenFileDialog();

            if (pathSelect.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                asmPath.Text = pathSelect.FileName;
            }
        }
    }
}
