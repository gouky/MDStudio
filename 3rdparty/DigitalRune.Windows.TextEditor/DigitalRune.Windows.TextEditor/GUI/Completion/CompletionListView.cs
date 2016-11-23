using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;


namespace DigitalRune.Windows.TextEditor.Completion
{
  /// <summary>
  /// The list view of a code completion window.
  /// </summary>
  public class CompletionListView : Control
  {
    private readonly ICompletionData[] _fullCompletionData;
    private List<ICompletionData> _filteredCompletionData;
    private ImageList _imageList;
    private int _firstVisibleItem;
    private int _selectedIndex = -1;
    private bool _filterList = true;


    /// <summary>
    /// Gets or sets a value indicating whether to automatically filter the list based on the current selection.
    /// </summary>
    /// <value><see langword="true"/> to filter the list; otherwise, <see langword="false"/>.</value>
    /// <remarks>
    /// If <see cref="FilterList"/> is <see langword="true"/> the list is narrowed down, while the user types.
    /// </remarks>
    public bool FilterList
    {
      get { return _filterList; }
      set { _filterList = value; }
    }


    /// <summary>
    /// Gets or sets the image list.
    /// </summary>
    /// <value>The image list.</value>
    public ImageList ImageList
    {
      get { return _imageList; }
      set { _imageList = value; }
    }


    /// <summary>
    /// Gets or sets the first visible item.
    /// </summary>
    /// <value>The first visible item.</value>
    public int FirstVisibleItem
    {
      get { return _firstVisibleItem; }
      set
      {
        if (_firstVisibleItem != value)
        {
          _firstVisibleItem = value;
          OnFirstItemChanged(EventArgs.Empty);
        }
      }
    }


    /// <summary>
    /// Gets the number of maximal visible items.
    /// </summary>
    /// <value>The maximal visible items.</value>
    public int MaxVisibleItem
    {
      get { return Height / ItemHeight; }
    }


    /// <summary>
    /// Gets the selected completion data.
    /// </summary>
    /// <value>The selected completion data.</value>
    public ICompletionData SelectedItem
    {
      get { return (_selectedIndex >= 0) ? _filteredCompletionData[_selectedIndex] : null; }
    }


    /// <summary>
    /// Gets the number of visible items in the list.
    /// </summary>
    /// <value>The number of visible items.</value>
    public int ItemCount
    {
      get { return _filteredCompletionData.Count; }
    }


    /// <summary>
    /// Gets the width of the image.
    /// </summary>
    /// <value>The width of the image.</value>
    public int ImageWidth
    {
      get
      {       
        // Maintain aspect ratio
        return ItemHeight * _imageList.ImageSize.Width / _imageList.ImageSize.Height;
      }
    }


    /// <summary>
    /// Gets the height of an item.
    /// </summary>
    /// <value>The height of a single item.</value>
    public int ItemHeight
    {
      get { return Math.Max(_imageList.ImageSize.Height, Font.Height); }
    }


    /// <summary>
    /// Occurs after the selected item has changed.
    /// </summary>
    public event EventHandler<EventArgs> SelectedItemChanged;


    /// <summary>
    /// Occurs when the first item has changed.
    /// </summary>
    public event EventHandler<EventArgs> FirstItemChanged;


    /// <summary>
    /// Occurs when number of items in the list has changed.
    /// </summary>
    public event EventHandler<EventArgs> ItemCountChanged;


    /// <summary>
    /// Initializes a new instance of the <see cref="CompletionListView"/> class.
    /// </summary>
    /// <param name="completionData">The completion data.</param>
    public CompletionListView(ICompletionData[] completionData)
    {
      if (completionData == null)
        throw new ArgumentNullException("completionData");

      _fullCompletionData = completionData;
      Array.Sort(_fullCompletionData, DefaultCompletionData.Compare);
      _filteredCompletionData = new List<ICompletionData>(_fullCompletionData);
    }


    /// <summary>
    /// Closes this instance.
    /// </summary>
    public void Close()
    {
      Dispose();
    }


