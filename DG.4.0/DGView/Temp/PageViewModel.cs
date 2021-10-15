using System;
using System.ComponentModel;
using System.Printing;
using System.Windows;
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

        //================
        public double MarginLeft
        {
            get => Math.Round(Margins.Left / PageSize.CurrentFactor, 2);
            set
            {
                Margins = new Thickness(value * PageSize.CurrentFactor, Margins.Top, Margins.Right, Margins.Bottom);
                OnPropertiesChanged(nameof(MarginLeft), nameof(Margins));
            }
        }
        public double MarginTop
        {
            get => Math.Round(Margins.Top / PageSize.CurrentFactor, 2);
            set
            {
                Margins = new Thickness(Margins.Left, value * PageSize.CurrentFactor, Margins.Right, Margins.Bottom);
                OnPropertiesChanged(nameof(MarginTop), nameof(Margins));
            }
        }
        public double MarginRight
        {
            get => Math.Round(Margins.Right / PageSize.CurrentFactor, 2);
            set
            {
                Margins = new Thickness(Margins.Left, Margins.Top, value * PageSize.CurrentFactor, Margins.Bottom);
                OnPropertiesChanged(nameof(MarginRight), nameof(Margins));
            }
        }
        public double MarginBottom
        {
            get => Math.Round(Margins.Bottom / PageSize.CurrentFactor, 2);
            set
            {
                Margins = new Thickness(Margins.Left, Margins.Top, Margins.Right, value * PageSize.CurrentFactor);
                OnPropertiesChanged(nameof(MarginBottom), nameof(Margins));
            }
        }
        //================

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

            PageSize.StaticPropertyChanged += PageSize_StaticPropertyChanged;
        }

        private void PageSize_StaticPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertiesChanged(nameof(MarginLeft), nameof(MarginTop), nameof(MarginRight), nameof(MarginBottom), nameof(Size));
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
    }
}
