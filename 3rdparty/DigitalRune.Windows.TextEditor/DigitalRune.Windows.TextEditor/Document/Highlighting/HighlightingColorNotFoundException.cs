using System;
using System.Runtime.Serialization;

namespace DigitalRune.Windows.TextEditor.Highlighting
{
  /// <summary>
  /// Exception which occurs when a highlighting color is not found.
  /// </summary>
	[Serializable]
	public class HighlightingColorNotFoundException : Exception
	{
    /// <summary>
    /// Initializes a new instance of the <see cref="HighlightingColorNotFoundException"/> class.
    /// </summary>
		public HighlightingColorNotFoundException()
		{
		}

    /// <summary>
    /// Initializes a new instance of the <see cref="HighlightingColorNotFoundException"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
		public HighlightingColorNotFoundException(string message) 
      : base(message)
		{
		}

    /// <summary>
    /// Initializes a new instance of the <see cref="HighlightingColorNotFoundException"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="innerException">The inner exception.</param>
		public HighlightingColorNotFoundException(string message, Exception innerException) 
      : base(message, innerException)
		{
		}

    /// <summary>
    /// Initializes a new instance of the <see cref="HighlightingColorNotFoundException"/> class.
    /// </summary>
    /// <param name="info">The <see cref="SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
    /// <param name="context">The <see cref="StreamingContext"></see> that contains contextual information about the source or destination.</param>
    /// <exception cref="SerializationException">The class name is null or <see cref="P:System.Exception.HResult"></see> is zero (0). </exception>
    /// <exception cref="ArgumentNullException">The info parameter is null. </exception>
		protected HighlightingColorNotFoundException(SerializationInfo info, StreamingContext context) 
      : base(info, context)
		{
		}
	}
}
