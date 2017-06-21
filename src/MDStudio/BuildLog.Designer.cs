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
            this.btnExportRaw = new System.Windows.Forms.Button();
            this.saveLogDialog = new System.Windows.Forms.SaveFileDialog();
            this.SuspendLayout();
            // 
            // listErrors
            // 
            this.listErrors.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader3,
            this.columnHeader2});
            this.listErrors.FullRowSelect = true;
            this.listErrors.Location = new System.Drawing.Point(0, 0);
            this.listErrors.Name = "listErrors";
            this.listErrors.Size = new System.Drawing.Size(643, 146);
            this.listErrors.TabIndex = 0;
            this.listErrors.UseCompatibleStateImageBehavior = false;
            this.listErrors.View = System.Windows.Forms.View.Details;
            this.listErrors.SelectedIndexChanged += new System.EventHandler(this.listErrors_SelectedIndexChanged);
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
            // btnExportRaw
            // 
            this.btnExportRaw.Location = new System.Drawing.Point(556, 152);
            this.btnExportRaw.Name = "btnExportRaw";
            this.btnExportRaw.Size = new System.Drawing.Size(75, 23);
            this.btnExportRaw.TabIndex = 1;
            this.btnExportRaw.Text = "Export raw";
            this.btnExportRaw.UseVisualStyleBackColor = true;
            this.btnExportRaw.Click += new System.EventHandler(this.button1_Click);
            // 
            // saveLogDialog
            // 
            this.saveLogDialog.Filter = "TXT files|*.txt";
            this.saveLogDialog.Title = "Export log";
            this.saveLogDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.saveLogDialog_FileOk);
            // 
            // BuildLog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(643, 187);
            this.Controls.Add(this.btnExportRaw);
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
        private System.Windows.Forms.Button btnExportRaw;
        private System.Windows.Forms.SaveFileDialog saveLogDialog;
    }
}