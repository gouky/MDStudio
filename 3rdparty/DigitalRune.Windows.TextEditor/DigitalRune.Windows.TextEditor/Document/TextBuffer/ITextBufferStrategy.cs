namespace DigitalRune.Windows.TextEditor.TextBuffer
{
	/// <summary>
	/// Describes a sequence of characters that can be edited. 	
	/// </summary>
	public interface ITextBufferStrategy
	{
    /// <summary>
    /// Gets the length of the sequence of characters.
    /// </summary>
    /// <value>
    /// The current length of the sequence of characters that can be edited.
    /// </value>
		int Length { get; }
		
		/// <summary>
		/// Inserts a string of characters into the sequence.
		/// </summary>
		/// <param name="offset">
		/// Offset where to insert the string.
		/// </param>
		/// <param name="text">
		/// Text to be inserted.
		/// </param>
		void Insert(int offset, string text);
		
		/// <summary>
		/// Removes some portion of the sequence.
		/// </summary>
		/// <param name="offset">
		/// Offset of the remove.
		/// </param>
		/// <param name="length">
		/// Number of characters to remove.
		/// </param>
		void Remove(int offset, int length);
		
		/// <summary>
		/// Replace some portion of the sequence.
		/// </summary>
		/// <param name="offset">
		/// Offset.
		/// </param>
		/// <param name="length">
		/// Number of characters to replace.
		/// </param>
		/// <param name="text">
		/// Text to be replaced with.
		/// </param>
		void Replace(int offset, int length, string text);

    /// <summary>
    /// Fetches a string of characters contained in the sequence.
    /// </summary>
    /// <param name="offset">Offset into the sequence to fetch</param>
    /// <param name="length">Number of characters to copy.</param>
    /// <returns>The string at the specified offset.</returns>
		string GetText(int offset, int length);

    /// <summary>
    /// Returns a specific character of the sequence.
    /// </summary>
    /// <param name="offset">Offset of the char to get.</param>
    /// <returns>The character at the specified offset.</returns>
		char GetCharAt(int offset);
		
		/// <summary>
		/// This method sets the stored content.
		/// </summary>
		/// <param name="text">
		/// The string that represents the character sequence.
		/// </param>
		void SetContent(string text);
	}
}
