using System;
using System.Collections.Generic;


namespace DigitalRune.Windows.TextEditor.Utilities
{
  /// <summary>
  /// A collection that allows its elements to be garbage-collected (unless there are other
  /// references to the elements). 
  /// </summary>
  /// <remarks>
  /// <para>
  /// Elements will disappear from the collection when they are garbage-collected.
  /// </para>
  /// <para>
  /// The <see cref="WeakCollection{T}"/> is not thread-safe, not even for read-only access!
  /// No methods may be called on the <see cref="WeakCollection{T}"/> while it is enumerated, not 
  /// even a <see cref="Contains"/> or creating a second enumerator.
  /// </para>
  /// <para>
  /// The <see cref="WeakCollection{T}"/> does not preserve any order among its contents; the 
  /// ordering may be different each time the collection is enumerated.
  /// </para>
  /// <para>
  /// Since items may disappear at any time when they are garbage collected, this class
  /// cannot provide a useful implementation for <see cref="ICollection{T}.Count"/> and thus cannot 
  /// implement the <see cref="ICollection{T}"/> interface.	
  /// </para>
  /// </remarks>
  public class WeakCollection<T> : IEnumerable<T> where T : class
  {
    private readonly List<WeakReference> _innerList = new List<WeakReference>();
    private bool _hasEnumerator;


    private static bool IsDeadPredicate(WeakReference r)
    {
      return !r.IsAlive;
    }


    /// <summary>
    /// Adds an element to the collection.
    /// </summary>
    /// <param name="item">The item to add.</param>
    /// <remarks>
    /// Runtime: O(n).
    /// </remarks>
    public void Add(T item)
    {
      if (item == null)
        throw new ArgumentNullException("item");

      CheckNoEnumerator();

      // Remove all weak references that reference dead objects
      // - to avoid reallocating memory for the list, and
      // - on every 32th item to remove dead references.
      if (_innerList.Count == _innerList.Capacity || (_innerList.Count % 32) == 31)
        _innerList.RemoveAll(IsDeadPredicate);

      _innerList.Add(new WeakReference(item));
    }


    /// <summary>
    /// Removes all elements from the collection.
    /// </summary>
    /// <remarks>
    /// Runtime: O(n).
    /// </remarks>
    public void Clear()
    {
      CheckNoEnumerator();

      _innerList.Clear();
    }


    /// <summary>
    /// Checks if the collection contains an item.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>
    /// 	<see langword="true"/> if the collection contains the specified item; otherwise, <see langword="false"/>.
    /// </returns>
    /// <remarks>
    /// Runtime: O(n).
    /// </remarks>
    public bool Contains(T item)
    {
      if (item == null)
        throw new ArgumentNullException("item");

      CheckNoEnumerator();

      foreach (T element in this)
      {
        if (item.Equals(element))
          return true;
      }
      return false;
    }


    /// <summary>
    /// Removes an element from the collection.
    /// </summary>
    /// <param name="item">The item to remove.</param>
    /// <returns>
    /// <see langword="true"/> if the item is found and removed, <see langword="false"/> when the item is not found.
    /// </returns>
    /// <remarks>
    /// Runtime: O(n).
    /// </remarks>
    public bool Remove(T item)
    {
      if (item == null)
        throw new ArgumentNullException("item");
      CheckNoEnumerator();
      for (int i = 0; i < _innerList.Count; )
      {
        T element = (T) _innerList[i].Target;
        if (element == null)
        {
          RemoveAt(i);
        }
        else if (element == item)
        {
          RemoveAt(i);
          return true;
        }
        else
        {
          i++;
        }
      }
      return false;
    }


    void RemoveAt(int i)
    {
      int lastIndex = _innerList.Count - 1;
      _innerList[i] = _innerList[lastIndex];
      _innerList.RemoveAt(lastIndex);
    }


    void CheckNoEnumerator()
    {
      if (_hasEnumerator)
        throw new InvalidOperationException("The WeakCollection is already being enumerated, it cannot be modified at the same time. Ensure you dispose the first enumerator before modifying the WeakCollection.");
    }


    /// <summary>
    /// Enumerates the collection. 
    /// </summary>
    /// <remarks>
    /// Each MoveNext() call on the enumerator is O(1), thus the enumeration is O(n).
    /// </remarks>
    public IEnumerator<T> GetEnumerator()
    {
      if (_hasEnumerator)
        throw new InvalidOperationException("The WeakCollection is already being enumerated, it cannot be enumerated twice at the same time. Ensure you dispose the first enumerator before using another enumerator.");
      try
      {
        _hasEnumerator = true;
        for (int i = 0; i < _innerList.Count; )
        {
          T element = (T) _innerList[i].Target;
          if (element == null)
          {
            RemoveAt(i);
          }
          else
          {
            yield return element;
            i++;
          }
        }
      }
      finally
      {
        _hasEnumerator = false;
      }
    }


    /// <summary>
    /// Returns an enumerator that iterates through a collection.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
    /// </returns>
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }
  }
}
