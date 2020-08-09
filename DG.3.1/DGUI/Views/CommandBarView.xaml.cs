using System.Windows;

namespace DGUI.Views
{
    /// <summary>
    /// Interaction logic for CommandBarView.xaml
    /// </summary>
    public partial class CommandBarView
    {
        public CommandBarView()
        {
            InitializeComponent();
        }

        private void OpenSettingButton_OnChecked(object sender, RoutedEventArgs e) => Controls.ToggleButtonHelper.OpenMenu_OnCheck(sender);

    }
}
