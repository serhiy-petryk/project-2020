﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using WpfSpLib.Common;

namespace DGView.Temp
{
    public class PrintPreviewViewModel: INotifyPropertyChanged
    {
        /*public static Printer[] Printers { get; } = new LocalPrintServer().GetPrintQueues().Select(p => new Printer(p)).ToArray();

        private static Printer _currentPrinter;
        public static Printer CurrentPrinter {
            get=> _currentPrinter;
            set
            {
                _currentPrinter = value;
                OnStaticPropertiesChanged(nameof(CurrentPrinter));
            }
        }*/

        //=====================================
        private readonly DocumentViewer _documentViewer;
        private readonly IPrintContentGenerator _printContentGenerator;

        // private FixedDocument _printDocument;
        public RelayCommand PageSetupCommand { get; set; }
        public RelayCommand PrintCommand { get; }
        public Visibility StopButtonVisibility { get; private set; } = Visibility.Collapsed;
        public PrintPreviewViewModel(DocumentViewer documentViewer, IPrintContentGenerator printContentGenerator)
        {
            _documentViewer = documentViewer;
            _printContentGenerator = printContentGenerator;

            PageSetupCommand = new RelayCommand(o =>
            {
                var wnd = new PageSetupWindow(Printer.CurrentPrinter.Page) {Owner = Window.GetWindow(_documentViewer)};
                if (wnd.ShowDialog() == true)
                {
                    Printer.CurrentPrinter.Page = wnd.ViewModel.GetPageModel();
                }
            });

            PrintCommand = new RelayCommand(o =>
            {
                if (Printer.CurrentPrinter != null)
                {
                    //cancelButton.IsEnabled = false;
                    //printButton.IsEnabled = false;
                    Helpers.DoEventsHelper.DoEvents();
                    var viewer = (DocumentViewer)o;
                    Printer.CurrentPrinter.PrintDocument(viewer.Document);

                }
            });

        }

        public void StopContentGeneration()
        {
            if (_printContentGenerator != null)
                _printContentGenerator.StopGeneration = true;
        }

