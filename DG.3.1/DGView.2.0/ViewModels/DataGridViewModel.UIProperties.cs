using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using DGCore.Common;
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
        public bool FilterOnValueEnable => DGControl.SelectedCells.Where(c => !string.IsNullOrEmpty(c.Column.SortMemberPath)).Count() > 0;

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
        private DGCore.Utils.IDGColumnHelper[] GetColumnHelpers() => DGControl.Columns.Where(c => c.Visibility == System.Windows.Visibility.Visible).Select(c => new Helpers.DGColumnHelper(c)).Where(h => h.IsValid).ToArray();
    }
}
