using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Xps.Packaging;

namespace DGView.Temp
{
    /// <summary>
    /// Interaction logic for PrintPreviewWindow.xaml
    /// </summary>
    public partial class PrintPreviewWindow : Window, INotifyPropertyChanged
    {
        private static readonly LocalPrintServer _printServer = new LocalPrintServer();
        private ComboBox _printerComboBox;
        public PrintQueue[] Printers { get; } = new LocalPrintServer().GetPrintQueues().ToArray();
        public PrintQueue CurrentPrinter => _printerComboBox?.SelectedItem as PrintQueue;
        public PrintCapabilities CurrentPrintCapabilities { get; private set; }


        private Size _pageSize = new Size(793, 1122);
        private int _savedPages = -3;
        public int SavedPages => _savedPages;

        private int _itemCount;

        public PrintPreviewWindow()
        {
            InitializeComponent();
            DataContext = this;

            var fixedDoc = new FixedDocument();
            fixedDoc.DocumentPaginator.PageSize = _pageSize;
            ((IAddChild)DocumentViewer).AddChild(fixedDoc);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _printerComboBox = DocumentViewer.Template.FindName("PrinterComboBox", DocumentViewer) as ComboBox;

            if (_printerComboBox != null)
            {
                _printerComboBox.ItemsSource = Printers;
                _printerComboBox.SelectedItem = Printers.FirstOrDefault(p => p.FullName == _printServer.DefaultPrintQueue.FullName);
                if (_printerComboBox.SelectedItem == null && Printers.Length > 0)
                    _printerComboBox.SelectedItem = Printers[0];
            }

            //Dispatcher.BeginInvoke(new Action(() => OnTopPanelSizeChanged(null, null)),
              //  DispatcherPriority.ApplicationIdle);

        }

        #region ========  Test methods  ===========
        private void OnAddDataClick(object sender, RoutedEventArgs e)
        {
            var fixedDoc = (FixedDocument)DocumentViewer.Document;
            for (var k = 0; k < 5; k++)
            {
                var a1 = GetPage(fixedDoc);
                fixedDoc.Pages.Add(a1);
            }
        }

        private PageContent GetPage(FixedDocument fixedDoc)
        {
            double margin = 60;

            //Create a new page and set its size
            //Page's size is equals document's size
            FixedPage fixedPage = new FixedPage()
            {
                Width = fixedDoc.DocumentPaginator.PageSize.Width,
                Height = fixedDoc.DocumentPaginator.PageSize.Height
            };

            //Create a StackPanel and make it cover entire page
            StackPanel stackPanel = CreateContent(fixedDoc.DocumentPaginator.PageSize.Width, fixedDoc.DocumentPaginator.PageSize.Height, margin);

            //Add the StackPanel into the page
            //You can add as many elements as you want into the page, but at here we only need to add one
            fixedPage.Children.Add(stackPanel);

            //Add the page into the document
            //You can't just add FixedPage into FixedDocument, you need use PageContent to host the FixedPage

            //Setup PrintDialog's properties
            /*printDialog.Document = fixedDocument; //Set document that need to print
            printDialog.DocumentName = "Test Document"; //Set document name that will display in print list.
            printDialog.DocumentMargin = margin; //Set document margin info.*/
            var content = new PageContent { Child = fixedPage };
            return content;
        }

