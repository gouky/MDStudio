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

        enum BreakMode
        {
            kBreakpoint,
            kStepOver
        };

        private readonly Timer m_Timer = new Timer();
        private string m_PathToProject;
        private string m_ProjectName;
        private string m_SourceFileName;
        private string m_CurrentSourcePath;     //  should be removed...
        private List<string> m_ProjectFiles;
        private string m_BuildArgs;

        private DGenThread m_DGenThread;
        private Symbols m_DebugSymbols;

        private MemoryView m_MemoryView;
        private BuildLog m_BuildLog;

        private Config m_Config;

        private VDPRegs m_VDPRegs;

        private bool m_Modified;

        private List<Marker> m_ErrorMarkers;

        private State m_State = State.kStopped;

        private BreakMode m_BreakMode = BreakMode.kBreakpoint;
        private int m_StepOverAddress;

        private const int kAutoScrollThreshold = 20;

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
                //Breakpoint hit, go to address
                int currentPC = DGenThread.GetDGen().GetCurrentPC();
                GoTo((uint)currentPC);

                //Set status
                statusLabel.Text = "PC 0x" + DGenThread.GetDGen().GetCurrentPC();

                //Bring window to front
                BringToFront();

                //Determine break mode
                if(m_BreakMode == BreakMode.kStepOver)
                {
                    //If hit desired step over address
                    if(currentPC == m_StepOverAddress)
                    {
                        //Return to breakpoint mode
                        m_StepOverAddress = 0;
                        m_BreakMode = BreakMode.kBreakpoint;

                        //Clear step over breakpoint
                        //TODO: Add ClearBreakpoint() to DGen, clear all for now
                        m_DGenThread.ClearBreakpoints();
                    }
                }
            }
        }

        List<string> ScanIncludes(string rootPath, string filename)
        {
            List<string> includes = new List<string>();
            List<string> localIncludes = new List<string>();

            using (System.IO.StreamReader file = System.IO.File.OpenText(filename))
            {
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    // all whitespace, followed by 'include', followed by all whitespace, followed by filename in quotes (relative to first assembled file)
                    // e.g. "	include '..\framewk\dmaqueue.asm'"
                    string pattern = "^\\sinclude(\\s+)*[\'\\\"](.+)*[\'\\\"]";
                    Match match = Regex.Match(line, pattern);

                    if (match.Success)
                    {
                        //Convert relative paths to absolute
                        string include = System.IO.Path.GetFullPath(System.IO.Path.Combine(rootPath, match.Groups[2].Value));
                        includes.Add(include);
                        localIncludes.Add(include);
                    }
                }
            }

            //Recurse
            foreach(string include in localIncludes)
            {
                includes.AddRange(ScanIncludes(rootPath, include));
            }

            return includes;
        }

        private void PopulateFileView()
        {
            TreeNode lastNode = null;
            string subPathAgg;

            foreach (string path in m_ProjectFiles)
            {
                subPathAgg = string.Empty;

                foreach (string subPath in path.Split(treeProjectFiles.PathSeparator[0]))
                {
                    subPathAgg += subPathAgg.Length > 0 ? treeProjectFiles.PathSeparator[0] + subPath : subPath;
                    string absPath = System.IO.Path.GetFullPath(subPathAgg);
                    TreeNode[] nodes = treeProjectFiles.Nodes.Find(absPath, true);
                    if (nodes.Length == 0)
                    {
                        if (lastNode == null)
                            lastNode = treeProjectFiles.Nodes.Add(absPath, subPath);
                        else
                            lastNode = lastNode.Nodes.Add(absPath, subPath);
                    }
                    else
                    {
                        lastNode = nodes[0];
                    }
                }

                lastNode = null;
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
                proc.StartInfo.Arguments =  @"/p /c /zd " + m_Config.Asm68kArgs + " " + m_SourceFileName + "," + m_ProjectName + ".bin," + m_ProjectName + ".symb," + m_ProjectName + ".list";
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

                    m_BuildLog.AddError(matchError.Groups[1].Value, lineNumber, matchError.Groups[3].Value);
                    Console.WriteLine("Error in '" + matchError.Groups[1].Value + "' (" + matchError.Groups[2].Value + "): " + matchError.Groups[3].Value);
                    errorCount++;

                    //  Mark the line
                    if(matchError.Groups[1].Value == m_SourceFileName)
                    {
                        int offset = codeEditor.Document.PositionToOffset(new TextLocation(0, lineNumber - 1));
                        Marker marker = new Marker(offset, codeEditor.Document.LineSegmentCollection[lineNumber - 1].Length, MarkerType.SolidBlock, Color.DarkRed, Color.Black);
                        codeEditor.Document.MarkerStrategy.AddMarker(marker);

                        m_ErrorMarkers.Add(marker);
                    }
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
            if (m_State == State.kRunning)
            {
                codeEditor.Document.MarkerStrategy.Clear();
                codeEditor.ActiveTextAreaControl.Invalidate();
                DGenThread.GetDGen().Resume();
                
                //TODO: Bring emu window to front
            }
            else
            {
                Save();

                if (Build() == 0)
                {
                    string initialSourceFile = m_PathToProject + @"\" + m_SourceFileName;
                    string listingFile = m_PathToProject + @"\" + m_ProjectName + ".list";
                    string symbolFile = m_PathToProject + @"\" + m_ProjectName + ".symb";
                    string baseDirectory = m_PathToProject + @"\";
                    string binaryFile = m_PathToProject + @"\" + m_ProjectName + ".bin";

                    //Read symbols
                    m_DebugSymbols = new Symbols();
                    m_DebugSymbols.Read(symbolFile);

                    //  Load Rom
                    m_DGenThread.LoadRom(binaryFile);

                    //  Set breakpoint
                    DGenThread.GetDGen().ClearBreakpoints();
                    foreach (Bookmark mark in codeEditor.Document.BookmarkManager.Marks)
                    {
                        DGenThread.GetDGen().AddBreakpoint((int)m_DebugSymbols.GetAddress(m_CurrentSourcePath, mark.LineNumber + 1));
                    }

                    //  Start
                    m_DGenThread.Start();
                    m_State = State.kRunning;
                }
            }
        }

        private void toggleBreakpoint_Click(object sender, EventArgs e)
        {
            int line = codeEditor.ActiveTextAreaControl.Caret.Line;
            codeEditor.Document.BookmarkManager.ToggleMarkAt(new TextLocation(0, line));
            codeEditor.Refresh();

            //If running, set on running instance
            if(m_State == State.kRunning)
            {
                DGenThread.GetDGen().AddBreakpoint((int)m_DebugSymbols.GetAddress(m_CurrentSourcePath, line + 1));
            }
        }

        private void debugToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void stepIntoMenu_Click(object sender, EventArgs e)
        {
            //Step to next instruction (blocking)
            DGenThread.GetDGen().StepInto();

            //Go to address
            int currentPC = DGenThread.GetDGen().GetCurrentPC();
            GoTo((uint)currentPC);

        }

        private void stepOverMenu_Click(object sender, EventArgs e)
        {
            //Get current address
            int currentPC = DGenThread.GetDGen().GetCurrentPC();
            int nextPC = currentPC;

            //Get current file/line
            Tuple<string,int> currentLine = m_DebugSymbols.GetFileLine((uint)currentPC);
            int nextLine = currentLine.Item2;

            //Get total num lines
            //TODO: Verify current filename in editor matched emulator?
            int fileSizeLines = codeEditor.Document.TotalNumberOfLines;

            //Ignore lines with same address as current
            while (currentPC == nextPC)
            {
                //Get next line
                nextLine++;

                //If next line is in another file, step into instead
                if (nextLine > fileSizeLines)
                {
                    stepIntoMenu_Click(sender, e);
                    return;
                }

                //Get address of next line
                nextPC = (int)m_DebugSymbols.GetAddress(currentLine.Item1, nextLine);
            }

            //Set breakpoint at next address
            DGenThread.GetDGen().AddBreakpoint(nextPC);

            //Set StepOver mode
            m_BreakMode = BreakMode.kStepOver;
            m_StepOverAddress = nextPC;

            //Run to StepOver breakpoint
            DGenThread.GetDGen().Resume();
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_DGenThread.Stop();
            DGenThread.GetDGen().Reset();
            DGenThread.GetDGen().Hide();

            codeEditor.Document.MarkerStrategy.Clear();
            codeEditor.Refresh();
            
            statusLabel.Text = "Stopped";

            m_State = State.kStopped;
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
            System.IO.File.WriteAllText(m_CurrentSourcePath, codeEditor.Document.TextContent);

            m_Modified = false;
        }

        public void GoTo(string filename, int lineNumber)
        {
            if (m_CurrentSourcePath.ToLower() != filename.ToLower())
            {
                string source = System.IO.File.ReadAllText(filename);
                codeEditor.Document.TextContent = source;
                codeEditor.Document.BookmarkManager.Clear();
                m_CurrentSourcePath = filename;
            }
            codeEditor.ActiveTextAreaControl.Caret.Line = lineNumber-1;
            this.Activate();
        }

        public void GoTo(uint address)
        {
            Tuple<string, int> currentLine = m_DebugSymbols.GetFileLine(address);

            string filename = currentLine.Item1;
            int lineNumber = currentLine.Item2 - 1;

            //Load file
            if (m_CurrentSourcePath.ToLower() != filename)
            {
                string source = System.IO.File.ReadAllText(filename);
                codeEditor.Document.TextContent = source;
                codeEditor.Document.BookmarkManager.Clear();
                m_CurrentSourcePath = filename;
            }

            int offset = codeEditor.Document.PositionToOffset(new TextLocation(0, lineNumber));

            codeEditor.Document.MarkerStrategy.Clear();
            Marker marker = new Marker(offset, codeEditor.Document.LineSegmentCollection[lineNumber].Length, MarkerType.SolidBlock, Color.Yellow, Color.Black);//selection.Offset, selection.Length, MarkerType.SolidBlock, Color.DarkRed, Color.White);
            codeEditor.Document.MarkerStrategy.AddMarker(marker);
            codeEditor.ActiveTextAreaControl.Caret.Line = lineNumber;
            codeEditor.ActiveTextAreaControl.Caret.Column = 0;
            codeEditor.ActiveTextAreaControl.CenterViewOn(lineNumber, kAutoScrollThreshold);
            codeEditor.ActiveTextAreaControl.Invalidate();
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
            configForm.asmArgs.Text = m_Config.Asm68kArgs;

            if (configForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                m_Config.Asm68kPath = configForm.asmPath.Text;
                m_Config.Asm68kArgs = configForm.asmArgs.Text;
                m_Config.Save();

                Console.WriteLine(configForm.asmPath.Text);
                Console.WriteLine(configForm.asmArgs.Text);
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

            pathSelect.Filter = "ASM|*.s;*.asm;*.68k";

            if (pathSelect.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                m_PathToProject = Path.GetDirectoryName(pathSelect.FileName); // @"D:\Devt\perso_nas\Megadrive\test\";
                m_CurrentSourcePath = pathSelect.FileName;

                string source = System.IO.File.ReadAllText(pathSelect.FileName);
                codeEditor.Document.TextContent = source;
                codeEditor.Document.BookmarkManager.Clear();
                undoMenu.Enabled = false;

                m_ProjectName = Path.GetFileNameWithoutExtension(pathSelect.FileName);
                m_SourceFileName = Path.GetFileName(pathSelect.FileName);

                m_ProjectFiles = ScanIncludes(m_PathToProject, pathSelect.FileName);
                m_ProjectFiles.Add(m_CurrentSourcePath);
                m_ProjectFiles.Sort();

                PopulateFileView();

                treeProjectFiles.ExpandAll();
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

        private void treeProjectFiles_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if(System.IO.Path.GetExtension(treeProjectFiles.SelectedNode.Name).Length > 0)
            {
                GoTo(treeProjectFiles.SelectedNode.Name, 0);
            }
        }
    }
}
