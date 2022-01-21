using System.Windows.Controls;

namespace DGView.Views
{
    /// <summary>
    /// Interaction logic for DBEditSetting.xaml
    /// </summary>
    public partial class DGEditSettingView : UserControl
    {
        public DGEditSettingView(DGCore.UserSettings.DGV settings)
        {
            InitializeComponent();
            PropertyList.ItemsSource = settings.AllColumns;
        }
    }
}
