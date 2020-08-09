using System;
using System.Linq;
using System.Windows;

namespace DG
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            var theme = Themes.ThemeInfo.Themes.FirstOrDefault(th => string.Equals(th.Id, Themes.ThemeInfo.StartupTheme, StringComparison.InvariantCultureIgnoreCase));
            theme?.ApplyTheme();
        }
    }
}
