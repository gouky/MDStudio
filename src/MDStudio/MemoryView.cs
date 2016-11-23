using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ComponentModel.Design;

namespace MDStudio
{
    public partial class MemoryView : Form
    {
        private ByteViewer m_ByteViewer;

        public MemoryView()
        {
            InitializeComponent();

            m_ByteViewer = new ByteViewer();
            m_ByteViewer.Dock = DockStyle.Fill;
            m_ByteViewer.Left = 0;
            m_ByteViewer.Top = 0;

            Controls.Add(m_ByteViewer);
            m_ByteViewer.SetFile(@"c:\windows\notepad.exe");
        }

        private void MemoryView_Load(object sender, EventArgs e)
        {
        }
    }
}
