using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DGView.ViewModels;
using WpfSpLib.Common;

namespace DGView.Views
{
    /// <summary>
    /// Interaction logic for CommandBarView.xaml
    /// </summary>
    public partial class CommandBarView : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty DGViewModelProperty = DependencyProperty.Register("DGViewModel", typeof(DataGridViewModel), typeof(CommandBarView), new FrameworkPropertyMetadata(null));
        public DataGridViewModel DGViewModel
        {
            get => (DataGridViewModel)GetValue(DGViewModelProperty);
            set => SetValue(DGViewModelProperty, value);
        }
        public DataGrid DGControl => DGViewModel?.DGControl;
        public string[] UserSettings { get; private set; } = new string[0];
        public bool IsSelectSettingEnabled => UserSettings != null && UserSettings.Length > 0;

        public CommandBarView()
        {
            InitializeComponent();
            DataContext = this;
        }

        #region ===========  INotifyPropertyChanged  ================

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertiesChanged(params string[] propertyNames)
        {
            foreach (var propertyName in propertyNames)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        private void CommandBarView_OnLoaded(object sender, RoutedEventArgs e)
        {
            var dgv = this.GetVisualParents().OfType<DataGridView>().FirstOrDefault();
            DGViewModel = dgv?.ViewModel;
            UserSettings = DGCore.UserSettings.UserSettingsUtils.GetKeysFromDb(DGViewModel).ToArray();

            OnPropertiesChanged(nameof(DGControl), nameof(UserSettings), nameof(IsSelectSettingEnabled));
        }

        private void OnUserSettingMenuItemClick(object sender, RoutedEventArgs e)
        {
            var item = (MenuItem) sender;
            var newSetting = (string)item.Header;
            DGCore.UserSettings.UserSettingsUtils.SetSetting(DGViewModel, newSetting);
        }
    }
}
