using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DGWnd.Misc;
using DGWnd.Utils;

namespace DGWnd.DGV {
  public partial class DGVCube : DataGridView /*Utils.ISettingTripleSupport,*/  {
    public new DGCore.DGVList.IDGVList DataSource => base.DataSource == null ? null : (DGCore.DGVList.IDGVList) base.DataSource;

    public string _startUpParameters;
    public string _layoutID;
    Bitmap[] _treeImages = { Properties.Resources.TreeClose, Properties.Resources.TreeOpen };
    private List<string> _allValidColumnNames = new List<string>();
    private int _columnIndexOfCurrentCell;

    // For _groupFonts
    private Font[] _sourceGroupFonts;
    private int _oldGroupCount;
    private int _oldExpandedGroupLevel = 1;
    private Font _startupFont;
    private Font _oldFontForGroup;
    private Font[] _groupFonts
    {
      get
      {
        if (_sourceGroupFonts == null || _oldExpandedGroupLevel != DataSource.ExpandedGroupLevel || _oldGroupCount != DataSource.Groups.Count || !Equals(_oldFontForGroup, Font))
        {
          _oldGroupCount = DataSource.Groups.Count;
          _oldExpandedGroupLevel = DataSource.ExpandedGroupLevel;
          _oldFontForGroup = Font;
          if (this._sourceGroupFonts != null)
            Array.ForEach(_sourceGroupFonts, (item)=> item?.Dispose());
          _sourceGroupFonts = new Font[DataSource.Groups.Count + 1];
          int maxLevel = Math.Min(DataSource.ExpandedGroupLevel, DataSource.Groups.Count);
          _sourceGroupFonts[0] = new Font(this.Font.FontFamily, this.Font.Size, FontStyle.Bold);// total row
          for (int i = 1; i < maxLevel; i++)
            _sourceGroupFonts[i] = new Font(this.Font.FontFamily, this.Font.Size + maxLevel - i - 2, FontStyle.Bold);
        }
        return _sourceGroupFonts;
      }
    }

    // public int _filteredRows;
    // public int _lastRefreshedTimeInMsecs;
    object _lastActiveItem;// Restore item posirtion after sort
    int _lastActiveItemScreenOffset;//// Restore item posirtion after sort

    public DGVCube()
    {
      AllowUserToOrderColumns = true;
      AllowUserToResizeColumns = true;
      ReadOnly = true;
      //this.CellBorderStyle = DataGridViewCellBorderStyle.None;

      RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.EnableResizing;
      _gridPen = new Pen(this.GridColor);
      DoubleBuffered = true;
      // this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
      Visible = false;
      //      this.PreviewKeyDown += new PreviewKeyDownEventHandler(BODGV_PreviewKeyDown);
      SelectionMode = DataGridViewSelectionMode.ColumnHeaderSelect;
    }

    protected override void Dispose(bool disposing)
    {
      Unwire();
      DataSource.UnderlyingData.DataLoadingCancelFlag = true;
      DataSource.Dispose();

      base.Dispose(disposing);

      _cellLast_PropertyDescriptor = null;
      this._gridPen.Dispose();
      this._groupColumns = null;
      //      foreach (Pen pen in this._groupPens) pen.Dispose();
      this._groupPens = null;
      this._lastActiveItem = null;
      // this._totalLines = null;
      foreach (Bitmap bm in this._treeImages) bm.Dispose();
      this._treeImages = null;
      this._visibleColumns = null;
      // this._whereFilter = null;
      base.DataSource = null;
      if (m_FindAndReplaceForm != null) m_FindAndReplaceForm.Dispose();
      if (_sourceGroupFonts != null)
        Array.ForEach(_sourceGroupFonts, (item) => item?.Dispose());
    }

