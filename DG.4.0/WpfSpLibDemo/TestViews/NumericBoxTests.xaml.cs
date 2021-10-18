using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;

namespace WpfSpLibDemo.TestViews
{
    /// <summary>
    /// Interaction logic for NumericBoxTests.xaml
    /// </summary>
    public partial class NumericBoxTests : INotifyPropertyChanged
    {
        private static string[] _cultures = { "", "sq-AL", "uk-UA", "en-US" };

        public List<CultureInfo> CultureAllInfos { get; set; } = CultureInfo.GetCultures(CultureTypes.InstalledWin32Cultures).OrderBy(c => c.DisplayName).ToList();

        public List<CultureInfo> CultureInfos { get; set; } = CultureInfo
            .GetCultures(CultureTypes.InstalledWin32Cultures).Where(c => Array.IndexOf(_cultures, c.Name) != -1)
            .OrderBy(c => c.DisplayName).ToList();

        public NumericBoxTests()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void XButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            AA.Value = (AA.Value ?? 100M) + 10;
            AA.ButtonsWidth += 2;
        }

        #region ===========  INotifyPropertyChanged  ==============
        public event PropertyChangedEventHandler PropertyChanged;

        internal void OnPropertiesChanged(params string[] propertyNames)
        {
            foreach (var propertyName in propertyNames)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        // ========  For static properties  ========
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;
        internal static void OnStaticPropertiesChanged(params string[] propertyNames)
        {
            foreach (var propertyName in propertyNames)
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        // public double MarginLeft { get; set; } = 5;
        public double _marginLeft = 5;

        public double MarginLeft
        {
            get => _marginLeft;
            set
            {
                _marginLeft = value;
                OnPropertiesChanged(nameof(MarginLeft));
            }
        }
        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Left.Value += 10;
            Left2.Value += 10;
        }

        public static readonly DependencyProperty MarginLeft2Property = DependencyProperty.Register("MarginLeft2", typeof(decimal?), typeof(NumericBoxTests), new FrameworkPropertyMetadata(5m, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public decimal? MarginLeft2
        {
            get => (decimal?)GetValue(MarginLeft2Property);
            set => SetValue(MarginLeft2Property, value);
        }
    }
}
