namespace DigitalRune.Windows.TextEditor.Undo
{
	/// <summary>
	/// This Interface describes a the basic Undo/Redo operation
	/// all undoable operations must implement this interface.
	/// </summary>
	public interface IUndoableOperation
	{
		/// <summary>
		/// Undoes the last operation
		/// </summary>
		void Undo();
		
		/// <summary>
		/// Redoes the last operation
		/// </summary>
		void Redo();
	}
}
