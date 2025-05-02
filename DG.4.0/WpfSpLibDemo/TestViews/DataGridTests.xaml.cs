using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfSpLib.Common.ColorSpaces;
using WpfSpLib.Helpers;
using WpfSpLibDemo.Samples;

namespace WpfSpLibDemo.TestViews
{
    /// <summary>
    /// Interaction logic for DataGridTests.xaml
    /// </summary>
    public partial class DataGridTests : Window
    {
        #region ============== Properties/Events  ===================
        public static readonly DependencyProperty BaseHslProperty = DependencyProperty.Register("BaseHsl",
            typeof(HSL_Observable), typeof(DataGridTests), new FrameworkPropertyMetadata(null));
        public HSL_Observable BaseHsl
        {
            get => (HSL_Observable)GetValue(BaseHslProperty);
            set => SetValue(BaseHslProperty, value);
        }
        #endregion

        public IList<AuthorIDataErrorInfo> Data { get; } = AuthorIDataErrorInfo.Authors;
        public Author.Level[] EnumList { get; } = Enum.GetValues<Author.Level>();

        public DataGridTests()
        {
            InitializeComponent();
            DataContext = this;
            var xhsl = new HSL(new RGB(0xB0 / 256.0, 0xC4 / 256.0, 0xDE / 256.0));//FFB0C4DE
            var hsl = new HSL(new RGB(0x78 / 256.0, 0xBC / 256.0, 0xFF / 256.0));//FF78BCFF
            BaseHsl = new HSL_Observable() { Hue = hsl.Hue, Saturation = hsl.Saturation, Lightness = hsl.Lightness };
        }

        private void DataGrid_OnThreeStateSorting(object sender, DataGridSortingEventArgs e) =>
            DataGridHelper.DataGrid_OnSorting((DataGrid)sender, e);

        private void ChangeHsl_OnClick(object sender, RoutedEventArgs e)
        {
            var hsl = BaseHsl;
            var a = (hsl.Hue + 30.0) % 360;
            hsl.Hue = a;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var currentCellInfo = TestDataGrid4.CurrentCell;
            var row = TestDataGrid4.ItemContainerGenerator.ContainerFromItem(currentCellInfo.Item) as DataGridRow;
            var cell = DataGridHelper.GetCell(TestDataGrid4, row, currentCellInfo.Column);

            var children = cell.GetVisualChildren().ToArray();
            var textBlock = children.OfType<TextBlock>().FirstOrDefault();
            if (textBlock != null)
            {
                textBlock.Margin = new Thickness(0);
            }
        }

        private void DataGridTests_OnClosing(object sender, CancelEventArgs e) => DataGridHelper.Control_OnClosing(this);

        private void TestDataGrid4_OnLoadingRow(object sender, DataGridRowEventArgs e)
        {
            string rowHeaderText;
            if (e.Row.IsNewItem)
                rowHeaderText = "*"; // ((char)9654).ToString() + "★✶";
            else
                rowHeaderText = (e.Row.GetIndex() + 1).ToString("N0", LocalizationHelper.CurrentCulture);

            if (!Equals(e.Row.Header, rowHeaderText)) e.Row.Header = rowHeaderText;
        }
    }
}
