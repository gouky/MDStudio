using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MDStudio
{
    public partial class ConfigForm : Form
    {

        public class ResolutionEntry
        {
            public string Text { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }

            public override string ToString()
            {
                return Text;
            }
        }

        public void GetEmuResolution(out int width, out int height)
        {
            width = (emuResolution.SelectedItem as ResolutionEntry).Width;
            height = (emuResolution.SelectedItem as ResolutionEntry).Height;
        }

        public ConfigForm()
        {
            InitializeComponent();

            AcceptButton = okBtn;
            CancelButton = cancelBtn;

            foreach(Tuple<int, int> resolution in MainForm.kValidResolutions)
            {
                ResolutionEntry entry = new ResolutionEntry();
                entry.Width = resolution.Item1;
                entry.Height = resolution.Item2;
                entry.Text = String.Format("{0} x {1}", entry.Width, entry.Height);
                emuResolution.Items.Add(entry);
            }

            emuResolution.SelectedIndex = MainForm.kDefaultResolutionEntry;
        }

        private void ConfigForm_Load(object sender, EventArgs e)
        {

        }

        private void pathButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog pathSelect = new OpenFileDialog();

            if (pathSelect.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                asmPath.Text = pathSelect.FileName;
            }
        }
    }
}
