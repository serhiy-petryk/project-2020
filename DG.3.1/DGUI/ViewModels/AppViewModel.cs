﻿using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DGUI.Common;
using DGUI.Mwi;
using DGUI.Themes;
using DGUI.Views;

namespace DGUI.ViewModels
{
    public class AppViewModel : DependencyObject, INotifyPropertyChanged
    {
        //==========  Static section  ===========
        public static AppViewModel Instance = new AppViewModel();

        //=========================================
        //==========  Instance section  ===========
        public static readonly DependencyProperty ScaleValueProperty = DependencyProperty.Register(nameof(ScaleValue), typeof(double), typeof(AppViewModel), new UIPropertyMetadata(1.0));
        public double ScaleValue
        {
            get => (double)GetValue(ScaleValueProperty);
            set => SetValue(ScaleValueProperty, value);
        }
        public MwiContainer MwiContainer { get; set; }
        public CommandBarView CommandBar { get; set; }
        public FontFamily DefaultFontFamily { get; } = new FontFamily("Segoe UI");
        public Dock WindowsBarLocation { get; } = Dock.Top;
        public RelayCommand CmdToggleScheme { get; }

        private ThemeInfo _currentTheme;
        public AppViewModel() => CmdToggleScheme = new RelayCommand(ToggleTheme);

        //=========================
        private void ToggleTheme(object parameter)
        {
            _currentTheme = ThemeInfo.Themes.First(t => t != _currentTheme);
            _currentTheme.ApplyTheme();
        }


        //===========  INotifyPropertyChanged  =======================
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string[] propertyNames) => propertyNames.AsParallel().ForAll(propertyName =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)));

        #endregion
    }
}