        public void GenerateContent()
        {
            var fixedDoc = new FixedDocument();
            _documentViewer.Document = fixedDoc;

            if (_printContentGenerator != null)
            {
                StopButtonVisibility = Visibility.Visible;
                OnPropertiesChanged(nameof(StopButtonVisibility));

                var pageSize = new Size(Printer.CurrentPrinter.Page.ActualPageWidth, Printer.CurrentPrinter.Page.ActualPageHeight);
                var margins = Printer.CurrentPrinter.Page.Margins;
                _printContentGenerator.GenerateContent(fixedDoc, pageSize, margins);

                StopButtonVisibility = Visibility.Collapsed;
                OnPropertiesChanged(nameof(StopButtonVisibility));
            }
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
            public static Printer[] Printers { get; } = new LocalPrintServer().GetPrintQueues().Select(p => new Printer(p)).ToArray();

            private static Printer _currentPrinter;
            public static Printer CurrentPrinter
            {
                get => _currentPrinter;
                set
                {
                    _currentPrinter = value;
                    OnStaticPropertiesChanged(nameof(CurrentPrinter));
                }
            }


            private static readonly string _defaultPrinterName = new LocalPrintServer().DefaultPrintQueue.FullName;

            //=======================
            public PrintQueue PrintQueue { get; }
            public PageViewModel Page { get; internal set; }
            public Geometry Icon { get; }
            public RelayCommand PrinterSelectCommand { get; }

            public Printer(PrintQueue printQueue)
            {
                PrintQueue = printQueue;
                if (CurrentPrinter == null || _defaultPrinterName == printQueue.FullName)
                    CurrentPrinter = this;

                var availablePageSizes = new List<PageViewModel.PageSize>();
                var printCapabilities = PrintQueue.GetPrintCapabilities();
                foreach (var mediaSize in printCapabilities.PageMediaSizeCapability.Where(a => a.PageMediaSizeName.HasValue))
                {
                    var pageSize = PageViewModel.PageSize.GetPageSize(mediaSize);
                    if (pageSize != null)
                        availablePageSizes.Add(pageSize);
                }

                Page = new PageViewModel(availablePageSizes.ToArray(),
                    PrintQueue.DefaultPrintTicket.PageOrientation ?? PageOrientation.Portrait,
                    PageViewModel.PageSize.GetPageSize(PrintQueue.DefaultPrintTicket.PageMediaSize),
                        new Thickness(67, 72,67,72));

                string iconGeometryName;
                var printerName = printQueue.FullName.ToUpper();
                if (printerName.Contains("XPS"))
                    iconGeometryName = "XPSGeometry";
                else if (printerName.Contains("PDF"))
                    iconGeometryName = "PDFGeometry";
                else if (printerName == "FAX")
                    iconGeometryName = "FaxGeometry";
                else
                    iconGeometryName = "PrinterGeometry";
                Icon = (Geometry) Application.Current.Resources[iconGeometryName];

                PrinterSelectCommand = new RelayCommand(o => CurrentPrinter = this);
            }

            public void PrintDocument(IDocumentPaginatorSource printDocument)
            {
                var printDialog = new System.Windows.Controls.PrintDialog();
                var printTicket = printDialog.PrintTicket;
                //var printer = PrintQueue;
                //if (printer == null)
                  //  return;
                /*if (equipmentComboBox.SelectedItem == null)
                {
                    return;
                }

                ReloadDocument();

                PrintQueue printer = _localDefaultPrintServer.GetPrintQueue((equipmentComboBox.SelectedItem as ComboBoxItem).Tag.ToString());

                PrintTicket printTicket = _systemPrintDialog.PrintTicket;
                printTicket.CopyCount = int.Parse(copiesNumberPicker.Text);
                if (collateCheckBox.IsChecked == true)
                {
                    printTicket.Collation = Collation.Collated;
                }
                else
                {
                    printTicket.Collation = Collation.Uncollated;
                }*/
                printTicket.PageOrientation = CurrentPrinter.Page.Orientation;
                /*if (orientationComboBox.SelectedIndex == 0)
                {
                    printTicket.PageOrientation = PageOrientation.Portrait;
                }
                else
                {
                    printTicket.PageOrientation = PageOrientation.Landscape;
                }*/
                printTicket.PageMediaSize = CurrentPrinter.Page.Size.MediaSize;
                /*if (printer.GetPrintCapabilities().PageMediaSizeCapability.Count > 0)
                {
                    printTicket.PageMediaSize = printer.GetPrintCapabilities().PageMediaSizeCapability[sizeComboBox.SelectedIndex];
                }
                if (printer.GetPrintCapabilities().PageMediaTypeCapability.Count > 0)
                {
                    printTicket.PageMediaType = printer.GetPrintCapabilities().PageMediaTypeCapability[typeComboBox.SelectedIndex];
                }
                if (printer.GetPrintCapabilities().OutputColorCapability.Count > 0)
                {
                    printTicket.OutputColor = printer.GetPrintCapabilities().OutputColorCapability[colorComboBox.SelectedIndex];
                }
                if (printer.GetPrintCapabilities().OutputQualityCapability.Count > 0)
                {
                    printTicket.OutputQuality = printer.GetPrintCapabilities().OutputQualityCapability[qualityComboBox.SelectedIndex];
                }
                if (printer.GetPrintCapabilities().InputBinCapability.Count > 0)
                {
                    printTicket.InputBin = printer.GetPrintCapabilities().InputBinCapability[sourceComboBox.SelectedIndex];
                }
                if (twoSidedCheckBox.IsChecked == true)
                {
                    if (twoSidedTypeComboBox.SelectedIndex == 0)
                    {
                        printTicket.Duplexing = Duplexing.TwoSidedLongEdge;
                    }
                    else
                    {
                        printTicket.Duplexing = Duplexing.TwoSidedShortEdge;
                    }
                }
                else
                {
                    printTicket.Duplexing = Duplexing.OneSided;
                }*/
                printTicket.PageScalingFactor = 100;
                printTicket.PagesPerSheet = 1;
                printTicket.PagesPerSheetDirection = PagesPerSheetDirection.RightBottom;

                printDialog.PrintQueue = PrintQueue;
                printDialog.PrintDocument(printDocument.DocumentPaginator, "_documentName");
            }

        }
        #endregion
    }
}
