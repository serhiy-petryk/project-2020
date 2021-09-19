using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using DGCore.Common;
using DGCore.DGVList;
using DGCore.Sql;
using DGView.Helpers;

namespace DGView.ViewModels
{
    public partial class DGViewModel
    {
        #region  ==========  Static section  ============
        private static string _plusSquareGeometryString = "M14 1a1 1 0 0 1 1 1v12a1 1 0 0 1-1 1H2a1 1 0 0 1-1-1V2a1 1 0 0 1 1-1h12zM2 0a2 2 0 0 0-2 2v12a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V2a2 2 0 0 0-2-2H2z M8 4a.5.5 0 0 1 .5.5v3h3a.5.5 0 0 1 0 1h-3v3a.5.5 0 0 1-1 0v-3h-3a.5.5 0 0 1 0-1h3v-3A.5.5 0 0 1 8 4z";
        private static string _minusSquareGeometryString = "M14 1a1 1 0 0 1 1 1v12a1 1 0 0 1-1 1H2a1 1 0 0 1-1-1V2a1 1 0 0 1 1-1h12zM2 0a2 2 0 0 0-2 2v12a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V2a2 2 0 0 0-2-2H2z M4 8a.5.5 0 0 1 .5-.5h7a.5.5 0 0 1 0 1h-7A.5.5 0 0 1 4 8z";
        internal static Geometry PlusSquareGeometry = Geometry.Parse(_plusSquareGeometryString);
        internal static Geometry MinusSquareGeometry = Geometry.Parse(_minusSquareGeometryString);
        #endregion

        #region ======= Status bar properties =======
        public string StatusText { get; set; } // any text

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

        #region =======  RowViewMode  ========
        private Enums.DGRowViewMode _rowViewMode = Enums.DGRowViewMode.OneRow;
        public Enums.DGRowViewMode RowViewMode
        {
            get => _rowViewMode;
            set
            {
                _rowViewMode = value;
                SetCellElementStyleAndWidth();

                if (RowViewMode == Enums.DGRowViewMode.OneRow)
                    DGControl.RowHeight = DGControl.FontSize * 1.5 + 2;
                else if (!double.IsNaN(DGControl.RowHeight))
                    DGControl.RowHeight = double.NaN;

                OnPropertiesChanged(nameof(RowViewMode), nameof(RowViewModeLabel));
            }
        }
        public string RowViewModeLabel => _rowViewMode.ToString();
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

        private bool _isGridLinesVisible = true;
        public bool IsGridLinesVisible
        {
            get => _isGridLinesVisible;
            set
            {
                _isGridLinesVisible = value;
                OnPropertiesChanged(nameof(IsGridLinesVisible));
                Data.RefreshData();
            }
        }

        public bool IsGroupLevelButtonEnabled => Data != null && Data.Groups.Count > 0;
        public bool IsSetFilterOnValueOrSortingEnable => DGControl.SelectedCells.Count == 1 && DGControl.SelectedCells.Any(c => c.Column.CanUserSort && !string.IsNullOrEmpty(c.Column.SortMemberPath));

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
                    return a1 != null;
                }

                // group row
                var a2 = Data.SortsOfGroups[groupItem.Level - 1].FirstOrDefault(a => a.PropertyDescriptor.Name == propertyName);
                return a2 != null;
            }
        }

        private void UpdateColumnSortGlyphs()
        {
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
                        properties.Remove(property);
                }
                foreach(var column in DGControl.Columns.Where(c=>!string.IsNullOrEmpty(c.SortMemberPath)))
                {
                    var sort = properties.ContainsKey(column.SortMemberPath) && properties[column.SortMemberPath].Count == levels.Length ? properties[column.SortMemberPath][0] : (ListSortDirection?)null;
                    if (!Equals(column.SortDirection, sort))
                        column.SortDirection = sort;
                }
            }
        }

        public bool IsClearFilterOnValueEnable => Data?.FilterByValue != null && !Data.FilterByValue.IsEmpty;

        //===================
        public string[] UserSettings => DesignerProperties.GetIsInDesignMode(this) ? new string[0] : DGCore.UserSettings.UserSettingsUtils.GetKeysFromDb(this).ToArray();
        public bool IsSelectSettingEnabled => UserSettings.Length > 0;

        private string StartUpParameters { get; set; }
        public string LastAppliedLayoutName { get; private set; }

        private DataGridCellInfo _lastCurrentCellInfo;
        internal DataGridColumn GroupItemCountColumn = null;
        internal List<DataGridColumn> _groupColumns = new List<DataGridColumn>();
        internal List<double> _fontFactors = new List<double>();
        private List<DGCore.UserSettings.Column> _columns = new List<DGCore.UserSettings.Column>();
        private List<string> _frozenColumns = new List<string>();

        //========================
        private DGColumnHelper[] GetColumnHelpers() => _columns.Where(c => !c.IsHidden)
            .Select(c => new Helpers.DGColumnHelper(DGControl.Columns.FirstOrDefault(c1 => c1.SortMemberPath == c.Id)))
            .Where(h => h.IsValid).ToArray();
    }
}