        private StackPanel CreateContent(double width, double height, double margin)
        {
            var scale = 0.7;
            //Create a StackPanel and make it cover entire page
            //FixedPage can contains any UIElement. But VerticalAlignment="Stretch" or HorizontalAlignment="Stretch" doesn't work, so you need calculate its size to make it cover the entire page
            var stackPanel = new StackPanel()
            {
                Orientation = Orientation.Vertical,
                Background = Brushes.LightYellow,
                Width = (width - margin * 2) / scale, //Width = Page width - (left margin + right margin)
                Height = (height - margin * 2) / scale //Height = Page height - (top margin + bottom margin)
            };
            stackPanel.LayoutTransform = new ScaleTransform(scale, scale);

            var cols = 14;
            var rows = 43;
            for (var k1 = 0; k1 < rows; k1++)
            {
                var row = new StackPanel { Orientation = Orientation.Horizontal };
                stackPanel.Children.Add(row);
                for (var k2 = 0; k2 < cols; k2++)
                {
                    var border = new Border
                    {
                        BorderBrush = Brushes.Black,
                        Background = Brushes.Aqua,
                        BorderThickness = new Thickness(0, 0, 1, 1),
                        Width = 70,
                        Height = 30
                    };
                    var text = k2 == 0 ? (_itemCount++).ToString() : $"{k1}, {k2}";
                    var cell = new TextBlock
                    {
                        Text = text,
                        Background = Brushes.Aqua,
                        Padding = new Thickness(4),
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    border.Child = cell;
                    row.Children.Add(border);
                }
            }

            stackPanel.Children.Add(new TextBlock()
            {
                Text = $"Page: {((FixedDocument)DocumentViewer.Document).Pages.Count + 1}",
                Margin = new Thickness(10),
                FontSize = 24
            });

            FixedPage.SetTop(stackPanel, 60); //Top margin
            FixedPage.SetLeft(stackPanel, 60); //Left margin

            //Return the StackPanel
            return stackPanel;
        }

        private void OnClearClick(object sender, RoutedEventArgs e)
        {
            var fixedDoc = new FixedDocument();
            fixedDoc.DocumentPaginator.PageSize = _pageSize;
            DocumentViewer.Document = fixedDoc;
            _itemCount = 0;
        }

        private void OnSaveToXpsClick(object sender, RoutedEventArgs e)
        {
            WriteXps(DocumentViewer.Document, @"E:\Users\System\Documents\a191.xps");
        }

        private Stopwatch _sw = new System.Diagnostics.Stopwatch();
        private void WriteXps(IDocumentPaginatorSource fixedDocument, string tempFileName)
        {
            _sw.Restart();
            _savedPages = 0;
            OnPropertiesChanged(nameof(SavedPages));
            if (File.Exists(tempFileName)) File.Delete(tempFileName);

            using (var stream = File.Open(tempFileName, FileMode.Create))
            {
                using (var package = Package.Open(stream, FileMode.Create, FileAccess.ReadWrite))
                {
                    using (var xpsDoc = new XpsDocument(package, CompressionOption.NotCompressed))
                    {
                        var docWriter = XpsDocument.CreateXpsDocumentWriter(xpsDoc);
                        docWriter.WritingProgressChanged += DocWriter_WritingProgressChanged;
                        docWriter.WritingCompleted += DocWriter_WritingCompleted;
                        docWriter.WritingCancelled += DocWriter_WritingCancelled;
                        docWriter.WritingPrintTicketRequired += DocWriter_WritingPrintTicketRequired;
                        docWriter.Write(fixedDocument.DocumentPaginator);
                        // docWriter.WriteAsync((FixedDocument)fixedDocument);
                        // await docWriter.WriteAsync(fixedDocument, XpsDocumentNotificationLevel.ReceiveNotificationEnabled);
                    }
                }
            }
            _sw.Stop();
            Debug.Print($"Sync print: {_sw.ElapsedMilliseconds}");
        }

        private static void DocWriter_WritingPrintTicketRequired(object sender, System.Windows.Documents.Serialization.WritingPrintTicketRequiredEventArgs e)
        {
            Debug.Print($"Event DocWriter_WritingPrintTicketRequired: {e}");
        }
        private static void DocWriter_WritingCancelled(object sender, System.Windows.Documents.Serialization.WritingCancelledEventArgs e)
        {
            Debug.Print($"Event DocWriter_WritingCancelled: {e}");
        }
        private static void DocWriter_WritingCompleted(object sender, System.Windows.Documents.Serialization.WritingCompletedEventArgs e)
        {
            Debug.Print($"Event DocWriter_WritingCompleted: {e}");
        }
        private void DocWriter_WritingProgressChanged(object sender, System.Windows.Documents.Serialization.WritingProgressChangedEventArgs e)
        {
            Debug.Print($"Event DocWriter_WritingProgressChanged: {e}");
            _savedPages = e.Number;
            if (e.Number == 1)
            {

            }
            // DoEvents();
            OnPropertiesChanged(nameof(SavedPages));
            Helpers.DoEventsHelper.DoEvents();
        }
        private void OnPrintDialogClick(object sender, RoutedEventArgs e)
        {
            // Create the print dialog object and set options.
            var printDialog = new PrintDialog
            {
                UserPageRangeEnabled = true
            };

            // Display the dialog. This returns true if the user presses the Print button.
            bool? isPrinted = printDialog.ShowDialog();
            if (isPrinted != true)
                return;

        }

        #endregion

        #region ===========  INotifyPropertyChanged  ==============
        public event PropertyChangedEventHandler PropertyChanged;

        internal void OnPropertiesChanged(params string[] propertyNames)
        {
            foreach (var propertyName in propertyNames)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        private void DocumentPreviewer_ManipulationBoundaryFeedback(object sender, ManipulationBoundaryFeedbackEventArgs e)
        {
            // see https://stackoverflow.com/questions/4505772/wpf-listbox-with-touch-inertia-pulls-down-entire-window
            e.Handled = true;
        }

        private void PrinterComboBox_DropDownOpened(object sender, EventArgs e)
        {
        }

        private void PrinerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void OnTopPanelSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var wrapPanel = DocumentViewer.Template.FindName("TopPanel", DocumentViewer) as WrapPanel;
            UpdateWrapPanelChildrenLayout(wrapPanel);
        }
        private void UpdateWrapPanelChildrenLayout(WrapPanel wrapPanel)
        {
            if (wrapPanel == null || !wrapPanel.IsVisible) return;

            var children = wrapPanel.Children.OfType<FrameworkElement>().Where(item => item.IsVisible).ToArray();
            var offset = 0.0;
            for (var k = 0; k < children.Length; k++)
            {
                var item = children[k];
                var itemWidth = item.ActualWidth + (k == (children.Length - 1) ? 0.0 : item.Margin.Left) + item.Margin.Right;
                if (offset + itemWidth > wrapPanel.ActualWidth)
                    offset = itemWidth;
                else
                    offset += itemWidth;
            }
            var lastItem = children[children.Length - 1];
            lastItem.Margin = new Thickness(wrapPanel.ActualWidth - offset, lastItem.Margin.Top, lastItem.Margin.Right, lastItem.Margin.Bottom);
        }
    }
}
