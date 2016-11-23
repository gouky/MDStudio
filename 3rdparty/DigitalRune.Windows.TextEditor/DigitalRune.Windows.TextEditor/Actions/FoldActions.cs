using System;
using System.Collections.Generic;
using DigitalRune.Windows.TextEditor.Document;
using DigitalRune.Windows.TextEditor.Folding;

namespace DigitalRune.Windows.TextEditor.Actions
{
  /// <summary>
  /// Toggles the folding of the current region.
  /// </summary>
  public class ToggleFolding : AbstractEditAction
  {
    /// <summary>
    /// Executes the action on a certain <see cref="TextArea"/>.
    /// </summary>
    /// <param name="textArea">The text area on which to execute the action.</param>
    public override void Execute(TextArea textArea)
    {
      List<Fold> folds = textArea.Document.FoldingManager.GetFoldsWithStartAt(textArea.Caret.Line);
      if (folds.Count != 0)
      {
        foreach (Fold fm in folds)
          fm.IsFolded = !fm.IsFolded;
      }
      else
      {
        folds = textArea.Document.FoldingManager.GetFoldsContainingLine(textArea.Caret.Line);
        if (folds.Count != 0)
        {
          Fold innerMost = folds[0];
          for (int i = 1; i < folds.Count; i++)
          {
            if (new TextLocation(folds[i].StartColumn, folds[i].StartLine) > new TextLocation(innerMost.StartColumn, innerMost.StartLine))
            {
              innerMost = folds[i];
            }
          }
          innerMost.IsFolded = !innerMost.IsFolded;
        }
      }
      textArea.Document.FoldingManager.NotifyFoldingChanged(EventArgs.Empty);
    }
  }


  /// <summary>
  /// Toggles the folding of all regions in the document.
  /// </summary>
  public class ToggleAllFoldings : AbstractEditAction
  {
    /// <summary>
    /// Executes the action on a certain <see cref="TextArea"/>.
    /// </summary>
    /// <param name="textArea">The text area on which to execute the action.</param>
    public override void Execute(TextArea textArea)
    {
      bool doFold = true;
      foreach (Fold fold in textArea.Document.FoldingManager.Folds)
      {
        if (fold.IsFolded)
        {
          doFold = false;
          break;
        }
      }
    
      foreach (Fold fold in textArea.Document.FoldingManager.Folds)
        fold.IsFolded = doFold;

      textArea.Document.FoldingManager.NotifyFoldingChanged(EventArgs.Empty);
    }
  }


  /// <summary>
  /// Folds (hides) all regions that match a certain criteria.
  /// </summary>
  public class FoldMarkersByCriteria : AbstractEditAction
  {
    private readonly Predicate<Fold> _predicate;


    /// <summary>
    /// Initializes a new instance of the <see cref="FoldMarkersByCriteria"/> class.
    /// </summary>
    /// <param name="predicate">
    /// The predicate that defines whether the region shall be folded.
    /// </param>
    public FoldMarkersByCriteria(Predicate<Fold> predicate)
    {
      _predicate = predicate;
    }


    /// <summary>
    /// Executes the action on a certain <see cref="TextArea"/>.
    /// </summary>
    /// <param name="textArea">The text area on which to execute the action.</param>
    public override void Execute(TextArea textArea)
    {
      foreach (Fold fold in textArea.Document.FoldingManager.Folds)
        fold.IsFolded = _predicate(fold);

      textArea.Document.FoldingManager.NotifyFoldingChanged(EventArgs.Empty);
    }
  }
}
