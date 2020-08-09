﻿using System.Windows;
using DGView.ViewModels;

namespace DGView
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private void App_OnStartup(object sender, StartupEventArgs e) => AppViewModel.Instance.CmdToggleScheme.Execute(null);
    }
}