    void RestorePositionOfLastActiveItem() {
      if (_lastActiveItem != null) {
        IList data = (IList)this.DataSource;
        int i = data.IndexOf(_lastActiveItem);
        if (i >= 0 && this.CurrentCell.ColumnIndex >= 0) {
          int visibleItems = this.Height / this.Rows[this.FirstDisplayedScrollingRowIndex].Height;
          _lastActiveItemScreenOffset = Math.Max(0, Math.Min(_lastActiveItemScreenOffset, visibleItems));
          this.FirstDisplayedScrollingRowIndex = Math.Max(0, i - _lastActiveItemScreenOffset);
          if (this.CurrentCell == null || this.CurrentCell.RowIndex != i) {
            this.CurrentCell = this[this.CurrentCell.ColumnIndex, i];
          }
        }
        _lastActiveItem = null;// Clear
      }
    }

    void AdjustCheckBoxColumns()
    {
      for (int i = 0; i < this.Columns.Count; i++)
      {
        DataGridViewColumn c = this.Columns[i];
        if (c is DataGridViewCheckBoxColumn)
        {// Set sort mode to allow sort
          if (c.SortMode == DataGridViewColumnSortMode.NotSortable && !string.IsNullOrEmpty(c.DataPropertyName))
          {
            c.SortMode = DataGridViewColumnSortMode.Programmatic;
          }
        }
        else if (this.AutoGenerateColumns && c is DataGridViewTextBoxColumn && DGCore.Utils.Types.GetNotNullableType(c.ValueType) == typeof(bool))
        {
          DataGridViewCheckBoxColumn c1 = new DataGridViewCheckBoxColumn(DGCore.Utils.Types.IsNullableType(c.ValueType));
          int i1 = c.Index;
          this.Columns.RemoveAt(i1);
          this.Columns.Insert(i1, c1);
          c1.ValueType = c.ValueType;
          c1.AutoSizeMode = c.AutoSizeMode;
          c1.HeaderText = c.HeaderText;
          c1.Name = c.Name;
          c1.ReadOnly = c.ReadOnly;
          c1.Resizable = c.Resizable;
          c1.SortMode = DataGridViewColumnSortMode.Programmatic;
          c1.DataPropertyName = c.DataPropertyName;
          c1.Visible = c.Visible;
          //          if (!String.IsNullOrEmpty(c.HeaderCell.ToolTipText)) c1.HeaderCell.ToolTipText = c.HeaderCell.ToolTipText;
        }
      }
    }

