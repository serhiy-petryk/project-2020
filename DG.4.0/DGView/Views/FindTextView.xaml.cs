using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using WpfSpLib.Controls;
using WpfSpLib.Effects;
using WpfSpLib.Helpers;

namespace DGView.Views
{
    /// <summary>
    /// Interaction logic for FindAndReplace.xaml
    /// </summary>
    public partial class FindTextView : UserControl
    {
        private static readonly CommandBinding _closeCommand = new CommandBinding(ApplicationCommands.Close, (s, e1) => ((FindTextView)s).Hide());

        private MwiChild _host;
        public FindTextView(MwiChild host)
        {
            InitializeComponent();
            _host = host;
            CommandBindings.Add(_closeCommand);
        }

        public void ToggleVisibility()
        {
            if (Parent == null)
            {
                var host = _host.GetInternalHost();
                var control = new ResizableControl
                {
                    Content = this,
                    LimitPositionToPanelBounds = true,
                    Resizable = false,
                    Focusable = false,
                    Visibility = Visibility.Hidden
                    // Opacity = 1,
                    // Background = Brushes.GreenYellow
                };
                CornerRadiusEffect.SetCornerRadius(control, CornerRadiusEffect.GetCornerRadius(this));
                control.CommandBindings.Add(new CommandBinding(ApplicationCommands.Close, (s, e1) => host.Children.Remove(control)));
                host.Children.Add(control);

                control.Dispatcher.BeginInvoke(new Action(Show), DispatcherPriority.Background);
            }
            else if (((FrameworkElement)Parent).Opacity < 0.1)
                Show();
            else
                Hide();
        }

        private async void Show()
        {
            var host = _host.GetInternalHost();
            var parent = (FrameworkElement)Parent;
            var left = Math.Max(0.0, (host.ActualWidth - parent.ActualWidth) / 2.0);
            var top = Math.Max(0.0, (host.ActualHeight - parent.ActualHeight) / 2.0);
            parent.Margin = new Thickness(left, top, 0, 0);
            parent.Visibility = Visibility.Visible;

            await Task.WhenAll(AnimationHelper.GetContentAnimations(parent, true));
            ControlHelper.SetFocus(this);
        }
        private async void Hide()
        {
            var parent = (FrameworkElement)Parent;
            await Task.WhenAll(AnimationHelper.GetContentAnimations(parent, false));
            _host.Focus();
        }

        public void Dispose()
        {
            _host = null;
            CommandBindings.Clear();
        }
    }
}
