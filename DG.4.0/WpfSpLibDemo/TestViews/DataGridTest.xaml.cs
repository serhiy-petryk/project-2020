using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfSpLib.Common.ColorSpaces;
using WpfSpLib.Helpers;
using WpfSpLibDemo.Samples;

namespace WpfSpLibDemo.TestViews
{
    /// <summary>
    /// Interaction logic for DataGridTest.xaml
    /// </summary>
    public partial class DataGridTest : Window
    {
        #region ============== Properties/Events  ===================
        public static readonly DependencyProperty BaseHslProperty = DependencyProperty.Register("BaseHsl",
            typeof(HSL_Observable), typeof(DataGridTest), new FrameworkPropertyMetadata(null));
        public HSL_Observable BaseHsl
        {
            get => (HSL_Observable)GetValue(BaseHslProperty);
            set => SetValue(BaseHslProperty, value);
        }
        #endregion
        public BindingList<FakeData> Data { get; } = new BindingList<FakeData>();
        public DataGridTest()
        {
            InitializeComponent();
            Grid.AutoGenerateColumns = true;
            Grid.ItemsSource = Data;
            BtnGenerate_OnClick(null, null);
            var hsl = new HSL(new RGB(11 * 16 / 256.0, (12 * 16 + 4) / 256.0, (13 * 16 + 14) / 256.0));//FFB0C4DE
            BaseHsl = new HSL_Observable() { Hue = hsl.Hue, Saturation = hsl.Saturation, Lightness = hsl.Lightness };
        }

        private void BtnGenerate_OnClick(object sender, RoutedEventArgs e)
        {
            Data.Clear();
            var cnt = Convert.ToInt32(ItemCount.Text);

            Data.RaiseListChangedEvents = false;
            for (var k=0; k<cnt;k++)
                Data.Add(new FakeData());
            Data.RaiseListChangedEvents = true;
            Data.ResetBindings();
        }

        private void Grid_OnLoadingRow(object sender, DataGridRowEventArgs e)
        {
            var rowHeaderText = (e.Row.GetIndex() + 1).ToString("N0", LocalizationHelper.CurrentCulture);
            if (!Equals(e.Row.Header, rowHeaderText)) e.Row.Header = rowHeaderText;
        }

        private void ChangeHsl_OnClick(object sender, RoutedEventArgs e)
        {
            /*var hsl = Keyboard.BaseHsl;
            var a = (hsl.Hue + 30.0) % 360;
            hsl.Hue = a;*/
        }
    }
}