    public string _lastAppliedLayoutName;
    public void Bind(DGCore.Sql.DataSourceBase ds, string layoutID, string startUpParameters, string startUpLayoutName, DGCore.UserSettings.DGV settings) {
      //1. Create DGV dataSource (BindingList). PropertyDescriptorCollection will be created
      //2. Create DGV WhereFilter & ColumnInfo
      //3. Create dGV columns & wherePredicates
      //4. RefreshData

      if (_startupFont == null)
        _startupFont = Font;

      this.UIThreadSync(() =>
      {
        Unwire();
        Visible = false;
        SuspendLayout();
      });

      DGCore.Misc.DependentObjectManager.Bind(ds, this); // Register object    
      this._startUpParameters = startUpParameters;
//      this.ReadOnly = true;
      this.AllowUserToAddRows = false;
      this._layoutID = layoutID;

      this.SelectionMode = DataGridViewSelectionMode.RowHeaderSelect;
      Type listType = typeof(DGCore.DGVList.DGVList<>).MakeGenericType(ds.ItemType);
      base.DataSource = Activator.CreateInstance(listType, ds, (Func<DGCore.Utils.IDGColumnHelper[]>)GetColumnHelpers);
      // Fix column order
      // Visible = true;

      Wire();

      // this._whereFilter = new Filters.FilterList(DataSource.Properties);
      // DataSource.FilterByValue = null;

      //      this._whereFilter.SetSettingInfo(filterInfo);
      //      this._columnsInfo = columnsInfo;
      //      this.DataSource = new DGVList(this);
      // Adjust columns
      AdjustCheckBoxColumns();
      // AdjustUidColumns();
      var properties = DataSource.Properties;
      foreach (DataGridViewColumn col in this.Columns)
      {
        if (col.SortMode == DataGridViewColumnSortMode.Automatic) col.SortMode = DataGridViewColumnSortMode.Programmatic;
        col.HeaderText = col.HeaderText.Replace("^", " ");// "^" is separator for Nested properties
        string s = col.DataPropertyName;
        if (!string.IsNullOrEmpty(s))
        {
          if (s.Contains(".")) col.Visible = false;// do not show nested properties on start DGV
          if (!string.IsNullOrEmpty(properties[s].Description))
          {
            col.HeaderCell.ToolTipText = properties[s].Description;
            //??? Как выделить поле, которое имеет ToolTip  col.HeaderCell.Value = col.HeaderCell.Value.ToString() + "`";
          }
        }
      }

      foreach (DataGridViewColumn c in this.Columns)
      {
        if (DGCore.Utils.Types.IsNumericType(c.ValueType) && c.CellTemplate.Style.Alignment == DataGridViewContentAlignment.NotSet)
        {
          c.CellTemplate.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
          c.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }
        Type t = DGCore.Utils.Types.GetNotNullableType(c.ValueType);
        if (t != null && (t == typeof(double) || t == typeof(Single)) && string.IsNullOrEmpty(c.CellTemplate.Style.Format))
        {
          c.CellTemplate.Style.Format = "N2";
          c.DefaultCellStyle.Format = "N2";
        }
        if (!string.IsNullOrEmpty(c.DataPropertyName))
        {
          PropertyDescriptor pd = properties[c.DataPropertyName];
          if (pd is DGCore.PD.IMemberDescriptor)
          {
            string format = ((DGCore.PD.IMemberDescriptor)pd).Format;
            ContentAlignment? alignment = Tips.ConvertAlignment(((DGCore.PD.IMemberDescriptor)pd).Alignment);
            if (!string.IsNullOrEmpty(format)) c.DefaultCellStyle.Format = format;
            //            if (alignment != null) c.DefaultCellStyle.Alignment = (DataGridViewContentAlignment)Convert.ChangeType((int)alignment.Value, typeof(DataGridViewContentAlignment));
            if (alignment != null) c.DefaultCellStyle.Alignment = (DataGridViewContentAlignment)((int)alignment.Value);
          }
        }
      }

      this.UIThreadSync(() =>
      {
        ResumeLayout(true);
        Visible = true;
        _visibleColumns = DGVUtils.GetColumnsInDisplayOrder(this, true);

        if (!string.IsNullOrEmpty(startUpLayoutName))
          _lastAppliedLayoutName = startUpLayoutName;

        if (settings != null)
          ((DGCore.UserSettings.IUserSettingSupport<DGCore.UserSettings.DGV>)this).SetSetting(settings);
        else
            DGCore.UserSettings.UserSettingsUtils.Init(this, startUpLayoutName);
      });

      DataSource.UnderlyingData.GetData(false);
    }

    //============   Layout  ==================
    public int _layoutCount = 0;
    public void ResizeColumnWidth()
    {
      this._layoutCount++;
      this.AutoResizeColumns(this._RowViewMode == DGCore.Common.Enums.DGRowViewMode.NotSet ? DataGridViewAutoSizeColumnsMode.DisplayedCells : DataGridViewAutoSizeColumnsMode.ColumnHeader, false);
      // this.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.ColumnHeader, true);
    }

    // ===============   Sort   ========================
    void RefreshSortColumnGlyphs()
    {
      Dictionary<string, ListSortDirection> dict = new Dictionary<string, ListSortDirection>();
      if (this._cellLast_Kind == -1 || this._cellLast_Kind == 0)
      {
        foreach (ListSortDescription x in DataSource.Sorts)
        {
          dict[x.PropertyDescriptor.Name] = x.SortDirection;
        }
      }
      else if (this._cellLast_Kind > 0)
      {
        foreach (ListSortDescription x in DataSource.SortsOfGroups[this._cellLast_Kind - 1])
        {
          dict[x.PropertyDescriptor.Name] = x.SortDirection;
        }
      }
      foreach (ListSortDescription x in DataSource.Groups)
      {
        dict[x.PropertyDescriptor.Name] = x.SortDirection;
      }
      foreach (DataGridViewColumn c in this.Columns)
      {
        if (dict.ContainsKey(c.DataPropertyName))
        {
          c.HeaderCell.SortGlyphDirection = dict[c.DataPropertyName] == ListSortDirection.Ascending ? System.Windows.Forms.SortOrder.Ascending : System.Windows.Forms.SortOrder.Descending;
        }
        else c.HeaderCell.SortGlyphDirection = System.Windows.Forms.SortOrder.None;
      }
    }
    //==============================================
    public int _cellLast_Kind;// -4 (Unbound cell); -1 (data region); 0 (total row); n (group row region (n==group level))
    public PropertyDescriptor _cellLast_PropertyDescriptor;
    public ListSortDirection? _cellLast_SortDirection;
    bool _isOnCellEnterActive = true;

