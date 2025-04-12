﻿using System;
using System.ComponentModel;
using System.Windows;
using WpfSpLib.Common.ColorSpaces;
using WpfSpLibDemo.Samples;

namespace WpfSpLibDemo.TestViews
{
    /// <summary>
    /// Interaction logic for ListViewTest.xaml
    /// </summary>
    public partial class ListViewTest : Window
    {
        #region ============== Properties/Events  ===================
        public static readonly DependencyProperty BaseHslProperty = DependencyProperty.Register("BaseHsl",
            typeof(HSL_Observable), typeof(ListViewTest), new FrameworkPropertyMetadata(null));
        public HSL_Observable BaseHsl
        {
            get => (HSL_Observable)GetValue(BaseHslProperty);
            set => SetValue(BaseHslProperty, value);
        }
        public static readonly DependencyProperty BaseHsl2Property = DependencyProperty.Register("BaseHsl2",
            typeof(HSL_Observable), typeof(ListViewTest), new FrameworkPropertyMetadata(null));
        public HSL_Observable BaseHsl2
        {
            get => (HSL_Observable)GetValue(BaseHsl2Property);
            set => SetValue(BaseHsl2Property, value);
        }
        #endregion
        public BindingList<FakeData> Data { get; } = new BindingList<FakeData>();
        public ListViewTest()
        {
            InitializeComponent();
            List1.ItemsSource = Data;

            BtnGenerate_OnClick(null, null);
            BtnColor1_OnClick(null, null);

            var hsl2 = new HSL(new RGB(0xF5 / 256.0, 0xFA / 256.0, 0xFF / 256.0));//FFB0C4DE
            BaseHsl2 = new HSL_Observable() { Hue = hsl2.Hue, Saturation = hsl2.Saturation, Lightness = hsl2.Lightness };
        }

        private void BtnGenerate_OnClick(object sender, RoutedEventArgs e)
        {
            var cnt = Convert.ToInt32(ItemCount.Text);
            FakeData.GenerateData(Data, cnt);
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
    }
}
