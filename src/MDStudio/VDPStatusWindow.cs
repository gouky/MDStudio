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
using DGenInterface;

namespace MDStudio
{
    public partial class VDPStatusWindow : Form
    {
        MainForm m_Parent;
        public delegate void FuncUpdateInfo(int itemIndex);

        private struct VDPRegsiterBitfield
        {
            public VDPRegsiterBitfield(int bit, string name)
            {
                m_Bit = bit;
                m_Name = name;
            }

            public int m_Bit;
            public string m_Name;
        }

        private struct VDPRegisterInfo
        {
            public VDPRegisterInfo(int itemIndex, int index, string name, FuncUpdateInfo funcUpdate, VDPRegsiterBitfield[] bitfield)
            {
                this.itemIndex = itemIndex;
                this.baseIndex = index;
                this.name = name;
                this.value = 0;
                this.funcUpdate = funcUpdate;
                this.bitField = bitfield;
            }

            public int itemIndex;
            public int baseIndex;
            public int value;
            public string name;
            public FuncUpdateInfo funcUpdate;
            public VDPRegsiterBitfield[] bitField;
        };

        private VDPRegisterInfo[] m_VDPRegisters;

        private VDPRegsiterBitfield[] m_VDPModeRegister3Bitfield;
        private VDPRegsiterBitfield[] m_VDPModeRegister4Bitfield;

        public VDPStatusWindow(MainForm parent)
        {
            InitializeComponent();

            m_Parent = parent;
            Owner = parent;

            m_VDPModeRegister3Bitfield = new VDPRegsiterBitfield[4]
            {
                new VDPRegsiterBitfield(0, "HS2"),
                new VDPRegsiterBitfield(1, "HS1"),
                new VDPRegsiterBitfield(2, "VS"),
                new VDPRegsiterBitfield(3, "IE2"),
            };

            m_VDPModeRegister4Bitfield = new VDPRegsiterBitfield[8]
            {
                new VDPRegsiterBitfield(0, "RS1"),
                new VDPRegsiterBitfield(1, "LSM0"),
                new VDPRegsiterBitfield(2, "LSM1"),
                new VDPRegsiterBitfield(3, "SHI"),
                new VDPRegsiterBitfield(4, "SPR"),
                new VDPRegsiterBitfield(5, "HSY"),
                new VDPRegsiterBitfield(6, "VSY"),
                new VDPRegsiterBitfield(7, "RS0")
            };

            m_VDPRegisters = new VDPRegisterInfo[23]
            {
                new VDPRegisterInfo(0, 0x0, "Mode Register 1", null, null ),
                new VDPRegisterInfo(1, 0x1, "Mode Register 2", null, null ),
                new VDPRegisterInfo(2, 0x2, "Plane A Name Table Location", UpdatePlaneATableLocation, null ),
                new VDPRegisterInfo(3, 0x3, "Window Name Table Location", UpdateWindowTableLocation, null ),
                new VDPRegisterInfo(4, 0x4, "Plane B Name Table Location", UpdatePlaneBTableLocation, null ),
                new VDPRegisterInfo(5, 0x5, "Sprite Table Location", UpdateSpriteTableLocation, null),
                new VDPRegisterInfo(6, 0x6, "Sprite Pattern Generator Base Address", null, null ),
                new VDPRegisterInfo(7, 0x7, "Background Colour", null, null ),
                new VDPRegisterInfo(8, 0xA, "Horizontal Interrupt Counter", null, null ),
                new VDPRegisterInfo(9, 0xB, "Mode Register 3", null, m_VDPModeRegister3Bitfield ),
                new VDPRegisterInfo(10, 0xC, "Mode Register 4", UpdateModeRegister4, m_VDPModeRegister4Bitfield ),
                new VDPRegisterInfo(11, 0xD, "Horizontal Scroll Data Location", null, null ),
                new VDPRegisterInfo(12, 0xE, "Nametable Pattern Generator Base Address", null, null ),
                new VDPRegisterInfo(13, 0xF, "Auto-Increment Value", null, null ),
                new VDPRegisterInfo(14, 0x10, "Plane Size", null, null ),
                new VDPRegisterInfo(15, 0x11, "Window Plane Horizontal Position", null, null ),
                new VDPRegisterInfo(16, 0x12, "Window Plane Vertical Position", null, null ),
                new VDPRegisterInfo(17, 0x13, "DMA Length lo", null, null ),
                new VDPRegisterInfo(18, 0x14, "DMA Length hi", null, null ),
                new VDPRegisterInfo(19, 0x15, "DMA Source", null, null ),
                new VDPRegisterInfo(20, 0x16, "DMA Source", null, null ),
                new VDPRegisterInfo(21, 0x17, "DMA Source", null, null ),
                new VDPRegisterInfo(22, 0x18, "DMA Source", null, null ),
            };

            foreach(VDPRegisterInfo register in m_VDPRegisters)
            {
                ListViewItem item = new ListViewItem();

                item.Text = String.Format("0x{0:X}", register.baseIndex.ToString());
                item.SubItems.Add(register.name);
                item.SubItems.Add(String.Format("0x{0:X}",register.value.ToString()));
                item.SubItems.Add("");

                listVDPStatus.Items.Add(item);
            }
        }

