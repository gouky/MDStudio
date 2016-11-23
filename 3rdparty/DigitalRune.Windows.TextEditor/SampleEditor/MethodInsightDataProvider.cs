using DigitalRune.Windows.TextEditor;
using DigitalRune.Windows.TextEditor.Document;
using DigitalRune.Windows.TextEditor.Insight;


namespace DigitalRune.Windows.SampleEditor
{
  class MethodInsightDataProvider : AbstractInsightDataProvider
  {
    int _argumentStartOffset;   // The offset where the method arguments starts.
    string[] _insightText;      // The insight information.


    protected override int ArgumentStartOffset
    {
      get { return _argumentStartOffset; }
    }


    public override int InsightDataCount
    {
      get { return (_insightText != null) ? _insightText.Length : 0; }
    }


    public override string GetInsightData(int number)
    {
      return (_insightText != null) ? _insightText[number] : string.Empty;
    }


    public override void SetupDataProvider(string fileName)
    {

      // This class provides the method insight information.
      // To find out which information is requested, it simply compares the
      // word before the caret with 3 hardcoded method names.

      int offset = TextArea.Caret.Offset;
      IDocument document = TextArea.Document;
      string methodName = TextHelper.GetIdentifierAt(document, offset - 1);

      if (methodName == "MethodA" || methodName == "MethodB" || methodName == "MethodC")
      {
        SetupDataProviderForMethod(methodName, offset);
      }
      else
      {
        // Perhaps the cursor is already inside the parameter list.
        offset = TextHelper.FindOpeningBracket(document, offset - 1, '(', ')');
        if (offset >= 1)
        {
          methodName = TextHelper.GetIdentifierAt(document, offset - 1);
          SetupDataProviderForMethod(methodName, offset);
        }
      }
    }


    private void SetupDataProviderForMethod(string methodName, int argumentStartOffset)
    {
      if (methodName == "MethodA")
      {
        // MethodA has 2 overloads
        _insightText = new string[2];
        _insightText[0] = "void MethodA()\n\n"
                          + "A simple method (first overload).";
        _insightText[1] = "int MethodA(int a, float f, string s)\n\n"
                          + "A simple method (second overload).";
        _argumentStartOffset = argumentStartOffset;
      }
      else if (methodName == "MethodB")
      {
        _insightText = new string[1];
        _insightText[0] = "string MethodB(int a, string s)\n\n"
                          + "A simple method.";
        _argumentStartOffset = argumentStartOffset;
      }
      else if (methodName == "MethodC")
      {
        // MethodC has 3 overloads
        _insightText = new string[3];
        _insightText[0] = "void MethodC()\n\n"
                          + "A simple method (first overload).";
        _insightText[1] = "void MethodC(int i)\n\n"
                          + "A simple method (second overload).";
        _insightText[2] = "void MethodC(float f)\n\n"
                          + "A simple method (third overload).";
        _argumentStartOffset = argumentStartOffset;
      }
    }
  }
}
