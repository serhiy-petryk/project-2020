using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using WpfSpLib.Common;
using WpfSpLib.Helpers;
using WpfSpLibDemo.Helpers;

namespace WpfSpLibDemo
{
    /// <summary>
    /// Interaction logic for MwiStartup.xaml
    /// </summary>
    public partial class MwiStartup
    {
        public RelayCommand CmdScaleSliderReset { get; }

        public MwiStartup()
        {
            InitializeComponent();

            CmdScaleSliderReset = new RelayCommand(p => ScaleSlider.Value = 1.0);
            LocalizationHelper.LanguageChanged += LocalizationHelper_LanguageChanged;

            // TopControl.RestoreRectFromSetting();

            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (TopControl.Template.FindName("ContentBorder", TopControl) is FrameworkElement topContentControl)
                {
                    if (!(topContentControl.LayoutTransform is ScaleTransform))
                        topContentControl.LayoutTransform = new ScaleTransform();
                    var transform = (ScaleTransform) topContentControl.LayoutTransform;
                    BindingOperations.SetBinding(transform, ScaleTransform.ScaleXProperty, new Binding("Value") { Source = ScaleSlider });
                    BindingOperations.SetBinding(transform, ScaleTransform.ScaleYProperty, new Binding("Value") { Source = ScaleSlider });

                }
            }), DispatcherPriority.Normal);

            Dispatcher.BeginInvoke(new Action(() => LocalizationHelper_LanguageChanged(null, null)), DispatcherPriority.ApplicationIdle);
        }

        private void LocalizationHelper_LanguageChanged(object sender, EventArgs e)
        {
            var iconContent = (LanguageButton.Content as FrameworkElement).GetVisualChildren().OfType<Viewbox>().FirstOrDefault(a => a.Name == "IconContent");
            if (iconContent != null)
                iconContent.Child = Application.Current.Resources[$"LanguageIcon{LocalizationHelper.CurrentCulture.IetfLanguageTag.ToUpper()}"] as FrameworkElement;

            iconContent = (LanguageButton2.Content as FrameworkElement).GetVisualChildren().OfType<Viewbox>().FirstOrDefault(a => a.Name == "IconContent2");
            if (iconContent != null)
                iconContent.Child = Application.Current.Resources[$"LanguageIcon{LocalizationHelper.CurrentCulture.IetfLanguageTag.ToUpper()}"] as FrameworkElement;

            iconContent = (LanguageButton3.Content as FrameworkElement).GetVisualChildren().OfType<Viewbox>().FirstOrDefault(a => a.Name == "IconContent3");
            if (iconContent != null)
                iconContent.Child = Application.Current.Resources[$"LanguageIcon{LocalizationHelper.CurrentCulture.IetfLanguageTag.ToUpper()}"] as FrameworkElement;
        }

        private void MwiStartup_OnKeyDown(object sender, KeyEventArgs e)
        {
            var mwiContainer = MwiContainer;
            if (Keyboard.Modifiers == ModifierKeys.Control && Keyboard.IsKeyDown(Key.F4) && mwiContainer.ActiveMwiChild != null && !mwiContainer.ActiveMwiChild.IsWindowed) // Is Ctrl+F4 key pressed
            {
                mwiContainer.ActiveMwiChild.CmdClose.Execute(null);
                e.Handled = true;
            }
        }
    }
}
