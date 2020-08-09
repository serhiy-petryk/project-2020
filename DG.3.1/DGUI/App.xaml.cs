using System.Windows;
using DGUI.ViewModels;

namespace DGUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private void App_OnStartup(object sender, StartupEventArgs e) => AppViewModel.Instance.CmdToggleScheme.Execute(null);
    }
}
