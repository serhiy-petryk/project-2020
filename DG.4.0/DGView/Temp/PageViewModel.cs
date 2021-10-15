using System;
using System.ComponentModel;
using System.Globalization;
using System.Printing;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using WpfSpLib.Common;

namespace DGView.Temp
{
    public partial class PageViewModel: INotifyPropertyChanged, ICloneable
    {
        public PageSize[] AvailableSizes { get; }
        public PageSize Size { get; set; }
        public PageOrientation Orientation { get; set; }
        public Thickness Margins { get; set; }
        public RelayCommand PageSizeSelectCommand { get; }
        public RelayCommand OkCommand { get; }
        public RelayCommand CloseCommand { get; }

        public PageViewModel(PageSize[] availableSizes)
        {
            AvailableSizes = availableSizes;
            if (AvailableSizes.Length > 0)
                Size = AvailableSizes[0];
            PageSizeSelectCommand = new RelayCommand(o =>
            {
                Size = (PageSize)o;
                OnPropertiesChanged(nameof(Size));
            });

            OkCommand = new RelayCommand(o =>
            {
                var wnd = (Window)o;
                wnd.DialogResult = true;
                wnd.Close();
            });

            CloseCommand = new RelayCommand(o =>
            {
                var wnd = (Window)o;
                wnd.DialogResult = false;
                wnd.Close();
            });
        }

        public object Clone() => new PageViewModel(AvailableSizes) {Size = Size, Orientation = Orientation, Margins = Margins};

        #region ===========  INotifyPropertyChanged  ==============
        public event PropertyChangedEventHandler PropertyChanged;

        internal void OnPropertiesChanged(params string[] propertyNames)
        {
            foreach (var propertyName in propertyNames)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region ===================
        public class PageSizeListBackgroundConverter : IMultiValueConverter
        {
            public static PageSizeListBackgroundConverter Instance = new PageSizeListBackgroundConverter();
            public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
            {
                return Equals(values[0], values[1]) ? Brushes.PaleGreen : null;
            }

            public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
        }
        public class PageSizeListFontWeightConverter : IMultiValueConverter
        {
            public static PageSizeListFontWeightConverter Instance = new PageSizeListFontWeightConverter();
            public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
            {
                return Equals(values[0], values[1]) ? FontWeights.SemiBold : FontWeights.Normal;
            }

            public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
        }
        #endregion

    }
}
