﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using DGCore.Common;
using DGCore.Sql;

namespace DGView.ViewModels
{
    public partial class DataGridViewModel
    {
        #region ======= Status properties =======

        private DataSourceBase.DataEventKind _dataStatus;
        public DataSourceBase.DataEventKind DataStatus
        {
            get => _dataStatus; private set
            {
                _dataStatus = value;
                //                OnPropertiesChanged(nameof(DataStatus), nameof(StatusLabel_PreparingData), nameof(StatusLabel_LoadingData));
                OnPropertiesChanged(nameof(DataStatus), nameof(StatusLabel_PreparingData), nameof(StatusLabel_LoadingData), nameof(StatusLoadingRows), nameof(IsPartiallyLoaded), nameof(StatusRowsLabel), nameof(DataLoadedTime));
            }
        }
        public bool StatusLabel_PreparingData => DataStatus == DataSourceBase.DataEventKind.Clear;
        public bool StatusLabel_LoadingData => DataStatus == DataSourceBase.DataEventKind.Loading;

        public bool IsPartiallyLoaded => Data.UnderlyingData.IsPartiallyLoaded;
        public string StatusRowsLabel
        {
            get
            {
                var totals = Data.UnderlyingData.GetData(false).Count;
                var filtered = Data.FilteredRowCount;
                if (totals == filtered)
                    return totals.ToString("N0");
                return $"{filtered:N0}/{totals:N0}";
            }
        }

        public int StatusLoadingRows { get; private set; }
        public int FilteredRowCount => Data.FilteredRowCount;
        public int? DataLoadedTime { get; private set; }

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
                    _quickFilterText = value;
                    OnPropertiesChanged(nameof(QuickFilterText));
                    SetQuickTextFilter(value);
                }
            }
        }
        public void SetQuickTextFilter(string filterText)
        {
            Data.A_FastFilterChanged(filterText);
            OnPropertiesChanged(nameof(Data));
        }
        #endregion

        public string[] UserSettings => DesignerProperties.GetIsInDesignMode(this) ? new string[0] : DGCore.UserSettings.UserSettingsUtils.GetKeysFromDb(this).ToArray();
        public bool IsSelectSettingEnabled => UserSettings.Length > 0;

        private string StartUpParameters { get; set; }
        private string _lastAppliedLayoutName { get; set; }

        internal DataGridColumn GroupItemCountColumn = null;
        private List<DataGridTextColumn> _groupColumns = new List<DataGridTextColumn>();
    }
}