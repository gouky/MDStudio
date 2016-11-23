using System.Collections.Generic;
using System.Windows.Forms;
using DigitalRune.Windows.TextEditor;
using DigitalRune.Windows.TextEditor.Completion;


namespace DigitalRune.Windows.SampleEditor
{
  class CodeCompletionDataProvider : AbstractCompletionDataProvider
  {
    private readonly ImageList _imageList;

    public override ImageList ImageList
    {
      get { return _imageList; }
    }


    public CodeCompletionDataProvider()
    {
      // Create the image-list that is needed by the completion windows
      _imageList = new ImageList();
      _imageList.Images.Add(Resources.TemplateIcon);
      _imageList.Images.Add(Resources.FieldIcon);
      _imageList.Images.Add(Resources.MethodIcon);
    }


    public override ICompletionData[] GenerateCompletionData(string fileName, TextArea textArea, char charTyped)
    {
      // This class provides the data for the Code-Completion-Window.
      // Some random variables and methods are returned as completion data.

      List<ICompletionData> completionData = new List<ICompletionData>
      {
        // Add some random variables
        new DefaultCompletionData("a", "A local variable", 1),
        new DefaultCompletionData("b", "A local variable", 1),
        new DefaultCompletionData("variableX", "A local variable", 1),
        new DefaultCompletionData("variableY", "A local variable", 1),

        // Add some random methods
        new DefaultCompletionData("MethodA", "A simple method.", 2),
        new DefaultCompletionData("MethodB", "A simple method.", 2),
        new DefaultCompletionData("MethodC", "A simple method.", 2)
      };


      // Add some snippets (text templates).
      List<Snippet> snippets = new List<Snippet>
      {
        new Snippet("for", "for (|;;)\n{\n}", "for loop"),
        new Snippet("if", "if (|)\n{\n}", "if statement"),
        new Snippet("ifel", "if (|)\n{\n}\nelse\n{\n}", "if-else statement"),
        new Snippet("while", "while (|)\n{\n}", "while loop"),
        new Snippet("dr", "Visit http://www.digitalrune.com/", "The URL of DigitalRune Software.")
      };

      // Add the snippets to the completion data
      foreach(Snippet snippet in snippets)
        completionData.Add(new SnippetCompletionData(snippet, 0));

      return completionData.ToArray();
    }
  }
}
