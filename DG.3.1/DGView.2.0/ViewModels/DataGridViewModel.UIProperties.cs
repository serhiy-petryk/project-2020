﻿using System.ComponentModel;
using DGCore.Common;

namespace DGView.ViewModels
{
    public partial class DataGridViewModel
    {
        #region ======= Status properties =======

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

        private int _statusLoadingRows;
        public int StatusLoadingRows
        {
            get => _statusLoadingRows;
            set
            {
                _statusLoadingRows = value;
                OnPropertiesChanged(nameof(StatusLoadingRows));
            }
        }

        public int FilteredRowCount => Data.FilteredRowCount;
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

    }
}
