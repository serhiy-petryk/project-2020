using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Printing;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Documents.Serialization;
using System.Windows.Media;
using System.Windows.Threading;
using WpfSpLib.Common;

namespace DGView.Temp
{
    public class PrintPreviewViewModel: INotifyPropertyChanged
    {
        private static readonly string _defaultPrinterName = new LocalPrintServer().DefaultPrintQueue.FullName;

        public static Printer[] Printers { get; } = new LocalPrintServer().GetPrintQueues().Select(p => new Printer(p)).ToArray();

        private static Printer _currentPrinter = Printers.FirstOrDefault(p => _defaultPrinterName == p.PrintQueue.FullName) ?? (Printers.Length > 0 ? Printers[0] : null);
        public static Printer CurrentPrinter {
            get=> _currentPrinter;
            set
            {
                _currentPrinter = value;
                OnStaticPropertiesChanged(nameof(CurrentPrinter));
            }
        }

        //=====================================
        private readonly DocumentViewer _documentViewer;
        private readonly IPrintContentGenerator _printContentGenerator;

        public RelayCommand PageSetupCommand { get; set; }
        public RelayCommand PrintCommand { get; }
        public Visibility StopButtonVisibility { get; private set; } = Visibility.Collapsed;
        public PrintPreviewViewModel(DocumentViewer documentViewer, IPrintContentGenerator printContentGenerator)
        {
            _documentViewer = documentViewer;
            _printContentGenerator = printContentGenerator;

            PageSetupCommand = new RelayCommand(o =>
            {
                var wnd = new PageSetupWindow(CurrentPrinter.Page) {Owner = Window.GetWindow(_documentViewer)};
                if (wnd.ShowDialog() == true)
                {
                    CurrentPrinter.Page = wnd.ViewModel.GetPageModel();
                    GenerateContent();
                }
            });

            PrintCommand = new RelayCommand(o =>
            {
                if (CurrentPrinter != null)
                {
                    //cancelButton.IsEnabled = false;
                    //printButton.IsEnabled = false;
                    Helpers.DoEventsHelper.DoEvents();
                    var viewer = (DocumentViewer)o;
                    CurrentPrinter.PrintDocument(viewer.Document);

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
            if (_printContentGenerator != null)
            {
                StopButtonVisibility = Visibility.Visible;
                OnPropertiesChanged(nameof(StopButtonVisibility));

                var margins = CurrentPrinter.Page.Margins;
                var fixedDoc = new FixedDocument();
                WpfSpLib.Helpers.ControlHelper.SetCurrentValueSmart(_documentViewer, DocumentViewerBase.DocumentProperty, fixedDoc);
                fixedDoc.DocumentPaginator.PageSize = new Size(CurrentPrinter.Page.ActualPageWidth, CurrentPrinter.Page.ActualPageHeight);

                _printContentGenerator.GenerateContent(fixedDoc, margins);

                StopButtonVisibility = Visibility.Collapsed;
                OnPropertiesChanged(nameof(StopButtonVisibility));

                // Invalidate PageSize while printing => to remove flickering/delay need to use timer (Dispatcher.BeginInvoke is not working)
                var timer = new DispatcherTimer {Interval = TimeSpan.FromMilliseconds(100)};
                timer.Tick += OnTimerTick;
                timer.Start();

                void OnTimerTick(object sender, EventArgs e)
                {
                    timer.Tick -= OnTimerTick;
                    timer.Stop();
                    var mi = typeof(DocumentViewerBase).GetMethod("DocumentChanged", BindingFlags.Instance | BindingFlags.NonPublic);
                    mi.Invoke(_documentViewer, new[] { fixedDoc, fixedDoc });
                }
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
            public PrintQueue PrintQueue { get; }
            public PageViewModel Page { get; internal set; }
            public Geometry Icon { get; }
            public RelayCommand PrinterSelectCommand { get; }

            public Printer(PrintQueue printQueue)
            {
                PrintQueue = printQueue;

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
                else if (printerName.Contains("ONENOTE"))
                    iconGeometryName = "OneNoteGeometry";
                else if (printerName == "FAX")
                    iconGeometryName = "FaxGeometry";
                else
                    iconGeometryName = "PrinterGeometry";
                Icon = (Geometry) Application.Current.Resources[iconGeometryName];

                PrinterSelectCommand = new RelayCommand(o => CurrentPrinter = this);
            }

            public void PrintDocument(IDocumentPaginatorSource printDocument)
            {
                var printTicket = new PrintTicket();
                // var printTicket = PrintQueue.DefaultPrintTicket;
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
                printTicket.PageOrientation = Page.Orientation;
                /*if (orientationComboBox.SelectedIndex == 0)
                {
                    printTicket.PageOrientation = PageOrientation.Portrait;
                }
                else
                {
                    printTicket.PageOrientation = PageOrientation.Landscape;
                }*/
                printTicket.PageMediaSize = Page.Size.MediaSize;
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

                // printDialog.PrintQueue = PrintQueue;

                /*try
                {
                    // printDialog.PrintDocument(printDocument.DocumentPaginator, "_documentName");
                    var mi = typeof(System.Windows.Controls.PrintDialog).GetMethod("CreateWriter", BindingFlags.Instance | BindingFlags.NonPublic);
                    var a1 = (XpsDocumentWriter)mi.Invoke(printDialog, new[] { "_documentName" });
                    a1.Write(printDocument.DocumentPaginator, printTicket);
                }
                catch (Exception ex)
                {
                }*/

                //Create a document writer to print to.
                var xpsDocumentWriter = PrintQueue.CreateXpsDocumentWriter(PrintQueue);

                //We want to know when the printing progress has changed so
                //we can update the UI.
                /*xpsDocumentWriter.WritingProgressChanged += PrintAsync_WritingProgressChanged;

                //We also want to know when the print job has finished, allowing
                //us to check for any problems.
                xpsDocumentWriter.WritingCompleted += PrintAsync_Completed;*/

                // StartLongPrintingOperation(fixedDocument.Pages.Count);

                //Print the FixedDocument asynchronously.
                // xpsDocumentWriter.WriteAsync(printDocument.DocumentPaginator, printTicket);
                // ((FixedDocument) printDocument).PrintTicket = printTicket.Clone();
                xpsDocumentWriter.WriteAsync((FixedDocument)printDocument);
            }

            private void PrintAsync_Completed(object sender, WritingCompletedEventArgs e)
            {
                Debug.Print($"PrintAsync_Completed. {e}");
            }

            private void PrintAsync_WritingProgressChanged(object sender, WritingProgressChangedEventArgs e)
            {
                Debug.Print($"PrintAsync_WritingProgressChanged. {e}");
            }
        }
        #endregion
    }
}
