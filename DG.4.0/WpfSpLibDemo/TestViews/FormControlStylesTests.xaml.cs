using System.ComponentModel;
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
            TestDataGrid1.ItemsSource = AuthorIDataErrorInfo.Authors;
            TestDataGrid2.ItemsSource = AuthorIDataErrorInfo.Authors;
            TestDataGrid_Original.ItemsSource = AuthorIDataErrorInfo.Authors;
        }
        // private void DataGrid_OnRowEditEnding(object sender, DataGridRowEditEndingEventArgs e) => DataGridHelper.DataGrid_OnRowEditEnding((DataGrid)sender, e);

        #region ========  IColorThemeSupport ==========
        public MwiThemeInfo Theme { get; set; } 
        public Color? ThemeColor { get; set; }
        public MwiThemeInfo ActualTheme { get; }
        public Color ActualThemeColor => (Color) ColorConverter.ConvertFromString("#FFF5FAFF");
        public IColorThemeSupport ColorThemeParent { get; }
        #endregion

        private void DataGrid_OnThreeStateSorting(object sender, DataGridSortingEventArgs e) =>
            DataGridHelper.DataGrid_OnSorting((DataGrid) sender, e);

        private void FormControlStylesTests_OnClosing(object sender, CancelEventArgs e) => DataGridHelper.Control_OnClosing(this);
    }
}
