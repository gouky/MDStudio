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
    public partial class SymbolView : Form
    {
        public class SymbolEntry
        {
            public string Text { get; set; }
            public uint Address { get; set; }

            public override string ToString()
            {
                return Text;
            }
        }

        MainForm m_mainForm;
        Symbols m_symbols;

        public SymbolView(MainForm mainForm, Symbols symbols)
        {
            m_mainForm = mainForm;
            m_symbols = symbols;
            InitializeComponent();
        }

        private void SymbolView_Load(object sender, EventArgs e)
        {
            AcceptButton = btnGoto;
            ActiveControl = textSearch;

            if(m_symbols == null)
            {
                listSymbols.Enabled = false;
            }
            else
            {
                listSymbols.Enabled = true;
                listSymbols.Items.Clear();

                foreach (Symbols.SymbolEntry symbol in m_symbols.m_Symbols)
                {
                    SymbolEntry entry = new SymbolEntry();
                    entry.Text = symbol.name;
                    entry.Address = symbol.address;
                    listSymbols.Items.Add(entry);
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnGoto_Click(object sender, EventArgs e)
        {
            if (listSymbols.SelectedItem != null)
            {
                uint address = (listSymbols.SelectedItem as SymbolEntry).Address;
                m_mainForm.GoTo(address);
                Close();
            }
        }

        private void listSymbols_DoubleClick(object sender, EventArgs e)
        {
            if(listSymbols.SelectedItem != null)
            {
                uint address = (listSymbols.SelectedItem as SymbolEntry).Address;
                m_mainForm.GoTo(address);
                Close();
            }
        }

        private void textSearch_TextChanged(object sender, EventArgs e)
        {
            if (m_symbols == null)
                return;

            string searchString = textSearch.Text.ToLower();
            string[] tokens = searchString.Split(' ');
            listSymbols.SelectedItem = null;

            if (searchString.Length > 0)
            {
                //Fuzzy (space delimited) search
                foreach (SymbolEntry symbol in listSymbols.Items)
                {
                    string symbolName = symbol.Text.ToLower();
                    bool match = true;

                    foreach (string token in tokens)
                    {
                        if (!symbolName.Contains(token))
                        {
                            //Mismatch
                            match = false;
                            break;
                        }
                    }

                    if (match)
                    {
                        //Symbol name contains all search tokens
                        listSymbols.SelectedItem = symbol;
                        return;
                    }
                }
            }
        }
    }
}
