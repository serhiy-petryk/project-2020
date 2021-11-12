﻿using System;
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

        private int _printedPageCount;
        public int PrintedPageCount
        {
            get => _printedPageCount;
            private set
            {
                _printedPageCount = value;
                OnPropertiesChanged(nameof(PrintedPageCount));
            }
        }

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
                    PrintDocument();
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
            }
        }

        public void PrintDocument()
        {
            // Invalidate PageSize before printing
            var mi = typeof(DocumentViewerBase).GetMethod("DocumentChanged", BindingFlags.Instance | BindingFlags.NonPublic);
            mi.Invoke(_documentViewer, new[] { _documentViewer.Document, _documentViewer.Document });

            //Create a document writer to print to.
            var xpsDocumentWriter = PrintQueue.CreateXpsDocumentWriter(CurrentPrinter.PrintQueue);

            //We want to know when the printing progress has changed so
            //we can update the UI.
            xpsDocumentWriter.WritingProgressChanged += PrintAsync_WritingProgressChanged;

            //We also want to know when the print job has finished, allowing
            //us to check for any problems.
            xpsDocumentWriter.WritingCompleted += PrintAsync_Completed;

            // StartLongPrintingOperation(fixedDocument.Pages.Count);

            //Print the FixedDocument asynchronously.
            // xpsDocumentWriter.WriteAsync(printDocument.DocumentPaginator, printTicket);
            // ((FixedDocument) printDocument).PrintTicket = printTicket.Clone();
            xpsDocumentWriter.WriteAsync((FixedDocument)_documentViewer.Document);
        }

        private void PrintAsync_Completed(object sender, WritingCompletedEventArgs e)
        {
            PrintedPageCount = 0;
            // Debug.Print($"PrintAsync_Completed. {e}");
        }

        private void PrintAsync_WritingProgressChanged(object sender, WritingProgressChangedEventArgs e)
        {
            PrintedPageCount = e.Number;
            // Debug.Print($"PrintAsync_WritingProgressChanged. {e}");
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
        }
        #endregion
    }
}
