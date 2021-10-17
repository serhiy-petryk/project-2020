using System;
using System.ComponentModel;
using System.Globalization;
using System.Printing;
using System.Windows;
using WpfSpLib.Common;
using WpfSpLib.Controls;

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
        private const double MaxEditorValue = 999999.0;
        private static double CurrentFactor => CurrentMeasurementSystem == MeasurementSystem.US ? USFactor : MetricFactor;
        private static double GetDimension(double value) => Math.Round(value / CurrentFactor, 2);

        #endregion

        #region =========  Instance section (properties)  ===========
        public PageSize[] AvailableSizes { get; }

        private PageOrientation _orientation;
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
                    OnPropertiesChanged(nameof(Orientation));
                    UpdateUI(null, null, null);
                }
            }
        }

        public PageSize Size { get; set; }

        private Thickness _margins { get; set; }
        public double MarginLeft
        {
            get => Math.Round(_margins.Left / CurrentFactor, 2);
            set
            {
                if (value > MaxLeftMargin) value = MaxLeftMargin;
                _margins = new Thickness(Math.Max(value, 0.0) * CurrentFactor, _margins.Top, _margins.Right, _margins.Bottom);
                UpdateUI(null, null, null);
            }
        }
        public double MarginTop
        {
            get => Math.Round(_margins.Top / CurrentFactor, 2);
            set
            {
                if (value > MaxTopMargin) value = MaxTopMargin;
                _margins = new Thickness(_margins.Left, Math.Max(value, 0.0) * CurrentFactor, _margins.Right, _margins.Bottom);
                UpdateUI(null, null, null);
            }
        }
        public double MarginRight
        {
            get => Math.Round(_margins.Right / CurrentFactor, 2);
            set
            {
                if (value > MaxRightMargin) value = MaxRightMargin;
                _margins = new Thickness(_margins.Left, _margins.Top, Math.Max(value, 0.0) * CurrentFactor, _margins.Bottom);
                UpdateUI(null, null, null);
            }
        }
        public double MarginBottom
        {
            get => Math.Round(_margins.Bottom / CurrentFactor, 2);
            set
            {
                if (value > MaxBottomMargin) value = MaxBottomMargin;
                _margins = new Thickness(_margins.Left, _margins.Top, _margins.Right, Math.Max(value, 0.0) * CurrentFactor);
                UpdateUI(null, null, null);
            }
        }
        
        public RelayCommand PageSizeSelectCommand { get; }
        public RelayCommand OkCommand { get; }
        public RelayCommand CloseCommand { get; }
        #endregion

        #region ============  PageSetup specific  ==============
        private double _areaSize;
        private double _minWidth;
        private double _minHeight;
        private double _actualPageWidth => Orientation == PageOrientation.Portrait ? Size._width : Size._height;
        private double _actualPageHeight => Orientation == PageOrientation.Portrait ? Size._height : Size._width;
        private double _areaFactor => Math.Max(Size._width, Size._height) / _areaSize;
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
        public double PrintingAreaWidth => (_actualPageWidth - _margins.Left - _margins.Right) / _areaFactor;

        public double PrintingAreaHeight => (_actualPageHeight - _margins.Top - _margins.Bottom) / _areaFactor;
        public Point PrintingAreaPosition => new Point(_margins.Left / _areaFactor, _margins.Top / _areaFactor);
        public double MaxLeftMargin
        {
            get
            {
                var actualMinWidth = _minWidth * _areaFactor;
                return double.IsNaN(actualMinWidth)
                    ? MaxEditorValue
                    : Math.Round((_actualPageWidth - _margins.Right - actualMinWidth) / CurrentFactor, 2);
            }
        }
        public double MaxRightMargin
        {
            get
            {
                var actualMinWidth = _minWidth * _areaFactor;
                return double.IsNaN(actualMinWidth)
                    ? MaxEditorValue
                    : Math.Round((_actualPageWidth - _margins.Left - actualMinWidth) / CurrentFactor, 2);
            }
        }
        public double MaxTopMargin
        {
            get
            {
                var actualMinHeight = _minHeight * _areaFactor;
                return double.IsNaN(actualMinHeight)
                    ? MaxEditorValue
                    : Math.Round((_actualPageHeight - _margins.Bottom - actualMinHeight) / CurrentFactor, 2);
            }
        }
        public double MaxBottomMargin
        {
            get
            {
                var actualMinHeight = _minHeight * _areaFactor;
                return double.IsNaN(actualMinHeight)
                    ? MaxEditorValue
                    : Math.Round((_actualPageHeight - _margins.Top - actualMinHeight) / CurrentFactor, 2);
            }
        }

        public void UpdateUI(double? areaSize, double? minWidth, double? minHeight)
        {
            if (areaSize.HasValue)
                _areaSize = areaSize.Value;
            if (minWidth.HasValue && !double.IsNaN(minWidth.Value))
                _minWidth = minWidth.Value;
            if (minHeight.HasValue && !double.IsNaN(minHeight.Value))
                _minHeight = minHeight.Value;

            OnPropertiesChanged(nameof(MarginLeft), nameof(MarginTop), nameof(MarginRight), nameof(MarginBottom));
            OnPropertiesChanged(nameof(PageAreaWidth), nameof(PageAreaHeight), nameof(PrintingAreaWidth),
                nameof(PrintingAreaHeight), nameof(PrintingAreaPosition));
            OnPropertiesChanged(nameof(MaxLeftMargin), nameof(MaxTopMargin), nameof(MaxRightMargin), nameof(MaxBottomMargin));
        }
        public void UpdateUiBySlider(double areaSize, ResizableControl printingArea)
        {
            if (!printingArea.IsLoaded) return;
            _areaSize = areaSize;
            var left = printingArea.Position.Value.X * _areaFactor;
            var top = printingArea.Position.Value.Y * _areaFactor;
            var right = _actualPageWidth - printingArea.ActualWidth * _areaFactor - left;
            var bottom = _actualPageHeight - printingArea.ActualHeight * _areaFactor - top;
            _margins = new Thickness(left, top, right, bottom);
            UpdateUI(null, null, null);
        }
        #endregion

        #region ==========  Constructor and methods  =============

        public PageViewModel(PageSize[] availableSizes, PageOrientation orientation, PageSize pageSize, Thickness margins)
        {
            AvailableSizes = availableSizes;
            if (AvailableSizes.Length > 0)
                Size = AvailableSizes[0];
            Orientation = orientation;
            Size = pageSize;
            _margins = margins;

            PageSizeSelectCommand = new RelayCommand(o =>
            {
                Size = (PageSize)o;
                OnPropertiesChanged(nameof(Size));
                UpdateUI(null, null, null);
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

        public PageViewModel GetPageSetupModel() => new PageViewModel(AvailableSizes, Orientation, Size, _margins);
        public PageViewModel GetPageModel() => new PageViewModel(AvailableSizes, Orientation, Size, _margins);

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
