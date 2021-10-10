using System;
using System.ComponentModel;
using System.Linq;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using WpfSpLib.Controls;
using WpfSpLib.Helpers;

namespace DGView.Temp
{
    /// <summary>
    /// Interaction logic for PrintForm2.xaml
    /// </summary>
    public partial class PrintForm2 : INotifyPropertyChanged
    {
        private readonly LocalPrintServer _printServer = new LocalPrintServer();
        public PrintQueue[] Printers { get; } = new LocalPrintServer().GetPrintQueues().ToArray();
        public PrintQueue CurrentPrinter => equipmentComboBox.SelectedItem as PrintQueue;
        public PrintCapabilities CurrentPrintCapabilities { get; private set; }

        private int _savedPages = -3;
        public int SavedPages => _savedPages;

        private Size _pageSize = new Size(793, 1122);
        private int _itemCount;
        private ComboBox equipmentComboBox;

        public PrintForm2()
        {
            InitializeComponent();
            DataContext = this;

            var fixedDoc = new FixedDocument();
            fixedDoc.DocumentPaginator.PageSize = _pageSize;
            ((IAddChild)DocumentViewer).AddChild(fixedDoc);

            /*equipmentComboBox.ItemsSource = Printers;
            equipmentComboBox.SelectedItem = Printers.FirstOrDefault(p => p.FullName == _printServer.DefaultPrintQueue.FullName);
            if (equipmentComboBox.SelectedItem == null && Printers.Length > 0)
                equipmentComboBox.SelectedItem = Printers[0];*/
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // equipmentComboBox = DocumentViewer.GetTemplateChild("equipmentComboBox") as ComboBox;
            equipmentComboBox = DocumentViewer.Template.FindName("equipmentComboBox", DocumentViewer) as ComboBox;
            if (equipmentComboBox != null)
            {
                equipmentComboBox.ItemsSource = Printers;
                equipmentComboBox.SelectedItem = Printers.FirstOrDefault(p => p.FullName == _printServer.DefaultPrintQueue.FullName);
                if (equipmentComboBox.SelectedItem == null && Printers.Length > 0)
                    equipmentComboBox.SelectedItem = Printers[0];
            }

            Dispatcher.BeginInvoke(new Action(() => OnTopPanelSizeChanged(null, null)),
                DispatcherPriority.ApplicationIdle);
            ;
        }

        #region =========  Data ==========
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
        #endregion
        private void OnPrintDialogClick(object sender, RoutedEventArgs e)
        {
        }

        private void OnSaveToXpsClick(object sender, RoutedEventArgs e)
        {
        }

        private void ActualSizeBtn_Click(object sender, RoutedEventArgs e)
        {
        }

        private void DocumentPreviewer_ManipulationBoundaryFeedback(object sender, ManipulationBoundaryFeedbackEventArgs e)
        {
        }

        private void FirstPageBtn_Click(object sender, RoutedEventArgs e)
        {
        }

        private void LastPageBtn_Click(object sender, RoutedEventArgs e)
        {
        }

        private void NextPageBtn_Click(object sender, RoutedEventArgs e)
        {
        }

        private void PreviousPageBtn_Click(object sender, RoutedEventArgs e)
        {
        }

        private void EquipmentComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CurrentPrintCapabilities = CurrentPrinter.GetPrintCapabilities();
            OnPropertiesChanged(nameof(CurrentPrinter), nameof(CurrentPrintCapabilities));
        }

        private void EquipmentComboBox_DropDownOpened(object sender, EventArgs e)
        {
        }

        #region ===========  INotifyPropertyChanged  ==============
        public event PropertyChangedEventHandler PropertyChanged;

        internal void OnPropertiesChanged(params string[] propertyNames)
        {
            foreach (var propertyName in propertyNames)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion


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

        private void OnPageSetupClick(object sender, RoutedEventArgs e)
        {
            /*var a1 = new DialogAdorner { CloseOnClickBackground = true };
            var content1 = new ResizingControl
            {
                Content = new PageSetupControl(),
                LimitPositionToPanelBounds = true,
                Resizable = false
            };
            a1.ShowContentDialog(content1);*/

            var aa1 = WpfSpLib.Common.Tips.GetVisualParents(this).OfType<IColorThemeSupport>().FirstOrDefault();

            var psControl = new PageSetupControl();
            var size = new Size(psControl.Width, psControl.Height);
            psControl.Width = double.NaN;
            psControl.Height = double.NaN;
            Helpers.Misc.OpenDialog(psControl, "Page Setup", size, null, null, MwiChild.Buttons.Close);
        }
    }
}
