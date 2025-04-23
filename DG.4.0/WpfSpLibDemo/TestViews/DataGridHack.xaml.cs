using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using WpfSpLib.Common.ColorSpaces;
using WpfSpLib.Helpers;
using WpfSpLibDemo.Samples;

namespace WpfSpLibDemo.TestViews
{
    /// <summary>
    /// Interaction logic for DataGridHack.xaml
    /// </summary>
    public partial class DataGridHack : Window
    {
        #region ============== Properties/Events  ===================
        public static readonly DependencyProperty BaseHslProperty = DependencyProperty.Register("BaseHsl",
            typeof(HSL_Observable), typeof(DataGridHack), new FrameworkPropertyMetadata(null));
        public HSL_Observable BaseHsl
        {
            get => (HSL_Observable)GetValue(BaseHslProperty);
            set => SetValue(BaseHslProperty, value);
        }
        #endregion

        public IList<Author> Data { get; } = Author.Authors;
        public Author.Level[] EnumList { get; } = Enum.GetValues<Author.Level>();

        public DataGridHack()
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

        private void TestDataGrid1_OnBeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            var cell = DataGridHelper.GetCell((DataGrid)sender, e.Row, e.Column);
            cell.Dispatcher.BeginInvoke(() =>
            {
                var a1 = cell.GetVisualChildren().OfType<TextBox>().FirstOrDefault();
                if (a1 != null)
                {
                    a1.VerticalAlignment = VerticalAlignment.Stretch;
                    a1.VerticalContentAlignment = VerticalAlignment.Center;
                    a1.TextWrapping = TextWrapping.Wrap; // Multiline text
                    a1.AcceptsReturn = true; // Multiline text
                    a1.Background = Brushes.LightCyan;
                }
            }, DispatcherPriority.Normal);
        }
    }
}
