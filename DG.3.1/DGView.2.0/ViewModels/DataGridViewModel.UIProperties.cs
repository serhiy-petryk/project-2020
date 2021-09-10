using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;
using DGCore.Common;
using DGCore.DGVList;
using DGCore.Sql;

namespace DGView.ViewModels
{
    public partial class DataGridViewModel
    {
        #region ======= Status bar properties =======

        private DataSourceBase.DataEventKind _dataStatus;
        public DataSourceBase.DataEventKind DataStatus
        {
            get => _dataStatus; private set
            {
                _dataStatus = value;
                OnPropertiesChanged(nameof(DataStatus), nameof(DataLoadingRows), nameof(DataLoadedTime), nameof(DataProcessedTime), nameof(IsPartiallyLoaded), nameof(StatusRowsLabel));
            }
        }
        public int DataLoadingRows { get; private set; }
        public int? DataLoadedTime { get; private set; }
        public int DataProcessedTime => Data == null ? 0 : Data.LastRefreshedTimeInMsecs;
        public bool IsPartiallyLoaded => Data == null ? false : Data.UnderlyingData.IsPartiallyLoaded;
        public string StatusRowsLabel
        {
            get
            {
                if (Data == null) return null;
                var totals = Data.UnderlyingData.GetData(false).Count;
                var filtered = Data.FilteredRowCount;
                if (totals == filtered)
                    return totals.ToString("N0");
                return $"{filtered:N0}/{totals:N0}";
            }
        }

        #endregion

        #region =======  CellViewMode  ========
        private Enums.DGCellViewMode _cellViewMode = Enums.DGCellViewMode.OneRow;
        public Enums.DGCellViewMode CellViewMode
        {
            get => _cellViewMode;
            set
            {
                _cellViewMode = value;
                SetCellElementStyleAndWidth();
                OnPropertiesChanged(nameof(CellViewMode), nameof(CellViewModeLabel));
            }
        }
        public string CellViewModeLabel => _cellViewMode.ToString();
        #endregion

        #region =====  Quick Filter ======
        private string _quickFilterText;
        public string QuickFilterText
        {
            get => _quickFilterText;
            set
            {
                if (!Equals(_quickFilterText, value))
                {
                    if (value != _quickFilterText)
                    {
                        _quickFilterText = value;
                        OnPropertiesChanged(nameof(QuickFilterText));
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            Data.A_FastFilterChanged(value);
                        }), System.Windows.Threading.DispatcherPriority.Background);
                    }
                }
            }
        }
        #endregion

        public bool IsGroupLevelButtonEnabled => Data == null ? false : Data.Groups.Count > 0;
        public bool IsSetFilterOnValueOrSortingEnable => DGControl.SelectedCells.Count == 1 && DGControl.SelectedCells.Where(c => !string.IsNullOrEmpty(c.Column.SortMemberPath)).Any();

        public bool IsClearSortingEnable {
            get
            {
                UpdateColumnSortGlyphs();
                if (Data == null || DGControl.SelectedCells.Count != 1)
                    return false;
                var cell = DGControl.SelectedCells[0];
                if (!cell.IsValid || cell.Item == null || string.IsNullOrEmpty(cell.Column.SortMemberPath))
                    return false;

                var propertyName = cell.Column.SortMemberPath;
                var groupItem = cell.Item as IDGVList_GroupItem;
                if (groupItem == null || groupItem.Level == 0) // detail area or total row
                {
                    var a1 = Data.Sorts.FirstOrDefault(a=> a.PropertyDescriptor.Name == propertyName);
                    return a1 == null ? false : true;
                }

                // group row
                var a2 = Data.SortsOfGroups[groupItem.Level - 1].FirstOrDefault(a => a.PropertyDescriptor.Name == propertyName);
                return a2 == null ? false : true;
            }
        }

        private void UpdateColumnSortGlyphs()
        {
            var sortedColumns = new List<ListSortDirection>();
            if (Data != null)
            {
                var levels = DGControl.SelectedCells.Where(c => c.IsValid).Select(c => c.Item is IDGVList_GroupItem ? ((IDGVList_GroupItem)c.Item).Level : 0).Distinct().ToArray();
                var properties = new Dictionary<string, List<ListSortDirection>>();
                foreach(var level in levels)
                {
                    var sorts = level == 0 ? Data.Sorts : Data.SortsOfGroups[level - 1];
                    foreach (var a1 in sorts)
                    {
                        if (!properties.ContainsKey(a1.PropertyDescriptor.Name))
                            properties.Add(a1.PropertyDescriptor.Name, new List<ListSortDirection>());
                        properties[a1.PropertyDescriptor.Name].Add(a1.SortDirection);
                    }
                }
                foreach(var property in properties.Keys.ToArray())
                {
                    var values = properties[property];
                    if (values.Min() != values.Max())
                    {
                        Debug.Print($"Remove: {property}");
                        properties.Remove(property);
                    }
                }
                foreach(var column in DGControl.Columns.Where(c=>!string.IsNullOrEmpty(c.SortMemberPath)))
                {
                    var sort = properties.ContainsKey(column.SortMemberPath) && properties[column.SortMemberPath].Count == levels.Length ? properties[column.SortMemberPath][0] : (ListSortDirection?)null;
                    if (!Equals(column.SortDirection, sort))
                        column.SortDirection = sort;
                }
            }
        }

        public bool IsClearFilterOnValueEnable => Data == null ? false : Data.FilterByValue != null && !Data.FilterByValue.IsEmpty;
        private DataGridCellInfo _lastCurrentCellInfo;

        //===================
        public string[] UserSettings => DesignerProperties.GetIsInDesignMode(this) ? new string[0] : DGCore.UserSettings.UserSettingsUtils.GetKeysFromDb(this).ToArray();
        public bool IsSelectSettingEnabled => UserSettings.Length > 0;

        private string StartUpParameters { get; set; }
        public string LastAppliedLayoutName { get; private set; }

        internal DataGridColumn GroupItemCountColumn = null;
        private List<DataGridTextColumn> _groupColumns = new List<DataGridTextColumn>();
        private List<DGCore.UserSettings.Column> _columns = new List<DGCore.UserSettings.Column>();
        private List<string> _frozenColumns = new List<string>();

        //========================
        private DGCore.Utils.IDGColumnHelper[] GetColumnHelpers() => _columns.Where(c => !c.IsHidden)
            .Select(c => new Helpers.DGColumnHelper(DGControl.Columns.FirstOrDefault(c1 => c1.SortMemberPath == c.Id)))
            .Where(h => h.IsValid).ToArray();
    }
}
