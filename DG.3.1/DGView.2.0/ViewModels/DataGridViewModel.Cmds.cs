using System;
using System.ComponentModel;
using WpfSpLib.Common;
using WpfSpLib.Controls;

namespace DGView.ViewModels
{
    public partial class DataGridViewModel
    {
        public string[] UserSettings => DesignerProperties.GetIsInDesignMode(this) ? new string[0] : DGCore.UserSettings.UserSettingsUtils.GetKeysFromDb(this).ToArray();
        public bool IsSelectSettingEnabled => UserSettings.Length > 0;

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

        public RelayCommand CmdSetSetting { get; private set; }
        public RelayCommand CmdEditSetting { get; private set; }
        public RelayCommand CmdRowDisplayMode { get; private set; }
        public RelayCommand CmdFastFilter { get; private set; }
        public RelayCommand CmdSortAsc { get; private set; }
        public RelayCommand CmdSortDesc { get; private set; }
        public RelayCommand CmdSortRemove { get; private set; }

        private void InitCommands()
        {
            CmdSetSetting = new RelayCommand(cmdSetSetting);
            CmdEditSetting = new RelayCommand(cmdEditSetting);
            CmdRowDisplayMode = new RelayCommand(cmdRowDisplayMode);
            CmdFastFilter = new RelayCommand(cmdFastFilter);
            CmdSortAsc = new RelayCommand(cmdSortAsc);
            CmdSortDesc = new RelayCommand(cmdSortDesc);
            CmdSortRemove = new RelayCommand(cmdSortRemove);
        }

        private void cmdSetSetting(object p)
        {
            var newSetting = (string)p;
            DGCore.UserSettings.UserSettingsUtils.SetSetting(this, newSetting);
        }
        private void cmdEditSetting(object p)
        {
            DialogMessage.ShowDialog($"cmdEditSetting: Not ready!", null, DialogMessage.DialogMessageIcon.Warning, new[] { "OK" });
        }
        private void cmdRowDisplayMode(object p)
        {
            var cellViewMode = (DGCore.Common.Enums.DGCellViewMode) Enum.Parse(typeof(DGCore.Common.Enums.DGCellViewMode), (string) p);
            CellViewMode = cellViewMode;
        }
        private void cmdFastFilter(object p)
        {
            DialogMessage.ShowDialog($"cmdFastFilter: Not ready!", null, DialogMessage.DialogMessageIcon.Warning, new[] { "OK" });
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


    }
}