    public void GetCellDataStatus(int columnIndex, int rowIndex)
    {
      // kind: -4 (Unbound cell); -1 (data region); 0 (total row); n (group row region (n==group level))
      if (columnIndex < 0 || rowIndex < 0)
      {
        this._cellLast_PropertyDescriptor = null;
        this._cellLast_SortDirection = null;
        this._cellLast_Kind = -4;// Unbound cell
        return;
      }

      DGCore.DGVList.IDGVList_GroupItem groupItem = this.Rows[rowIndex].DataBoundItem as DGCore.DGVList.IDGVList_GroupItem;
      this._cellLast_Kind = (groupItem == null ? -1 : groupItem.Level);
      this._cellLast_PropertyDescriptor = DGVUtils.GetInternalPropertyDescriptorCollection(this)[this.Columns[columnIndex].DataPropertyName];
      int displayIndex = Array.IndexOf<DataGridViewColumn>(this._visibleColumns, this.Columns[columnIndex]);
      if (displayIndex < DataSource.Groups.Count)
      {// header group column
        this._cellLast_PropertyDescriptor = null;
        this._cellLast_SortDirection = null;
        this._cellLast_Kind = displayIndex + 1;// group
        return;
      }
      if (this._cellLast_PropertyDescriptor != null)
      {
        // Is Group column ?
        int groupCnt = 1;
        foreach (ListSortDescription x in DataSource.Groups)
        {
          if (x.PropertyDescriptor.Name == this._cellLast_PropertyDescriptor.Name)
          {// Group column
            this._cellLast_Kind = groupCnt;
            this._cellLast_SortDirection = x.SortDirection;
            return;
          }
          groupCnt++;
        }
      }
      if (this._cellLast_Kind == 0)
      {// total row
        if (this._cellLast_PropertyDescriptor != null)
        {
          foreach (ListSortDescription x in DataSource.Sorts)
          {
            if (x.PropertyDescriptor.Name == this._cellLast_PropertyDescriptor.Name)
            {
              this._cellLast_SortDirection = x.SortDirection;
              return;
            }
          }
        }
        this._cellLast_SortDirection = null;
        return;
      }
      if (this._cellLast_Kind > 0)
      {// group row
        if (this._cellLast_PropertyDescriptor != null)
        {
          //          foreach (Misc.TotalLine x in this._totalLines) {
          //          if (x.ID == this._cellLast_PropertyDescriptor.Name) {
          for (int i = 0; i < DataSource.SortsOfGroups[this._cellLast_Kind - 1].Count; i++)
          {
            if (DataSource.SortsOfGroups[this._cellLast_Kind - 1][i].PropertyDescriptor.Name == this._cellLast_PropertyDescriptor.Name)
            {
              this._cellLast_SortDirection = DataSource.SortsOfGroups[this._cellLast_Kind - 1][i].SortDirection;
              return; //Sorted cell in data group column
            }
          }
          this._cellLast_SortDirection = null;// Not sorted cell in data group column
          return;
          //      }
          //      }
        }
        this._cellLast_PropertyDescriptor = null;// Unbound cell in group row
        this._cellLast_SortDirection = null;
        return;
      }
      if (this._cellLast_PropertyDescriptor == null)
      {// Unbound cell
        this._cellLast_SortDirection = null;
        this._cellLast_Kind = -4;
        return;
      }
      foreach (ListSortDescription x in DataSource.Sorts)
      {
        if (x.PropertyDescriptor.Name == this._cellLast_PropertyDescriptor.Name)
        {
          this._cellLast_SortDirection = x.SortDirection;// sorted data cell
          return;
        }
      }
      this._cellLast_SortDirection = null;// not sorted data cell
    }

