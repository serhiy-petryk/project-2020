using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using WpfSpLib.Controls;
using WpfSpLib.Themes;
using WpfSpLibDemo.Samples;

namespace WpfSpLibDemo.TestViews
{
    /// <summary>
    /// Interaction logic for MwiTests.xaml
    /// </summary>
    public partial class MwiTests : Window
    {
        public MwiTests()
        {
            InitializeComponent();
        }

        private int cnt = 0;
        private void AddChild_OnClick(object sender, RoutedEventArgs e)
        {
            MwiContainer.Children.Add(new MwiChild
            {
                Title = "Window Using Code",
                Content = $"New MwiChild: {cnt++}",
                Width = 300,
                Height = 200,
                StatusBar = new MwiStatusBarSample(),
                CommandBar = new MwiCommandBarSample()
            // AllowDetach = false, AllowMinimize = false
        });
        }

        private void AddChild2_OnClick(object sender, RoutedEventArgs e)
        {
            MwiContainer.Children.Add(new MwiChild
            {
                Title = "Window Using Code",
                Content = new ResizableSample { Width = double.NaN, Height = double.NaN },
                Width = 300,
                Height = 200,
                Position = new Point(300, 80)
            });
        }

        private void Test_OnClick(object sender, RoutedEventArgs e)
        {
            // foreach (var c in MwiContainer.Children)
            // c.AllowDetach = !c.AllowDetach;
            ((MwiChild)MwiContainer.Children[0]).Focus();
            var a1 = Keyboard.FocusedElement;
        }

        private void OpenWindow_OnClick(object sender, RoutedEventArgs e)
        {
            var wnd = new Window();
            wnd.Show();
        }

        //============  Test window  =============
        private void AddDialog_OnClick(object sender, RoutedEventArgs e)
        {
            var content = new ResizableSample{HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top};
            var adorner = new DialogAdorner(MwiContainer) { CloseOnClickBackground = true };
            adorner.ShowContentDialog(content);
        }

        private void AddResizableDialog_OnClick(object sender, RoutedEventArgs e)
        {
            var content = new ResizableSample {Width = double.NaN, Height = double.NaN};
            var resizable = new ResizableControl{Content = content};
            var adorner = new DialogAdorner(MwiContainer);
            adorner.ShowContentDialog(resizable);
        }

        private void AddMwiDialog_OnClick(object sender, RoutedEventArgs e)
        {
            var content = new MwiChild
            {
                Content = new ResizableSample(),
                LimitPositionToPanelBounds = true,
                VisibleButtons = MwiChild.Buttons.Close | MwiChild.Buttons.Maximize,
                Title = "Dialog window"
            };
            var adorner = new DialogAdorner(MwiContainer) { CloseOnClickBackground = true };
            content.IsActive = true;
            adorner.ShowContentDialog(content);
        }

        private void MwiTests_OnKeyDown(object sender, KeyEventArgs e)
        {
            var mwiContainer = MwiContainer;
            if (Keyboard.Modifiers == ModifierKeys.Control && Keyboard.IsKeyDown(Key.F4) && mwiContainer.ActiveMwiChild != null && !mwiContainer.ActiveMwiChild.IsWindowed) // Is Ctrl+F4 key pressed
            {
                mwiContainer.ActiveMwiChild.CmdClose.Execute(null);
                e.Handled = true;
            }
        }

        private void OnChangeColorClick(object sender, RoutedEventArgs e)
        {
            if (MwiContainer.ThemeColor != Colors.Green)
                MwiContainer.ThemeColor = Colors.Green;
            else
                MwiContainer.ThemeColor = Colors.Yellow;
        }

        private void OnChangeThemeClick(object sender, RoutedEventArgs e)
        {
            MwiContainer.Theme = MwiThemeInfo.GetNexThemeInfo(MwiContainer.ActualTheme);
            Debug.Print($"Theme: {MwiContainer.Theme.Name}");
        }
    }
}
