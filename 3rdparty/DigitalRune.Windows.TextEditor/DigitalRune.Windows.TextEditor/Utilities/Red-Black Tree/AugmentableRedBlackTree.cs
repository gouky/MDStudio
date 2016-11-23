using System;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;


namespace DigitalRune.Windows.TextEditor.Utilities
{
  /// <summary>
  /// Description of RedBlackTree.
  /// </summary>
  internal sealed class AugmentableRedBlackTree<T, Host> : ICollection<T> where Host : IRedBlackTreeHost<T>
  {
    private const bool RED = true;
    private const bool BLACK = false;

    private readonly Host _host;
    private int _count;
    internal RedBlackTreeNode<T> Root;


    public AugmentableRedBlackTree(Host host)
    {
      if (host == null) 
        throw new ArgumentNullException("host");
      _host = host;
    }


    public int Count
    {
      get { return _count; }
    }


    public void Clear()
    {
      Root = null;
      _count = 0;
    }


    #region Debugging code
#if DEBUG
    /// <summary>
    /// Check tree for consistency and being balanced.
    /// </summary>
    [Conditional("DATACONSISTENCYTEST")]
    void CheckProperties()
    {
      int blackCount = -1;
      CheckNodeProperties(Root, null, RED, 0, ref blackCount);

      int nodeCount = 0;
      foreach (T value in this)
        nodeCount++;

      Debug.Assert(_count == nodeCount);
    }


    /*
    1. A node is either red or black.
    2. The root is black.
    3. All leaves are black. (The leaves are the NIL children.)
    4. Both children of every red node are black. (So every red node must have a black parent.)
    5. Every simple path from a node to a descendant leaf contains the same number of black nodes. (Not counting the leaf node.)
     */
    static void CheckNodeProperties(RedBlackTreeNode<T> node, RedBlackTreeNode<T> parentNode, bool parentColor, int blackCount, ref int expectedBlackCount)
    {
      if (node == null) return;

      Debug.Assert(node.Parent == parentNode);

      if (parentColor == RED)
        Debug.Assert(node.Color == BLACK);

      if (node.Color == BLACK)
        blackCount++;

      if (node.Left == null && node.Right == null)
      {
        // node is a leaf node:
        if (expectedBlackCount == -1)
          expectedBlackCount = blackCount;
        else
          Debug.Assert(expectedBlackCount == blackCount);
      }

      CheckNodeProperties(node.Left, node, node.Color, blackCount, ref expectedBlackCount);
      CheckNodeProperties(node.Right, node, node.Color, blackCount, ref expectedBlackCount);
    }


    public string GetTreeAsString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      AppendTreeToString(Root, stringBuilder, 0);
      return stringBuilder.ToString();
    }


    static void AppendTreeToString(RedBlackTreeNode<T> node, StringBuilder b, int indent)
    {
      if (node.Color == RED)
        b.Append("RED   ");
      else
        b.Append("BLACK ");
      b.AppendLine(node.Value.ToString());
      indent += 2;
      if (node.Left != null)
      {
        b.Append(' ', indent);
        b.Append("L: ");
        AppendTreeToString(node.Left, b, indent);
      }
      if (node.Right != null)
      {
        b.Append(' ', indent);
        b.Append("R: ");
        AppendTreeToString(node.Right, b, indent);
      }
    }
#endif
    #endregion


    #region Add
    public void Add(T item)
    {
      AddInternal(new RedBlackTreeNode<T>(item));
#if DEBUG
      CheckProperties();
#endif
    }


    void AddInternal(RedBlackTreeNode<T> newNode)
    {
      Debug.Assert(newNode.Color == BLACK);
      if (Root == null)
      {
        _count = 1;
        Root = newNode;
        return;
      }
      // Insert into the tree
      RedBlackTreeNode<T> parentNode = Root;
      while (true)
      {
        if (_host.Compare(newNode.Value, parentNode.Value) <= 0)
        {
          if (parentNode.Left == null)
          {
            InsertAsLeft(parentNode, newNode);
            return;
          }
          parentNode = parentNode.Left;
        }
        else
        {
          if (parentNode.Right == null)
          {
            InsertAsRight(parentNode, newNode);
            return;
          }
          parentNode = parentNode.Right;
        }
      }
    }


