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
using System.Runtime.Remoting.Metadata.W3cXsd2001;

namespace MDStudio
{
    public partial class RegisterView : Form
    {
        private enum kViewType
        {
            eDecimal,
            eHexadecimal,
            eBinary,
            eString
        }

        private enum kDataSize
        {
            eByte,
            eWord,
            eLong
        }

        private kViewType m_ViewType;
        private kViewType m_ViewTypeData = kViewType.eHexadecimal;

        private byte[][] m_DataCache;

        private const int kNumDataRegs = 8;

        public void SetRegs(uint d0, uint d1, uint d2, uint d3, uint d4, uint d5, uint d6, uint d7,
                            uint a0, uint a1, uint a2, uint a3, uint a4, uint a5, uint a6, uint sp, uint usp,
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

            SetRegText(txt_sp, sp);
            SetRegText(txt_usp, usp);
            SetRegText(txt_pc, pc);
            SetRegText(txt_sr, sr);
        }

        public void SetZ80Regs( uint af, uint bc, uint de, uint hl,
                                uint af2, uint bc2, uint de2, uint hl2,
                                uint ix, uint iy, uint sr, uint pc)
        {
            SetRegText(txt_a, af >> 8, kDataSize.eByte);
            SetRegText(txt_f, af & 0xF, kDataSize.eByte);
            SetRegText(txt_b, bc >> 8, kDataSize.eByte);
            SetRegText(txt_c, bc & 0xF, kDataSize.eByte);
            SetRegText(txt_d, de >> 8, kDataSize.eByte);
            SetRegText(txt_e, de & 0xF, kDataSize.eByte);
            SetRegText(txt_h, hl >> 8, kDataSize.eByte);
            SetRegText(txt_l, hl & 0xF, kDataSize.eByte);
            SetRegText(txt_a_alt, af2 >> 8, kDataSize.eByte);
            SetRegText(txt_f_alt, af2 & 0xF, kDataSize.eByte);
            SetRegText(txt_b_alt, bc2 >> 8, kDataSize.eByte);
            SetRegText(txt_c_alt, bc2 & 0xF, kDataSize.eByte);
            SetRegText(txt_d_alt, de2 >> 8, kDataSize.eByte);
            SetRegText(txt_e_alt, de2 & 0xF, kDataSize.eByte);
            SetRegText(txt_h_alt, hl2 >> 8, kDataSize.eByte);
            SetRegText(txt_l_alt, hl2 & 0xF, kDataSize.eByte);
            SetRegText(txt_ix, ix, kDataSize.eWord);
            SetRegText(txt_iy, iy, kDataSize.eWord);
            SetRegText(txt_z80_sr, sr, kDataSize.eWord);
            SetRegText(txt_z80_pc, pc, kDataSize.eWord);
        }

        private void SetDataView(TextBox textBox, byte[] data, kViewType viewType)
        {
            switch(viewType)
            {
                case kViewType.eHexadecimal:
                    SoapHexBinary text = new SoapHexBinary(data);
                    textBox.Text = text.ToString();
                    break;
                case kViewType.eString:
                    textBox.Text = System.Text.Encoding.ASCII.GetString(data);
                    break;
                default:
                    //TODO
                    break;
            }
        }

        public void SetData_a0(byte[] data)
        {
            SetDataView(txt_a0_data, data, m_ViewTypeData);
            Array.Copy(data, m_DataCache[0], MainForm.kMaxMemPreviewSize);
        }

        public void SetData_a1(byte[] data)
        {
            SetDataView(txt_a1_data, data, m_ViewTypeData);
            Array.Copy(data, m_DataCache[2], MainForm.kMaxMemPreviewSize);
        }

        public void SetData_a2(byte[] data)
        {
            SetDataView(txt_a2_data, data, m_ViewTypeData);
            Array.Copy(data, m_DataCache[2], MainForm.kMaxMemPreviewSize);
        }

        public void SetData_a3(byte[] data)
        {
            SetDataView(txt_a3_data, data, m_ViewTypeData);
            Array.Copy(data, m_DataCache[3], MainForm.kMaxMemPreviewSize);
        }

        public void SetData_a4(byte[] data)
        {
            SetDataView(txt_a4_data, data, m_ViewTypeData);
            Array.Copy(data, m_DataCache[4], MainForm.kMaxMemPreviewSize);
        }

