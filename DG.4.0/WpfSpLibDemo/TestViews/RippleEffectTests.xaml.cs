using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WpfSpLib.Helpers;
using WpfSpLib.Themes;

namespace WpfSpLibDemo.TestViews
{
    /// <summary>
    /// Interaction logic for RippleEffectTests.xaml
    /// </summary>
    public partial class RippleEffectTests : Window, IColorThemeSupport
    {
        #region ========  IColorThemeSupport ==========
        public MwiThemeInfo Theme { get; set; }
        public Color? ThemeColor { get; set; }
        public MwiThemeInfo ActualTheme { get; }
        public Color ActualThemeColor => Colors.White;
        public IColorThemeSupport ColorThemeParent { get; }
        #endregion

        public RippleEffectTests()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Debug.Print($"Click: {((ContentControl)sender).Content}");
        }
    }
}
