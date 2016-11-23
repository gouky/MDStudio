using System;
using System.Windows.Forms;


namespace DigitalRune.Windows.TextEditor.Utilities
{
	/// <summary>
	/// Accumulates mouse wheel deltas and reports the actual number of lines to scroll.
	/// </summary>
	internal class MouseWheelHandler
	{
		const int WHEEL_DELTA = 120;	
		int _mouseWheelDelta;


    /// <summary>
    /// Gets the amount of lines to scroll.
    /// </summary>
    /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
    /// <returns>The number of lines to scroll.</returns>
		public int GetScrollAmount(MouseEventArgs e)
		{
			// accumulate the delta to support high-resolution mice
			_mouseWheelDelta += e.Delta;
			
			int linesPerClick = Math.Max(SystemInformation.MouseWheelScrollLines, 1);
			
			int scrollDistance = _mouseWheelDelta * linesPerClick / WHEEL_DELTA;
			_mouseWheelDelta %= Math.Max(1, WHEEL_DELTA / linesPerClick);
			return scrollDistance;
		}
	}
}