    /// <summary>
    /// Selects item.
    /// </summary>
    /// <param name="index">The index.</param>
    public void SelectItem(int index)
    {
      int oldSelectedItem = _selectedIndex;
      int oldFirstItem = _firstVisibleItem;

      index = Math.Max(0, index);
      _selectedIndex = Math.Max(0, Math.Min(_filteredCompletionData.Count - 1, index));
      if (_selectedIndex < _firstVisibleItem)
      {
        FirstVisibleItem = _selectedIndex;
      }
      if (_firstVisibleItem + MaxVisibleItem <= _selectedIndex)
      {
        FirstVisibleItem = _selectedIndex - MaxVisibleItem + 1;
      }
      if (oldSelectedItem != _selectedIndex)
      {
        if (_firstVisibleItem == oldFirstItem)
        {
          // Invalidate the items between old and new selection
          int min = Math.Min(_selectedIndex, oldSelectedItem) - _firstVisibleItem;
          int max = Math.Max(_selectedIndex, oldSelectedItem) - _firstVisibleItem;
          Invalidate(new Rectangle(0, 1 + min * ItemHeight, Width, (max - min + 1) * ItemHeight));
        }
        OnSelectedItemChanged(EventArgs.Empty);
      }
    }


    /// <summary>
    /// Centers the view on certain item.
    /// </summary>
    /// <param name="index">The index.</param>
    public void CenterViewOn(int index)
    {
      int firstItem = index - MaxVisibleItem / 2;
      if (firstItem < 0)
        FirstVisibleItem = 0;
      else if (firstItem >= _filteredCompletionData.Count - MaxVisibleItem)
        FirstVisibleItem = _filteredCompletionData.Count - MaxVisibleItem;
      else
        FirstVisibleItem = firstItem;
    }


    /// <summary>
    /// Clears the selection.
    /// </summary>
    public void ClearSelection()
    {
      if (_selectedIndex < 0)
        return;

      int itemNum = _selectedIndex - _firstVisibleItem;
      _selectedIndex = -1;
      Invalidate(new Rectangle(0, itemNum * ItemHeight, Width, (itemNum + 1) * ItemHeight + 1));
      Update();
      OnSelectedItemChanged(EventArgs.Empty);
    }


    /// <summary>
    /// Scrolls one page down.
    /// </summary>
    public void PageDown()
    {
      SelectItem(_selectedIndex + MaxVisibleItem);
    }


    /// <summary>
    /// Scrolls one page up.
    /// </summary>
    public void PageUp()
    {
      SelectItem(_selectedIndex - MaxVisibleItem);
    }


    /// <summary>
    /// Selects the next item.
    /// </summary>
    public void SelectNextItem()
    {
      SelectItem(_selectedIndex + 1);
    }


    /// <summary>
    /// Selects the previous item.
    /// </summary>
    public void SelectPrevItem()
    {
      SelectItem(_selectedIndex - 1);
    }


    /// <summary>
    /// Selects the item that starts with a certain string.
    /// </summary>
    /// <param name="startText">The start of the item.</param>
    public void SelectItemWithStart(string startText)
    {
      ApplyFilter(startText);

      if (String.IsNullOrEmpty(startText)) 
        return;

      int bestIndex = -1;
      int bestQuality = -1;
      // Qualities: 0 = match start
      //            1 = match start case-sensitive
      //            2 = full match
      //            3 = full match case-sensitive
      double bestPriority = 0;
      for (int i = 0; i < _filteredCompletionData.Count; ++i)
      {
        string itemText = _filteredCompletionData[i].Text;
        if (itemText.StartsWith(startText, StringComparison.InvariantCultureIgnoreCase))
        {
          double priority = _filteredCompletionData[i].Priority;
          int quality;
          if (String.Compare(itemText, startText, StringComparison.InvariantCultureIgnoreCase) == 0)
          {
            if (String.Compare(itemText, startText, StringComparison.InvariantCulture) == 0)
            {
              // full match case-sensitive
              quality = 3;
            }
            else
            {
              // full match case-insensitive
              quality = 2;
            }
          }
          else if (itemText.StartsWith(startText, StringComparison.InvariantCulture))
          {
            // match start case-sensitive
            quality = 1;
          }
          else
          {
            // match start case-insensitive
            quality = 0;
          }

          bool useThisItem;
          if (bestQuality < quality)
          {
            useThisItem = true;
          }
          else
          {
            if (bestIndex == _selectedIndex)
            {
              useThisItem = false;
            }
            else if (i == _selectedIndex)
            {
              useThisItem = (bestQuality == quality);
            }
            else
            {
              useThisItem = (bestQuality == quality && bestPriority < priority);
            }
          }
          if (useThisItem)
          {
            bestIndex = i;
            bestPriority = priority;
            bestQuality = quality;
          }
        }
      }

      // Update selection
      if (bestIndex < 0)
      {
        ClearSelection();
      }
      else
      {
        if (bestIndex < _firstVisibleItem || _firstVisibleItem + MaxVisibleItem <= bestIndex)
        {
          SelectItem(bestIndex);
          CenterViewOn(bestIndex);
        }
        else
        {
          SelectItem(bestIndex);
        }
      }
    }


