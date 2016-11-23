namespace MDStudio
{
    partial class BuildLog
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
            this.listErrors = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // listErrors
            // 
            this.listErrors.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader3,
            this.columnHeader2});
            this.listErrors.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listErrors.FullRowSelect = true;
            this.listErrors.Location = new System.Drawing.Point(0, 0);
            this.listErrors.Name = "listErrors";
            this.listErrors.Size = new System.Drawing.Size(643, 114);
            this.listErrors.TabIndex = 0;
            this.listErrors.UseCompatibleStateImageBehavior = false;
            this.listErrors.View = System.Windows.Forms.View.Details;
            this.listErrors.DoubleClick += new System.EventHandler(this.listErrors_DoubleClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Line";
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "File";
            this.columnHeader3.Width = 120;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Message";
            this.columnHeader2.Width = 400;
            // 
            // BuildLog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(643, 114);
            this.Controls.Add(this.listErrors);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "BuildLog";
            this.Text = "BuildLog";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BuildLog_FormClosing);
            this.Load += new System.EventHandler(this.BuildLog_Load);
            this.VisibleChanged += new System.EventHandler(this.BuildLog_Shown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listErrors;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;

    }
}