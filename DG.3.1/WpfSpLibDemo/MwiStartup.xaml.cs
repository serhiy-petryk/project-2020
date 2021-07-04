using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using WpfSpLib.Common;
using WpfSpLib.Helpers;

namespace WpfSpLibDemo
{
    /// <summary>
    /// Interaction logic for MwiStartup.xaml
    /// </summary>
    public partial class MwiStartup: INotifyPropertyChanged
    {
        public RelayCommand CmdScaleSliderReset { get; }

        public MwiStartup()
        {
            InitializeComponent();
            CmdScaleSliderReset = new RelayCommand(p => ScaleSlider.Value = 1.0);

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
        }

        public string LanguageChangeHook
        {
            get
            {
                OnPropertiesChanged(nameof(CurrentLanguageIcon));
                return null;
            }
        }

        public Canvas CurrentLanguageIcon => LocalizationHelper.GetLanguageIcon(LocalizationHelper.CurrentCulture);

        private void MwiStartup_OnKeyDown(object sender, KeyEventArgs e)
        {
            var mwiContainer = MwiContainer;
            if (Keyboard.Modifiers == ModifierKeys.Control && Keyboard.IsKeyDown(Key.F4) && mwiContainer.ActiveMwiChild != null && !mwiContainer.ActiveMwiChild.IsWindowed) // Is Ctrl+F4 key pressed
            {
                mwiContainer.ActiveMwiChild.CmdClose.Execute(null);
                e.Handled = true;
            }
        }

        #region ===========  INotifyPropertyChanged  ===============
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertiesChanged(params string[] propertyNames)
        {
            foreach (var propertyName in propertyNames)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
