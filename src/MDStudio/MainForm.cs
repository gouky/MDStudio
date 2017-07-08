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
using System.Diagnostics;
using DGenInterface;
using DigitalRune.Windows.TextEditor.Document;
using DigitalRune.Windows.TextEditor.Bookmarks;
using DigitalRune.Windows.TextEditor.Highlighting;
using DigitalRune.Windows.TextEditor.Markers;
using System.Text.RegularExpressions;
using System.IO;
using MDStudio.Properties;

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
        private string m_ProjectFile;
        private string m_PathToProject;
        private string m_ProjectName;
        private string m_SourceFileName;
        private string m_CurrentSourcePath;     //  should be removed...
        private List<string> m_ProjectFiles;
        private string m_BuildArgs;

        private DGenThread m_DGenThread;
        public Symbols m_DebugSymbols;

        private RegisterView m_RegisterView;
        private MemoryView m_MemoryView;
        private BuildLog m_BuildLog;
        private CRamViewer m_CRAMViewer;
        private VDPStatusWindow m_VDPStatus;
        private ProfilerView m_ProfilerView;

        private Config m_Config;

        private VDPRegs m_VDPRegs;

        private bool m_Modified;

        private List<Marker> m_ErrorMarkers;

        private List<Marker> m_SearchMarkers;
        private List<TextLocation> m_SearchResults;
        private string m_ReplaceString;
        private int m_SearchIndex;
        
        private State m_State = State.kStopped;

        private FileSystemWatcher m_SourceWatcher;

        private BreakMode m_BreakMode = BreakMode.kBreakpoint;
        private int m_StepOverAddress;

        public class ProfilerEntry
        {
            public uint address { get; set; }
            public uint hitCount { get; set; }
            public uint cyclesPerHit { get; set; }
            public uint totalCycles { get; set; }
            public float percentCost { get; set; }
            public string filename { get; set; }
            public int line { get; set; }
        };

        private List<ProfilerEntry> m_ProfileResults;
        private bool m_Profile;

        public static readonly ReadOnlyCollection<Tuple<int, int>> kValidResolutions = new ReadOnlyCollection<Tuple<int, int>>(new[]
        {
            new Tuple<int,int>( 320, 240 ),
            new Tuple<int,int>( 640, 480 ),
            new Tuple<int,int>( 960, 720 )
        });

        //Default config
        public const int kDefaultResolutionEntry = 1;

        private static readonly ReadOnlyCollection<string> kStepIntoInstrs = new ReadOnlyCollection<string>(new[]
        {
            "RTS",
            "JMP",
            "BRA",
            "BCC",
            "BCS",
            "BEQ",
            "BGE",
            "BGT",
            "BHI",
            "BLE",
            "BLS",
            "BLT",
            "BMI",
            "BNE",
            "BPL",
            "BVC",
            "BVS",
            "DBCC",
            "DBCS",
            "DBEQ",
            "DBGE",
            "DBGT",
            "DBHI",
            "DBLE",
            "DBLS",
            "DBLT",
            "DBMI",
            "DBNE",
            "DBPL",
            "DBVC",
            "DBVS",
        });

        public MainForm()
        {
            InitializeComponent();
            
            m_ErrorMarkers = new List<Marker>();
            m_SearchMarkers= new List<Marker>();
            m_SearchResults = new List<TextLocation>();

            //
            m_Config = new Config();
            m_Config.Read();

            m_VDPRegs = new VDPRegs(this);

            //
            m_BuildLog = new BuildLog(this);
            m_BuildLog.Hide();

            //Show profiler results
            m_ProfilerView = new ProfilerView(this);
            m_ProfilerView.Hide();

            //
            m_RegisterView = new RegisterView();
            m_RegisterView.Hide();

            m_CRAMViewer = new CRamViewer(this);
            if(Settings.Default.CRAMWindowVisible)
                m_CRAMViewer.Show();
            else
                m_CRAMViewer.Hide();

            m_VDPStatus = new VDPStatusWindow(this);
            if (Settings.Default.VDPStatusWindowVisible)
                m_VDPStatus.Show();
            else
                m_VDPStatus.Hide();

            m_SourceWatcher = new FileSystemWatcher();
            m_SourceWatcher.EnableRaisingEvents = false;

            // Set the syntax-highlighting for C#
            codeEditor.Document.HighlightingStrategy = HighlightingManager.Manager.FindHighlighter("ASM68k");

            m_DGenThread = new DGenThread();

            m_Timer.Interval = 16;
            m_Timer.Tick += TimerTick;
            m_Timer.Enabled = true;

            if(m_Config.AutoOpenLastProject)
            {
                //Open last project
                OpenProject(m_Config.LastProject);
            }

            StopDebugging(); 
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void StartDebugging()
        {
            stepIntoMenu.Enabled = true;
            stepOverMenu.Enabled = true;
            stopToolStripMenuItem.Enabled = true;
            breakMenu.Enabled = true;
        }

        private void StopDebugging()
        {
            stepIntoMenu.Enabled = false;
            stepOverMenu.Enabled = false;
            stopToolStripMenuItem.Enabled = false;
            breakMenu.Enabled = false;
        }

        private void UpdateCRAM()
        {
            // Update palette
            if (m_CRAMViewer.Visible)
            {
                for (int i = 0; i < 64; i++)
                {
                    uint rgb = (uint)DGenThread.GetDGen().GetColor(i);
                    m_CRAMViewer.SetColor(i, rgb);
                }
            }
        }

        void TimerTick(object sender, EventArgs e)
        {
            if (DGenThread.GetDGen() != null && DGenThread.GetDGen().IsDebugging() && m_State == State.kRunning)
            {
                //Breakpoint hit, go to address
                int currentPC = DGenThread.GetDGen().GetCurrentPC();
                GoTo((uint)currentPC);

                //Get regs
                uint[] dregs = new uint[8];
                for (int i = 0; i < 8; i++)
                {
                    dregs[i] = (uint)DGenThread.GetDGen().GetDReg(i);
                }

                uint[] aregs = new uint[7];
                for (int i = 0; i < 7; i++)
                {
                    aregs[i] = (uint)DGenThread.GetDGen().GetAReg(i);
                }

                uint sr = (uint)DGenThread.GetDGen().GetSR();

                m_RegisterView.SetRegs(dregs[0], dregs[1], dregs[2], dregs[3], dregs[4], dregs[5], dregs[6], dregs[7],
                                        aregs[0], aregs[1], aregs[2], aregs[3], aregs[4], aregs[5], aregs[6], 0,
                                        sr, (uint)currentPC);

                //Set status
                statusLabel.Text = "PC 0x" + DGenThread.GetDGen().GetCurrentPC();

                //Bring window to front
                BringToFront();

                UpdateCRAM();
                m_VDPStatus.UpdateView();

                //Determine break mode
                if (m_BreakMode == BreakMode.kStepOver)
                {
                    //If hit desired step over address
                    if (currentPC == m_StepOverAddress)
                    {
                        //Return to breakpoint mode
                        m_StepOverAddress = 0;
                        m_BreakMode = BreakMode.kBreakpoint;

                        //Clear step over breakpoint
                        //TODO: Add ClearBreakpoint() to DGen, clear all for now
                        m_DGenThread.ClearBreakpoints();
                    }
                }

                //In breakpoint state
                m_State = State.kDebugging;
            }
            else if (DGenThread.GetDGen() != null && m_State == State.kRunning)
            {
                UpdateCRAM();
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

        void p_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            //HANDLE STDERR
            if (e.Data != null && !e.Data.Equals(""))
            {
                if (!e.Data.Contains("Something"))
                {
                }
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

            StringBuilder processOutput = new StringBuilder();

            try
            {
                // For some reason we don't get the standardoutput for asm68k when running in release
                // using standarderror seems to work but standardoutput needs to be enabled as well for some reason however it's not working
                // with the debugger dettached ¯\_(ツ)_/¯
                Process proc = new Process();

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
                    processOutput.Append(proc.StandardError.ReadToEnd());
                }
                processOutput.Append(proc.StandardError.ReadToEnd());
                proc.WaitForExit();
            }
            catch (Exception e)
            {
                Console.WriteLine("build exception: " + e.Message);
            }

            //()\((\d+)\)\s: Error : (.+)
            string[] output = processOutput.ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

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

                    string filename = matchError.Groups[1].Value;

                    m_BuildLog.AddError(filename, lineNumber, matchError.Groups[3].Value);
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
            {
                statusLabel.Text = errorCount + " Error(s)";
            }   
            else
            {
                statusLabel.Text = "Build ok!";

                //Success, read symbols
                string symbolFile = m_PathToProject + @"\" + m_ProjectName + ".symb";
                m_DebugSymbols = new Symbols();
                m_DebugSymbols.Read(symbolFile);
            }

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
            if (m_State == State.kDebugging)
            {
                codeEditor.Document.MarkerStrategy.Clear();
                codeEditor.ActiveTextAreaControl.Invalidate();
                codeEditor.Refresh();

                DGenThread.GetDGen().Resume();

                statusLabel.Text = "Running...";
                //TODO: Bring emu window to front

                m_State = State.kRunning;
            }
            else if(m_State == State.kStopped)
            {
                Save();

                if (Build() == 0)
                {
                    string initialSourceFile = m_PathToProject + @"\" + m_SourceFileName;
                    string listingFile = m_PathToProject + @"\" + m_ProjectName + ".list";
                    string symbolFile = m_PathToProject + @"\" + m_ProjectName + ".symb";
                    string baseDirectory = m_PathToProject + @"\";
                    string binaryFile = m_PathToProject + @"\" + m_ProjectName + ".bin";

                    //  Show tools windows first, so emu window gets foreground focus
                    m_RegisterView.Show();

                    //Init emu
                    Tuple<int, int> resolution = kValidResolutions[m_Config.EmuResolution];
                    m_DGenThread.Init(resolution.Item1, resolution.Item2, this.Handle);
                    
                    //Set input mappings
                    DGenThread.GetDGen().SetInputMapping(DGen.SDLInputs.eInputUp, (int)Enum.GetValues(typeof(SDL_Keycode.Keycode)).GetValue(m_Config.KeycodeUp));
                    DGenThread.GetDGen().SetInputMapping(DGen.SDLInputs.eInputDown, (int)Enum.GetValues(typeof(SDL_Keycode.Keycode)).GetValue(m_Config.KeycodeDown));
                    DGenThread.GetDGen().SetInputMapping(DGen.SDLInputs.eInputLeft, (int)Enum.GetValues(typeof(SDL_Keycode.Keycode)).GetValue(m_Config.KeycodeLeft));
                    DGenThread.GetDGen().SetInputMapping(DGen.SDLInputs.eInputRight, (int)Enum.GetValues(typeof(SDL_Keycode.Keycode)).GetValue(m_Config.KeycodeRight));
                    DGenThread.GetDGen().SetInputMapping(DGen.SDLInputs.eInputA, (int)Enum.GetValues(typeof(SDL_Keycode.Keycode)).GetValue(m_Config.KeycodeA));
                    DGenThread.GetDGen().SetInputMapping(DGen.SDLInputs.eInputB, (int)Enum.GetValues(typeof(SDL_Keycode.Keycode)).GetValue(m_Config.KeycodeB));
                    DGenThread.GetDGen().SetInputMapping(DGen.SDLInputs.eInputC, (int)Enum.GetValues(typeof(SDL_Keycode.Keycode)).GetValue(m_Config.KeycodeC));
                    DGenThread.GetDGen().SetInputMapping(DGen.SDLInputs.eInputStart, (int)Enum.GetValues(typeof(SDL_Keycode.Keycode)).GetValue(m_Config.KeycodeStart));

                    //Read symbols
                    m_DebugSymbols = new Symbols();
                    m_DebugSymbols.Read(symbolFile);

                    //  Load Rom
                    m_DGenThread.LoadRom(binaryFile);

                    // Reset the vdp status
                    m_VDPStatus.Reset();

                    //  Set breakpoint
                    DGenThread.GetDGen().ClearBreakpoints();
                    foreach (Bookmark mark in codeEditor.Document.BookmarkManager.Marks)
                    {
                        DGenThread.GetDGen().AddBreakpoint((int)m_DebugSymbols.GetAddress(m_CurrentSourcePath, mark.LineNumber + 1));
                    }

                    //  Start
                    m_DGenThread.Start();
                    m_State = State.kRunning;

                    statusLabel.Text = "Running...";

                    codeEditor.Document.ReadOnly = true;

                    //  Hide the build window
                    m_BuildLog.Hide();

                    //  profiling?
                    m_Profile = profilerEnabledMenuOptions.Checked;

                    StartDebugging();
                }
            }
        }

        private void toggleBreakpoint_Click(object sender, EventArgs e)
        {
            int line = codeEditor.ActiveTextAreaControl.Caret.Line;
            codeEditor.Document.BookmarkManager.ToggleMarkAt(new TextLocation(0, line));
            codeEditor.Refresh();

            //If running, set on running instance
            if(m_State == State.kRunning || m_State == State.kDebugging)
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

            //Re-evaluate on next timer tick
            m_State = State.kRunning;
        }

        private void stepOverMenu_Click(object sender, EventArgs e)
        {
            //Get current address
            int currentPC = DGenThread.GetDGen().GetCurrentPC();
            int nextPC = currentPC;

            //Get current file/line
            Tuple<string,int> currentLine = m_DebugSymbols.GetFileLine((uint)currentPC);
            int nextLine = currentLine.Item2;

            //Determine if current instruction should be stepped into
            //TODO: Add instruction peek to DGen, determine by opcode
            string currentLineText = codeEditor.Document.GetText(codeEditor.Document.GetLineSegment(currentLine.Item2 - 1));
            Match match = Regex.Match(currentLineText, "\\s*?([a-zA-Z.]+)");
            if(match.Success)
            {
                string opcode = match.Groups[1].ToString().ToUpper();
                if (kStepIntoInstrs.Contains(opcode))
                {
                    stepIntoMenu_Click(sender, e);
                    return;
                }
            }

            //Get total num lines
            //TODO: Verify current filename in editor matches emulator?
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
            m_State = State.kRunning;
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(m_State != State.kStopped)
            {
                if (m_Profile)
                {
                    uint totalCycles = 0;

                    unsafe
                    {
                        int numInstructions = 0;
                        uint* profileResults = DGenThread.GetDGen().GetProfilerResults(&numInstructions);

                        m_ProfileResults = new List<ProfilerEntry>();

                        for (int i = 0; i < numInstructions; i++)
                        {
                            if (profileResults[i] > 0)
                            {
                                ProfilerEntry entry = new ProfilerEntry();

                                entry.address = (uint)i * sizeof(short);
                                entry.hitCount = profileResults[i];
                                Tuple<string, int> line = m_DebugSymbols.GetFileLine(entry.address);
                                entry.cyclesPerHit = DGenThread.GetDGen().GetInstructionCycleCount(entry.address);
                                entry.totalCycles = entry.cyclesPerHit * entry.hitCount;
                                entry.filename = line.Item1;
                                entry.line = line.Item2;

                                m_ProfileResults.Add(entry);

                                totalCycles += entry.totalCycles;
                            }
                        }
                    }

                    if (m_ProfileResults.Count > 0)
                    {
                        //Calcuate percentage cost
                        foreach (var entry in m_ProfileResults)
                        {
                            entry.percentCost = ((float)entry.totalCycles / (float)totalCycles);
                        }

                        //Sort by hit count
                        m_ProfileResults.Sort((a, b) => (int)(b.totalCycles - a.totalCycles));

                        m_ProfilerView.SetResults(m_ProfileResults);
                        m_ProfilerView.Show();
                    }
                }

                m_DGenThread.Stop();
                m_DGenThread.Destroy();

                m_RegisterView.Hide();

                codeEditor.Document.MarkerStrategy.Clear();
                codeEditor.Refresh();

                statusLabel.Text = "Stopped";

                m_State = State.kStopped;
                codeEditor.Document.ReadOnly = false;

                StopDebugging();
            }
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

            Settings.Default.WindowState = this.WindowState;

            if (this.WindowState == FormWindowState.Normal)
            {
                Settings.Default.WindowLocation = this.Location;
                Settings.Default.WindowSize = this.Size;
            }
            else
            {
                Settings.Default.WindowLocation = this.RestoreBounds.Location;
                Settings.Default.WindowSize = this.RestoreBounds.Size;
            }

            Settings.Default.CRAMWindowVisible = m_CRAMViewer.Visible;
            Settings.Default.VDPStatusWindowVisible = m_VDPStatus.Visible;
            Settings.Default.ProfilerEnabled = profilerEnabledMenuOptions.Checked;

            Settings.Default.Save();
        }

        private void saveMenu_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void Save()
        {
            m_SourceWatcher.EnableRaisingEvents = false;

            System.IO.File.WriteAllText(m_CurrentSourcePath, codeEditor.Document.TextContent);

            m_SourceWatcher.EnableRaisingEvents = true;

            m_Modified = false;
            UpdateTitle();
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
            if (m_CurrentSourcePath.ToLower() != filename.ToLower())
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
            codeEditor.ActiveTextAreaControl.CenterViewOn(lineNumber, -1);
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

        public void UpdateViewCRAM(bool flag)
        {
            viewCRAMmenu.Checked = flag;
        }

        public void UpdateViewVDPStatus(bool flag)
        {
            viewVDPStatusMenu.Checked = flag;
        }

        public void UpdateViewProfiler(bool flag)
        {
            viewVDPStatusMenu.Checked = flag;
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
            configForm.emuResolution.SelectedIndex = m_Config.EmuResolution;
            configForm.autoOpenLastProject.Checked = m_Config.AutoOpenLastProject;

            configForm.inputUp.SelectedIndex = m_Config.KeycodeUp;
            configForm.inputDown.SelectedIndex = m_Config.KeycodeDown;
            configForm.inputLeft.SelectedIndex = m_Config.KeycodeLeft;
            configForm.inputRight.SelectedIndex = m_Config.KeycodeRight;
            configForm.inputA.SelectedIndex = m_Config.KeycodeA;
            configForm.inputB.SelectedIndex = m_Config.KeycodeB;
            configForm.inputC.SelectedIndex = m_Config.KeycodeC;
            configForm.inputStart.SelectedIndex = m_Config.KeycodeStart;

            configForm.megaUSBPath.Text = m_Config.MegaUSBPath;

            if (configForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                m_Config.Asm68kPath = configForm.asmPath.Text;
                m_Config.Asm68kArgs = configForm.asmArgs.Text;
                m_Config.EmuResolution = configForm.emuResolution.SelectedIndex;
                m_Config.AutoOpenLastProject = configForm.autoOpenLastProject.Checked;
                m_Config.LastProject = m_ProjectFile;

                m_Config.KeycodeUp = configForm.inputUp.SelectedIndex;
                m_Config.KeycodeDown = configForm.inputDown.SelectedIndex;
                m_Config.KeycodeLeft = configForm.inputLeft.SelectedIndex;
                m_Config.KeycodeRight = configForm.inputRight.SelectedIndex;
                m_Config.KeycodeA = configForm.inputA.SelectedIndex;
                m_Config.KeycodeB = configForm.inputB.SelectedIndex;
                m_Config.KeycodeC = configForm.inputC.SelectedIndex;
                m_Config.KeycodeStart = configForm.inputStart.SelectedIndex;

                m_Config.MegaUSBPath = configForm.megaUSBPath.Text;

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

        private void OpenProject(string filename)
        {
            if(System.IO.File.Exists(filename))
            {
                m_SourceWatcher.EnableRaisingEvents = false;

                // Remove events
                codeEditor.Document.DocumentChanged -= documentChanged;
                m_Modified = false;

                m_ProjectFile = filename;
                m_PathToProject = Path.GetDirectoryName(filename); // @"D:\Devt\perso_nas\Megadrive\test\";
                m_CurrentSourcePath = filename;

                string source = System.IO.File.ReadAllText(filename);
                codeEditor.Document.TextContent = source;
                codeEditor.Document.BookmarkManager.Clear();
                undoMenu.Enabled = false;

                m_ProjectName = Path.GetFileNameWithoutExtension(filename);
                m_SourceFileName = Path.GetFileName(filename);

                m_ProjectFiles = ScanIncludes(m_PathToProject, filename);
                m_ProjectFiles.Add(m_CurrentSourcePath);
                m_ProjectFiles.Sort();

                PopulateFileView();

                treeProjectFiles.ExpandAll();

                // Set events
                codeEditor.Document.DocumentChanged += documentChanged;

                this.Text = "MDStudio - " + m_CurrentSourcePath;

                //  Set watcher
                m_SourceWatcher.Path = Path.GetDirectoryName(m_CurrentSourcePath);
                m_SourceWatcher.NotifyFilter = NotifyFilters.LastWrite;
                m_SourceWatcher.Filter = Path.GetFileName(m_CurrentSourcePath);
                m_SourceWatcher.Changed += new FileSystemEventHandler(OnSourceChanged);
                m_SourceWatcher.EnableRaisingEvents = true;
            }
        }

        private void OnSourceChanged(object source, FileSystemEventArgs e)
        {
            this.Invoke(new Action(() =>
            {
                m_SourceWatcher.EnableRaisingEvents = false;

                DialogResult dialogResult = MessageBox.Show(this, m_CurrentSourcePath + Environment.NewLine + Environment.NewLine + "This file has been modified by an another program." + Environment.NewLine + Environment.NewLine + "Do you want to reload it?", "Reload", MessageBoxButtons.YesNo);

                if (dialogResult == DialogResult.Yes)
                {
                    codeEditor.Document.TextContent = System.IO.File.ReadAllText(m_CurrentSourcePath);

                }

                m_SourceWatcher.EnableRaisingEvents = true;
            }));
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog pathSelect = new OpenFileDialog();

            pathSelect.Filter = "ASM|*.s;*.asm;*.68k";

            if (pathSelect.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                OpenProject(pathSelect.FileName);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        public void UpdateTitle()
        {
            if (m_CurrentSourcePath != null && m_CurrentSourcePath.Length > 0)
            {
                if (m_Modified)
                    this.Text = "MDStudio - " + m_CurrentSourcePath + "*";
                else
                    this.Text = "MDStudio - " + m_CurrentSourcePath;
            }
            else
            {
                this.Text = "MDStudio";
            }
        }
        private void documentChanged(object sender, EventArgs e)
        {
            m_Modified = true;

            UpdateTitle();

            ClearSearch();
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

        private void searchSymbolsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SymbolView dialog = new SymbolView(this, m_DebugSymbols);
            dialog.ShowDialog(this);
        }

        IEnumerable<TreeNode> Collect(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                yield return node;

                foreach (var child in Collect(node.Nodes))
                    yield return child;
            }
        }

        private void searchFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<string> filenames = new List<string>();
            foreach(TreeNode node in Collect(treeProjectFiles.Nodes))
            {
                if (System.IO.Path.GetExtension(node.Name).Length > 0)
                {
                    filenames.Add(node.Name);
                }
            }

            FileView dialog = new MDStudio.FileView(this, filenames);
            dialog.ShowDialog(this);
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            m_Config.LastProject = m_ProjectFile;
            m_Config.Save();

            
        }

        private void runMegaUSB_Click(object sender, EventArgs e)
        {
            string binaryFile = m_PathToProject + @"\" + m_ProjectName + ".bin";

            if(File.Exists(binaryFile) && m_Config.MegaUSBPath != null && File.Exists(m_Config.MegaUSBPath))
            {
                try
                {
                    System.Diagnostics.Process proc = new System.Diagnostics.Process();
                    proc.StartInfo.FileName = m_Config.MegaUSBPath;
                    proc.StartInfo.WorkingDirectory = m_PathToProject + @"\";
                    proc.StartInfo.Arguments = binaryFile;
                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.RedirectStandardOutput = true;
                    proc.StartInfo.RedirectStandardError = true;
                    proc.StartInfo.CreateNoWindow = true;
                    proc.Start();
                    proc.WaitForExit();
                }
                catch
                {
                }
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if(Settings.Default.WindowLocation != null)
            {
                this.Location = Settings.Default.WindowLocation;
            }

            if(Settings.Default.WindowSize != null)
            {
                this.Size = Settings.Default.WindowSize;
            }

            if(Settings.Default.WindowState != null)
            {
                this.WindowState = Settings.Default.WindowState;
                if (this.WindowState == FormWindowState.Minimized)
                    this.WindowState = FormWindowState.Normal;
            }

            profilerEnabledMenuOptions.Checked = Settings.Default.ProfilerEnabled;
        }

        private void MainForm_Activated(object sender, EventArgs e)
        {

        }

        private void MainForm_Enter(object sender, EventArgs e)
        {
            if (DGenThread.GetDGen() != null && m_State != State.kStopped)
            {
                DGenThread.GetDGen().BringToFront();
            }
        }

        private void toolsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void viewCRAMBtn_Click(object sender, EventArgs e)
        {
            if (!viewCRAMmenu.Checked)
                m_CRAMViewer.Show();
            else
                m_CRAMViewer.Hide();
        }

        private void viewVDPStatusMenu_Click(object sender, EventArgs e)
        {
            if (!viewVDPStatusMenu.Checked)
                m_VDPStatus.Show();
            else
                m_VDPStatus.Hide();
        }

        private void ClearSearch()
        {
            bool requestUpdate = m_SearchMarkers.Count>0;

            foreach (Marker marker in m_SearchMarkers)
            {
                codeEditor.Document.MarkerStrategy.RemoveMarker(marker);
            }

            m_SearchMarkers.Clear();
            m_SearchResults.Clear();
            m_SearchIndex = -1;

            if(requestUpdate)
            {
                codeEditor.Refresh();
            }
        }

        private void searchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SearchForm search = new SearchForm();

            if(search.ShowDialog() == DialogResult.OK)
            {
                bool firstFind = true;

                ClearSearch();

                if (search.searchString.Text.Length > 0)
                {
                    Regex rx = new Regex(search.checkMatchCase.Checked ? search.searchString.Text : "(?i)" + search.searchString.Text);
                    foreach (Match match in rx.Matches(codeEditor.Document.TextContent))
                    {
                        TextLocation matchLocation = codeEditor.Document.OffsetToPosition(match.Index);

                        Marker marker = new Marker(match.Index, match.Length, MarkerType.SolidBlock, Color.Orange, Color.Black);
                        codeEditor.Document.MarkerStrategy.AddMarker(marker);

                        m_SearchMarkers.Add(marker);
                        m_SearchResults.Add(matchLocation);

                        if (firstFind)
                        {
                            m_SearchIndex++;
                            if (matchLocation.Line >= codeEditor.ActiveTextAreaControl.Caret.Line)
                            {
                                codeEditor.ActiveTextAreaControl.Caret.Position = matchLocation;
                                codeEditor.ActiveTextAreaControl.CenterViewOn(matchLocation.Line, -1);
                                firstFind = false;
                            }
                        }
                    }
                    Console.WriteLine("search: " + search.searchString);
                }
            }
        }

        private void searchNextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(m_SearchResults.Count>0 && (m_SearchIndex+1)< m_SearchResults.Count)
            {
                m_SearchIndex++;

                codeEditor.ActiveTextAreaControl.Caret.Position = m_SearchResults[m_SearchIndex];
                codeEditor.ActiveTextAreaControl.CenterViewOn(m_SearchResults[m_SearchIndex].Line, -1);
            }
        }

        private void searchPreviousToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (m_SearchResults.Count > 0 && (m_SearchIndex - 1) >= 0)
            {
                m_SearchIndex--;

                codeEditor.ActiveTextAreaControl.Caret.Position = m_SearchResults[m_SearchIndex];
                codeEditor.ActiveTextAreaControl.CenterViewOn(m_SearchResults[m_SearchIndex].Line, -1);
            }
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
        }

        private void profileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void profilerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(viewVDPStatusMenu.Checked)
                m_ProfilerView.Show();
            else
                m_ProfilerView.Hide();
        }

        private void searchReplaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SearchForm search = new SearchForm(true);

            if (search.ShowDialog() == DialogResult.OK)
            {
                ClearSearch();

                m_ReplaceString = search.replaceString.Text;

                if (search.searchString.Text.Length > 0)
                {
                    Regex rx = new Regex(search.checkMatchCase.Checked ? search.searchString.Text : "(?i)" + search.searchString.Text);

                    //  should do one by one ideally
                    codeEditor.Document.TextContent = rx.Replace(codeEditor.Document.TextContent, m_ReplaceString);
                }
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutForm about = new AboutForm();

            about.ShowDialog();
        }

        public void OnKeyDown(int vkCode)
        {
        }

        public void OnKeyUp(int vkCode)
        {

        }

        private void goToToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GoToForm gotoForm = new GoToForm();

            if(gotoForm.ShowDialog() == DialogResult.OK)
            {
                int lineNumber;

                if(int.TryParse(gotoForm.textLineNumber.Text, out lineNumber))
                {
                    codeEditor.ActiveTextAreaControl.Caret.Line = lineNumber;
                }
            }
        }
    }
}
