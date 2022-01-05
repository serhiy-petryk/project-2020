using System.Windows.Controls;
using System.Windows.Input;

namespace DGView.Views
{
    /// <summary>
    /// Interaction logic for SaveSettingView.xaml
    /// </summary>
    public partial class DGSaveSettingView : UserControl
    {
        public DGSaveSettingView()
        {
            InitializeComponent();
        }

        private void OnNewSettingNameLabelPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            NewSettingName.Focus();
        }
    }
}
