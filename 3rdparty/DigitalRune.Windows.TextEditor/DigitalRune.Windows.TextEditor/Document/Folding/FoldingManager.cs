using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DigitalRune.Windows.TextEditor.Document;


namespace DigitalRune.Windows.TextEditor.Folding
{
  /// <summary>
  /// Manages the folding (<see cref="Folds"/>) of a text buffer.
  /// </summary>
  public class FoldingManager
  {
    private List<Fold> _folds = new List<Fold>();
    private List<Fold> _foldsByEnd = new List<Fold>();
    private IFoldingStrategy _foldingStrategy;
    private readonly IDocument _document;
    private List<Fold> _topLevelFolds;


    /// <summary>
    /// Gets a list of all <see cref="Folds"/>.
    /// </summary>
    /// <value>The fold marker.</value>
    public IList<Fold> Folds
    {
      get { return _folds.AsReadOnly(); }
    }


    /// <summary>
    /// Gets or sets the folding strategy.
    /// </summary>
    /// <value>The folding strategy.</value>
    public IFoldingStrategy FoldingStrategy
    {
      get { return _foldingStrategy; }
      set { _foldingStrategy = value; }
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="FoldingManager"/> class.
    /// </summary>
    /// <param name="document">The document.</param>
    internal FoldingManager(IDocument document)
    {
      _document = document;
      document.DocumentChanged += DocumentChanged;
    }


    void DocumentChanged(object sender, DocumentEventArgs e)
    {
      int oldCount = _folds.Count;
      _document.UpdateSegmentListOnDocumentChange(_folds, e);
      if (oldCount != _folds.Count)
        _document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.WholeTextArea));
    }


    /// <summary>
    /// Gets the folds from position.
    /// </summary>
    /// <param name="line">The line.</param>
    /// <param name="column">The column.</param>
    /// <returns>The folds that include the specified position.</returns>
    public List<Fold> GetFoldsFromPosition(int line, int column)
    {
      List<Fold> foldings = new List<Fold>();
      if (_folds != null)
      {
        for (int i = 0; i < _folds.Count; ++i)
        {
          Fold fold = _folds[i];
          if ((fold.StartLine == line && column > fold.StartColumn && !(fold.EndLine == line && column >= fold.EndColumn)) 
              || (fold.EndLine == line && column < fold.EndColumn && !(fold.StartLine == line && column <= fold.StartColumn)) 
              || (line > fold.StartLine && line < fold.EndLine))
          {
            foldings.Add(fold);
          }
        }
      }
      return foldings;
    }


    private class FoldStartComparer : IComparer<Fold>
    {
      public readonly static FoldStartComparer Instance = new FoldStartComparer();

      public int Compare(Fold x, Fold y)
      {
        if (x.StartLine < y.StartLine)
          return -1;
        else if (x.StartLine == y.StartLine)
          return x.StartColumn.CompareTo(y.StartColumn);
        else
          return 1;
      }
    }


    private class FoldEndComparer : IComparer<Fold>
    {
      public readonly static FoldEndComparer Instance = new FoldEndComparer();

      public int Compare(Fold x, Fold y)
      {
        if (x.EndLine < y.EndLine)
          return -1;
        else if (x.EndLine == y.EndLine)
          return x.EndColumn.CompareTo(y.EndColumn);
        else
          return 1;
      }
    }


    /// <summary>
    /// Gets the folds that start at certain line.
    /// </summary>
    /// <param name="lineNumber">The line number.</param>
    /// <returns>The foldings that start <paramref name="lineNumber"/>.</returns>
    public List<Fold> GetFoldsWithStartAt(int lineNumber)
    {
      return GetFoldsByStartAfterColumn(lineNumber, -1, false);
    }


    /// <summary>
    /// Gets the folds that ends in a certain line.
    /// </summary>
    /// <param name="lineNumber">The line number.</param>
    /// <returns>The folds that end at <paramref name="lineNumber"/>.</returns>
    public List<Fold> GetFoldsWithEndAt(int lineNumber)
    {
      return GetFoldsByEndAfterColumn(lineNumber, -1, false);
    }


    List<Fold> GetFoldsByStartAfterColumn(int lineNumber, int column, bool forceFolded)
    {
      List<Fold> foldings = new List<Fold>();

      if (_folds != null)
      {
        Fold reference = new Fold(_document, lineNumber, column, lineNumber, column);
        int index = _folds.BinarySearch(reference, FoldStartComparer.Instance);

        if (index < 0) 
          index = ~index;

        for (; index < _folds.Count; index++)
        {
          Fold fold = _folds[index];

          if (fold.StartLine < lineNumber)
            continue;
          else if (fold.StartLine > lineNumber)
            break;

          if (fold.StartColumn <= column)
            continue;

          if (!forceFolded || fold.IsFolded)
            foldings.Add(fold);
        }
      }
      return foldings;
    }


