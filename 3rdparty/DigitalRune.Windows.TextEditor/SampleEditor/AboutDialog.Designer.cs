namespace DigitalRune.Windows.SampleEditor
{
  partial class AboutDialog
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutDialog));
      this.labelTitle = new System.Windows.Forms.Label();
      this.labelDescription1 = new System.Windows.Forms.Label();
      this.linkDigitalRune = new System.Windows.Forms.LinkLabel();
      this.labelDescription2 = new System.Windows.Forms.Label();
      this.buttonOK = new System.Windows.Forms.Button();
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.tableLayoutPanel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // labelTitle
      // 
      this.labelTitle.AutoSize = true;
      this.labelTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
      this.labelTitle.Location = new System.Drawing.Point(12, 12);
      this.labelTitle.Margin = new System.Windows.Forms.Padding(12, 12, 12, 3);
      this.labelTitle.Name = "labelTitle";
      this.labelTitle.Size = new System.Drawing.Size(81, 13);
      this.labelTitle.TabIndex = 2;
      this.labelTitle.Text = "SampleEditor\r\n";
      // 
      // labelDescription1
      // 
      this.labelDescription1.AutoSize = true;
      this.labelDescription1.Location = new System.Drawing.Point(12, 31);
      this.labelDescription1.Margin = new System.Windows.Forms.Padding(12, 3, 12, 3);
      this.labelDescription1.Name = "labelDescription1";
      this.labelDescription1.Size = new System.Drawing.Size(251, 104);
      this.labelDescription1.TabIndex = 3;
      this.labelDescription1.Text = resources.GetString("labelDescription1.Text");
      // 
      // linkDigitalRune
      // 
      this.linkDigitalRune.AutoSize = true;
      this.linkDigitalRune.Location = new System.Drawing.Point(12, 141);
      this.linkDigitalRune.Margin = new System.Windows.Forms.Padding(12, 3, 12, 3);
      this.linkDigitalRune.Name = "linkDigitalRune";
      this.linkDigitalRune.Size = new System.Drawing.Size(141, 13);
      this.linkDigitalRune.TabIndex = 1;
      this.linkDigitalRune.TabStop = true;
      this.linkDigitalRune.Text = "http://www.digitalrune.com/";
      this.linkDigitalRune.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkDigitalRune_LinkClicked);
      // 
      // labelDescription2
      // 
      this.labelDescription2.AutoSize = true;
      this.labelDescription2.Location = new System.Drawing.Point(12, 160);
      this.labelDescription2.Margin = new System.Windows.Forms.Padding(12, 3, 12, 3);
      this.labelDescription2.Name = "labelDescription2";
      this.labelDescription2.Size = new System.Drawing.Size(240, 39);
      this.labelDescription2.TabIndex = 4;
      this.labelDescription2.Text = "\r\nThe TextEditor control is licensed under the terms\r\nof the GNU Lesser General P" +
          "ublic License.";
      // 
      // buttonOK
      // 
      this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.buttonOK.Location = new System.Drawing.Point(194, 214);
      this.buttonOK.Margin = new System.Windows.Forms.Padding(12);
      this.buttonOK.Name = "buttonOK";
      this.buttonOK.Size = new System.Drawing.Size(75, 23);
      this.buttonOK.TabIndex = 0;
      this.buttonOK.Text = "OK";
      this.buttonOK.UseVisualStyleBackColor = true;
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.AutoSize = true;
      this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.tableLayoutPanel1.ColumnCount = 1;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel1.Controls.Add(this.labelTitle, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.labelDescription2, 0, 3);
      this.tableLayoutPanel1.Controls.Add(this.linkDigitalRune, 0, 2);
      this.tableLayoutPanel1.Controls.Add(this.labelDescription1, 0, 1);
      this.tableLayoutPanel1.Controls.Add(this.buttonOK, 0, 4);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(13);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 5;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(281, 249);
      this.tableLayoutPanel1.TabIndex = 5;
      // 
      // AboutDialog
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.AutoSize = true;
      this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.ClientSize = new System.Drawing.Size(281, 249);
      this.Controls.Add(this.tableLayoutPanel1);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "AboutDialog";
      this.Text = "About \"SampleEditor\"";
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label labelTitle;
    private System.Windows.Forms.Label labelDescription1;
    private System.Windows.Forms.LinkLabel linkDigitalRune;
    private System.Windows.Forms.Label labelDescription2;
    private System.Windows.Forms.Button buttonOK;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
  }
}