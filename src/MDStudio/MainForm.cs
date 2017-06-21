using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DGenInterface;
using DigitalRune.Windows.TextEditor.Document;
using DigitalRune.Windows.TextEditor.Bookmarks;
using DigitalRune.Windows.TextEditor.Highlighting;
using DigitalRune.Windows.TextEditor.Markers;
using System.Text.RegularExpressions;
using System.IO;

namespace MDStudio
{
    //  Code line regex
    //  \s*?([a-zA-Z.]+)\s+([a-zA-Z._0-9#$*+<>\(\)]+)\s*?,? ?([a-zA-Z._0-9#$\(\)]+)?
    public partial class MainForm : Form
    {
        enum State
        {
            kStopped,
            kRunning,
            kDebugging
        };

        private readonly Timer m_Timer = new Timer();
        private string m_PathToProject;
        private string m_ProjectName;
        private string m_SourceFileName;

        private DGenThread m_DGenThread;
        private DebugSource m_DebugSource;

        private MemoryView m_MemoryView;
        private BuildLog m_BuildLog;

        private Config m_Config;

        private VDPRegs m_VDPRegs;

        private bool m_Modified;

        private List<Marker> m_ErrorMarkers;

        public MainForm()
        {
            InitializeComponent();
            
            m_ErrorMarkers = new List<Marker>();

            //
            m_Config = new Config();
            m_Config.Read();

            m_VDPRegs = new VDPRegs(this);

            //
            m_BuildLog = new BuildLog(this);
            m_BuildLog.Hide();

            // Set the syntax-highlighting for C#
            codeEditor.Document.HighlightingStrategy = HighlightingManager.Manager.FindHighlighter("ASM68k");

            // Set events
            codeEditor.Document.DocumentChanged += documentChanged;

            m_DGenThread = new DGenThread();
            m_DGenThread.Init();

            m_Timer.Interval = 16;
            m_Timer.Tick += TimerTick;
            m_Timer.Enabled = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        void TimerTick(object sender, EventArgs e)
        {
            if (DGenThread.GetDGen().IsDebugging())
            {
                 statusLabel.Text = "PC 0x" + DGenThread.GetDGen().GetCurrentPC();
            }
        }

        private void undoMenu_Click(object sender, EventArgs e)
        {
            codeEditor.Undo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            codeEditor.Redo();
        }

        private void codeEditor_DocumentChanged(object sender, DocumentEventArgs e)
        {
            if (codeEditor.Document.UndoStack.CanUndo)
            {
                undoMenu.Enabled = true;
            }
            else
            {
                undoMenu.Enabled = false;
            }

            if (codeEditor.Document.UndoStack.CanRedo)
            {
                redoMenu.Enabled = true;
            }
            else
            {
                redoMenu.Enabled = false;
            }

        }

        private int Build()
        {
            if (m_Config.Asm68kPath == null || m_Config.Asm68kPath.Length == 0)
            {
                MessageBox.Show("ASM68k Path not set");
                return 0;
            }

            if (!File.Exists(m_Config.Asm68kPath))
            {
                MessageBox.Show("Cannot find '" + m_Config.Asm68kPath + "'");
                return 0;
            }
            
            m_BuildLog.Clear();
            if(!m_BuildLog.Visible)
                m_BuildLog.Show();

            Console.WriteLine("compile");

            StringBuilder q = new StringBuilder();

            try
            {
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo.FileName = m_Config.Asm68kPath;
                proc.StartInfo.WorkingDirectory = m_PathToProject + @"\";
                proc.StartInfo.Arguments =  @"/o l+ /p " + m_SourceFileName + "," + m_ProjectName + ".bin," + m_ProjectName + ".symb," + m_ProjectName + ".list";
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.CreateNoWindow = true;
                proc.Start();
                while (!proc.HasExited)
                {
                    q.Append(proc.StandardOutput.ReadToEnd());
                }

                q.Append(proc.StandardOutput.ReadToEnd());

                proc.WaitForExit();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            //()\((\d+)\)\s: Error : (.+)
            string foo = q.ToString();
            string[] output = q.ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            int errorCount = 0;

            foreach (Marker marker in m_ErrorMarkers)
            {
                codeEditor.Document.MarkerStrategy.RemoveMarker(marker);
            }

            m_ErrorMarkers.Clear();

            foreach (string line in output)
            {
                Console.WriteLine(line);
                m_BuildLog.AddRaw(line);
                string patternError = @"([\w:\\.]*)\((\d+)\) : Error : (.+)";
                Match matchError = Regex.Match(line, patternError);

                if (matchError.Success)
                {
                    int lineNumber;

                    int.TryParse(matchError.Groups[2].Value, out lineNumber);

                    m_BuildLog.AddError(lineNumber, matchError.Groups[3].Value);
                    Console.WriteLine("Error in '" + matchError.Groups[1].Value + "' (" + matchError.Groups[2].Value + "): " + matchError.Groups[3].Value);
                    errorCount++;

                    //  Mark the line
                    int offset = codeEditor.Document.PositionToOffset(new TextLocation(0, lineNumber-1));
                    Marker marker = new Marker(offset, codeEditor.Document.LineSegmentCollection[lineNumber-1].Length, MarkerType.SolidBlock, Color.DarkRed, Color.Black);
                    codeEditor.Document.MarkerStrategy.AddMarker(marker);

                    m_ErrorMarkers.Add(marker);
                }
            }
            Console.WriteLine(errorCount + " Error(s)");

            codeEditor.Refresh(); ;

            if (errorCount > 0)
                statusLabel.Text = errorCount + " Error(s)";
            else
                statusLabel.Text = "Build ok!";

            return errorCount;
        }

        private void compileMenu_Click(object sender, EventArgs e)
        {
            Save();

            Build();
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void runToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (DGenThread.GetDGen().IsDebugging())
            {
                DGenThread.GetDGen().Resume();
            }
            else
            {
                Save();

                if (Build() == 0)
                {
                    m_DebugSource = new DebugSource();
                    m_DebugSource.Init(m_PathToProject + @"\" + m_SourceFileName, m_PathToProject + @"\" + m_ProjectName + ".list", m_PathToProject+@"\");

                    //  Load Rom
                    m_DGenThread.LoadRom(m_PathToProject + @"\"+m_ProjectName+".bin");

                    //  Set breakpoint
                    DGenThread.GetDGen().ClearBreakpoints();
                    foreach (Bookmark mark in codeEditor.Document.BookmarkManager.Marks)
                    {
                        DGenThread.GetDGen().AddBreakpoint(m_DebugSource.GetLineAddress(mark.LineNumber));
                    }

                    //  Start
                    m_DGenThread.Start();
                }
            }
        }

        private void toggleBreakpoint_Click(object sender, EventArgs e)
        {
            int line = codeEditor.ActiveTextAreaControl.Caret.Line;
            codeEditor.Document.BookmarkManager.ToggleMarkAt(new TextLocation(0, line));
            codeEditor.Refresh();
        }

        private void debugToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void stepIntoMenu_Click(object sender, EventArgs e)
        {
            //  if runnnig
            DGenThread.GetDGen().StepInto();

            int currentPC = DGenThread.GetDGen().GetCurrentPC();
            int currentLine = m_DebugSource.GetSourceLine(currentPC) + 1;

            int offset = codeEditor.Document.PositionToOffset(new TextLocation(0, currentLine));

            codeEditor.Document.MarkerStrategy.Clear();
            Marker marker = new Marker(offset, codeEditor.Document.LineSegmentCollection[currentLine].Length, MarkerType.SolidBlock, Color.Yellow, Color.Black);//selection.Offset, selection.Length, MarkerType.SolidBlock, Color.DarkRed, Color.White);
            codeEditor.Document.MarkerStrategy.AddMarker(marker);
            codeEditor.Refresh();

            codeEditor.ActiveTextAreaControl.Caret.Line = m_DebugSource.GetSourceLine(currentPC) + 1;
            codeEditor.ActiveTextAreaControl.Caret.Column = 0;

        }

        private void stepOverMenu_Click(object sender, EventArgs e)
        {

        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_DGenThread.Stop();
            DGenThread.GetDGen().Reset();
            DGenThread.GetDGen().Hide();

            codeEditor.Document.MarkerStrategy.Clear();
            codeEditor.Refresh();
            
            statusLabel.Text = "Stopped";
        }

        private void breakMenu_Click(object sender, EventArgs e)
        {
            DGenThread.GetDGen().Break();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_DGenThread.Stop();
            m_DGenThread.Destroy();

            if (m_Modified)
            {
                if (MessageBox.Show("Save changes?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Save();
                }
            }
        }

        private void saveMenu_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void Save()
        {
            System.IO.File.WriteAllText(m_PathToProject + @"\" + m_SourceFileName, codeEditor.Document.TextContent);

            m_Modified = false;
        }

        public void GoTo(int lineNumber)
        {
            codeEditor.ActiveTextAreaControl.Caret.Line = lineNumber;
            this.Activate();
        }

        public void UpdateViewBuildLog(bool flag)
        {
            viewBuildLogMenu.Checked = flag;
        }

        public void UpdateVDPHelperMenu(bool flag)
        {
            vdpToolsRegistersMenu.Checked = flag;
        }

        private void viewBuildLogMenu_Click(object sender, EventArgs e)
        {
            if (!viewBuildLogMenu.Checked)
                m_BuildLog.Show();
            else
            {
                m_BuildLog.Hide();
            }
        }

        private void configMenu_Click(object sender, EventArgs e)
        {
            ConfigForm configForm = new ConfigForm();

            configForm.StartPosition = FormStartPosition.CenterParent;
            
            configForm.asmPath.Text = m_Config.Asm68kPath;

            if (configForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                m_Config.Asm68kPath = configForm.asmPath.Text;
                m_Config.Save();

                Console.WriteLine(configForm.asmPath.Text);
            }
        }

        private void vDPRegistersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!vdpToolsRegistersMenu.Checked)
            {
                m_VDPRegs.Show();
                vdpToolsRegistersMenu.Checked = true;
            }
            else
            {
                m_VDPRegs.Hide();
                vdpToolsRegistersMenu.Checked = false;
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog pathSelect = new OpenFileDialog();

            pathSelect.Filter = "ASM|*.s;*.asm";

            if (pathSelect.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                m_PathToProject = Path.GetDirectoryName(pathSelect.FileName); // @"D:\Devt\perso_nas\Megadrive\test\";
                
                string source = System.IO.File.ReadAllText(pathSelect.FileName);
                codeEditor.Document.TextContent = source;
                codeEditor.Document.BookmarkManager.Clear();
                undoMenu.Enabled = false;

                m_ProjectName = Path.GetFileNameWithoutExtension(pathSelect.FileName);
                m_SourceFileName = Path.GetFileName(pathSelect.FileName);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void documentChanged(object sender, EventArgs e)
        {
            m_Modified = true;
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