        public void SetData_a5(byte[] data)
        {
            SetDataView(txt_a5_data, data, m_ViewTypeData);
            Array.Copy(data, m_DataCache[5], MainForm.kMaxMemPreviewSize);
        }

        public void SetData_a6(byte[] data)
        {
            SetDataView(txt_a6_data, data, m_ViewTypeData);
            Array.Copy(data, m_DataCache[6], MainForm.kMaxMemPreviewSize);
        }

        public void SetData_sp(byte[] data)
        {
            SetDataView(txt_sp_data, data, m_ViewTypeData);
            Array.Copy(data, m_DataCache[7], MainForm.kMaxMemPreviewSize);
        }

        private void RefreshDataViews()
        {
            SetDataView(txt_a0_data, m_DataCache[0], m_ViewTypeData);
            SetDataView(txt_a1_data, m_DataCache[1], m_ViewTypeData);
            SetDataView(txt_a2_data, m_DataCache[2], m_ViewTypeData);
            SetDataView(txt_a3_data, m_DataCache[3], m_ViewTypeData);
            SetDataView(txt_a4_data, m_DataCache[4], m_ViewTypeData);
            SetDataView(txt_a5_data, m_DataCache[5], m_ViewTypeData);
            SetDataView(txt_a6_data, m_DataCache[6], m_ViewTypeData);
            SetDataView(txt_sp_data, m_DataCache[7], m_ViewTypeData);
        }

        private void SetRegText(TextBox textBox, uint value, kDataSize dataSize = kDataSize.eLong)
        {
            string text;
            
            switch(m_ViewType)
            {
                case kViewType.eBinary:
                    text = Convert.ToString(value, 2);
                    break;
                case kViewType.eDecimal:
                    text = value.ToString("D8");
                    break;
                default:
                case kViewType.eHexadecimal:
                    if(dataSize == kDataSize.eByte)
                        text = value.ToString("X2");
                    else if (dataSize == kDataSize.eWord)
                        text = value.ToString("X4");
                    else
                        text = value.ToString("X8");
                    break;
            }
            if (textBox.Text == text)
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
            m_DataCache = new byte[kNumDataRegs][];
            for(int i = 0; i < kNumDataRegs; i++)
            {
                m_DataCache[i] = new byte[MainForm.kMaxMemPreviewSize];
            }

            InitializeComponent();
        }

        private void RegisterView_Load(object sender, EventArgs e)
        {
            Owner = Application.OpenForms[0];

            m_ViewType = kViewType.eHexadecimal;
            hexaViewMenu.Checked = true;

            if (Settings.Default.RegisterWindowLocation != null)
            {
                this.Location = Settings.Default.RegisterWindowLocation;
            }
        }

        private void RegisterView_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.RegisterWindowLocation = this.Location;
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }
        
        private void ChangeView(kViewType viewType)
        {
            m_ViewType = viewType;

            switch(viewType)
            {
                case kViewType.eDecimal:
                    hexaViewMenu.Checked = false;
                    binaryViewMenu.Checked = false;
                    decimalViewMenu.Checked = true;
                    break;
                case kViewType.eHexadecimal:
                    hexaViewMenu.Checked = true;
                    binaryViewMenu.Checked = false;
                    decimalViewMenu.Checked = false;
                    break;
                case kViewType.eBinary:
                    hexaViewMenu.Checked = false;
                    binaryViewMenu.Checked = true;
                    decimalViewMenu.Checked = false;
                    break;
            }
        }

        private void decimalViewMenu_Click(object sender, EventArgs e)
        {
            ChangeView(kViewType.eDecimal);
        }

        private void hexaViewMenu_Click(object sender, EventArgs e)
        {
            ChangeView(kViewType.eHexadecimal);
        }

        private void binaryViewMenu_Click(object sender, EventArgs e)
        {
            ChangeView(kViewType.eBinary);
        }

        private void viewToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void dataHexaClick(object sender, EventArgs e)
        {
            dataStringToolStripMenuItem.Checked = false;
            m_ViewTypeData = kViewType.eHexadecimal;
            RefreshDataViews();
        }

        private void dataStringClick(object sender, EventArgs e)
        {
            dataHexadecimalToolStripMenuItem.Checked = false;
            m_ViewTypeData = kViewType.eString;
            RefreshDataViews();
        }
    }
}