    /// <summary>
    /// Filters all items and keeps only those that start with the given text.
    /// </summary>
    /// <param name="filter">The filter string (case-insensitive).</param>
    private void ApplyFilter(string filter)
    {
      if (!FilterList)
        return;

      if (filter == null)
        filter = String.Empty;

      int previousSelectedIndex = _selectedIndex;
      ICompletionData previousSelection = SelectedItem;
      List<ICompletionData> previousCompletionData = _filteredCompletionData;

      _selectedIndex = -1;
      _filteredCompletionData = new List<ICompletionData>(previousCompletionData.Count);
      for (int i = 0; i < _fullCompletionData.Length; i++)
      {
        if (_fullCompletionData[i].Text.StartsWith(filter, StringComparison.InvariantCultureIgnoreCase))
        {
          _filteredCompletionData.Add(_fullCompletionData[i]);
          if (_fullCompletionData[i] == previousSelection)
            _selectedIndex = i;
        }
      }

      if (_filteredCompletionData.Count == 0)
      {
        // After filtering the list is empty. Avoid empty list!
        // Keep the previous list.
        _selectedIndex = previousSelectedIndex;
        _filteredCompletionData = previousCompletionData;
      }
      else
      {
        OnItemCountChanged(EventArgs.Empty);
        Invalidate();
      }
    }


    /// <summary>
    /// Raises the <see cref="Control.Paint"/> event.
    /// </summary>
    /// <param name="pe">The <see cref="System.Windows.Forms.PaintEventArgs"/> instance containing the event data.</param>
    protected override void OnPaint(PaintEventArgs pe)
    {
      float yPos = 1;
      float itemHeight = ItemHeight;
      int imageWidth = ImageWidth;

      int curItem = _firstVisibleItem;
      Graphics g = pe.Graphics;
      while (curItem < _filteredCompletionData.Count && yPos < Height)
      {
        RectangleF drawingBackground = new RectangleF(1, yPos, Width - 2, itemHeight);
        if (drawingBackground.IntersectsWith(pe.ClipRectangle))
        {
          // draw Background
          Brush brush = (curItem == _selectedIndex) ? SystemBrushes.Highlight : SystemBrushes.Window;
          g.FillRectangle(brush, drawingBackground);

          // draw Icon
          int imageIndex = _filteredCompletionData[curItem].ImageIndex;
          if (_imageList != null &&  0 <= imageIndex && imageIndex < _imageList.Images.Count)
            g.DrawImage(_imageList.Images[imageIndex], new RectangleF(1, yPos, imageWidth, itemHeight));

          // draw text
          int xPos = imageWidth + 3;
          brush = (curItem == _selectedIndex) ? SystemBrushes.HighlightText : SystemBrushes.WindowText;
          g.DrawString(_filteredCompletionData[curItem].Text, Font, brush, xPos, yPos);
        }

        yPos += itemHeight;
        ++curItem;
      }
      g.DrawRectangle(SystemPens.Control, new Rectangle(0, 0, Width - 1, Height - 1));
    }


    /// <summary>
    /// Handles the <see cref="Control.MouseDown"/> event.
    /// </summary>
    /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
    protected override void OnMouseDown(MouseEventArgs e)
    {
      float yPos = 1;
      int curItem = _firstVisibleItem;
      float itemHeight = ItemHeight;

      while (curItem < _filteredCompletionData.Count && yPos < Height)
      {
        RectangleF drawingBackground = new RectangleF(1, yPos, Width - 2, itemHeight);
        if (drawingBackground.Contains(e.X, e.Y))
        {
          SelectItem(curItem);
          break;
        }
        yPos += itemHeight;
        ++curItem;
      }
    }


    /// <summary>
    /// Raises the <see cref="FirstItemChanged"/> event.
    /// </summary>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected virtual void OnFirstItemChanged(EventArgs e)
    {
      Invalidate();

      if (FirstItemChanged != null)
        FirstItemChanged(this, e);
    }


    /// <summary>
    /// Raises the <see cref="ItemCountChanged"/> event.
    /// </summary>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected virtual void OnItemCountChanged(EventArgs e)
    {
      if (ItemCountChanged != null)
        ItemCountChanged(this, e);
    }


    /// <summary>
    /// Raises the <see cref="SelectedItemChanged"/> event.
    /// </summary>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected virtual void OnSelectedItemChanged(EventArgs e)
    {
      if (SelectedItemChanged != null)
        SelectedItemChanged(this, e);
    }
  }
}
