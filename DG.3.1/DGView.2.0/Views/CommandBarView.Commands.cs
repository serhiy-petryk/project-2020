using WpfSpLib.Common;
using WpfSpLib.Controls;

namespace DGView.Views
{
    public partial class CommandBarView
    {
        public RelayCommand CmdEditSetting { get; }
        public RelayCommand CmdRowDisplayMode { get; }
        public RelayCommand CmdFastFilter { get; }
        public RelayCommand CmdSortAsc { get; }
        public RelayCommand CmdSortDesc { get; }
        public RelayCommand CmdSortRemove { get; }

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
                    DGViewModel.SetQuickTextFilter(value);
                }
            }
        }

        private void cmdEditSetting(object p)
        {
            DialogMessage.ShowDialog($"cmdEditSetting: Not ready!", null, DialogMessage.DialogMessageIcon.Warning, new[] { "OK" });
        }
        private void cmdRowDisplayMode(object p)
        {
            DialogMessage.ShowDialog($"cmdRowDisplayMode: Not ready!", null, DialogMessage.DialogMessageIcon.Warning, new[] { "OK" });
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