        private void VDPStatusWindow_VisibleChanged(object sender, EventArgs e)
        {
            m_Parent.UpdateViewVDPStatus(Visible);
        }

        private void VDPStatusWindow_Load(object sender, EventArgs e)
        {
            if (Settings.Default.VDPStatusWindowPosition != null)
            {
                this.Location = Settings.Default.VDPStatusWindowPosition;
            }
        }

        private void VDPStatusWindow_Move(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                Settings.Default.VDPStatusWindowPosition = this.Location;
            }
        }

        public void UpdateView()
        {
            /*
            if (DGenThread.GetDGen() != null)
            {
                for(int i=0; i<m_VDPRegisters.Length; ++i)
                {
                    int value = DGenThread.GetDGen().GetVDPRegisterValue(m_VDPRegisters[i].baseIndex);
                    
                    if(value != m_VDPRegisters[i].value)
                    {
                        this.listVDPStatus.Items[i].SubItems[2].Text = String.Format("0x{0:X}", value);

                        m_VDPRegisters[i].value = value;
                        if(m_VDPRegisters[i].funcUpdate != null)
                        {
                            m_VDPRegisters[i].funcUpdate(i);
                        }

                        this.listVDPStatus.Items[i].ForeColor = Color.Red;
                    }
                    else
                    {
                        this.listVDPStatus.Items[i].ForeColor = Color.Black;
                    }
                }
            }
            */
        }

        public void Reset()
        {
            for (int i = 0; i < m_VDPRegisters.Length; ++i)
            {
                m_VDPRegisters[i].value = DGenThread.GetDGen().GetVDPRegisterValue(m_VDPRegisters[i].baseIndex);
            }
        }

        public void UpdatePlaneATableLocation(int itemIndex)
        {
            int address = ((m_VDPRegisters[itemIndex].value >> 3) &0xF) << 13;

           this.listVDPStatus.Items[itemIndex].SubItems[3].Text = String.Format("0x{0:X}", address);
        }

        public void UpdatePlaneBTableLocation(int itemIndex)
        {
            int address = (m_VDPRegisters[itemIndex].value & 0xF) << 13;

            this.listVDPStatus.Items[itemIndex].SubItems[3].Text = String.Format("0x{0:X}", address);
        }

        public void UpdateSpriteTableLocation(int itemIndex)
        {
            int address = (((m_VDPRegisters[itemIndex].value) >> 4)&0x1) << 16;

            this.listVDPStatus.Items[itemIndex].SubItems[3].Text = String.Format("0x{0:X}", address);
        }

        public void UpdateWindowTableLocation(int itemIndex)
        {
            int address = ((m_VDPRegisters[itemIndex].value >> 1) & 0x3F) << 11;

            this.listVDPStatus.Items[itemIndex].SubItems[3].Text = String.Format("0x{0:X}", address);
        }

        private string GetRegisterBitFieldInfo(int value, VDPRegsiterBitfield[] bitfields)
        {
            string info = "";
            bool coma = false;
            
            foreach(VDPRegsiterBitfield bitfield in bitfields)
            {
                if((value & (1<<bitfield.m_Bit)) != 0)
                {
                    info += (coma ? ", " : "") + bitfield.m_Name;
                    coma = true;
                }
            }

            return info;
        }

        public void UpdateModeRegister4(int itemIndex)
        {
            this.listVDPStatus.Items[itemIndex].SubItems[3].Text = GetRegisterBitFieldInfo(m_VDPRegisters[itemIndex].value, m_VDPRegisters[itemIndex].bitField);
        }
    }
}
