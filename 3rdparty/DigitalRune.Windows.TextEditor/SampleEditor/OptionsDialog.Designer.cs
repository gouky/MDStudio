namespace DigitalRune.Windows.SampleEditor
{
  partial class OptionsDialog
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
      this.okButton = new System.Windows.Forms.Button();
      this.propertyGrid = new System.Windows.Forms.PropertyGrid();
      this.SuspendLayout();
      // 
      // okButton
      // 
      this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.okButton.Location = new System.Drawing.Point(257, 409);
      this.okButton.Name = "okButton";
      this.okButton.Size = new System.Drawing.Size(75, 23);
      this.okButton.TabIndex = 0;
      this.okButton.Text = "OK";
      this.okButton.UseVisualStyleBackColor = true;
      // 
      // propertyGrid
      // 
      this.propertyGrid.Anchor = ((System.Windows.Forms.AnchorStyles) ((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.propertyGrid.Location = new System.Drawing.Point(12, 12);
      this.propertyGrid.Name = "propertyGrid";
      this.propertyGrid.Size = new System.Drawing.Size(320, 391);
      this.propertyGrid.TabIndex = 1;
      // 
      // OptionsDialog
      // 
      this.AcceptButton = this.okButton;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.okButton;
      this.ClientSize = new System.Drawing.Size(344, 444);
      this.Controls.Add(this.propertyGrid);
      this.Controls.Add(this.okButton);
      this.Name = "OptionsDialog";
      this.Text = "Options";
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button okButton;
    private System.Windows.Forms.PropertyGrid propertyGrid;
  }
}