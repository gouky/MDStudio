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
        public enum Type
        {
            Line,
            Address
        }

        public GoToForm(Type type)
        {
            InitializeComponent();

            switch(type)
            {
                case Type.Address:
                    Text = "GoTo address";
                    label.Text = "Hex address:";
                    break;

                case Type.Line:
                    Text = "GoTo line";
                    label.Text = "Line number:";
                    break;
            }
        }

        private void GoToForm_Load(object sender, EventArgs e)
        {
            this.AcceptButton = button2;
            this.CancelButton = button1;
        }
    }
}
