namespace MDStudio
{
    partial class ConfigForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.asmPath = new System.Windows.Forms.TextBox();
            this.okBtn = new System.Windows.Forms.Button();
            this.pathButton = new System.Windows.Forms.Button();
            this.cancelBtn = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.asmArgs = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.emuResolution = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.inputStart = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.inputC = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.inputB = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.inputA = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.inputRight = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.inputLeft = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.inputDown = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.inputUp = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.autoOpenLastProject = new System.Windows.Forms.CheckBox();
            this.label13 = new System.Windows.Forms.Label();
            this.megaUSBPath = new System.Windows.Forms.TextBox();
            this.pathMegaUSBButton = new System.Windows.Forms.Button();
            this.modeNTSC = new System.Windows.Forms.RadioButton();
            this.label14 = new System.Windows.Forms.Label();
            this.modePAL = new System.Windows.Forms.RadioButton();
            this.label15 = new System.Windows.Forms.Label();
            this.emuRegion = new System.Windows.Forms.ComboBox();
            this.targetList = new System.Windows.Forms.ComboBox();
            this.label16 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(54, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "ASM68k Path:";
            // 
            // asmPath
            // 
            this.asmPath.Location = new System.Drawing.Point(136, 33);
            this.asmPath.Name = "asmPath";
            this.asmPath.Size = new System.Drawing.Size(281, 20);
            this.asmPath.TabIndex = 1;
            // 
            // okBtn
            // 
            this.okBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okBtn.Location = new System.Drawing.Point(342, 356);
            this.okBtn.Name = "okBtn";
            this.okBtn.Size = new System.Drawing.Size(75, 23);
            this.okBtn.TabIndex = 2;
            this.okBtn.Text = "&Ok";
            this.okBtn.UseVisualStyleBackColor = true;
            this.okBtn.Click += new System.EventHandler(this.okBtn_Click);
            // 
            // pathButton
            // 
            this.pathButton.Location = new System.Drawing.Point(423, 31);
            this.pathButton.Name = "pathButton";
            this.pathButton.Size = new System.Drawing.Size(24, 23);
            this.pathButton.TabIndex = 3;
            this.pathButton.Text = "...";
            this.pathButton.UseVisualStyleBackColor = true;
            this.pathButton.Click += new System.EventHandler(this.pathButton_Click);
            // 
            // cancelBtn
            // 
            this.cancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelBtn.Location = new System.Drawing.Point(245, 356);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Size = new System.Drawing.Size(75, 23);
            this.cancelBtn.TabIndex = 4;
            this.cancelBtn.Text = "&Cancel";
            this.cancelBtn.UseVisualStyleBackColor = true;
            this.cancelBtn.Click += new System.EventHandler(this.cancelBtn_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(71, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Extra args:";
            // 
            // asmArgs
            // 
            this.asmArgs.Location = new System.Drawing.Point(136, 60);
            this.asmArgs.Name = "asmArgs";
            this.asmArgs.Size = new System.Drawing.Size(281, 20);
            this.asmArgs.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(29, 89);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(99, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Emulator resolution:";
            // 
            // emuResolution
            // 
            this.emuResolution.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.emuResolution.FormattingEnabled = true;
            this.emuResolution.Location = new System.Drawing.Point(136, 86);
            this.emuResolution.Name = "emuResolution";
            this.emuResolution.Size = new System.Drawing.Size(144, 21);
            this.emuResolution.TabIndex = 8;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.inputStart);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.inputC);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.inputB);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.inputA);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.inputRight);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.inputLeft);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.inputDown);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.inputUp);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Location = new System.Drawing.Point(9, 212);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(408, 138);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Input";
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // inputStart
            // 
            this.inputStart.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.inputStart.FormattingEnabled = true;
            this.inputStart.Location = new System.Drawing.Point(256, 100);
            this.inputStart.Name = "inputStart";
            this.inputStart.Size = new System.Drawing.Size(121, 21);
            this.inputStart.TabIndex = 15;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(218, 103);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(32, 13);
            this.label11.TabIndex = 14;
            this.label11.Text = "Start:";
            // 
            // inputC
            // 
            this.inputC.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.inputC.FormattingEnabled = true;
            this.inputC.Location = new System.Drawing.Point(256, 73);
            this.inputC.Name = "inputC";
            this.inputC.Size = new System.Drawing.Size(121, 21);
            this.inputC.TabIndex = 13;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(233, 76);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(17, 13);
            this.label10.TabIndex = 12;
            this.label10.Text = "C:";
            // 
            // inputB
            // 
            this.inputB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.inputB.FormattingEnabled = true;
            this.inputB.Location = new System.Drawing.Point(256, 46);
            this.inputB.Name = "inputB";
            this.inputB.Size = new System.Drawing.Size(121, 21);
            this.inputB.TabIndex = 11;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(233, 49);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(17, 13);
            this.label9.TabIndex = 10;
            this.label9.Text = "B:";
            // 
            // inputA
            // 
            this.inputA.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.inputA.FormattingEnabled = true;
            this.inputA.Location = new System.Drawing.Point(256, 19);
            this.inputA.Name = "inputA";
            this.inputA.Size = new System.Drawing.Size(121, 21);
            this.inputA.TabIndex = 9;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(233, 22);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(17, 13);
            this.label8.TabIndex = 8;
            this.label8.Text = "A:";
            // 
            // inputRight
            // 
            this.inputRight.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.inputRight.FormattingEnabled = true;
            this.inputRight.Location = new System.Drawing.Point(73, 100);
            this.inputRight.Name = "inputRight";
            this.inputRight.Size = new System.Drawing.Size(121, 21);
            this.inputRight.TabIndex = 7;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(32, 103);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(35, 13);
            this.label7.TabIndex = 6;
            this.label7.Text = "Right:";
            // 
            // inputLeft
            // 
            this.inputLeft.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.inputLeft.FormattingEnabled = true;
            this.inputLeft.Location = new System.Drawing.Point(73, 73);
            this.inputLeft.Name = "inputLeft";
            this.inputLeft.Size = new System.Drawing.Size(121, 21);
            this.inputLeft.TabIndex = 5;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(39, 76);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(28, 13);
            this.label6.TabIndex = 4;
            this.label6.Text = "Left:";
            // 
            // inputDown
            // 
            this.inputDown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.inputDown.FormattingEnabled = true;
            this.inputDown.Location = new System.Drawing.Point(73, 46);
            this.inputDown.Name = "inputDown";
            this.inputDown.Size = new System.Drawing.Size(121, 21);
            this.inputDown.TabIndex = 3;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(29, 49);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(38, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "Down:";
            // 
            // inputUp
            // 
            this.inputUp.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.inputUp.FormattingEnabled = true;
            this.inputUp.Location = new System.Drawing.Point(73, 19);
            this.inputUp.Name = "inputUp";
            this.inputUp.Size = new System.Drawing.Size(121, 21);
            this.inputUp.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(43, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(24, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Up:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(15, 113);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(115, 13);
            this.label12.TabIndex = 10;
            this.label12.Text = "Remember last project:";
            // 
            // autoOpenLastProject
            // 
            this.autoOpenLastProject.AutoSize = true;
            this.autoOpenLastProject.Checked = true;
            this.autoOpenLastProject.CheckState = System.Windows.Forms.CheckState.Checked;
            this.autoOpenLastProject.Location = new System.Drawing.Point(136, 113);
            this.autoOpenLastProject.Name = "autoOpenLastProject";
            this.autoOpenLastProject.Size = new System.Drawing.Size(15, 14);
            this.autoOpenLastProject.TabIndex = 11;
            this.autoOpenLastProject.UseVisualStyleBackColor = true;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(41, 189);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(87, 13);
            this.label13.TabIndex = 12;
            this.label13.Text = "Mega-USB Path:";
            // 
            // megaUSBPath
            // 
            this.megaUSBPath.Location = new System.Drawing.Point(134, 186);
            this.megaUSBPath.Name = "megaUSBPath";
            this.megaUSBPath.Size = new System.Drawing.Size(281, 20);
            this.megaUSBPath.TabIndex = 13;
            // 
            // pathMegaUSBButton
            // 
            this.pathMegaUSBButton.Location = new System.Drawing.Point(423, 184);
            this.pathMegaUSBButton.Name = "pathMegaUSBButton";
            this.pathMegaUSBButton.Size = new System.Drawing.Size(24, 23);
            this.pathMegaUSBButton.TabIndex = 14;
            this.pathMegaUSBButton.Text = "...";
            this.pathMegaUSBButton.UseVisualStyleBackColor = true;
            this.pathMegaUSBButton.Click += new System.EventHandler(this.pathMegaUSBButton_Click);
            // 
            // modeNTSC
            // 
            this.modeNTSC.AutoSize = true;
            this.modeNTSC.Location = new System.Drawing.Point(134, 136);
            this.modeNTSC.Name = "modeNTSC";
            this.modeNTSC.Size = new System.Drawing.Size(54, 17);
            this.modeNTSC.TabIndex = 15;
            this.modeNTSC.TabStop = true;
            this.modeNTSC.Text = "NTSC";
            this.modeNTSC.UseVisualStyleBackColor = true;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(81, 135);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(47, 13);
            this.label14.TabIndex = 16;
            this.label14.Text = "Refresh:";
            this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // modePAL
            // 
            this.modePAL.AutoSize = true;
            this.modePAL.Location = new System.Drawing.Point(194, 136);
            this.modePAL.Name = "modePAL";
            this.modePAL.Size = new System.Drawing.Size(45, 17);
            this.modePAL.TabIndex = 17;
            this.modePAL.TabStop = true;
            this.modePAL.Text = "PAL";
            this.modePAL.UseVisualStyleBackColor = true;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(81, 159);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(44, 13);
            this.label15.TabIndex = 18;
            this.label15.Text = "Region:";
            // 
            // emuRegion
            // 
            this.emuRegion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.emuRegion.FormattingEnabled = true;
            this.emuRegion.Location = new System.Drawing.Point(134, 156);
            this.emuRegion.Name = "emuRegion";
            this.emuRegion.Size = new System.Drawing.Size(144, 21);
            this.emuRegion.TabIndex = 19;
            // 
            // targetList
            // 
            this.targetList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.targetList.FormattingEnabled = true;
            this.targetList.Location = new System.Drawing.Point(136, 6);
            this.targetList.Name = "targetList";
            this.targetList.Size = new System.Drawing.Size(279, 21);
            this.targetList.TabIndex = 20;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(87, 9);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(41, 13);
            this.label16.TabIndex = 21;
            this.label16.Text = "Target:";
            // 
            // ConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(456, 394);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.targetList);
            this.Controls.Add(this.emuRegion);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.modePAL);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.modeNTSC);
            this.Controls.Add(this.pathMegaUSBButton);
            this.Controls.Add(this.megaUSBPath);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.autoOpenLastProject);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.emuResolution);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.asmArgs);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cancelBtn);
            this.Controls.Add(this.pathButton);
            this.Controls.Add(this.okBtn);
            this.Controls.Add(this.asmPath);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ConfigForm";
            this.Text = "Configuration";
            this.Load += new System.EventHandler(this.ConfigForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button okBtn;
        private System.Windows.Forms.Button pathButton;
        private System.Windows.Forms.Button cancelBtn;
        public System.Windows.Forms.TextBox asmPath;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.TextBox asmArgs;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.ComboBox emuResolution;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        public System.Windows.Forms.ComboBox inputUp;
        public System.Windows.Forms.ComboBox inputStart;
        public System.Windows.Forms.ComboBox inputC;
        public System.Windows.Forms.ComboBox inputB;
        public System.Windows.Forms.ComboBox inputA;
        public System.Windows.Forms.ComboBox inputRight;
        public System.Windows.Forms.ComboBox inputLeft;
        public System.Windows.Forms.ComboBox inputDown;
        private System.Windows.Forms.Label label12;
        public System.Windows.Forms.CheckBox autoOpenLastProject;
        private System.Windows.Forms.Label label13;
        public System.Windows.Forms.TextBox megaUSBPath;
        private System.Windows.Forms.Button pathMegaUSBButton;
        private System.Windows.Forms.Label label14;
        public System.Windows.Forms.RadioButton modeNTSC;
        public System.Windows.Forms.RadioButton modePAL;
        private System.Windows.Forms.Label label15;
        public System.Windows.Forms.ComboBox emuRegion;
        public System.Windows.Forms.ComboBox targetList;
        private System.Windows.Forms.Label label16;
    }
}