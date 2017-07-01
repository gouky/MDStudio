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
    public partial class CRamViewer : Form
    {
        private PictureBox[] m_PictureBoxes;
        MainForm m_Parent;

        public CRamViewer(MainForm parent)
        {
            InitializeComponent();

            m_Parent = parent;
            Owner = parent;

            m_PictureBoxes = new PictureBox[64] {
                pictureBox1,                pictureBox2,                pictureBox3,                pictureBox4,                pictureBox5,                pictureBox6,
                pictureBox7,                pictureBox8,                pictureBox9,                pictureBox10,                pictureBox11,                pictureBox12,
                pictureBox13,                pictureBox14,                pictureBox15,                pictureBox16,                pictureBox17,                pictureBox18,
                pictureBox19,                pictureBox20,                pictureBox21,                pictureBox22,                pictureBox23,                pictureBox24,
                pictureBox25,                pictureBox26,                pictureBox27,                pictureBox28,                pictureBox29,                pictureBox30,
                pictureBox31,                pictureBox32,                pictureBox33,                pictureBox34,                pictureBox35,                pictureBox36,
                pictureBox37,                pictureBox38,                pictureBox39,                pictureBox40,                pictureBox41,                pictureBox42,
                pictureBox43,                pictureBox44,                pictureBox45,                pictureBox46,                pictureBox47,                pictureBox48,
                pictureBox49,                pictureBox50,                pictureBox51,                pictureBox52,                pictureBox53,                pictureBox54,
                pictureBox55,                pictureBox56,                pictureBox57,                pictureBox58,                pictureBox59,                pictureBox60,
                pictureBox61,                pictureBox62,                pictureBox63,                pictureBox64
            };
        }

        private void CRamViewer_Load(object sender, EventArgs e)
        {
            if (Settings.Default.CRAMWindowPosition != null)
            {
                this.Location = Settings.Default.CRAMWindowPosition;
            }
        }

        public void SetColor(int index, uint rgb)
        {
            m_PictureBoxes[index].BackColor = Color.FromArgb(255, (int)((rgb >> 16) & 0xFF), (int)((rgb >> 8) & 0xFF), (int)(rgb & 0xFF));
        }

        private void CRamViewer_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.Visible)
            {
                Settings.Default.CRAMWindowPosition = this.Location;
            }
        }

        private void CRamViewer_Shown(object sender, EventArgs e)
        {
            m_Parent.UpdateViewCRAM(Visible);
        }
    }
}