        //==================================================
    ThirdParty.FindAndReplaceForm m_FindAndReplaceForm;
    public void Find_OpenForm()
    {
      if (m_FindAndReplaceForm == null)
      {
        m_FindAndReplaceForm = new ThirdParty.FindAndReplaceForm(this);
        m_FindAndReplaceForm.FormClosed += FindAndReplaceForm_FormClosed;
        m_FindAndReplaceForm.Show();
      }
      else
      {
        m_FindAndReplaceForm.InitializeForm(this);
        m_FindAndReplaceForm.BringToFront();
      }
    }

    void FindAndReplaceForm_FormClosed(object sender, FormClosedEventArgs e)
    {
      m_FindAndReplaceForm.FormClosed -= FindAndReplaceForm_FormClosed;
      m_FindAndReplaceForm = null;
    }

    //==================================
    private void Wire() => DataSource.DataStateChanged += DataSource_DataStateChanged;
    private void Unwire()
    {
      if (DataSource != null)
        DataSource.DataStateChanged -= DataSource_DataStateChanged;
    }

    //========  DataState events  ============
    private void DataSource_DataStateChanged(object sender, DGCore.Sql.DataSourceBase.SqlDataEventArgs e)
    {
      this.UIThreadAsync(() =>
      {
        switch (e.EventKind)
        {
          case DGCore.Sql.DataSourceBase.DataEventKind.Loaded:
            DataSource_Loaded();
            break;
          case DGCore.Sql.DataSourceBase.DataEventKind.BeforeRefresh:
            DataSource_BeforeRefresh();
            break;
          case DGCore.Sql.DataSourceBase.DataEventKind.Refreshed:
            DataSource_AfterRefresh();
            break;
        }
      });
    }

    private void DataSource_Loaded()
    {
      // Set row headers width
      if (RowHeadersWidthSizeMode == DataGridViewRowHeadersWidthSizeMode.DisableResizing || RowHeadersWidthSizeMode == DataGridViewRowHeadersWidthSizeMode.EnableResizing)
        using (Graphics g = CreateGraphics())
          RowHeadersWidth = (int)g.MeasureString(DataSource.UnderlyingData.GetData(false).Count.ToString(), RowHeadersDefaultCellStyle.Font).Width + 34;// Convert.ToInt32(30 / 8.25 * this._owner.Font.Size);
    }

    private void DataSource_BeforeRefresh()
    {
      // doesn't work = apply to form (dgv + manage controls)
      // ((Form)TopLevelControl).ActiveControl = this;
      // Application.DoEvents();

      SetEnabledColors(false);
      Enabled = false;
      // SuspendLayout();
      _columnIndexOfCurrentCell = CurrentCell?.OwningColumn.Index ?? -1;
      _isOnCellEnterActive = false;// switch off CellEnter 
    }

