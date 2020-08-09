using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DG.Annotations;
using DG.Common;
using MahApps.Metro;

namespace DG.ViewModels
{
    public class AppViewModel : INotifyPropertyChanged
    {
        //==========  Static section  ===========
        public static AppViewModel GlobalContext { get; } = new AppViewModel();

        //==========  Instance section  ===========
        public Brush PathForeground => (Brush)ThemeManager.DetectAppStyle(Application.Current).Item1.Resources[SystemColors.ControlTextBrushKey];
        public RelayCommand CmdToggleTheme { get; }
        public RelayCommand CmdRestoreScalingFactor { get; }

        public AppViewModel()
        {
            CmdToggleTheme = new RelayCommand(Do_ToggleTheme);
            CmdRestoreScalingFactor = new RelayCommand((object o) =>
            {
                if (o is Slider slider)
                    slider.Value = 1.0;
            });
        }
        //=========================
        private void Do_ToggleTheme(object parameter)
        {
            var themes = ThemeManager.AppThemes.ToArray();
            if (themes.Length > 1)
            {
                var themeAndAccent = ThemeManager.DetectAppStyle(Application.Current);
                var k = Array.IndexOf(themes, themeAndAccent.Item1);
                var newTheme = themes[k == 0 ? 1 : 0];
                ThemeManager.ChangeAppStyle(Application.Current, themeAndAccent.Item2, newTheme);

                // Notify about new PathForeground value
                OnPropertyChanged(nameof(PathForeground));
            }
        }

        //===========  INotifyPropertyChanged  ========== 
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
