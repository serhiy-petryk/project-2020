using WpfSpLib.Common;
using WpfSpLib.Controls;

namespace DGView.Views
{
    public partial class CommandBarView
    {
        public RelayCommand CmdEditSetting { get; }
        public RelayCommand CmdRowDisplayMode { get; }
        public RelayCommand CmdFastFilter { get; }

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
    }
}
