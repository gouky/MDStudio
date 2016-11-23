using System;
using System.Collections.Generic;


namespace DigitalRune.Windows.TextEditor.Highlighting
{
  /// <summary>
  /// A stack of Span instances. 
  /// </summary>
  /// <remarks>
  /// Works like Stack&lt;Span&gt;, but can be cloned quickly because it is 
  /// implemented as linked list.
  /// </remarks>
  public sealed class SpanStack : ICloneable, IEnumerable<Span>
  {
    internal sealed class StackNode
    {
      public readonly StackNode Previous;
      public readonly Span Data;

      public StackNode(StackNode previous, Span data)
      {
        Previous = previous;
        Data = data;
      }
    }


    private StackNode _top;


    /// <summary>
    /// Gets a value indicating whether stack is empty.
    /// </summary>
    /// <value><see langword="true"/> if the stack is empty; otherwise, <see langword="false"/>.</value>
    public bool IsEmpty
    {
      get { return _top == null; }
    }


    /// <summary>
    /// Removes a span from the top of the stack.
    /// </summary>
    /// <returns>The span on top of the stack.</returns>
    public Span Pop()
    {
      Span s = _top.Data;
      _top = _top.Previous;
      return s;
    }


    /// <summary>
    /// Returns the span on top of the stack (but does not remove it from the stack).
    /// </summary>
    /// <returns>The span on top of the stack.</returns>
    public Span Peek()
    {
      return _top.Data;
    }


    /// <summary>
    /// Puts a span on top of the stack.
    /// </summary>
    /// <param name="s">The span.</param>
    public void Push(Span s)
    {
      _top = new StackNode(_top, s);
    }


    /// <summary>
    /// Clones this instance.
    /// </summary>
    /// <returns>A clone of this stack.</returns>
    public SpanStack Clone()
    {
      SpanStack n = new SpanStack();
      n._top = _top;
      return n;
    }


    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>
    /// A new object that is a copy of this instance.
    /// </returns>
    object ICloneable.Clone()
    {
      return Clone();
    }


    /// <summary>
    /// Gets the enumerator for this stack.
    /// </summary>
    /// <returns>The enumerator.</returns>
    public Enumerator GetEnumerator()
    {
      return new Enumerator(new StackNode(_top, null));
    }


    IEnumerator<Span> IEnumerable<Span>.GetEnumerator()
    {
      return GetEnumerator();
    }


    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }


    /// <summary>
    /// Enumerates a <see cref="SpanStack"/> from top to bottom of the stack.
    /// </summary>
    public struct Enumerator : IEnumerator<Span>
    {
      private StackNode _currentNode;

      internal Enumerator(StackNode node)
      {
        _currentNode = node;
      }

      /// <summary>
      /// Gets the current span.
      /// </summary>
      /// <value>The current.</value>
      public Span Current
      {
        get { return _currentNode.Data; }
      }

      object System.Collections.IEnumerator.Current
      {
        get { return _currentNode.Data; }
      }

      /// <summary>
      /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
      /// </summary>
      public void Dispose()
      {
        _currentNode = null;
      }

      /// <summary>
      /// Advances the enumerator to the next element of the stack (top-down).
      /// </summary>
      /// <returns>
      /// <see langword="true"/> if the enumerator was successfully advanced to the next element; <see langword="false"/> if the enumerator has passed the end of the collection.
      /// </returns>
      /// <exception cref="InvalidOperationException">The collection was modified after the enumerator was created.</exception>
      public bool MoveNext()
      {
        _currentNode = _currentNode.Previous;
        return _currentNode != null;
      }

      /// <summary>
      /// Sets the enumerator to its initial position, which is before the first element in the collection.
      /// </summary>
      /// <exception cref="InvalidOperationException">The collection was modified after the enumerator was created. </exception>
      public void Reset()
      {
        throw new NotSupportedException();
      }
    }
  }
}
