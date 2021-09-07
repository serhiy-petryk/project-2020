using System;
using System.ComponentModel;
using WpfSpLib.Common;
using WpfSpLib.Controls;

namespace DGView.ViewModels
{
    public partial class DataGridViewModel
    {
        public RelayCommand CmdSetSetting { get; private set; }
        public RelayCommand CmdEditSetting { get; private set; }
        public RelayCommand CmdRowDisplayMode { get; private set; }
        public RelayCommand CmdSetGroupLevel { get; private set; }
        public RelayCommand CmdSetSortAsc { get; private set; }
        public RelayCommand CmdSetSortDesc { get; private set; }
        public RelayCommand CmdClearSortings { get; private set; }
        public RelayCommand CmdSetFilterOnValue { get; private set; }
        public RelayCommand CmdClearFilterOnValue { get; private set; }
        public RelayCommand CmdSearch { get; private set; }
        public RelayCommand CmdRequery { get; private set; }

        private void InitCommands()
        {
            CmdSetSetting = new RelayCommand(cmdSetSetting);
            CmdEditSetting = new RelayCommand(cmdEditSetting);
            CmdRowDisplayMode = new RelayCommand(cmdRowDisplayMode);
            CmdSetGroupLevel = new RelayCommand(cmdSetGroupLevel);

            CmdSetSortAsc = new RelayCommand(cmdSetSortAsc);
            CmdSetSortDesc = new RelayCommand(cmdSetSortDesc);
            CmdClearSortings = new RelayCommand(cmdClearSortings);

            CmdSetFilterOnValue = new RelayCommand(cmdSetFilterOnValue);
            CmdClearFilterOnValue = new RelayCommand(cmdClearFilterOnValue);

            CmdSearch = new RelayCommand(cmdSearch);
            CmdRequery = new RelayCommand(cmdRequery);
        }

        private void cmdSetSetting(object p)
        {
            var newSetting = (string)p;
            DGCore.UserSettings.UserSettingsUtils.SetSetting(this, newSetting);
            LastAppliedLayoutName = newSetting;
        }
        private void cmdEditSetting(object p)
        {
            DialogMessage.ShowDialog($"cmdEditSetting: Not ready!", null, DialogMessage.DialogMessageIcon.Warning, new[] { "OK" });
        }
        private void cmdRowDisplayMode(object p)
        {
            var cellViewMode = (DGCore.Common.Enums.DGCellViewMode)Enum.Parse(typeof(DGCore.Common.Enums.DGCellViewMode), (string)p);
            CellViewMode = cellViewMode;
        }
        private void cmdSetGroupLevel(object p)
        {
            var i = (int?)p;
            Data.A_SetGroupLevel(i.HasValue ? Math.Abs(i.Value) : (int?)null, (i ?? 0) >= 0);
        }
        private void cmdSetSortAsc(object p)
        {
            if (SetFilterOnValueOrSortingEnable)
            {
                _lastCurrentCellInfo = DGControl.SelectedCells[0];
                Data.A_ApplySorting(DGControl.CurrentCell.Column.SortMemberPath, DGControl.CurrentCell.Item, ListSortDirection.Ascending);
            }
        }
        private void cmdSetSortDesc(object p)
        {
            if (SetFilterOnValueOrSortingEnable)
            {
                _lastCurrentCellInfo = DGControl.SelectedCells[0];
                Data.A_ApplySorting(DGControl.CurrentCell.Column.SortMemberPath, DGControl.CurrentCell.Item, ListSortDirection.Descending);
            }
        }
        private void cmdClearSortings(object p)
        {
            if (DGControl.CurrentCell != null && !string.IsNullOrEmpty(DGControl.CurrentCell.Column.SortMemberPath))
            {
                _lastCurrentCellInfo = DGControl.SelectedCells[0];
                Data.A_RemoveSorting(DGControl.CurrentCell.Column.SortMemberPath, DGControl.CurrentCell.Item);
            }
        }
        private void cmdSetFilterOnValue(object p)
        {
            if (SetFilterOnValueOrSortingEnable)
            {
                _lastCurrentCellInfo = DGControl.SelectedCells[0];
                var pd = Data.Properties[_lastCurrentCellInfo.Column.SortMemberPath];
                var value = pd.GetValue(_lastCurrentCellInfo.Item);
                Data.A_SetByValueFilter(_lastCurrentCellInfo.Column.SortMemberPath, value);
            }
            OnPropertiesChanged(nameof(ClearFilterOnValueEnable));
        }
        private void cmdClearFilterOnValue(object p)
        {
            if (SetFilterOnValueOrSortingEnable)
                _lastCurrentCellInfo = DGControl.SelectedCells[0];
            Data.A_ClearByValueFilter();
            OnPropertiesChanged(nameof(ClearFilterOnValueEnable));
        }
        private void cmdSearch(object p)
        {
            var cellInfo = DGControl.CurrentCell;
            /*var cellContent = cellInfo.Column.GetCellContent(cellInfo.Item);
            if (cellContent != null)
            {
                var cell = (DataGridCell)cellContent.Parent;
                cell.Tag = "1";
                var row = DataGridRow.GetRowContainingElement(cell);
                Debug.Print($"DG.CurrentCell: {row.GetIndex()}, {cell.Column.DisplayIndex}");
            }
            else
            {
                Debug.Print($"DG.CurrentCell: null");
            }*/
        }
        private void cmdRequery(object p)
        {
            Data.RequeryData();
        }
    }
}