    private void DataSource_AfterRefresh()
    {
      _isOnCellEnterActive = true;// switch on CellEnter 
      if (_columnIndexOfCurrentCell >= 0)
      {
        // Restore current column position
        if (CurrentCell != null)
        {
          var rowIndex = CurrentCell.OwningRow.Index;

          // After ResetBindings CurrentCell column == 0; 
          // we need to call OnCellEnter if cell column = 0 to refresh sort glyphs
          // or to reactivate current cell if cell column <> 0
          if (_columnIndexOfCurrentCell == 0)
            OnCellEnter(new DataGridViewCellEventArgs(_columnIndexOfCurrentCell, rowIndex));
          else
            CurrentCell = this[_columnIndexOfCurrentCell, rowIndex];

          // ??? can not use: cells are selected after +/- pressed           Application.DoEvents();
          // Restore column sort glyphs, etc ..
          //            this._owner.OnCellEnter(new DataGridViewCellEventArgs(colIndex, rowIndex));
          // Scroll into current cell
          Rectangle r1 = GetCellDisplayRectangle(_columnIndexOfCurrentCell, rowIndex, false);
          Rectangle r2 = GetCellDisplayRectangle(_columnIndexOfCurrentCell, rowIndex, true);
          Rectangle r3 = new Rectangle();// Blank rectangle
          if (r1 == r3 || r1 != r2 || FirstDisplayedScrollingColumnIndex == _columnIndexOfCurrentCell)
              DGVUtils.ScrollIntoCurrentCell(this);
        }
        else
          OnCellEnter(new DataGridViewCellEventArgs(-1, -1));
      }
      else
      {
        // Restore column sort glyphs, etc ..
        if (CurrentCell == null)
          OnCellEnter(new DataGridViewCellEventArgs(-1, -1));
        else
          OnCellEnter(new DataGridViewCellEventArgs(CurrentCell.OwningColumn.Index, CurrentCell.OwningRow.Index));
      }

      // Set columns visibility
      foreach (var columnName in _allValidColumnNames)
      {
        var visible = DataSource.IsPropertyVisible(columnName);
        var col = Columns.Cast<DataGridViewColumn>().FirstOrDefault(c => c.DataPropertyName == columnName);
        if (col != null && col.Visible != visible)
          col.Visible = !col.Visible;
        // throw new Exception("ToDo visible");
        // if (columnName.Visible != visible)
        // columnName.Visible = visible;
      }

      // Set group columns visibility
      foreach (var c in _groupColumns.Where((c, index) =>
        DataSource.IsGroupColumnVisible(index) != c.Visible))
        c.Visible = !c.Visible;

      _visibleColumns = DGVUtils.GetColumnsInDisplayOrder(this, true);

      // ResumeLayout(true);
      // TopLevelControl?.ResumeLayout(true);
      if (!Visible)
        Visible = true;
      Enabled = true;
      SetEnabledColors(true);
    }

    private DGCore.Utils.IDGColumnHelper[] GetColumnHelpers() => _allValidColumnNames
      .Select(c => new Utils.DGVColumnHelper(Columns.OfType<DataGridViewColumn>().FirstOrDefault(c1 => c1.DataPropertyName == c)))
      .Where(h => h.IsValid).ToArray();

    private List<object> _enabled = new List<object>();
    // protected override void OnEnabledChanged(EventArgs e)
    // see https://stackoverflow.com/questions/8715459/disabling-or-greying-out-a-datagridview
    // є мерехтіння (см. FastFilter для набора данных MastAltCoA - причина: EnableHeadersVisualStyles)
    private void SetEnabledColors(bool enabled)
    {
      if (!enabled)
      {
        if (_enabled.Count == 0)
          _enabled.AddRange(new object[]
          {
            DefaultCellStyle.BackColor, DefaultCellStyle.ForeColor, ColumnHeadersDefaultCellStyle.BackColor,
            ColumnHeadersDefaultCellStyle.ForeColor, EnableHeadersVisualStyles, ReadOnly
          });
        DefaultCellStyle.BackColor = SystemColors.Control;
        DefaultCellStyle.ForeColor = SystemColors.GrayText;
        ColumnHeadersDefaultCellStyle.BackColor = SystemColors.Control;
        ColumnHeadersDefaultCellStyle.ForeColor = SystemColors.GrayText;
        // EnableHeadersVisualStyles = false;
        ReadOnly = true;
      }
      else
      {
        if (_enabled.Count > 0)
        {
          DefaultCellStyle.BackColor = (Color)_enabled[0];
          DefaultCellStyle.ForeColor = (Color)_enabled[1];
          ColumnHeadersDefaultCellStyle.BackColor = (Color)_enabled[2];
          ColumnHeadersDefaultCellStyle.ForeColor = (Color)_enabled[3];
          // EnableHeadersVisualStyles = (bool)_enabled[4];
          ReadOnly = (bool)_enabled[5];
        }
      }
    }
  }

}

