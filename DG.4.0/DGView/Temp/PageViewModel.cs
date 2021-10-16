using System;
using System.ComponentModel;
using System.Globalization;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using WpfSpLib.Common;

namespace DGView.Temp
{
    public partial class PageViewModel: INotifyPropertyChanged
    {
        #region ==========  Static section  ==========
        public enum MeasurementSystem { Metric, US }

        private static MeasurementSystem _currentMeasurementSystem = RegionInfo.CurrentRegion.IsMetric ? MeasurementSystem.Metric : MeasurementSystem.US;
        public static MeasurementSystem CurrentMeasurementSystem
        {
            get => _currentMeasurementSystem;
            set
            {
                _currentMeasurementSystem = value;
                OnStaticPropertiesChanged(nameof(CurrentMeasurementSystem));
            }
        }

        private const double MetricFactor = 96.0 / 2.54;
        private const double USFactor = 96.0;
        private static double CurrentFactor => CurrentMeasurementSystem == MeasurementSystem.US ? USFactor : MetricFactor;
        private static double GetDimension(double value) => Math.Round(value / CurrentFactor, 2);

        #endregion

        #region =========  Instance section (properties)  ===========
        public PageSize[] AvailableSizes { get; }
        public PageSize Size { get; set; }
        private PageOrientation _orientation;
        public PageOrientation Orientation {
            get=>_orientation;
            set
            {
                if (!Equals(_orientation, value))
                {
                    if (value == PageOrientation.Landscape)
                        Margins = new Thickness(Margins.Top, Margins.Right, Margins.Bottom, Margins.Left);
                    else
                        Margins = new Thickness(Margins.Bottom, Margins.Left, Margins.Top, Margins.Right);

                    _orientation = value;
                    OnPropertiesChanged(nameof(Orientation), nameof(Margins), nameof(MarginLeft), nameof(MarginTop), nameof(MarginRight), nameof(MarginBottom));
                    UpdateUI();
                }
            }
        }
        public Thickness Margins { get; set; }
        public RelayCommand PageSizeSelectCommand { get; }
        public RelayCommand OkCommand { get; }
        public RelayCommand CloseCommand { get; }

        //================
        public double MarginLeft
        {
            get => Math.Round(Margins.Left / CurrentFactor, 2);
            set
            {
                Margins = new Thickness(value * CurrentFactor, Margins.Top, Margins.Right, Margins.Bottom);
                OnPropertiesChanged(nameof(MarginLeft), nameof(Margins));
            }
        }
        public double MarginTop
        {
            get => Math.Round(Margins.Top / CurrentFactor, 2);
            set
            {
                Margins = new Thickness(Margins.Left, value * CurrentFactor, Margins.Right, Margins.Bottom);
                OnPropertiesChanged(nameof(MarginTop), nameof(Margins));
            }
        }
        public double MarginRight
        {
            get => Math.Round(Margins.Right / CurrentFactor, 2);
            set
            {
                Margins = new Thickness(Margins.Left, Margins.Top, value * CurrentFactor, Margins.Bottom);
                OnPropertiesChanged(nameof(MarginRight), nameof(Margins));
            }
        }
        public double MarginBottom
        {
            get => Math.Round(Margins.Bottom / CurrentFactor, 2);
            set
            {
                Margins = new Thickness(Margins.Left, Margins.Top, Margins.Right, value * CurrentFactor);
                OnPropertiesChanged(nameof(MarginBottom), nameof(Margins));
            }
        }
        #endregion

        #region ============  PageSetup specific  ==============
        //================
        private Panel _pageArea;
        private Control _printingArea;
        private double _areaSize => ((FrameworkElement) _pageArea.Parent).ActualHeight;
        private double _actualPageWidth => Orientation == PageOrientation.Portrait ? Size._width : Size._height;
        private double _actualPageHeight => Orientation == PageOrientation.Portrait ? Size._height : Size._width;
        public double PageAreaWidth
        {
            get
            {
                if (_actualPageWidth > _actualPageHeight)
                    return _areaSize;
                return _areaSize * _actualPageWidth / _actualPageHeight;
            }
        }
        public double PageAreaHeight
        {
            get
            {
                if (_actualPageHeight > _actualPageWidth)
                    return _areaSize;
                return _areaSize * _actualPageHeight / _actualPageWidth;
            }
        }

        public void UpdateUI()
        {
            OnPropertiesChanged(nameof(PageAreaWidth), nameof(PageAreaHeight));
        }
        #endregion

        #region ==========  Constructor and methods  =============

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

            StaticPropertyChanged += PageSize_StaticPropertyChanged;
        }

        private void PageSize_StaticPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertiesChanged(nameof(MarginLeft), nameof(MarginTop), nameof(MarginRight), nameof(MarginBottom), nameof(Size));
        }
        public PageViewModel GetPageSetupModel(Panel pageArea, Control printingArea) => new PageViewModel(AvailableSizes) { Size = Size, Orientation = Orientation, Margins = Margins, _pageArea = pageArea, _printingArea = printingArea };
        public PageViewModel GetPageModel()
        {
            _pageArea = null;
            _printingArea = null;
            return new PageViewModel(AvailableSizes) {Size = Size, Orientation = Orientation, Margins = Margins};
        }

        #endregion


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
    }
}
