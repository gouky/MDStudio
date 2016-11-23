using System.Collections.Generic;


namespace DigitalRune.Windows.TextEditor.Utilities
{
  internal interface IRedBlackTreeHost<T> : IComparer<T>
  {
    bool Equals(T a, T b);
    void UpdateAfterChildrenChange(RedBlackTreeNode<T> node);
    void UpdateAfterRotateLeft(RedBlackTreeNode<T> node);
    void UpdateAfterRotateRight(RedBlackTreeNode<T> node);
  }
}