    internal void InsertAsLeft(RedBlackTreeNode<T> parentNode, RedBlackTreeNode<T> newNode)
    {
      Debug.Assert(parentNode.Left == null);
      parentNode.Left = newNode;
      newNode.Parent = parentNode;
      newNode.Color = RED;
      _host.UpdateAfterChildrenChange(parentNode);
      FixTreeOnInsert(newNode);
      _count++;
    }


    internal void InsertAsRight(RedBlackTreeNode<T> parentNode, RedBlackTreeNode<T> newNode)
    {
      Debug.Assert(parentNode.Right == null);
      parentNode.Right = newNode;
      newNode.Parent = parentNode;
      newNode.Color = RED;
      _host.UpdateAfterChildrenChange(parentNode);
      FixTreeOnInsert(newNode);
      _count++;
    }


    void FixTreeOnInsert(RedBlackTreeNode<T> node)
    {
      Debug.Assert(node != null);
      Debug.Assert(node.Color == RED);
      Debug.Assert(node.Left == null || node.Left.Color == BLACK);
      Debug.Assert(node.Right == null || node.Right.Color == BLACK);

      RedBlackTreeNode<T> parentNode = node.Parent;
      if (parentNode == null)
      {
        // we inserted in the root -> the node must be black
        // since this is a root node, making the node black increments the number of black nodes
        // on all paths by one, so it is still the same for all paths.
        node.Color = BLACK;
        return;
      }
      if (parentNode.Color == BLACK)
      {
        // if the parent node where we inserted was black, our red node is placed correctly.
        // since we inserted a red node, the number of black nodes on each path is unchanged
        // -> the tree is still balanced
        return;
      }
      // parentNode is red, so there is a conflict here!

      // because the root is black, parentNode is not the root -> there is a grandparent node
      RedBlackTreeNode<T> grandparentNode = parentNode.Parent;
      RedBlackTreeNode<T> uncleNode = Sibling(parentNode);
      if (uncleNode != null && uncleNode.Color == RED)
      {
        parentNode.Color = BLACK;
        uncleNode.Color = BLACK;
        grandparentNode.Color = RED;
        FixTreeOnInsert(grandparentNode);
        return;
      }
      // now we know: parent is red but uncle is black
      // First rotation:
      if (node == parentNode.Right && parentNode == grandparentNode.Left)
      {
        RotateLeft(parentNode);
        node = node.Left;
      }
      else if (node == parentNode.Left && parentNode == grandparentNode.Right)
      {
        RotateRight(parentNode);
        node = node.Right;
      }
      // because node might have changed, reassign variables:
      parentNode = node.Parent;
      grandparentNode = parentNode.Parent;

      // Now recolor a bit:
      parentNode.Color = BLACK;
      grandparentNode.Color = RED;
      // Second rotation:
      if (node == parentNode.Left && parentNode == grandparentNode.Left)
      {
        RotateRight(grandparentNode);
      }
      else
      {
        // because of the first rotation, this is guaranteed:
        Debug.Assert(node == parentNode.Right && parentNode == grandparentNode.Right);
        RotateLeft(grandparentNode);
      }
    }


    void ReplaceNode(RedBlackTreeNode<T> replacedNode, RedBlackTreeNode<T> newNode)
    {
      if (replacedNode.Parent == null)
      {
        Debug.Assert(replacedNode == Root);
        Root = newNode;
      }
      else
      {
        if (replacedNode.Parent.Left == replacedNode)
          replacedNode.Parent.Left = newNode;
        else
          replacedNode.Parent.Right = newNode;
      }
      if (newNode != null)
      {
        newNode.Parent = replacedNode.Parent;
      }
      replacedNode.Parent = null;
    }


    void RotateLeft(RedBlackTreeNode<T> p)
    {
      // let q be p's right child
      RedBlackTreeNode<T> q = p.Right;
      Debug.Assert(q != null);
      Debug.Assert(q.Parent == p);
      // set q to be the new root
      ReplaceNode(p, q);

      // set p's right child to be q's left child
      p.Right = q.Left;
      if (p.Right != null) p.Right.Parent = p;
      // set q's left child to be p
      q.Left = p;
      p.Parent = q;
      _host.UpdateAfterRotateLeft(p);
    }


