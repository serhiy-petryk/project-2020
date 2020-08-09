using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DGUI.Common;
using DGUI.Mwi;
using DGUI.ViewModels;
using DGUI.Views;

namespace DGUI
{
    /// <summary>
    /// Interaction logic for MwiStartup.xaml
    /// </summary>
    public partial class MwiStartup
    {
        public RelayCommand CmdScaleSliderReset { get; private set; }

        public static readonly DependencyProperty ScaleSliderProperty = DependencyProperty.Register(nameof(ScaleSlider), typeof(Slider), typeof(MwiStartup), new UIPropertyMetadata(null));
        public Slider ScaleSlider
        {
            get => (Slider)GetValue(ScaleSliderProperty);
            set => SetValue(ScaleSliderProperty, value);
        }

        public MwiStartup()
        {
            InitializeComponent();
            DataContext = this;
            TopControl.RestoreRectFromSetting();
            TopControl.CommandBar = new CommandBarView();
        }

        private void MwiStartup_OnLoaded(object sender, RoutedEventArgs e)
        {
            ScaleSlider = Tips.FindVisualChildren<Slider>(this).First(s => s.Uid == "ScaleSlider");
            AppViewModel.Instance.CommandBar = (CommandBarView)TopControl.CommandBar;
            AppViewModel.Instance.MwiContainer = Tips.FindVisualChildren<MwiContainer>(this).First(s => s.Uid == "Container");
            CmdScaleSliderReset = new RelayCommand(p => ScaleSlider.Value = 1.0);
            Window1 = Tips.FindVisualChildren<MwiContainer>(this).First().Children.FirstOrDefault(w => w.Title == "Window Using XAML");
        }

        private void MwiStartup_OnUnloaded(object sender, RoutedEventArgs e) => TopControl.SaveRectToSetting();

        private void MwiStartup_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control && Keyboard.IsKeyDown(Key.F4) && AppViewModel.Instance.MwiContainer.ActiveMwiChild != null && !AppViewModel.Instance.MwiContainer.ActiveMwiChild.IsWindowed) // Is Ctrl+F4 key pressed
            {
                AppViewModel.Instance.MwiContainer.ActiveMwiChild.CmdClose.Execute(null);
                e.Handled = true;
            }
        }

        private void MwiStartup_Active_OnChanged(object sender, EventArgs e) => TopControl.Focused = IsActive;

        //============  Test window  =============
        private static MwiChild Window1;
        public RelayCommand CmdDisableDetach { get; } = new RelayCommand(o => Window1.AllowDetach = false);
        public RelayCommand CmdEnableDetach { get; } = new RelayCommand(o => Window1.AllowDetach = true);
        public RelayCommand CmdDisableMinimize { get; } = new RelayCommand(o => Window1.AllowMinimize = false);
        public RelayCommand CmdEnableMinimize { get; } = new RelayCommand(o => Window1.AllowMinimize = true);
        public RelayCommand CmdDisableMaximize { get; } = new RelayCommand(o => Window1.AllowMaximize = false);
        public RelayCommand CmdEnableMaximize { get; } = new RelayCommand(o => Window1.AllowMaximize = true);
        public RelayCommand CmdDisableClose { get; } = new RelayCommand(o => Window1.AllowClose = false);
        public RelayCommand CmdEnableClose { get; } = new RelayCommand(o => Window1.AllowClose = true);
        public RelayCommand CmdHideIcon { get; } = new RelayCommand(o => Window1.ShowIcon = false);
        public RelayCommand CmdShowIcon { get; } = new RelayCommand(o => Window1.ShowIcon = true);
    }
}
