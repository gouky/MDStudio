using System;
using System.Diagnostics;
using System.Text;


namespace DigitalRune.Windows.TextEditor.TextBuffer
{
  /// <summary>
  /// Implements a text buffer using the 'gap text' strategy.
  /// </summary>
  /// <remark>
  /// <para>
  /// Internal implementation: The text buffer is implemented as an array of 
  /// characters. A gap is inserted at the position where characters are inserted
  /// or removed. This minimizes the number of copy operations and memory 
  /// reallocations. Subsequent insert operations are just inserted into the 
  /// gap - no memory copy/allocation. Only when the insert position changes or 
  /// the gap is full the buffer needs to be restructured.
  /// </para>
  /// <para>
  /// <b>Warning: </b><see cref="GapTextBufferStrategy"/> is not thread-safe.
  /// </para>
  /// </remark>
  public class GapTextBufferStrategy : ITextBufferStrategy
  {
#if DEBUG
    private readonly int _creatorThread = System.Threading.Thread.CurrentThread.ManagedThreadId;
#endif

    [Conditional("DEBUG")]
    void CheckThread()
    {
#if DEBUG
      if (System.Threading.Thread.CurrentThread.ManagedThreadId != _creatorThread)
        throw new InvalidOperationException("GapTextBufferStategy is not thread-safe!");
#endif
    }


    private const int minGapLength = 128;
    private const int maxGapLength = 2048;

    private char[] buffer = new char[0];
    string cachedContent;

    private int gapBeginOffset;
    private int gapEndOffset;
    private int gapLength;  // gapLength == gapEndOffset - gapBeginOffset


    /// <summary>
    /// Gets the length of the sequence of characters.
    /// </summary>
    /// <value>
    /// The current length of the sequence of characters that can be edited.
    /// </value>
    public int Length
    {
      get { return buffer.Length - gapLength; }
    }


    /// <summary>
    /// This method sets the stored content.
    /// </summary>
    /// <param name="text">The string that represents the character sequence.</param>
    public void SetContent(string text)
    {
      if (text == null)
        text = String.Empty;

      cachedContent = text; 
      buffer = text.ToCharArray();
      gapBeginOffset = gapEndOffset = gapLength = 0;
    }


    /// <summary>
    /// Returns a specific character of the sequence.
    /// </summary>
    /// <param name="offset">Offset of the char to get.</param>
    /// <returns>The character at the specified offset.</returns>
    public char GetCharAt(int offset)
    {
      CheckThread();

      if (offset < 0 || offset >= Length)
      {
        throw new ArgumentOutOfRangeException("offset", offset, "0 <= offset < " + Length);
      }
      return offset < gapBeginOffset ? buffer[offset] : buffer[offset + gapLength];
    }


    /// <summary>
    /// Fetches a string of characters contained in the sequence.
    /// </summary>
    /// <param name="offset">Offset into the sequence to fetch</param>
    /// <param name="length">Number of characters to copy.</param>
    /// <returns>The string at the specified offset.</returns>
    public string GetText(int offset, int length)
    {
      CheckThread();

      if (offset < 0 || offset > Length)
        throw new ArgumentOutOfRangeException("offset", offset, "0 <= offset <= " + Length);

      if (length < 0 || offset + length > Length)
        throw new ArgumentOutOfRangeException("length", length, "0 <= length, offset(" + offset + ")+length <= " + Length);

      if (offset == 0 && length == Length)
      {
        if (cachedContent != null)
          return cachedContent;
        else
          return cachedContent = GetTextInternal(offset, length);
      }
      else
      {
        return GetTextInternal(offset, length);
      }
    }


    string GetTextInternal(int offset, int length)
    {
      int end = offset + length;

      if (end < gapBeginOffset)
        return new string(buffer, offset, length);

      if (offset > gapBeginOffset)
        return new string(buffer, offset + gapLength, length);

      int block1Size = gapBeginOffset - offset;
      int block2Size = end - gapBeginOffset;

      StringBuilder buf = new StringBuilder(block1Size + block2Size);
      buf.Append(buffer, offset, block1Size);
      buf.Append(buffer, gapEndOffset, block2Size);
      return buf.ToString();
    }


