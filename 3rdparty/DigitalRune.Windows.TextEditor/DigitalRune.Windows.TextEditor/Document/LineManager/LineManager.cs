using System;
using System.Collections.Generic;
using System.Diagnostics;
using DigitalRune.Windows.TextEditor.Folding;
using DigitalRune.Windows.TextEditor.Highlighting;


namespace DigitalRune.Windows.TextEditor.Document
{
	internal sealed class LineManager
  {
    private sealed class DelimiterSegment
    {
      public int Offset;
      public int Length;
    }

    // use always the same DelimiterSegment object for the NextDelimiter
    private readonly DelimiterSegment _delimiterSegment = new DelimiterSegment();

    private readonly LineSegmentTree _lineSegments = new LineSegmentTree();
    private readonly IDocument _document;
    private IHighlightingStrategy _highlightingStrategy;



    public IList<LineSegment> LineSegments
    {
      get { return _lineSegments; }
    }


    public int TotalNumberOfLines
    {
      get { return _lineSegments.Count; }
    }


    public IHighlightingStrategy HighlightingStrategy
    {
      get { return _highlightingStrategy; }
      set
      {
        if (_highlightingStrategy != value)
        {
          _highlightingStrategy = value;
          if (_highlightingStrategy != null)
          {
            _highlightingStrategy.MarkTokens(_document);
          }
        }
      }
    }


    public event EventHandler<LineLengthChangedEventArgs> LineLengthChanged;
    public event EventHandler<LineCountChangedEventArgs> LineCountChanged;
    public event EventHandler<LineEventArgs> LineDeleted;


    public LineManager(IDocument document, IHighlightingStrategy highlightingStrategy)
    {
      _document = document;
      _highlightingStrategy = highlightingStrategy;
    }


    public int GetLineNumberForOffset(int offset)
    {
      return GetLineSegmentForOffset(offset).LineNumber;
    }


    public LineSegment GetLineSegmentForOffset(int offset)
    {
      return _lineSegments.GetByOffset(offset);
    }


    public LineSegment GetLineSegment(int lineNr)
    {
      return _lineSegments[lineNr];
    }


    public void Insert(int offset, string text)
    {
      Replace(offset, 0, text);
    }


    public void Remove(int offset, int length)
    {
      Replace(offset, length, String.Empty);
    }


    /// <summary>
    /// Replaces the specified offset.
    /// </summary>
    /// <param name="offset">The offset.</param>
    /// <param name="length">The length.</param>
    /// <param name="text">The text.</param>
    public void Replace(int offset, int length, string text)
    {
      int lineStart = GetLineNumberForOffset(offset);
      int oldNumberOfLines = TotalNumberOfLines;
      DeferredEventList deferredEventList = new DeferredEventList();
      RemoveInternal(ref deferredEventList, offset, length);
      int numberOfLinesAfterRemoving = TotalNumberOfLines;
      if (!string.IsNullOrEmpty(text))
      {
        InsertInternal(offset, text);
      }
      // Only fire events after RemoveInternal+InsertInternal finished completely:
      // Otherwise we would expose inconsistent state to the event handlers.
      RunHighlighter(lineStart, 1 + Math.Max(0, TotalNumberOfLines - numberOfLinesAfterRemoving));

      if (deferredEventList.RemovedLines != null)
      {
        foreach (LineSegment ls in deferredEventList.RemovedLines)
          OnLineDeleted(new LineEventArgs(_document, ls));
      }
      deferredEventList.RaiseEvents();
      if (TotalNumberOfLines != oldNumberOfLines)
      {
        OnLineCountChanged(new LineCountChangedEventArgs(_document, lineStart, TotalNumberOfLines - oldNumberOfLines));
      }
    }


