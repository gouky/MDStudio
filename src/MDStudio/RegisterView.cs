using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MDStudio.Properties;

namespace MDStudio
{
    public partial class RegisterView : Form
    {
        public void SetRegs(uint d0, uint d1, uint d2, uint d3, uint d4, uint d5, uint d6, uint d7,
                            uint a0, uint a1, uint a2, uint a3, uint a4, uint a5, uint a6, uint usp,
                            uint sr, uint pc)
        {
            SetRegText(txt_d0, d0);
            SetRegText(txt_d1, d1);
            SetRegText(txt_d2, d2);
            SetRegText(txt_d3, d3);
            SetRegText(txt_d4, d4);
            SetRegText(txt_d5, d5);
            SetRegText(txt_d6, d6);
            SetRegText(txt_d7, d7);

            SetRegText(txt_a0, a0);
            SetRegText(txt_a1, a1);
            SetRegText(txt_a2, a2);
            SetRegText(txt_a3, a3);
            SetRegText(txt_a4, a4);
            SetRegText(txt_a5, a5);
            SetRegText(txt_a6, a6);

            SetRegText(txt_usp, usp);
            SetRegText(txt_pc, pc);
            SetRegText(txt_sr, sr);
        }

        private void SetRegText(TextBox textBox, uint value)
        {
            string text = value.ToString("X8");
            if(textBox.Text == text)
            {
                textBox.ForeColor = Color.Black;
            }
            else
            {
                textBox.Text = text;
                textBox.ForeColor = Color.Red;
            }
        }

        public RegisterView()
        {
            InitializeComponent();
        }

        private void RegisterView_Load(object sender, EventArgs e)
        {
            Owner = Application.OpenForms[0];

            if(Settings.Default.RegisterWindowLocation != null)
            {
                this.Location = Settings.Default.RegisterWindowLocation;
            }
        }

        private void RegisterView_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.RegisterWindowLocation = this.Location;
        }
    }
}
