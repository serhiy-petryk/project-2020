using System;
using System.ComponentModel;
using System.Windows.Controls;
using DGCore.Common;
using DGCore.DGVList;
using DGView.Views;

namespace DGView.ViewModels
{
    public partial class DataGridViewModel
    {
        public DataGridView View { get; }
        public DataGrid DGControl => View.DataGrid;
        public IDGVList Data;
        public Type ItemType;

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
