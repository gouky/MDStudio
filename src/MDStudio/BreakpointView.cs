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
    public partial class BreakpointView : Form
    {
        private MainForm m_mainForm;
        private Symbols m_symbols;

        class BreakpointEntry
        {
            public BreakpointEntry(string filename, int lineNo, uint address)
            {
                m_filename = filename;
                m_lineNo = lineNo;
                m_address = address;
            }

            public override string ToString()
            {
                return m_filename + ", " + m_lineNo;
            }

            public string m_filename;
            public int m_lineNo;
            public uint m_address;
        }

        public enum BreakpointType
        {
            Execute,
            Write
        }

        public BreakpointView(MainForm mainForm, Symbols symbols)
        {
            InitializeComponent();
            m_mainForm = mainForm;
            m_symbols = symbols;
        }

        public void UpdateSymbols(Symbols symbols)
        {
            m_symbols = symbols;
        }

        public void ClearBreakpoints()
        {
            breakpointList.Items.Clear();
        }

        public void SetBreakpoint(uint address)
        {
            if(FindBreakpoint(address) == null)
            {
                Tuple<string, int> fileLine = m_symbols.GetFileLine(address);
                if (fileLine != null)
                {
                    breakpointList.Items.Add(new BreakpointEntry(fileLine.Item1, fileLine.Item2, address));
                }
            }
        }

        public void RemoveBreakpoint(uint address)
        {
            breakpointList.Items.Remove(FindBreakpoint(address));
        }

        private BreakpointEntry FindBreakpoint(uint address)
        {
            return breakpointList.Items.Cast<BreakpointEntry>().ToList().Find(item => item.m_address == address);
        }

        private void BreakpointView_Load(object sender, EventArgs e)
        {

        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void pCBreakpointToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void writeBreakpointToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void breakpointList_DoubleClick(object sender, EventArgs e)
        {
            if (breakpointList.SelectedItem != null)
            {
                BreakpointEntry breakpoint = breakpointList.SelectedItem as BreakpointEntry;
                m_mainForm.GoTo(breakpoint.m_filename, breakpoint.m_lineNo);
            }
        }
    }
}
