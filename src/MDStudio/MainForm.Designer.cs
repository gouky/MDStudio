namespace MDStudio
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fooToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undoMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.redoMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.searchFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchSymbolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.searchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchReplaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchNextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchPreviousToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.goToToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.goToAddressToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewBuildLogMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.viewCRAMmenu = new System.Windows.Forms.ToolStripMenuItem();
            this.viewVDPStatusMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.profilerVDPStatusMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.buildToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.compileMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.runMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.runMegaUSB = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toggleBreakpoint = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.debugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stepIntoMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.stepOverMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.stopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.breakMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.vdpToolsRegistersMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.configMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.profilerEnabledMenuOptions = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.codeEditor = new DigitalRune.Windows.TextEditor.TextEditorControl();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.treeProjectFiles = new System.Windows.Forms.TreeView();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.UMDKEnabledMenuOption = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.statusStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.buildToolStripMenuItem,
            this.debugToolStripMenuItem,
            this.toolsToolStripMenuItem1,
            this.optionsToolStripMenuItem,
            this.toolStripMenuItem1});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(860, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.menuStrip1_ItemClicked);
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.saveMenu,
            this.closeToolStripMenuItem,
            this.toolStripSeparator3,
            this.exitToolStripMenuItem,
            this.fooToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.openToolStripMenuItem.Text = "&Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveMenu
            // 
            this.saveMenu.Name = "saveMenu";
            this.saveMenu.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveMenu.Size = new System.Drawing.Size(149, 22);
            this.saveMenu.Text = "&Save";
            this.saveMenu.Click += new System.EventHandler(this.saveMenu_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F4)));
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(146, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.exitToolStripMenuItem.Text = "&Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // fooToolStripMenuItem
            // 
            this.fooToolStripMenuItem.Name = "fooToolStripMenuItem";
            this.fooToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.fooToolStripMenuItem.Text = "Foo";
            this.fooToolStripMenuItem.Click += new System.EventHandler(this.fooToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoMenu,
            this.redoMenu,
            this.toolStripSeparator6,
            this.searchFilesToolStripMenuItem,
            this.searchSymbolsToolStripMenuItem,
            this.toolStripSeparator5,
            this.searchToolStripMenuItem,
            this.searchReplaceToolStripMenuItem,
            this.searchNextToolStripMenuItem,
            this.searchPreviousToolStripMenuItem,
            this.toolStripSeparator7,
            this.goToToolStripMenuItem,
            this.goToAddressToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "&Edit";
            this.editToolStripMenuItem.Click += new System.EventHandler(this.editToolStripMenuItem_Click);
            // 
            // undoMenu
            // 
            this.undoMenu.Name = "undoMenu";
            this.undoMenu.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.undoMenu.Size = new System.Drawing.Size(225, 22);
            this.undoMenu.Text = "&Undo";
            this.undoMenu.Click += new System.EventHandler(this.undoMenu_Click);
            // 
            // redoMenu
            // 
            this.redoMenu.Name = "redoMenu";
            this.redoMenu.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
            this.redoMenu.Size = new System.Drawing.Size(225, 22);
            this.redoMenu.Text = "&Redo";
            this.redoMenu.Click += new System.EventHandler(this.redoToolStripMenuItem_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(222, 6);
            // 
            // searchFilesToolStripMenuItem
            // 
            this.searchFilesToolStripMenuItem.Name = "searchFilesToolStripMenuItem";
            this.searchFilesToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.O)));
            this.searchFilesToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.searchFilesToolStripMenuItem.Text = "Search Files";
            this.searchFilesToolStripMenuItem.Click += new System.EventHandler(this.searchFilesToolStripMenuItem_Click);
            // 
            // searchSymbolsToolStripMenuItem
            // 
            this.searchSymbolsToolStripMenuItem.Name = "searchSymbolsToolStripMenuItem";
            this.searchSymbolsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
            this.searchSymbolsToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.searchSymbolsToolStripMenuItem.Text = "Search Symbols";
            this.searchSymbolsToolStripMenuItem.Click += new System.EventHandler(this.searchSymbolsToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(222, 6);
            // 
            // searchToolStripMenuItem
            // 
            this.searchToolStripMenuItem.Name = "searchToolStripMenuItem";
            this.searchToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.searchToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.searchToolStripMenuItem.Text = "Search";
            this.searchToolStripMenuItem.Click += new System.EventHandler(this.searchToolStripMenuItem_Click);
            // 
            // searchReplaceToolStripMenuItem
            // 
            this.searchReplaceToolStripMenuItem.Name = "searchReplaceToolStripMenuItem";
            this.searchReplaceToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.H)));
            this.searchReplaceToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.searchReplaceToolStripMenuItem.Text = "Search && Replace";
            this.searchReplaceToolStripMenuItem.Click += new System.EventHandler(this.searchReplaceToolStripMenuItem_Click);
            // 
            // searchNextToolStripMenuItem
            // 
            this.searchNextToolStripMenuItem.Name = "searchNextToolStripMenuItem";
            this.searchNextToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F3;
            this.searchNextToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.searchNextToolStripMenuItem.Text = "Search Next";
            this.searchNextToolStripMenuItem.Click += new System.EventHandler(this.searchNextToolStripMenuItem_Click);
            // 
            // searchPreviousToolStripMenuItem
            // 
            this.searchPreviousToolStripMenuItem.Name = "searchPreviousToolStripMenuItem";
            this.searchPreviousToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F3)));
            this.searchPreviousToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.searchPreviousToolStripMenuItem.Text = "Search Previous";
            this.searchPreviousToolStripMenuItem.Click += new System.EventHandler(this.searchPreviousToolStripMenuItem_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(222, 6);
            // 
            // goToToolStripMenuItem
            // 
            this.goToToolStripMenuItem.Name = "goToToolStripMenuItem";
            this.goToToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.G)));
            this.goToToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.goToToolStripMenuItem.Text = "Go To Line";
            this.goToToolStripMenuItem.Click += new System.EventHandler(this.goToToolStripMenuItem_Click);
            // 
            // goToAddressToolStripMenuItem
            // 
            this.goToAddressToolStripMenuItem.Name = "goToAddressToolStripMenuItem";
            this.goToAddressToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.G)));
            this.goToAddressToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.goToAddressToolStripMenuItem.Text = "Go To Address";
            this.goToAddressToolStripMenuItem.Click += new System.EventHandler(this.goToAddressToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewBuildLogMenu,
            this.viewCRAMmenu,
            this.viewVDPStatusMenu,
            this.profilerVDPStatusMenu});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.toolsToolStripMenuItem.Text = "&View";
            this.toolsToolStripMenuItem.Click += new System.EventHandler(this.toolsToolStripMenuItem_Click);
            // 
            // viewBuildLogMenu
            // 
            this.viewBuildLogMenu.Name = "viewBuildLogMenu";
            this.viewBuildLogMenu.Size = new System.Drawing.Size(146, 22);
            this.viewBuildLogMenu.Text = "Build Log";
            this.viewBuildLogMenu.Click += new System.EventHandler(this.viewBuildLogMenu_Click);
            // 
            // viewCRAMmenu
            // 
            this.viewCRAMmenu.Name = "viewCRAMmenu";
            this.viewCRAMmenu.Size = new System.Drawing.Size(146, 22);
            this.viewCRAMmenu.Text = "CRAM Viewer";
            this.viewCRAMmenu.Click += new System.EventHandler(this.viewCRAMBtn_Click);
            // 
            // viewVDPStatusMenu
            // 
            this.viewVDPStatusMenu.Name = "viewVDPStatusMenu";
            this.viewVDPStatusMenu.Size = new System.Drawing.Size(146, 22);
            this.viewVDPStatusMenu.Text = "VDP Status";
            this.viewVDPStatusMenu.Click += new System.EventHandler(this.viewVDPStatusMenu_Click);
            // 
            // profilerVDPStatusMenu
            // 
            this.profilerVDPStatusMenu.Name = "profilerVDPStatusMenu";
            this.profilerVDPStatusMenu.Size = new System.Drawing.Size(146, 22);
            this.profilerVDPStatusMenu.Text = "&Profiler";
            this.profilerVDPStatusMenu.Click += new System.EventHandler(this.profilerToolStripMenuItem_Click);
            // 
            // buildToolStripMenuItem
            // 
            this.buildToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.compileMenu,
            this.runMenu,
            this.runMegaUSB,
            this.toolStripSeparator1,
            this.toggleBreakpoint,
            this.toolStripSeparator4});
            this.buildToolStripMenuItem.Name = "buildToolStripMenuItem";
            this.buildToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.buildToolStripMenuItem.Text = "&Build";
            // 
            // compileMenu
            // 
            this.compileMenu.Name = "compileMenu";
            this.compileMenu.ShortcutKeys = System.Windows.Forms.Keys.F7;
            this.compileMenu.Size = new System.Drawing.Size(218, 22);
            this.compileMenu.Text = "&Compile";
            this.compileMenu.Click += new System.EventHandler(this.compileMenu_Click);
            // 
            // runMenu
            // 
            this.runMenu.Name = "runMenu";
            this.runMenu.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.runMenu.Size = new System.Drawing.Size(218, 22);
            this.runMenu.Text = "&Run";
            this.runMenu.Click += new System.EventHandler(this.runToolStripMenuItem_Click);
            // 
            // runMegaUSB
            // 
            this.runMegaUSB.Name = "runMegaUSB";
            this.runMegaUSB.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F5)));
            this.runMegaUSB.Size = new System.Drawing.Size(218, 22);
            this.runMegaUSB.Text = "Launch Mega-&USB";
            this.runMegaUSB.Click += new System.EventHandler(this.runMegaUSB_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(215, 6);
            // 
            // toggleBreakpoint
            // 
            this.toggleBreakpoint.Name = "toggleBreakpoint";
            this.toggleBreakpoint.ShortcutKeys = System.Windows.Forms.Keys.F9;
            this.toggleBreakpoint.Size = new System.Drawing.Size(218, 22);
            this.toggleBreakpoint.Text = "Toggle Breakpoint";
            this.toggleBreakpoint.Click += new System.EventHandler(this.toggleBreakpoint_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(215, 6);
            // 
            // debugToolStripMenuItem
            // 
            this.debugToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.stepIntoMenu,
            this.stepOverMenu,
            this.toolStripSeparator2,
            this.stopToolStripMenuItem,
            this.breakMenu});
            this.debugToolStripMenuItem.Name = "debugToolStripMenuItem";
            this.debugToolStripMenuItem.Size = new System.Drawing.Size(54, 20);
            this.debugToolStripMenuItem.Text = "&Debug";
            this.debugToolStripMenuItem.Click += new System.EventHandler(this.debugToolStripMenuItem_Click);
            // 
            // stepIntoMenu
            // 
            this.stepIntoMenu.Name = "stepIntoMenu";
            this.stepIntoMenu.ShortcutKeys = System.Windows.Forms.Keys.F11;
            this.stepIntoMenu.Size = new System.Drawing.Size(208, 22);
            this.stepIntoMenu.Text = "Step Into";
            this.stepIntoMenu.Click += new System.EventHandler(this.stepIntoMenu_Click);
            // 
            // stepOverMenu
            // 
            this.stepOverMenu.Name = "stepOverMenu";
            this.stepOverMenu.ShortcutKeys = System.Windows.Forms.Keys.F10;
            this.stepOverMenu.Size = new System.Drawing.Size(208, 22);
            this.stepOverMenu.Text = "Step Over";
            this.stepOverMenu.Click += new System.EventHandler(this.stepOverMenu_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(205, 6);
            // 
            // stopToolStripMenuItem
            // 
            this.stopToolStripMenuItem.Name = "stopToolStripMenuItem";
            this.stopToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F5)));
            this.stopToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.stopToolStripMenuItem.Text = "Stop";
            this.stopToolStripMenuItem.Click += new System.EventHandler(this.stopToolStripMenuItem_Click);
            // 
            // breakMenu
            // 
            this.breakMenu.Name = "breakMenu";
            this.breakMenu.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt) 
            | System.Windows.Forms.Keys.Pause)));
            this.breakMenu.Size = new System.Drawing.Size(208, 22);
            this.breakMenu.Text = "Break All";
            this.breakMenu.Click += new System.EventHandler(this.breakMenu_Click);
            // 
            // toolsToolStripMenuItem1
            // 
            this.toolsToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.vdpToolsRegistersMenu});
            this.toolsToolStripMenuItem1.Name = "toolsToolStripMenuItem1";
            this.toolsToolStripMenuItem1.Size = new System.Drawing.Size(48, 20);
            this.toolsToolStripMenuItem1.Text = "&Tools";
            // 
            // vdpToolsRegistersMenu
            // 
            this.vdpToolsRegistersMenu.Name = "vdpToolsRegistersMenu";
            this.vdpToolsRegistersMenu.Size = new System.Drawing.Size(146, 22);
            this.vdpToolsRegistersMenu.Text = "VDP Registers";
            this.vdpToolsRegistersMenu.Click += new System.EventHandler(this.vDPRegistersToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.configMenu,
            this.profilerEnabledMenuOptions,
            this.UMDKEnabledMenuOption});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "&Options";
            // 
            // configMenu
            // 
            this.configMenu.Name = "configMenu";
            this.configMenu.Size = new System.Drawing.Size(198, 22);
            this.configMenu.Text = "&Config";
            this.configMenu.Click += new System.EventHandler(this.configMenu_Click);
            // 
            // profilerEnabledMenuOptions
            // 
            this.profilerEnabledMenuOptions.CheckOnClick = true;
            this.profilerEnabledMenuOptions.Name = "profilerEnabledMenuOptions";
            this.profilerEnabledMenuOptions.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.profilerEnabledMenuOptions.Size = new System.Drawing.Size(198, 22);
            this.profilerEnabledMenuOptions.Text = "&Profiler Enabled";
            this.profilerEnabledMenuOptions.Click += new System.EventHandler(this.profileToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(24, 20);
            this.toolStripMenuItem1.Text = "&?";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.aboutToolStripMenuItem.Text = "&About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(860, 25);
            this.toolStrip1.TabIndex = 4;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton1.Text = "toolStripButton1";
            // 
            // codeEditor
            // 
            this.codeEditor.AutoScroll = true;
            this.codeEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.codeEditor.IsIconBarVisible = true;
            this.codeEditor.Location = new System.Drawing.Point(0, 0);
            this.codeEditor.Name = "codeEditor";
            this.codeEditor.Size = new System.Drawing.Size(701, 420);
            this.codeEditor.TabIndex = 5;
            this.codeEditor.DocumentChanged += new System.EventHandler<DigitalRune.Windows.TextEditor.Document.DocumentEventArgs>(this.codeEditor_DocumentChanged);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 469);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(860, 22);
            this.statusStrip.TabIndex = 6;
            this.statusStrip.Text = "statusStrip1";
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(57, 17);
            this.statusLabel.Text = "Welcome";
            this.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // treeProjectFiles
            // 
            this.treeProjectFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeProjectFiles.Location = new System.Drawing.Point(0, 0);
            this.treeProjectFiles.Name = "treeProjectFiles";
            this.treeProjectFiles.Size = new System.Drawing.Size(155, 420);
            this.treeProjectFiles.TabIndex = 7;
            this.treeProjectFiles.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeProjectFiles_AfterSelect);
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 49);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.treeProjectFiles);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.codeEditor);
            this.splitContainer.Size = new System.Drawing.Size(860, 420);
            this.splitContainer.SplitterDistance = 155;
            this.splitContainer.TabIndex = 8;
            // 
            // UMDKEnabledMenuOption
            // 
            this.UMDKEnabledMenuOption.CheckOnClick = true;
            this.UMDKEnabledMenuOption.Name = "UMDKEnabledMenuOption";
            this.UMDKEnabledMenuOption.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.U)));
            this.UMDKEnabledMenuOption.Size = new System.Drawing.Size(198, 22);
            this.UMDKEnabledMenuOption.Text = "UMDK Enabled";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(860, 491);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "MDStudio";
            this.Activated += new System.EventHandler(this.MainForm_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Enter += new System.EventHandler(this.MainForm_Enter);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem configMenu;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private DigitalRune.Windows.TextEditor.TextEditorControl codeEditor;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem undoMenu;
        private System.Windows.Forms.ToolStripMenuItem redoMenu;
        private System.Windows.Forms.ToolStripMenuItem buildToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem compileMenu;
        private System.Windows.Forms.ToolStripMenuItem runMenu;
        private System.Windows.Forms.ToolStripMenuItem saveMenu;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem toggleBreakpoint;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.ToolStripMenuItem debugToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stepIntoMenu;
        private System.Windows.Forms.ToolStripMenuItem stepOverMenu;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem stopToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem breakMenu;
        private System.Windows.Forms.ToolStripMenuItem viewBuildLogMenu;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem vdpToolsRegistersMenu;
        private System.Windows.Forms.TreeView treeProjectFiles;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem searchFilesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem searchSymbolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem runMegaUSB;
        private System.Windows.Forms.ToolStripMenuItem viewCRAMmenu;
        private System.Windows.Forms.ToolStripMenuItem viewVDPStatusMenu;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem searchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem searchNextToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem searchPreviousToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem profilerEnabledMenuOptions;
        private System.Windows.Forms.ToolStripMenuItem profilerVDPStatusMenu;
        private System.Windows.Forms.ToolStripMenuItem searchReplaceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripMenuItem goToToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem goToAddressToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fooToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem UMDKEnabledMenuOption;
    }
}

