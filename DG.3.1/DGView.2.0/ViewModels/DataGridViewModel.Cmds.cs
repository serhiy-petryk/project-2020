using System;
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
        public RelayCommand CmdSortAsc { get; private set; }
        public RelayCommand CmdSortDesc { get; private set; }
        public RelayCommand CmdSortRemove { get; private set; }
        public RelayCommand CmdRequery { get; private set; }

        private void InitCommands()
        {
            CmdSetSetting = new RelayCommand(cmdSetSetting);
            CmdEditSetting = new RelayCommand(cmdEditSetting);
            CmdRowDisplayMode = new RelayCommand(cmdRowDisplayMode);
            CmdSetGroupLevel = new RelayCommand(cmdSetGroupLevel);
            CmdSortAsc = new RelayCommand(cmdSortAsc);
            CmdSortDesc = new RelayCommand(cmdSortDesc);
            CmdSortRemove = new RelayCommand(cmdSortRemove);
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
        private void cmdSortAsc(object p)
        {
            DialogMessage.ShowDialog($"cmdSortAsc: Not ready!", null, DialogMessage.DialogMessageIcon.Warning, new[] { "OK" });
        }
        private void cmdSortDesc(object p)
        {
            DialogMessage.ShowDialog($"cmdSortDessc: Not ready!", null, DialogMessage.DialogMessageIcon.Warning, new[] { "OK" });
        }
        private void cmdSortRemove(object p)
        {
            DialogMessage.ShowDialog($"cmdSortRemove: Not ready!", null, DialogMessage.DialogMessageIcon.Warning, new[] { "OK" });
        }
        private void cmdRequery(object p)
        {
            Data.RequeryData();
        }
    }
}
