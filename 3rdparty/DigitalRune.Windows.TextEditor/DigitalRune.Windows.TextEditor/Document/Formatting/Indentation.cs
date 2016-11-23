using System;
using System.Collections.Generic;
using System.Text;


namespace DigitalRune.Windows.TextEditor.Formatting
{
  internal sealed class IndentationSettings
  {
    public string IndentString = "\t";

    /// <summary>Leave empty lines empty.</summary>
    public bool LeaveEmptyLines = true;
  }


  internal sealed class CSharpIndentationReformatter
  {
    public struct Block
    {
      public string OuterIndent;
      public string InnerIndent;
      public string LastWord;
      public char Bracket;
      public bool Continuation;
      public bool OneLineBlock;
      public int StartLine;

      public void Indent(IndentationSettings set)
      {
        Indent(set, set.IndentString);
      }

      public void Indent(IndentationSettings set, string str)
      {
        OuterIndent = InnerIndent;
        InnerIndent += str;
        Continuation = false;
        OneLineBlock = false;
        LastWord = "";
      }
    }


    private static readonly Dictionary<string, object> _singleStatementKeywords = new Dictionary<string, object>();
    private StringBuilder _wordBuilder;
    private Stack<Block> _blocks;       // blocks contains all blocks outside of the current
    private Block _block;               // block is the current block
    private bool _inString;
    private bool _inChar;
    private bool _verbatim;
    private bool _escape;
    private bool _lineComment;
    private bool _blockComment;
    private char _lastRealChar = ' ';   // last non-comment char


    static CSharpIndentationReformatter()
    {
      _singleStatementKeywords.Add("if", null);
      _singleStatementKeywords.Add("for", null);
      _singleStatementKeywords.Add("while", null);
      _singleStatementKeywords.Add("do", null);
      _singleStatementKeywords.Add("foreach", null);
      _singleStatementKeywords.Add("using", null);
      _singleStatementKeywords.Add("lock", null);
    }


    static bool IsSingleStatementKeyword(string keyword)
    {
      return _singleStatementKeywords.ContainsKey(keyword);
    }


    public void Reformat(IDocumentAccessor document, IndentationSettings settings)
    {
      Init();
      while (document.Next())
        Step(document, settings);
    }


    public void Init()
    {
      _wordBuilder = new StringBuilder();
      _blocks = new Stack<Block>();
      _block = new Block();
      _block.InnerIndent = "";
      _block.OuterIndent = "";
      _block.Bracket = '{';
      _block.Continuation = false;
      _block.LastWord = "";
      _block.OneLineBlock = false;
      _block.StartLine = 0;

      _inString = false;
      _inChar = false;
      _verbatim = false;
      _escape = false;

      _lineComment = false;
      _blockComment = false;

      _lastRealChar = ' '; // last non-comment char
    }