    void RemoveInternal(ref DeferredEventList deferredEventList, int offset, int length)
    {
      Debug.Assert(length >= 0);
      if (length == 0) return;
      LineSegmentTree.Enumerator it = _lineSegments.GetEnumeratorForOffset(offset);
      LineSegment startSegment = it.Current;
      int startSegmentOffset = startSegment.Offset;
      if (offset + length < startSegmentOffset + startSegment.TotalLength)
      {
        // just removing a part of this line segment
        startSegment.RemovedLinePart(ref deferredEventList, offset - startSegmentOffset, length);
        SetSegmentLength(startSegment, startSegment.TotalLength - length);
        return;
      }
      // merge startSegment with another line segment because startSegment's delimiter was deleted
      // possibly remove lines in between if multiple delimiters were deleted
      int charactersRemovedInStartLine = startSegmentOffset + startSegment.TotalLength - offset;
      Debug.Assert(charactersRemovedInStartLine > 0);
      startSegment.RemovedLinePart(ref deferredEventList, offset - startSegmentOffset, charactersRemovedInStartLine);


      LineSegment endSegment = _lineSegments.GetByOffset(offset + length);
      if (endSegment == startSegment)
      {
        // special case: we are removing a part of the last line up to the
        // end of the document
        SetSegmentLength(startSegment, startSegment.TotalLength - length);
        return;
      }
      int endSegmentOffset = endSegment.Offset;
      int charactersLeftInEndLine = endSegmentOffset + endSegment.TotalLength - (offset + length);
      endSegment.RemovedLinePart(ref deferredEventList, 0, endSegment.TotalLength - charactersLeftInEndLine);
      startSegment.MergedWith(endSegment, offset - startSegmentOffset);
      SetSegmentLength(startSegment, startSegment.TotalLength - charactersRemovedInStartLine + charactersLeftInEndLine);
      startSegment.DelimiterLength = endSegment.DelimiterLength;
      // remove all segments between startSegment (excl.) and endSegment (incl.)
      it.MoveNext();
      LineSegment segmentToRemove;
      do
      {
        segmentToRemove = it.Current;
        it.MoveNext();
        _lineSegments.RemoveSegment(segmentToRemove);
        segmentToRemove.Deleted(ref deferredEventList);
      } while (segmentToRemove != endSegment);
    }


    void InsertInternal(int offset, string text)
    {
      LineSegment segment = _lineSegments.GetByOffset(offset);
      DelimiterSegment ds = NextDelimiter(text, 0);
      if (ds == null)
      {
        // no newline is being inserted, all text is inserted in a single line
        segment.InsertedLinePart(offset - segment.Offset, text.Length);
        SetSegmentLength(segment, segment.TotalLength + text.Length);
        return;
      }
      LineSegment firstLine = segment;
      firstLine.InsertedLinePart(offset - firstLine.Offset, ds.Offset);
      int lastDelimiterEnd = 0;
      while (ds != null)
      {
        // split line segment at line delimiter
        int lineBreakOffset = offset + ds.Offset + ds.Length;
        int segmentOffset = segment.Offset;
        int lengthAfterInsertionPos = segmentOffset + segment.TotalLength - (offset + lastDelimiterEnd);
        _lineSegments.SetSegmentLength(segment, lineBreakOffset - segmentOffset);
        LineSegment newSegment = _lineSegments.InsertSegmentAfter(segment, lengthAfterInsertionPos);
        segment.DelimiterLength = ds.Length;

        segment = newSegment;
        lastDelimiterEnd = ds.Offset + ds.Length;

        ds = NextDelimiter(text, lastDelimiterEnd);
      }
      firstLine.SplitTo(segment);
      // insert rest after last delimiter
      if (lastDelimiterEnd != text.Length)
      {
        segment.InsertedLinePart(0, text.Length - lastDelimiterEnd);
        SetSegmentLength(segment, segment.TotalLength + text.Length - lastDelimiterEnd);
      }
    }


    void SetSegmentLength(LineSegment segment, int newTotalLength)
    {
      int delta = newTotalLength - segment.TotalLength;
      if (delta != 0)
      {
        _lineSegments.SetSegmentLength(segment, newTotalLength);
        OnLineLengthChanged(new LineLengthChangedEventArgs(_document, segment, delta));
      }
    }


    void RunHighlighter(int firstLine, int lineCount)
    {
      if (_highlightingStrategy != null)
      {
        List<LineSegment> markLines = new List<LineSegment>();
        LineSegmentTree.Enumerator it = _lineSegments.GetEnumeratorForIndex(firstLine);
        for (int i = 0; i < lineCount && it.IsValid; i++)
        {
          markLines.Add(it.Current);
          it.MoveNext();
        }
        _highlightingStrategy.MarkTokens(_document, markLines);
      }
    }


