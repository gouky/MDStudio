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
    public partial class BuildLog : Form
    {
        private struct ErrorEntry
        {
            public ErrorEntry(string _file, int _line, string _message)
            {
                file = _file;
                line = _line;
                message = _message;
            }

            public string file;
            public int line;
            public string message;
        };

        MainForm m_Parent;
        List<ErrorEntry> m_Errors;
        string m_RawLog;

        public BuildLog(MainForm parent)
        {
            m_Parent = parent;

            InitializeComponent();

            listErrors.MultiSelect = false;

        }

        public void Clear()
        {
            listErrors.Items.Clear();
            m_Errors = new List<ErrorEntry>();
            txtRawLog.Clear();
            m_RawLog = string.Empty;
        }

        public void AddError(string file, int line, string message)
        {
            string fileShort = System.IO.Path.GetFileName(file);
            listErrors.Items.Add(new ListViewItem(new[] { line.ToString(), fileShort, message }));
            m_Errors.Add(new ErrorEntry(file, line, message));
        }

        public void AddRaw(string line)
        {
            txtRawLog.AppendText(line + Environment.NewLine);
            m_RawLog += line + Environment.NewLine;
        }

        private void BuildLog_Load(object sender, EventArgs e)
        {
        }

        private void listErrors_DoubleClick(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection selection = listErrors.SelectedItems;

            if (selection.Count > 0 && selection[0].Index < m_Errors.Count())
            {
                m_Parent.GoTo(m_Errors[selection[0].Index].file, m_Errors[selection[0].Index].line);
            }
        }

        private void BuildLog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }

        private void BuildLog_Shown(object sender, EventArgs e)
        {
            m_Parent.UpdateViewBuildLog(Visible);
        }

        private void listErrors_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            saveLogDialog.ShowDialog();
        }

        private void saveLogDialog_FileOk(object sender, CancelEventArgs e)
        {
            using (System.IO.FileStream file = System.IO.File.OpenWrite(saveLogDialog.FileName))
            {
                file.Write(Encoding.ASCII.GetBytes(m_RawLog), 0, m_RawLog.Count());
            }
        }
    }
}
