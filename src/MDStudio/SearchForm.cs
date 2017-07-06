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
        public SearchForm(bool replace = false)
        {
            InitializeComponent();

            this.CancelButton = btnCancel;
            this.AcceptButton = btnSearch;

            if(replace)
            {
                btnSearch.Text = "Replace";
                replaceString.Show();
                label2.Show();
            }
            else
            {
                btnSearch.Text = "Search";
                replaceString.Hide();
                label2.Hide();
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            Close();
        }
    }
}
