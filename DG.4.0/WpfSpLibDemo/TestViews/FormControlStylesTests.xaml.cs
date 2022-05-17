using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WpfSpLib.Helpers;
using WpfSpLib.Themes;
using WpfSpLibDemo.Samples;

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
            TestDataGrid1.ItemsSource = Author.Authors;
            TestDataGrid2.ItemsSource = Author.Authors;
        }

        public MwiThemeInfo Theme { get; set; } 
        public Color? ThemeColor { get; set; }
        public MwiThemeInfo ActualTheme { get; }
        public Color ActualThemeColor => (Color) ColorConverter.ConvertFromString("#FFF5FAFF");
        public IColorThemeSupport ColorThemeParent { get; }

        private void DataGrid_OnThreeStateSorting(object sender, DataGridSortingEventArgs e) =>
            DataGridHelper.DataGrid_OnSorting((DataGrid) sender, e);
    }
}