    void RotateRight(RedBlackTreeNode<T> p)
    {
      // let q be p's left child
      RedBlackTreeNode<T> q = p.Left;
      Debug.Assert(q != null);
      Debug.Assert(q.Parent == p);
      // set q to be the new root
      ReplaceNode(p, q);

      // set p's left child to be q's right child
      p.Left = q.Right;
      if (p.Left != null) p.Left.Parent = p;
      // set q's right child to be p
      q.Right = p;
      p.Parent = q;
      _host.UpdateAfterRotateRight(p);
    }


    static RedBlackTreeNode<T> Sibling(RedBlackTreeNode<T> node)
    {
      if (node == node.Parent.Left)
        return node.Parent.Right;
      else
        return node.Parent.Left;
    }
    #endregion


    #region Remove
    public void RemoveAt(RedBlackTreeIterator<T> iterator)
    {
      RedBlackTreeNode<T> node = iterator.Node;
      if (node == null)
        throw new ArgumentException("Invalid iterator");
      while (node.Parent != null)
        node = node.Parent;
      if (node != Root)
        throw new ArgumentException("Iterator does not belong to this tree");
      RemoveNode(iterator.Node);
#if DEBUG
      CheckProperties();
#endif
    }


    internal void RemoveNode(RedBlackTreeNode<T> removedNode)
    {
      if (removedNode.Left != null && removedNode.Right != null)
      {
        // replace removedNode with it's in-order successor

        RedBlackTreeNode<T> leftMost = removedNode.Right.LeftMost;
        RemoveNode(leftMost); // remove leftMost from its current location

        // and overwrite the removedNode with it
        ReplaceNode(removedNode, leftMost);
        leftMost.Left = removedNode.Left;
        if (leftMost.Left != null) leftMost.Left.Parent = leftMost;
        leftMost.Right = removedNode.Right;
        if (leftMost.Right != null) leftMost.Right.Parent = leftMost;
        leftMost.Color = removedNode.Color;

        _host.UpdateAfterChildrenChange(leftMost);
        if (leftMost.Parent != null) _host.UpdateAfterChildrenChange(leftMost.Parent);
        return;
      }

      _count--;

      // now either removedNode.left or removedNode.right is null
      // get the remaining child
      RedBlackTreeNode<T> parentNode = removedNode.Parent;
      RedBlackTreeNode<T> childNode = removedNode.Left ?? removedNode.Right;
      ReplaceNode(removedNode, childNode);
      if (parentNode != null) _host.UpdateAfterChildrenChange(parentNode);
      if (removedNode.Color == BLACK)
      {
        if (childNode != null && childNode.Color == RED)
        {
          childNode.Color = BLACK;
        }
        else
        {
          FixTreeOnDelete(childNode, parentNode);
        }
      }
    }


    static RedBlackTreeNode<T> Sibling(RedBlackTreeNode<T> node, RedBlackTreeNode<T> parentNode)
    {
      Debug.Assert(node == null || node.Parent == parentNode);
      if (node == parentNode.Left)
        return parentNode.Right;
      else
        return parentNode.Left;
    }


    static bool GetColor(RedBlackTreeNode<T> node)
    {
      return node != null ? node.Color : BLACK;
    }


