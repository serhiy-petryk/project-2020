using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using WpfSpLib.Common.ColorSpaces;
using WpfSpLib.Helpers;
using WpfSpLibDemo.Samples;

namespace WpfSpLibDemo.TestViews
{
    /// <summary>
    /// Interaction logic for DataGridWithSliderTest.xaml
    /// </summary>
    public partial class DataGridWithSliderTest : Window
    {
        #region ============== Properties/Events  ===================
        public static readonly DependencyProperty BaseHslProperty = DependencyProperty.Register("BaseHsl",
            typeof(HSL_Observable), typeof(DataGridWithSliderTest), new FrameworkPropertyMetadata(null));
        public HSL_Observable BaseHsl
        {
            get => (HSL_Observable)GetValue(BaseHslProperty);
            set => SetValue(BaseHslProperty, value);
        }
        public static readonly DependencyProperty BaseHsl2Property = DependencyProperty.Register("BaseHsl2",
            typeof(HSL_Observable), typeof(DataGridWithSliderTest), new FrameworkPropertyMetadata(null));
        public HSL_Observable BaseHsl2
        {
            get => (HSL_Observable)GetValue(BaseHsl2Property);
            set => SetValue(BaseHsl2Property, value);
        }
        #endregion
        public BindingList<FakeData> Data { get; } = new BindingList<FakeData>();
        public DataGridWithSliderTest()
        {
            InitializeComponent();
            Grid1.AutoGenerateColumns = true;
            Grid1.ItemsSource = Data;
            Grid2.AutoGenerateColumns = true;
            Grid2.ItemsSource = Data;
            BtnGenerate_OnClick(null, null);
            BtnColor1_OnClick(null, null);

            var hsl2 = new HSL(new RGB(0xF5 / 256.0, 0xFA / 256.0, 0xFF / 256.0));//FFB0C4DE
            BaseHsl2 = new HSL_Observable() { Hue = hsl2.Hue, Saturation = hsl2.Saturation, Lightness = hsl2.Lightness };
        }


        private void ResetSlider()
        {
            DGSlider.Maximum = Data.Count - 1;
            DGSlider.Value = 0;
        }

        private void BtnGenerate_OnClick(object sender, RoutedEventArgs e)
        {
            var cnt = Convert.ToInt32(ItemCount.Text);
            FakeData.GenerateData(Data, cnt);
            ResetSlider();
        }

        private void Grid_OnLoadingRow(object sender, DataGridRowEventArgs e)
        {
            var rowHeaderText = (e.Row.GetIndex() + 1).ToString("N0", LocalizationHelper.CurrentCulture);
            if (!Equals(e.Row.Header, rowHeaderText)) e.Row.Header = rowHeaderText;
        }

        private void ChangeHsl_OnClick(object sender, RoutedEventArgs e)
        {
            var hsl = BaseHsl;
            var a = (hsl.Hue + 30.0) % 360;
            hsl.Hue = a;
        }

        private void BtnColor1_OnClick(object sender, RoutedEventArgs e)
        {
            var hsl = new HSL(new RGB(0xB0 / 256.0, 0xC4 / 256.0, 0xDE / 256.0));//FFB0C4DE
            BaseHsl = new HSL_Observable() { Hue = hsl.Hue, Saturation = hsl.Saturation, Lightness = hsl.Lightness };
        }

        private void BtnColor2_OnClick(object sender, RoutedEventArgs e)
        {
            var hsl = new HSL(new RGB(0xF5 / 256.0, 0xFA / 256.0, 0xFF / 256.0));//FFB0C4DE
            BaseHsl = new HSL_Observable() { Hue = hsl.Hue, Saturation = hsl.Saturation, Lightness = hsl.Lightness };
        }


        private int _lastSlidedRowNo = -1;
        private object _scrollLock = new object();
        private void DGSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            System.Threading.Monitor.Enter(_scrollLock);
            try
            {
                if (!_isSliderBusy)
                    Slide();
            }
            finally
            {
                System.Threading.Monitor.Exit(_scrollLock);
            }
            //throw new NotImplementedException();
            var a1 = Convert.ToInt32(DGSlider.Value);
            Grid1.ScrollIntoView(Data[a1]);

            var aa1 = Grid1.GetVisualChildren<ScrollViewer>().ToArray();
            var aa2 = Grid1.GetVisualChildren<ScrollBar>().Where(a=>a.Orientation == Orientation.Vertical).ToArray()[0];
            // aa2.Value += 1000;
        }

        private bool _isSliderBusy = false;
        private void Slide()
        {
            _isSliderBusy = true;
            var rowNo = Convert.ToInt32(DGSlider.Value);
            if (rowNo != _lastSlidedRowNo)
            {
                Debug.Print($"Slide: {rowNo} {DateTime.Now}");
                Grid1.ScrollIntoView(Data[rowNo]);
                Grid1.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        _lastSlidedRowNo = rowNo;
                        _isSliderBusy = false; 
                        Slide();
                    }),
                    DispatcherPriority.Render);
            }
            else
                _isSliderBusy = false;
        }
    }
}
