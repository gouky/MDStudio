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
    public partial class VDPRegs : Form
    {
        private CheckBox[] checkBoxes;
        MainForm m_Parent;

        public VDPRegs(MainForm parent)
        {
            m_Parent = parent;

            InitializeComponent();

            checkBoxes = new CheckBox[] { checkBox7, checkBox6, checkBox5, checkBox4, checkBox3, checkBox2, checkBox1, checkBox0 };
        }

        private void VDPRegs_Load(object sender, EventArgs e)
        {
            comboReg.SelectedIndex = 0;
        }

        private void comboReg_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (CheckBox check in checkBoxes)
            {
                check.Checked = false;
                check.Text = "Unused";
            }
            labelDesc.Text = "";

            switch (comboReg.SelectedIndex)
            {
                case 0:
                    checkBox4.Text = "IE1 (Horizontal interrupt enable)";
                    checkBox3.Text = "1 = invalid display setting";
                    checkBox2.Text = "Palette select";
                    checkBox1.Text = "M3(HV counter latch enable)";
                    checkBox0.Text = "Display disable";
                    break;

                case 1:
                    checkBox7.Text = "TMS9918 / Genesis display select";
                    checkBox6.Text = "DISP (Display Enable)";
                    checkBox5.Text = "IE0 (Vertical interrupt enable)";
                    checkBox4.Text = "M1 (DMA Enable)";
                    checkBox3.Text = "M2 (PAL/NTSC)";
                    checkBox2.Text = "SMS/Genesis display select";
                    checkBox0.Text = "?";
                    break;

                case 2:
                    checkBox5.Text = "Bit 15 name table address";
                    checkBox4.Text = "Bit 14 name table address";
                    checkBox3.Text = "Bit 13 name table address";

                    labelDesc.Text = "Pattern Name Tble Address for Plane A: $000";
                    break;

                case 3:
                    checkBox5.Text = "Bit 15 Pattern Name table address";
                    checkBox4.Text = "Bit 14 Pattern Name table address";
                    checkBox3.Text = "Bit 13 Pattern Name table address";
                    checkBox2.Text = "Bit 12 Pattern Name table address";
                    checkBox1.Text = "Bit 11 Pattern Name table address";

                    labelDesc.Text = "Pattern Name Table Address for Window: $000";
                    break;
            }

            int i = 7;
            foreach (CheckBox check in checkBoxes)
            {
                check.Text = "$" + i.ToString("X2") + ": " +check.Text;
                i--;
            }

            valueLabel.Text = "Value: $00";
        }

        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            int i = 7;
            int val = 0;
            foreach (CheckBox check in checkBoxes)
            {
                if (check.Checked)
                {
                    val |= (1 << i);
                }
                i--;
            }

            valueLabel.Text = "Value: $" + val.ToString("X2");
        }

        private void VDPRegs_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }

        private void VDPRegs_VisibleChanged(object sender, EventArgs e)
        {
            m_Parent.UpdateVDPHelperMenu(Visible);
        }
    }
}
