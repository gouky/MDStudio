using System.Windows.Forms;
using DigitalRune.Windows.TextEditor;


namespace DigitalRune.Windows.SampleEditor
{
  public partial class OptionsDialog : Form
  {
    public OptionsDialog(TextEditorControl textEditorControl)
    {
      InitializeComponent();

      // Show the properties of the TextEditorControl
      propertyGrid.SelectedObject = textEditorControl;
    }
  }
}