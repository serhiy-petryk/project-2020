using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using WpfSpLib.Common;
using WpfSpLib.Helpers;
using WpfSpLib.Themes;

namespace WpfSpLibDemo.TestViews
{
    /// <summary>
    /// Interaction logic for ButtonStyleTests.xaml
    /// </summary>
    public partial class ButtonStyleTests : Window, IColorThemeSupport
    {
        #region ========  IColorThemeSupport ==========
        public MwiThemeInfo Theme { get; set; }
        public Color? ThemeColor { get; set; }
        public MwiThemeInfo ActualTheme { get; }
        public Color ActualThemeColor => (Color)ColorConverter.ConvertFromString("#FFF5FAFF");
        public IColorThemeSupport ColorThemeParent { get; }
        #endregion

        public ButtonStyleTests()
        {
            InitializeComponent();
        }

        private void ChangeContent_OnClick(object sender, RoutedEventArgs e)
        {
            TB1.Content = TB1.Content.ToString() + "X";
        }

        private void ChangeBorder_OnClick(object sender, RoutedEventArgs e)
        {
            if (Tips.AreEqual(TestTB.BorderThickness.Right, 2))
                TestTB.BorderThickness = new Thickness(4.0);
            else
                TestTB.BorderThickness = new Thickness(2.0);
        }

        private void UIElement_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Click");
        }
    }
}
