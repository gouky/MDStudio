using System;
using System.Runtime.Serialization;

namespace DigitalRune.Windows.TextEditor.Highlighting
{
	/// <summary>
	/// Indicates that the highlighting definition that was tried to load was invalid.
	/// </summary>
  /// <remarks>
  /// You get this exception only once per highlighting definition, after that the definition
  /// is replaced with the default highlighter.
  /// </remarks>
	[Serializable]
	public class HighlightingDefinitionInvalidException : Exception
	{
    /// <summary>
    /// Initializes a new instance of the <see cref="HighlightingDefinitionInvalidException"/> class.
    /// </summary>
		public HighlightingDefinitionInvalidException()
		{
		}

    /// <summary>
    /// Initializes a new instance of the <see cref="HighlightingDefinitionInvalidException"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
		public HighlightingDefinitionInvalidException(string message) 
      : base(message)
		{
		}

    /// <summary>
    /// Initializes a new instance of the <see cref="HighlightingDefinitionInvalidException"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="innerException">The inner exception.</param>
		public HighlightingDefinitionInvalidException(string message, Exception innerException) 
      : base(message, innerException)
		{
		}

    /// <summary>
    /// Initializes a new instance of the <see cref="HighlightingDefinitionInvalidException"/> class.
    /// </summary>
    /// <param name="info">The <see cref="SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
    /// <param name="context">The <see cref="StreamingContext"></see> that contains contextual information about the source or destination.</param>
    /// <exception cref="SerializationException">The class name is null or <see cref="P:System.Exception.HResult"></see> is zero (0). </exception>
    /// <exception cref="ArgumentNullException">The info parameter is null. </exception>
		protected HighlightingDefinitionInvalidException(SerializationInfo info, StreamingContext context) 
      : base(info, context)
		{
		}
	}
}
