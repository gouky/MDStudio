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

        private struct VDPRegisterInfo
        {
            public VDPRegisterInfo(int index, string name)
            {
                this.baseIndex = index;
                this.name = name;
                this.value = 0;
            }

            public int baseIndex;
            public int value;
            public string name;
        };

        private VDPRegisterInfo[] m_VDPRegisters;

        public VDPStatusWindow(MainForm parent)
        {
            InitializeComponent();

            m_Parent = parent;
            Owner = parent;
            
            m_VDPRegisters = new VDPRegisterInfo[23]
            {
                new VDPRegisterInfo(0x0, "Mode Register 1" ),
                new VDPRegisterInfo(0x1, "Mode Register 2" ),
                new VDPRegisterInfo(0x2, "Plane A Name Table Location" ),
                new VDPRegisterInfo(0x3, "Window Name Table Location" ),
                new VDPRegisterInfo(0x4, "Plane B Name Table Location" ),
                new VDPRegisterInfo(0x5, "Sprite Table Location" ),
                new VDPRegisterInfo(0x6, "Sprite Pattern Generator Base Address" ),
                new VDPRegisterInfo(0x7, "Background Colour" ),
                new VDPRegisterInfo(0xA, "Horizontal Interrupt Counter" ),
                new VDPRegisterInfo(0xB, "Mode Register 3" ),
                new VDPRegisterInfo(0xC, "Mode Register 4" ),
                new VDPRegisterInfo(0xD, "Horizontal Scroll Data Location" ),
                new VDPRegisterInfo(0xE, "Nametable Pattern Generator Base Address" ),
                new VDPRegisterInfo(0xF, "Auto-Increment Value" ),
                new VDPRegisterInfo(0x10, "Plane Size" ),
                new VDPRegisterInfo(0x11, "Window Plane Horizontal Position" ),
                new VDPRegisterInfo(0x12, "Window Plane Vertical Position" ),
                new VDPRegisterInfo(0x13, "DMA Length lo" ),
                new VDPRegisterInfo(0x14, "DMA Length hi" ),
                new VDPRegisterInfo(0x15, "DMA Source" ),
                new VDPRegisterInfo(0x16, "DMA Source" ),
                new VDPRegisterInfo(0x17, "DMA Source" ),
                new VDPRegisterInfo(0x18, "DMA Source" ),
            };

            foreach(VDPRegisterInfo register in m_VDPRegisters)
            {
                ListViewItem item = new ListViewItem();

                item.Text = String.Format("0x{0:X}", register.baseIndex.ToString());
                item.SubItems.Add(register.name);
                item.SubItems.Add(String.Format("0x{0:X}",register.value.ToString()));

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
            if (DGenThread.GetDGen() != null)
            {
                for(int i=0; i<m_VDPRegisters.Length; ++i)
                {
                    int value = DGenThread.GetDGen().GetVDPRegisterValue(m_VDPRegisters[i].baseIndex);
                    
                    if(value != m_VDPRegisters[i].value)
                    {
                        this.listVDPStatus.Items[i].SubItems[2].Text = String.Format("0x{0:X}", value);

                        m_VDPRegisters[i].value = value;
                    }
                    else
                    {
//                         if(this.listVDPStatus.Items[0].SubItems[1].ForeColor == Color.Red)
//                         {
//                             this.listVDPStatus.Items[0].SubItems[1].ForeColor = Color.Black;
//                         }
                    }
                }
            }
        }
    }
}
