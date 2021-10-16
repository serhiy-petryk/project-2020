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
        public PageOrientation Orientation {
            get=>_orientation;
            set
            {
                if (!Equals(_orientation, value))
                {
                    if (value == PageOrientation.Landscape)
                        _margins = new Thickness(_margins.Top, _margins.Right, _margins.Bottom, _margins.Left);
                    else
                        _margins = new Thickness(_margins.Bottom, _margins.Left, _margins.Top, _margins.Right);

                    _orientation = value;
                    OnPropertiesChanged(nameof(Orientation), nameof(_margins), nameof(MarginLeft), nameof(MarginTop), nameof(MarginRight), nameof(MarginBottom));
                    UpdateUI();
                }
            }
        }
        private Thickness _margins { get; set; }
        public RelayCommand PageSizeSelectCommand { get; }
        public RelayCommand OkCommand { get; }
        public RelayCommand CloseCommand { get; }

        //================
        public double MarginLeft
        {
            get => Math.Round(_margins.Left / CurrentFactor, 2);
            set
            {
                _margins = new Thickness(value * CurrentFactor, _margins.Top, _margins.Right, _margins.Bottom);
                OnPropertiesChanged(nameof(MarginLeft), nameof(_margins));
            }
        }
        public double MarginTop
        {
            get => Math.Round(_margins.Top / CurrentFactor, 2);
            set
            {
                _margins = new Thickness(_margins.Left, value * CurrentFactor, _margins.Right, _margins.Bottom);
                OnPropertiesChanged(nameof(MarginTop), nameof(_margins));
            }
        }
        public double MarginRight
        {
            get => Math.Round(_margins.Right / CurrentFactor, 2);
            set
            {
                _margins = new Thickness(_margins.Left, _margins.Top, value * CurrentFactor, _margins.Bottom);
                OnPropertiesChanged(nameof(MarginRight), nameof(_margins));
            }
        }
        public double MarginBottom
        {
            get => Math.Round(_margins.Bottom / CurrentFactor, 2);
            set
            {
                _margins = new Thickness(_margins.Left, _margins.Top, _margins.Right, value * CurrentFactor);
                OnPropertiesChanged(nameof(MarginBottom), nameof(_margins));
            }
        }
        private PageSize _size { get; set; }
        private PageOrientation _orientation;
        #endregion

        #region ============  PageSetup specific  ==============
        private Panel _pageArea;
        private Control _printingArea;
        private double _areaSize => ((FrameworkElement) _pageArea.Parent).ActualHeight;
        private double _actualPageWidth => Orientation == PageOrientation.Portrait ? _size._width : _size._height;
        private double _actualPageHeight => Orientation == PageOrientation.Portrait ? _size._height : _size._width;
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

        public PageViewModel(PageSize[] availableSizes, PageOrientation orientation, PageSize pageSize, Thickness margins)
        {
            AvailableSizes = availableSizes;
            if (AvailableSizes.Length > 0)
                _size = AvailableSizes[0];
            Orientation = orientation;
            _size = pageSize;
            _margins = margins;

            PageSizeSelectCommand = new RelayCommand(o =>
            {
                _size = (PageSize)o;
                OnPropertiesChanged(nameof(_size));
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
            OnPropertiesChanged(nameof(MarginLeft), nameof(MarginTop), nameof(MarginRight), nameof(MarginBottom), nameof(_size));
        }

        public PageViewModel GetPageSetupModel(Panel pageArea, Control printingArea) =>
            new PageViewModel(AvailableSizes, Orientation, _size, _margins) {_pageArea = pageArea, _printingArea = printingArea};
        public PageViewModel GetPageModel()
        {
            _pageArea = null;
            _printingArea = null;
            return new PageViewModel(AvailableSizes, Orientation, _size, _margins);
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
