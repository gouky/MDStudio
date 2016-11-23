namespace DigitalRune.Windows.TextEditor.Utilities
{
  internal sealed class RedBlackTreeNode<T>
  {
    internal RedBlackTreeNode<T> Left, Right, Parent;
    internal T Value;
    internal bool Color;


    internal RedBlackTreeNode(T value)
    {
      Value = value;
    }


    internal RedBlackTreeNode<T> LeftMost
    {
      get
      {
        RedBlackTreeNode<T> node = this;
        while (node.Left != null)
          node = node.Left;
        return node;
      }
    }


    internal RedBlackTreeNode<T> RightMost
    {
      get
      {
        RedBlackTreeNode<T> node = this;
        while (node.Right != null)
          node = node.Right;
        return node;
      }
    }
  }
}