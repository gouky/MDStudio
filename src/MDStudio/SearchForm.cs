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
    public partial class SearchForm : Form
    {
        public SearchForm()
        {
            InitializeComponent();

            this.CancelButton = btnCancel;
            this.AcceptButton = btnSearch;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            Close();
        }
    }
}
