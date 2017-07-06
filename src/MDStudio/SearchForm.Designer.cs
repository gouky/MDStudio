namespace MDStudio
{
    partial class SearchForm
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
            this.btnSearch = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.checkMatchCase = new System.Windows.Forms.CheckBox();
            this.searchString = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.replaceString = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.checkMatchInSelection = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(343, 81);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSearch.TabIndex = 5;
            this.btnSearch.Text = "&Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(262, 81);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // checkMatchCase
            // 
            this.checkMatchCase.AutoSize = true;
            this.checkMatchCase.Location = new System.Drawing.Point(12, 64);
            this.checkMatchCase.Name = "checkMatchCase";
            this.checkMatchCase.Size = new System.Drawing.Size(82, 17);
            this.checkMatchCase.TabIndex = 2;
            this.checkMatchCase.Text = "Match case";
            this.checkMatchCase.UseVisualStyleBackColor = true;
            // 
            // searchString
            // 
            this.searchString.Location = new System.Drawing.Point(59, 12);
            this.searchString.Name = "searchString";
            this.searchString.Size = new System.Drawing.Size(359, 20);
            this.searchString.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Search:";
            // 
            // replaceString
            // 
            this.replaceString.Location = new System.Drawing.Point(59, 38);
            this.replaceString.Name = "replaceString";
            this.replaceString.Size = new System.Drawing.Size(359, 20);
            this.replaceString.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Replace:";
            // 
            // checkMatchInSelection
            // 
            this.checkMatchInSelection.AutoSize = true;
            this.checkMatchInSelection.Location = new System.Drawing.Point(12, 85);
            this.checkMatchInSelection.Name = "checkMatchInSelection";
            this.checkMatchInSelection.Size = new System.Drawing.Size(82, 17);
            this.checkMatchInSelection.TabIndex = 3;
            this.checkMatchInSelection.Text = "In Selection";
            this.checkMatchInSelection.UseVisualStyleBackColor = true;
            this.checkMatchInSelection.Visible = false;
            // 
            // SearchForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(430, 116);
            this.Controls.Add(this.checkMatchInSelection);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.replaceString);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.checkMatchCase);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.searchString);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "SearchForm";
            this.Text = "Search";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnCancel;
        public System.Windows.Forms.CheckBox checkMatchCase;
        public System.Windows.Forms.TextBox searchString;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.TextBox replaceString;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.CheckBox checkMatchInSelection;
    }
}