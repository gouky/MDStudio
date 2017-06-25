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
    public partial class RegisterView : Form
    {
        public void SetRegs(uint d0, uint d1, uint d2, uint d3, uint d4, uint d5, uint d6, uint d7,
                            uint a0, uint a1, uint a2, uint a3, uint a4, uint a5, uint a6, uint usp,
                            uint sr, uint pc)
        {
            txt_d0.Text = "0x" + d0.ToString("X8");
            txt_d1.Text = "0x" + d1.ToString("X8");
            txt_d2.Text = "0x" + d2.ToString("X8");
            txt_d3.Text = "0x" + d3.ToString("X8");
            txt_d4.Text = "0x" + d4.ToString("X8");
            txt_d5.Text = "0x" + d5.ToString("X8");
            txt_d6.Text = "0x" + d6.ToString("X8");
            txt_d7.Text = "0x" + d7.ToString("X8");

            txt_a0.Text = "0x" + a0.ToString("X8");
            txt_a1.Text = "0x" + a1.ToString("X8");
            txt_a2.Text = "0x" + a2.ToString("X8");
            txt_a3.Text = "0x" + a3.ToString("X8");
            txt_a4.Text = "0x" + a4.ToString("X8");
            txt_a5.Text = "0x" + a5.ToString("X8");
            txt_a6.Text = "0x" + a6.ToString("X8");

            txt_usp.Text = "0x" + usp.ToString("X8");
            txt_pc.Text = "0x" + pc.ToString("X8");
            txt_sr.Text = "0x" + sr.ToString("X4");
        }

        public RegisterView()
        {
            InitializeComponent();
        }

        private void RegisterView_Load(object sender, EventArgs e)
        {
            Owner = Application.OpenForms[0];
        }
    }
}