    List<Fold> GetFoldsByEndAfterColumn(int lineNumber, int column, bool forceFolded)
    {
      List<Fold> foldings = new List<Fold>();

      if (_folds != null)
      {
        Fold reference = new Fold(_document, lineNumber, column, lineNumber, column);
        int index = _foldsByEnd.BinarySearch(reference, FoldEndComparer.Instance);
        if (index < 0) index = ~index;

        for (; index < _foldsByEnd.Count; index++)
        {
          Fold fold = _foldsByEnd[index];

          if (fold.EndLine < lineNumber)
            continue;
          else if (fold.EndLine > lineNumber)
            break;

          if (fold.EndColumn <= column)
            continue;

          if (!forceFolded || fold.IsFolded)
            foldings.Add(fold);
        }
      }
      return foldings;
    }


    /// <summary>
    /// Gets the collapsed folds that start at certain line.
    /// </summary>
    /// <param name="lineNumber">The line number.</param>
    /// <returns>The collapsed folds that start at <paramref name="lineNumber"/>.</returns>
    public List<Fold> GetFoldedFoldsWithStartAt(int lineNumber)
    {
      return GetFoldsByStartAfterColumn(lineNumber, -1, true);
    }


    /// <summary>
    /// Gets the collapsed folds that start after a certain column.
    /// </summary>
    /// <param name="lineNumber">The line number.</param>
    /// <param name="column">The column.</param>
    /// <returns>The collapsed folds that start in <paramref name="lineNumber"/> somewhere after <paramref name="column"/>.</returns>
    public List<Fold> GetFoldedFoldsWithStartAfterColumn(int lineNumber, int column)
    {
      return GetFoldsByStartAfterColumn(lineNumber, column, true);
    }


    /// <summary>
    /// Gets the folded foldings that end at certain line.
    /// </summary>
    /// <param name="lineNumber">The line number.</param>
    /// <returns>The folded foldings that end at <paramref name="lineNumber"/>.</returns>
    public List<Fold> GetFoldedFoldsWithEndAt(int lineNumber)
    {
      return GetFoldsByEndAfterColumn(lineNumber, -1, true);
    }


    /// <summary>
    /// Determines whether any folds start at the line.
    /// </summary>
    /// <param name="lineNumber">The line number.</param>
    /// <returns>
    /// 	<see langword="true"/> if any folds start at <paramref name="lineNumber"/>; otherwise, <see langword="false"/>.
    /// </returns>
    public bool IsFoldStart(int lineNumber)
    {
      return GetFoldsWithStartAt(lineNumber).Count > 0;
    }


    /// <summary>
    /// Determines whether any folds end at the line.
    /// </summary>
    /// <param name="lineNumber">The line number.</param>
    /// <returns>
    /// 	<see langword="true"/> if any folds end at <paramref name="lineNumber"/>; otherwise, <see langword="false"/>.
    /// </returns>
    public bool IsFoldEnd(int lineNumber)
    {
      return GetFoldsWithEndAt(lineNumber).Count > 0;
    }


    /// <summary>
    /// Gets the folds that contain a certain line number (excluding start and end line).
    /// </summary>
    /// <param name="lineNumber">The line number.</param>
    /// <returns>The folds that contain a certain line number.</returns>
    public List<Fold> GetFoldsContainingLine(int lineNumber)
    {
      List<Fold> foldings = new List<Fold>();
      if (_folds != null)
      {
        foreach (Fold fold in _folds)
        {
          if (fold.StartLine < lineNumber)
          {
            if (lineNumber < fold.EndLine)
              foldings.Add(fold);
          }
          else
          {
            break;
          }
        }
      }
      return foldings;
    }


    /// <summary>
    /// Determines whether a line belongs to a fold.
    /// </summary>
    /// <param name="lineNumber">The line number.</param>
    /// <returns>
    /// 	<see langword="true"/> if <paramref name="lineNumber"/> belongs to a fold; otherwise, <see langword="false"/>.
    /// </returns>
    public bool IsLineInFolds(int lineNumber)
    {
      if (_folds != null)
      {
        foreach (Fold fold in _folds)
          if (fold.StartLine < lineNumber && lineNumber < fold.EndLine)
            return true;
      }
      return false;
    }


    /// <summary>
    /// Determines whether a line is visible (not folded).
    /// </summary>
    /// <param name="lineNumber">The line number.</param>
    /// <returns>
    /// 	<see langword="true"/> if <paramref name="lineNumber"/> is visible; otherwise, <see langword="false"/>.
    /// </returns>
    public bool IsLineVisible(int lineNumber)
    {
      foreach (Fold fold in GetTopLevelFoldedFoldings())
        if (fold.StartLine < lineNumber && lineNumber < fold.EndLine && fold.IsFolded)
          return false;

      return true;
    }


