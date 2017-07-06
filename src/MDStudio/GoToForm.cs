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
    public partial class GoToForm : Form
    {
        public GoToForm()
        {
            InitializeComponent();
        }

        private void GoToForm_Load(object sender, EventArgs e)
        {
            this.AcceptButton = button2;
            this.CancelButton = button1;
        }
    }
}
