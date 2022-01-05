using System.Windows;
using System.Windows.Media;
using WpfSpLib.Helpers;
using WpfSpLib.Themes;

namespace WpfSpLibDemo.TestViews
{
    /// <summary>
    /// Interaction logic for FormControlStylesTests.xaml
    /// </summary>
    public partial class FormControlStylesTests : Window, IColorThemeSupport
    {
        public FormControlStylesTests()
        {
            InitializeComponent();
        }

        public MwiThemeInfo Theme { get; set; } 
        public Color? ThemeColor { get; set; }
        public MwiThemeInfo ActualTheme { get; }
        public Color ActualThemeColor => (Color) ColorConverter.ConvertFromString("#FFF5FAFF");
        public IColorThemeSupport ColorThemeParent { get; }
    }
}