    /// <summary>
    /// Gets the top level collapsed folds.
    /// </summary>
    /// <returns>The top level collapsed folds.</returns>
    public ReadOnlyCollection<Fold> GetTopLevelFoldedFoldings()
    {
      if (_topLevelFolds != null)
        return _topLevelFolds.AsReadOnly();

      _topLevelFolds = new List<Fold>();
      if (_folds != null)
      {
        TextLocation end = new TextLocation(0, 0);
        foreach (Fold fold in _folds)
        {
          if (fold.IsFolded && (fold.StartLine > end.Line || fold.StartLine == end.Line && fold.StartColumn >= end.Column))
          {
            _topLevelFolds.Add(fold);
            end = new TextLocation(fold.EndColumn, fold.EndLine);
          }
        }
      }
      return _topLevelFolds.AsReadOnly();
    }


    internal void ClearCache()
    {
      _topLevelFolds = null; 
    }


    /// <summary>
    /// Updates the folds.
    /// </summary>
    public void UpdateFolds()
    {
      UpdateFolds(null, null);
    }


    /// <summary>
    /// Updates the folds.
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="parseInfo">The parse info.</param>
    public void UpdateFolds(string fileName, object parseInfo)
    {
      if (_foldingStrategy == null || !_document.TextEditorProperties.EnableFolding)
        return;

      UpdateFolds(_foldingStrategy.GenerateFolds(_document, fileName, parseInfo));
    }


    /// <summary>
    /// Updates the folds.
    /// </summary>
    /// <param name="newFolds">The new folds.</param>
    public void UpdateFolds(List<Fold> newFolds)
    {
      int oldFoldingsCount = _folds.Count;
      lock (this)
      {
        ClearCache();
        if (newFolds != null && newFolds.Count != 0)
        {
          newFolds.Sort();
          if (_folds.Count == newFolds.Count)
          {
            for (int i = 0; i < _folds.Count; ++i)
            {
              newFolds[i].IsFolded = _folds[i].IsFolded;
            }
            _folds = newFolds;
          }
          else
          {
            for (int i = 0, j = 0; i < _folds.Count && j < newFolds.Count; )
            {
              int n = newFolds[j].CompareTo(_folds[i]);
              if (n > 0)
              {
                ++i;
              }
              else
              {
                if (n == 0)
                  newFolds[j].IsFolded = _folds[i].IsFolded;

                ++j;
              }
            }
          }
        }
        if (newFolds != null)
        {
          _folds = newFolds;
          _foldsByEnd = new List<Fold>(newFolds);
          _foldsByEnd.Sort(FoldEndComparer.Instance);
        }
        else
        {
          _folds.Clear();
          _foldsByEnd.Clear();
        }
      }
      if (oldFoldingsCount != _folds.Count)
      {
        _document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.WholeTextArea));
        _document.CommitUpdate();
      }
    }


    //public string SerializeToString()
    //{
    //  StringBuilder sb = new StringBuilder();
    //  foreach (Fold fold in this._folds)
    //  {
    //    sb.Append(fold.Offset); sb.Append("\n");
    //    sb.Append(fold.Length); sb.Append("\n");
    //    sb.Append(fold.FoldText); sb.Append("\n");
    //    sb.Append(fold.IsFolded); sb.Append("\n");
    //  }
    //  return sb.ToString();
    //}


    //public void DeserializeFromString(string str)
    //{
    //  try
    //  {
    //    string[] lines = str.Split('\n');
    //    for (int i = 0; i < lines.Length && lines[i].Length > 0; i += 4)
    //    {
    //      int offset = Int32.Parse(lines[i]);
    //      int length = Int32.Parse(lines[i + 1]);
    //      string text = lines[i + 2];
    //      bool isFolded = Boolean.Parse(lines[i + 3]);
    //      bool found = false;
    //      foreach (Fold fold in _folds)
    //      {
    //        if (fold.Offset == offset && marker.Length == length)
    //        {
    //          fold.IsFolded = isFolded;
    //          found = true;
    //          break;
    //        }
    //      }
    //      if (!found)
    //      {
    //        _folds.Add(new Fold(document, offset, length, text, isFolded));
    //      }
    //    }
    //    if (lines.Length > 0)
    //    {
    //      NotifyFoldingChanged(EventArgs.Empty);
    //    }
    //  }
    //  catch (Exception)
    //  {
    //  }
    //}


    /// <summary>
    /// Raises the <see cref="FoldingChanged"/> event.
    /// </summary>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    public void NotifyFoldingChanged(EventArgs e)
    {
      ClearCache();

      if (FoldingChanged != null)
        FoldingChanged(this, e);
    }


    /// <summary>
    /// Occurs when the foldings have been changed.
    /// </summary>
    public event EventHandler FoldingChanged;
  }
}
