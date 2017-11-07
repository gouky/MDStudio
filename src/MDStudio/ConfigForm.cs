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

        public class RegionEntry
        {
            public string Text { get; set; }
            public char RegionCode { get; set; }

            public override string ToString()
            {
                return Text;
            }
        }

        public class KeycodeEntry
        {
            public SDL_Keycode.Keycode Keycode { get; set; }

            public override string ToString()
            {
                return Enum.GetName(typeof(SDL_Keycode.Keycode), Keycode);
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

            //Populate resolution fields
            foreach(Tuple<int, int> resolution in MainForm.kValidResolutions)
            {
                ResolutionEntry entry = new ResolutionEntry();
                entry.Width = resolution.Item1;
                entry.Height = resolution.Item2;
                entry.Text = String.Format("{0} x {1}", entry.Width, entry.Height);
                emuResolution.Items.Add(entry);
            }

            emuResolution.SelectedIndex = MainForm.kDefaultResolutionEntry;

            //Populate region fields
            foreach (Tuple<char, string> region in MainForm.kRegions)
            {
                RegionEntry entry = new RegionEntry();
                entry.RegionCode = region.Item1;
                entry.Text = region.Item2;
                emuRegion.Items.Add(entry);
            }

            emuRegion.SelectedIndex = MainForm.kDefaultRegion;

            //Populate keycode fields
            foreach (SDL_Keycode.Keycode keycode in Enum.GetValues(typeof(SDL_Keycode.Keycode)))
            {
                KeycodeEntry entry = new KeycodeEntry();
                entry.Keycode = keycode;

                inputUp.Items.Add(entry);
                inputDown.Items.Add(entry);
                inputLeft.Items.Add(entry);
                inputRight.Items.Add(entry);
                inputA.Items.Add(entry);
                inputB.Items.Add(entry);
                inputC.Items.Add(entry);
                inputStart.Items.Add(entry);
            }
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

        private void cancelBtn_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void okBtn_Click(object sender, EventArgs e)
        {

        }

        private void pathMegaUSBButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog pathSelect = new OpenFileDialog();

            if (pathSelect.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                megaUSBPath.Text = pathSelect.FileName;
            }
        }
    }
}