    void FixTreeOnDelete(RedBlackTreeNode<T> node, RedBlackTreeNode<T> parentNode)
    {
      Debug.Assert(node == null || node.Parent == parentNode);
      if (parentNode == null)
        return;

      // warning: node may be null
      RedBlackTreeNode<T> sibling = Sibling(node, parentNode);
      if (sibling.Color == RED)
      {
        parentNode.Color = RED;
        sibling.Color = BLACK;
        if (node == parentNode.Left)
        {
          RotateLeft(parentNode);
        }
        else
        {
          RotateRight(parentNode);
        }

        sibling = Sibling(node, parentNode); // update value of sibling after rotation
      }

      if (parentNode.Color == BLACK
          && sibling.Color == BLACK
          && GetColor(sibling.Left) == BLACK
          && GetColor(sibling.Right) == BLACK)
      {
        sibling.Color = RED;
        FixTreeOnDelete(parentNode, parentNode.Parent);
        return;
      }

      if (parentNode.Color == RED
          && sibling.Color == BLACK
          && GetColor(sibling.Left) == BLACK
          && GetColor(sibling.Right) == BLACK)
      {
        sibling.Color = RED;
        parentNode.Color = BLACK;
        return;
      }

      if (node == parentNode.Left &&
          sibling.Color == BLACK &&
          GetColor(sibling.Left) == RED &&
          GetColor(sibling.Right) == BLACK)
      {
        sibling.Color = RED;
        sibling.Left.Color = BLACK;
        RotateRight(sibling);
      }
      else if (node == parentNode.Right &&
               sibling.Color == BLACK &&
               GetColor(sibling.Right) == RED &&
               GetColor(sibling.Left) == BLACK)
      {
        sibling.Color = RED;
        sibling.Right.Color = BLACK;
        RotateLeft(sibling);
      }
      sibling = Sibling(node, parentNode); // update value of sibling after rotation

      sibling.Color = parentNode.Color;
      parentNode.Color = BLACK;
      if (node == parentNode.Left)
      {
        if (sibling.Right != null)
        {
          Debug.Assert(sibling.Right.Color == RED);
          sibling.Right.Color = BLACK;
        }
        RotateLeft(parentNode);
      }
      else
      {
        if (sibling.Left != null)
        {
          Debug.Assert(sibling.Left.Color == RED);
          sibling.Left.Color = BLACK;
        }
        RotateRight(parentNode);
      }
    }
    #endregion


    #region Find/LowerBound/UpperBound/GetEnumerator
    /// <summary>
    /// Returns the iterator pointing to the specified item, or an iterator in End state if the item is not found.
    /// </summary>
    public RedBlackTreeIterator<T> Find(T item)
    {
      RedBlackTreeIterator<T> it = LowerBound(item);
      while (it.IsValid && _host.Compare(it.Current, item) == 0)
      {
        if (_host.Equals(it.Current, item))
          return it;
        it.MoveNext();
      }
      return default(RedBlackTreeIterator<T>);
    }


    /// <summary>
    /// Returns the iterator pointing to the first item greater or equal to <paramref name="item"/>.
    /// </summary>
    public RedBlackTreeIterator<T> LowerBound(T item)
    {
      RedBlackTreeNode<T> node = Root;
      RedBlackTreeNode<T> resultNode = null;
      while (node != null)
      {
        if (_host.Compare(node.Value, item) < 0)
        {
          node = node.Right;
        }
        else
        {
          resultNode = node;
          node = node.Left;
        }
      }
      return new RedBlackTreeIterator<T>(resultNode);
    }


    /// <summary>
    /// Returns the iterator pointing to the first item greater than <paramref name="item"/>.
    /// </summary>
    public RedBlackTreeIterator<T> UpperBound(T item)
    {
      RedBlackTreeIterator<T> it = LowerBound(item);
      while (it.IsValid && _host.Compare(it.Current, item) == 0)
      {
        it.MoveNext();
      }
      return it;
    }


    /// <summary>
    /// Gets a tree iterator that starts on the first node.
    /// </summary>
    public RedBlackTreeIterator<T> Begin()
    {
      if (Root == null) return default(RedBlackTreeIterator<T>);
      return new RedBlackTreeIterator<T>(Root.LeftMost);
    }


    /// <summary>
    /// Gets a tree iterator that starts one node before the first node.
    /// </summary>
    public RedBlackTreeIterator<T> GetEnumerator()
    {
      if (Root == null) return default(RedBlackTreeIterator<T>);
      RedBlackTreeNode<T> dummyNode = new RedBlackTreeNode<T>(default(T));
      dummyNode.Right = Root;
      return new RedBlackTreeIterator<T>(dummyNode);
    }
    #endregion


    #region ICollection members
    public bool Contains(T item)
    {
      return Find(item).IsValid;
    }


    public bool Remove(T item)
    {
      RedBlackTreeIterator<T> it = Find(item);
      if (!it.IsValid)
      {
        return false;
      }
      else
      {
        RemoveAt(it);
        return true;
      }
    }


    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
      return GetEnumerator();
    }


    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }


    bool ICollection<T>.IsReadOnly
    {
      get { return false; }
    }


    public void CopyTo(T[] array, int arrayIndex)
    {
      if (array == null) throw new ArgumentNullException("array");
      foreach (T val in this)
      {
        array[arrayIndex++] = val;
      }
    }
    #endregion
  }
}
