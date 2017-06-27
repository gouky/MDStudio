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
    public partial class FileView : Form
    {
        public class FileEntry
        {
            public string Text { get; set; }

            public override string ToString()
            {
                return Text;
            }
        }

        MainForm m_mainForm;
        List<string> m_files;

        public FileView(MainForm mainForm, List<string> files)
        {
            m_mainForm = mainForm;
            m_files = files;
            InitializeComponent();
        }

        private void SymbolView_Load(object sender, EventArgs e)
        {
            AcceptButton = btnGoto;
            ActiveControl = textSearch;

           foreach (string file in m_files)
           {
               FileEntry entry = new FileEntry();
                entry.Text = file;
               listFiles.Items.Add(entry);
           }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnGoto_Click(object sender, EventArgs e)
        {
            if (listFiles.SelectedItem != null)
            {
                string file = (listFiles.SelectedItem as FileEntry).Text;
                m_mainForm.GoTo(file, 0);
                Close();
            }
        }

        private void listSymbols_DoubleClick(object sender, EventArgs e)
        {
            if (listFiles.SelectedItem != null)
            {
                string file = (listFiles.SelectedItem as FileEntry).Text;
                m_mainForm.GoTo(file, 0);
                Close();
            }
        }

        private void textSearch_TextChanged(object sender, EventArgs e)
        {
            string searchString = textSearch.Text.ToLower();
            string[] tokens = searchString.Split(' ');
            listFiles.SelectedItem = null;

            if (searchString.Length > 0)
            {
                //Fuzzy (space delimited) search
                foreach (FileEntry file in listFiles.Items)
                {
                    string filename = file.Text.ToLower();
                    bool match = true;

                    foreach (string token in tokens)
                    {
                        if (!filename.Contains(token))
                        {
                            //Mismatch
                            match = false;
                            break;
                        }
                    }

                    if (match)
                    {
                        //Filename contains all search tokens
                        listFiles.SelectedItem = file;
                        return;
                    }
                }
            }
        }
    }
}
