using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Printing;
using System.Windows;
using System.Windows.Media;
using WpfSpLib.Common;
using WpfSpLib.Helpers;

namespace DGView.Temp
{
    public partial class PrintPreviewViewModel: INotifyPropertyChanged
    {
        public enum MeasurementSystem { Metric, US }
        public static MeasurementSystem CurrentMeasurementSystem { get; set; } = RegionInfo.CurrentRegion.IsMetric ? MeasurementSystem.Metric : MeasurementSystem.US;


        public static Printer[] Printers { get; } = new LocalPrintServer().GetPrintQueues().Select(p => new Printer(p)).ToArray();

        private static Printer _currentPrinter;
        public static Printer CurrentPrinter {
            get=> _currentPrinter;
            set
            {
                _currentPrinter = value;
                OnStaticPropertiesChanged(nameof(CurrentPrinter));
            }
        }
        // public PrintCapabilities CurrentPrintCapabilities { get; private set; }

        private int _savedPages = -3;

        public int SavedPages
        {
            get => _savedPages;
            set
            {
                _savedPages = value;
                OnPropertiesChanged(nameof(SavedPages));
            }
        }

        public PrintPreviewViewModel()
        {

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


        #region ==========  Printer subclass  ===========
        public class Printer
        {
            private static readonly string _defaultPrinterName = new LocalPrintServer().DefaultPrintQueue.FullName;

            public PrintQueue PrintQueue { get; }
            public ImageSource Icon { get; }
            public RelayCommand PrinterSelectCommand { get; }

            public PrintPaperSize CurrentPaperSize;
            public Thickness CurrentMargins;

            public Printer(PrintQueue printQueue)
            {
                PrintQueue = printQueue;
                if (CurrentPrinter == null || _defaultPrinterName == printQueue.FullName)
                    CurrentPrinter = this;
                PrinterSelectCommand = new RelayCommand(o => CurrentPrinter = this);
                Icon = LocalizationHelper.GetLanguageIcon("DE");
            }
        }
        #endregion
    }
}