    public void SetContent(string text)
    {
      _lineSegments.Clear();
      if (text != null)
      {
        Replace(0, 0, text);
      }
    }


    public int GetVisibleLine(int logicalLineNumber)
    {
      if (!_document.TextEditorProperties.EnableFolding)
      {
        return logicalLineNumber;
      }

      int visibleLine = 0;
      int foldEnd = 0;
      IList<Fold> foldings = _document.FoldingManager.GetTopLevelFoldedFoldings();
      foreach (Fold fold in foldings)
      {
        if (fold.StartLine >= logicalLineNumber)
          break;

        if (fold.StartLine >= foldEnd)
        {
          visibleLine += fold.StartLine - foldEnd;
          if (fold.EndLine > logicalLineNumber)
            return visibleLine;

          foldEnd = fold.EndLine;
        }
      }
      visibleLine += logicalLineNumber - foldEnd;
      return visibleLine;
    }


    public int GetFirstLogicalLine(int visibleLineNumber)
    {
      if (!_document.TextEditorProperties.EnableFolding)
        return visibleLineNumber;

      int v = 0;
      int foldEnd = 0;
      IList<Fold> foldings = _document.FoldingManager.GetTopLevelFoldedFoldings();
      foreach (Fold fold in foldings)
      {
        if (fold.StartLine >= foldEnd)
        {
          if (v + fold.StartLine - foldEnd >= visibleLineNumber)
            break;

          v += fold.StartLine - foldEnd;
          foldEnd = fold.EndLine;
        }
      }
      return foldEnd + visibleLineNumber - v;
    }

    public int GetLastLogicalLine(int visibleLineNumber)
    {
      if (!_document.TextEditorProperties.EnableFolding)
        return visibleLineNumber;

      return GetFirstLogicalLine(visibleLineNumber + 1) - 1;
    }


    // TODO : speedup the next/prev visible line search
    // HOW? : save the foldings in a sorted list and lookup the
    //        line numbers in this list
    public int GetNextVisibleLineAfter(int lineNumber, int lineCount)
    {
      int curLineNumber = lineNumber;
      if (_document.TextEditorProperties.EnableFolding)
      {
        for (int i = 0; i < lineCount && curLineNumber < TotalNumberOfLines; ++i)
        {
          ++curLineNumber;
          while (curLineNumber < TotalNumberOfLines && (curLineNumber >= _lineSegments.Count || !_document.FoldingManager.IsLineVisible(curLineNumber)))
            ++curLineNumber;
        }
      }
      else
      {
        curLineNumber += lineCount;
      }
      return Math.Min(TotalNumberOfLines - 1, curLineNumber);
    }


    public int GetNextVisibleLineBefore(int lineNumber, int lineCount)
    {
      int curLineNumber = lineNumber;
      if (_document.TextEditorProperties.EnableFolding)
      {
        for (int i = 0; i < lineCount; ++i)
        {
          --curLineNumber;
          while (curLineNumber >= 0 && !_document.FoldingManager.IsLineVisible(curLineNumber))
            --curLineNumber;
        }
      }
      else
      {
        curLineNumber -= lineCount;
      }
      return Math.Max(0, curLineNumber);
    }


    DelimiterSegment NextDelimiter(string text, int offset)
    {
      for (int i = offset; i < text.Length; i++)
      {
        switch (text[i])
        {
          case '\r':
            if (i + 1 < text.Length)
            {
              if (text[i + 1] == '\n')
              {
                _delimiterSegment.Offset = i;
                _delimiterSegment.Length = 2;
                return _delimiterSegment;
              }
            }
            #if DATACONSISTENCYTEST
						Debug.Assert(false, "Found lone \\r, data consistency problems?");
            #endif
            goto case '\n';
          case '\n':
            _delimiterSegment.Offset = i;
            _delimiterSegment.Length = 1;
            return _delimiterSegment;
        }
      }
      return null;
    }


    void OnLineCountChanged(LineCountChangedEventArgs e)
    {
      if (LineCountChanged != null)
        LineCountChanged(this, e);
    }


    void OnLineLengthChanged(LineLengthChangedEventArgs e)
    {
      if (LineLengthChanged != null)
        LineLengthChanged(this, e);
    }


    void OnLineDeleted(LineEventArgs e)
    {
      if (LineDeleted != null)
        LineDeleted(this, e);
    }
  }
}