    /// <summary>
    /// Inserts a string of characters into the sequence.
    /// </summary>
    /// <param name="offset">Offset where to insert the string.</param>
    /// <param name="text">Text to be inserted.</param>
    public void Insert(int offset, string text)
    {
      Replace(offset, 0, text);
    }


    /// <summary>
    /// Removes some portion of the sequence.
    /// </summary>
    /// <param name="offset">Offset of the remove.</param>
    /// <param name="length">Number of characters to remove.</param>
    public void Remove(int offset, int length)
    {
      Replace(offset, length, String.Empty);
    }


    /// <summary>
    /// Replace some portion of the sequence.
    /// </summary>
    /// <param name="offset">Offset.</param>
    /// <param name="length">Number of characters to replace.</param>
    /// <param name="text">Text to be replaced with.</param>
    public void Replace(int offset, int length, string text)
    {
      if (text == null)
        text = String.Empty;

      CheckThread();

      if (offset < 0 || offset > Length)
        throw new ArgumentOutOfRangeException("offset", offset, "0 <= offset <= " + Length);

      if (length < 0 || offset + length > Length)
        throw new ArgumentOutOfRangeException("length", length, "0 <= length, offset+length <= " + Length);

      cachedContent = null;

      // Math.Max is used so that if we need to resize the array
      // the new array has enough space for all old chars
      PlaceGap(offset, text.Length - length);
      gapEndOffset += length; // delete removed text
      text.CopyTo(0, buffer, gapBeginOffset, text.Length);
      gapBeginOffset += text.Length;
      gapLength = gapEndOffset - gapBeginOffset;
      if (gapLength > maxGapLength)
      {
        MakeNewBuffer(gapBeginOffset, minGapLength);
      }
    }

    void PlaceGap(int newGapOffset, int minRequiredGapLength)
    {
      if (gapLength < minRequiredGapLength)
      {
        // enlarge gap
        MakeNewBuffer(newGapOffset, minRequiredGapLength);
      }
      else
      {
        while (newGapOffset < gapBeginOffset)
        {
          buffer[--gapEndOffset] = buffer[--gapBeginOffset];
        }
        while (newGapOffset > gapBeginOffset)
        {
          buffer[gapBeginOffset++] = buffer[gapEndOffset++];
        }
      }
    }

    void MakeNewBuffer(int newGapOffset, int newGapLength)
    {
      if (newGapLength < minGapLength) newGapLength = minGapLength;

      char[] newBuffer = new char[Length + newGapLength];
      if (newGapOffset < gapBeginOffset)
      {
        // gap is moving backwards

        // first part:
        Array.Copy(buffer, 0, newBuffer, 0, newGapOffset);
        // moving middle part:
        Array.Copy(buffer, newGapOffset, newBuffer, newGapOffset + newGapLength, gapBeginOffset - newGapOffset);
        // last part:
        Array.Copy(buffer, gapEndOffset, newBuffer, newBuffer.Length - (buffer.Length - gapEndOffset), buffer.Length - gapEndOffset);
      }
      else
      {
        // gap is moving forwards
        // first part:
        Array.Copy(buffer, 0, newBuffer, 0, gapBeginOffset);
        // moving middle part:
        Array.Copy(buffer, gapEndOffset, newBuffer, gapBeginOffset, newGapOffset - gapBeginOffset);
        // last part:
        int lastPartLength = newBuffer.Length - (newGapOffset + newGapLength);
        Array.Copy(buffer, buffer.Length - lastPartLength, newBuffer, newGapOffset + newGapLength, lastPartLength);
      }

      gapBeginOffset = newGapOffset;
      gapEndOffset = newGapOffset + newGapLength;
      gapLength = newGapLength;
      buffer = newBuffer;
    }
  }
}
