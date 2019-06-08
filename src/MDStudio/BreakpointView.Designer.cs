namespace MDStudio
{
    partial class BreakpointView
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
            this.breakpointList = new System.Windows.Forms.CheckedListBox();
            this.SuspendLayout();
            // 
            // breakpointList
            // 
            this.breakpointList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.breakpointList.FormattingEnabled = true;
            this.breakpointList.Location = new System.Drawing.Point(12, 12);
            this.breakpointList.Name = "breakpointList";
            this.breakpointList.Size = new System.Drawing.Size(560, 274);
            this.breakpointList.TabIndex = 1;
            this.breakpointList.SelectedIndexChanged += new System.EventHandler(this.checkedListBox1_SelectedIndexChanged);
            this.breakpointList.DoubleClick += new System.EventHandler(this.breakpointList_DoubleClick);
            // 
            // BreakpointView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 292);
            this.Controls.Add(this.breakpointList);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "BreakpointView";
            this.Text = "Breakpoints";
            this.Load += new System.EventHandler(this.BreakpointView_Load);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.CheckedListBox breakpointList;
    }
}