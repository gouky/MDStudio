using System;
using System.Collections.Generic;


namespace DigitalRune.Windows.TextEditor.Utilities
{
  internal struct RedBlackTreeIterator<T> : IEnumerator<T>
  {
    internal RedBlackTreeNode<T> Node;


    internal RedBlackTreeIterator(RedBlackTreeNode<T> node)
    {
      Node = node;
    }


    public bool IsValid
    {
      get { return Node != null; }
    }


    public T Current
    {
      get
      {
        if (Node != null)
          return Node.Value;
        else
          throw new InvalidOperationException();
      }
    }


    object System.Collections.IEnumerator.Current
    {
      get { return Current; }
    }


    void IDisposable.Dispose()
    {
    }


    void System.Collections.IEnumerator.Reset()
    {
      throw new NotSupportedException();
    }


    public bool MoveNext()
    {
      if (Node == null)
        return false;
      if (Node.Right != null)
      {
        Node = Node.Right.LeftMost;
      }
      else
      {
        RedBlackTreeNode<T> oldNode;
        do
        {
          oldNode = Node;
          Node = Node.Parent;
          // we are on the way up from the right part, don't output node again
        } while (Node != null && Node.Right == oldNode);
      }
      return Node != null;
    }


    public bool MoveBack()
    {
      if (Node == null)
        return false;
      if (Node.Left != null)
      {
        Node = Node.Left.RightMost;
      }
      else
      {
        RedBlackTreeNode<T> oldNode;
        do
        {
          oldNode = Node;
          Node = Node.Parent;
          // we are on the way up from the left part, don't output node again
        } while (Node != null && Node.Left == oldNode);
      }
      return Node != null;
    }
  }
}
