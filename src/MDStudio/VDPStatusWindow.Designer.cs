namespace MDStudio
{
    partial class VDPStatusWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VDPStatusWindow));
            this.listVDPStatus = new System.Windows.Forms.ListView();
            this.Index = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Name = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Value = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Info = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // listVDPStatus
            // 
            this.listVDPStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listVDPStatus.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Index,
            this.Name,
            this.Value,
            this.Info});
            this.listVDPStatus.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listVDPStatus.Location = new System.Drawing.Point(12, 12);
            this.listVDPStatus.MultiSelect = false;
            this.listVDPStatus.Name = "listVDPStatus";
            this.listVDPStatus.Size = new System.Drawing.Size(744, 209);
            this.listVDPStatus.TabIndex = 0;
            this.listVDPStatus.UseCompatibleStateImageBehavior = false;
            this.listVDPStatus.View = System.Windows.Forms.View.Details;
            // 
            // Index
            // 
            this.Index.Text = "Index";
            this.Index.Width = 62;
            // 
            // Name
            // 
            this.Name.Text = "Name";
            this.Name.Width = 170;
            // 
            // Value
            // 
            this.Value.Text = "Value";
            this.Value.Width = 90;
            // 
            // Info
            // 
            this.Info.Text = "Info";
            this.Info.Width = 318;
            // 
            // VDPStatusWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(768, 233);
            this.Controls.Add(this.listVDPStatus);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Text = "VDP Status";
            this.Load += new System.EventHandler(this.VDPStatusWindow_Load);
            this.VisibleChanged += new System.EventHandler(this.VDPStatusWindow_VisibleChanged);
            this.Move += new System.EventHandler(this.VDPStatusWindow_Move);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listVDPStatus;
        private System.Windows.Forms.ColumnHeader Index;
        private System.Windows.Forms.ColumnHeader Value;
        private System.Windows.Forms.ColumnHeader Info;
        private System.Windows.Forms.ColumnHeader Name;
    }
}