    public void Step(IDocumentAccessor doc, IndentationSettings set)
    {
      string line = doc.Text;
      if (set.LeaveEmptyLines && line.Length == 0) 
        return; // leave empty lines empty
      line = line.TrimStart();

      StringBuilder indent = new StringBuilder();
      if (line.Length == 0)
      {
        // Special treatment for empty lines:
        if (_blockComment || (_inString && _verbatim))
          return;
        indent.Append(_block.InnerIndent);
        if (_block.OneLineBlock)
          indent.Append(set.IndentString);
        if (_block.Continuation)
          indent.Append(set.IndentString);
        if (doc.Text != indent.ToString())
          doc.Text = indent.ToString();
        return;
      }

      if (TrimEnd(doc))
        line = doc.Text.TrimStart();

      Block oldBlock = _block;
      bool startInComment = _blockComment;
      bool startInString = (_inString && _verbatim);

      #region Parse char by char
      _lineComment = false;
      _inChar = false;
      _escape = false;
      if (!_verbatim) 
        _inString = false;

      _lastRealChar = '\n';

      char c = ' ';
      char nextchar = line[0];
      for (int i = 0; i < line.Length; i++)
      {
        if (_lineComment) break; // cancel parsing current line

        char lastchar = c;
        c = nextchar;
        nextchar = (i + 1 < line.Length) ? line[i + 1] : '\n';

        if (_escape)
        {
          _escape = false;
          continue;
        }

        #region Check for comment/string chars
        switch (c)
        {
          case '/':
            if (_blockComment && lastchar == '*')
              _blockComment = false;
            if (!_inString && !_inChar)
            {
              if (!_blockComment && nextchar == '/')
                _lineComment = true;
              if (!_lineComment && nextchar == '*')
                _blockComment = true;
            }
            break;
          case '#':
            if (!(_inChar || _blockComment || _inString))
              _lineComment = true;
            break;
          case '"':
            if (!(_inChar || _lineComment || _blockComment))
            {
              _inString = !_inString;
              if (!_inString && _verbatim)
              {
                if (nextchar == '"')
                {
                  _escape = true; // skip escaped quote
                  _inString = true;
                }
                else
                {
                  _verbatim = false;
                }
              }
              else if (_inString && lastchar == '@')
              {
                _verbatim = true;
              }
            }
            break;
          case '\'':
            if (!(_inString || _lineComment || _blockComment))
            {
              _inChar = !_inChar;
            }
            break;
          case '\\':
            if ((_inString && !_verbatim) || _inChar)
              _escape = true; // skip next character
            break;
        }
        #endregion

        if (_lineComment || _blockComment || _inString || _inChar)
        {
          if (_wordBuilder.Length > 0)
            _block.LastWord = _wordBuilder.ToString();
          _wordBuilder.Length = 0;
          continue;
        }

        if (!Char.IsWhiteSpace(c) && c != '[' && c != '/')
        {
          if (_block.Bracket == '{')
            _block.Continuation = true;
        }

        if (Char.IsLetterOrDigit(c))
        {
          _wordBuilder.Append(c);
        }
        else
        {
          if (_wordBuilder.Length > 0)
            _block.LastWord = _wordBuilder.ToString();
          _wordBuilder.Length = 0;
        }

        #region Push/Pop the blocks
        switch (c)
        {
          case '{':
            _block.OneLineBlock = false;
            _blocks.Push(_block);
            _block.StartLine = doc.LineNumber;
            if (_block.LastWord == "switch")
            {
              _block.Indent(set, set.IndentString + set.IndentString);
              /* oldBlock refers to the previous line, not the previous block
               * The block we want is not available anymore because it was never pushed.
               * } else if (oldBlock.OneLineBlock) {
              // Inside a one-line-block is another statement
              // with a full block: indent the inner full block
              // by one additional level
              block.Indent(set, set.IndentString + set.IndentString);
              block.OuterIndent += set.IndentString;
              // Indent current line if it starts with the '{' character
              if (i == 0) {
                oldBlock.InnerIndent += set.IndentString;
              }*/
            }
            else
            {
              _block.Indent(set);
            }
            _block.Bracket = '{';
            break;
          case '}':
            while (_block.Bracket != '{')
            {
              if (_blocks.Count == 0) break;
              _block = _blocks.Pop();
            }
            if (_blocks.Count == 0) break;
            _block = _blocks.Pop();
            _block.Continuation = false;
            _block.OneLineBlock = false;
            break;
          case '(':
          case '[':
            _blocks.Push(_block);
            if (_block.StartLine == doc.LineNumber)
              _block.InnerIndent = _block.OuterIndent;
            else
              _block.StartLine = doc.LineNumber;
            _block.Indent(set,
                         (oldBlock.OneLineBlock ? set.IndentString : "") +
                         (oldBlock.Continuation ? set.IndentString : "") +
                         (i == line.Length - 1 ? set.IndentString : new String(' ', i + 1)));
            _block.Bracket = c;
            break;
          case ')':
            if (_blocks.Count == 0) break;
            if (_block.Bracket == '(')
            {
              _block = _blocks.Pop();
              if (IsSingleStatementKeyword(_block.LastWord))
                _block.Continuation = false;
            }
            break;
          case ']':
            if (_blocks.Count == 0) break;
            if (_block.Bracket == '[')
              _block = _blocks.Pop();
            break;
          case ';':
          case ',':
            _block.Continuation = false;
            _block.OneLineBlock = false;
            break;
          case ':':
            if (_block.LastWord == "case" || line.StartsWith("case ") || line.StartsWith(_block.LastWord + ":"))
            {
              _block.Continuation = false;
              _block.OneLineBlock = false;
            }
            break;
        }

        if (!Char.IsWhiteSpace(c))
        {
          // register this char as last char
          _lastRealChar = c;
        }
        #endregion
      }
      #endregion

      if (_wordBuilder.Length > 0)
        _block.LastWord = _wordBuilder.ToString();
      _wordBuilder.Length = 0;

      if (startInString) return;
      if (startInComment && line[0] != '*') return;
      if (doc.Text.StartsWith("//\t") || doc.Text == "//")
        return;

      if (line[0] == '}')
      {
        indent.Append(oldBlock.OuterIndent);
        oldBlock.OneLineBlock = false;
        oldBlock.Continuation = false;
      }
      else
      {
        indent.Append(oldBlock.InnerIndent);
      }

      if (indent.Length > 0 && oldBlock.Bracket == '(' && line[0] == ')')
      {
        indent.Remove(indent.Length - 1, 1);
      }
      else if (indent.Length > 0 && oldBlock.Bracket == '[' && line[0] == ']')
      {
        indent.Remove(indent.Length - 1, 1);
      }

      if (line[0] == ':')
      {
        oldBlock.Continuation = true;
      }
      else if (_lastRealChar == ':' && indent.Length >= set.IndentString.Length)
      {
        if (_block.LastWord == "case" || line.StartsWith("case ") || line.StartsWith(_block.LastWord + ":"))
          indent.Remove(indent.Length - set.IndentString.Length, set.IndentString.Length);
      }
      else if (_lastRealChar == ')')
      {
        if (IsSingleStatementKeyword(_block.LastWord))
        {
          _block.OneLineBlock = true;
        }
      }
      else if (_lastRealChar == 'e' && _block.LastWord == "else")
      {
        _block.OneLineBlock = true;
        _block.Continuation = false;
      }

      if (doc.ReadOnly)
      {
        // We can't change the current line, but we should accept the existing
        // indentation if possible (=if the current statement is not a multiline
        // statement).
        if (!oldBlock.Continuation && !oldBlock.OneLineBlock &&
            oldBlock.StartLine == _block.StartLine &&
            _block.StartLine < doc.LineNumber && _lastRealChar != ':')
        {
          // use indent StringBuilder to get the indentation of the current line
          indent.Length = 0;
          line = doc.Text; // get untrimmed line
          for (int i = 0; i < line.Length; ++i)
          {
            if (!Char.IsWhiteSpace(line[i]))
              break;
            indent.Append(line[i]);
          }
          // /* */ multiline comments have an extra space - do not count it
          // for the block's indentation.
          if (startInComment && indent.Length > 0 && indent[indent.Length - 1] == ' ')
          {
            indent.Length -= 1;
          }
          _block.InnerIndent = indent.ToString();
        }
        return;
      }

      if (line[0] != '{')
      {
        if (line[0] != ')' && oldBlock.Continuation && oldBlock.Bracket == '{')
          indent.Append(set.IndentString);
        if (oldBlock.OneLineBlock)
          indent.Append(set.IndentString);
      }

      // this is only for blockcomment lines starting with *,
      // all others keep their old indentation
      if (startInComment)
        indent.Append(' ');

      if (indent.Length != (doc.Text.Length - line.Length) ||
          !doc.Text.StartsWith(indent.ToString()) ||
          Char.IsWhiteSpace(doc.Text[indent.Length]))
      {
        doc.Text = indent + line;
      }
    }


    static bool TrimEnd(IDocumentAccessor doc)
    {
      string line = doc.Text;
      if (!Char.IsWhiteSpace(line[line.Length - 1])) return false;

      // one space after an empty comment is allowed
      if (line.EndsWith("// ") || line.EndsWith("* "))
        return false;

      doc.Text = line.TrimEnd();
      return true;
    }
  }
}
