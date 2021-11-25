using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using WpfSpLib.Helpers;

namespace DGView.Temp
{
    /// <summary>
    /// Interaction logic for PrintPreviewWindow.xaml
    /// </summary>
    public partial class PrintPreviewWindow : Window
    {
        private readonly PrintPreviewViewModel _viewModel;

        public PrintPreviewWindow(IPrintContentGenerator printContentGenerator)
        {
            InitializeComponent();
            _viewModel = new PrintPreviewViewModel(DocumentViewer, printContentGenerator);
            DataContext = _viewModel;

            if (DesignerProperties.GetIsInDesignMode(this)) return;

            Closed += (sender, args) => _viewModel.StopContentGeneration();

            Dispatcher.BeginInvoke(new Action(()=>
            {
                if (DocumentViewer.Template.FindName("PrintSelector", DocumentViewer) is Control printSelector2)
                    printSelector2.Width = PrintPreviewViewModel.Printers.Max(p =>
                                               ControlHelper.MeasureString(p.PrintQueue.FullName, printSelector2,
                                                   TextFormattingMode.Display).Width) + 28.0;
            }), DispatcherPriority.Loaded);

            Dispatcher.BeginInvoke(new Action(()=>
            {
                if (DocumentViewer.Template.FindName("NotificationOfGenerating", DocumentViewer) is FrameworkElement notificationOfGenerating)
                    _viewModel._notificationOfGeneration = notificationOfGenerating;
                if (DocumentViewer.Template.FindName("NotificationOfPrinting", DocumentViewer) is FrameworkElement notificationOfPrinting)
                    _viewModel._notificationOfPrinting = notificationOfPrinting;
                _viewModel.GenerateContent();
            }), DispatcherPriority.ApplicationIdle);
        }

        private void DocumentPreviewer_ManipulationBoundaryFeedback(object sender, ManipulationBoundaryFeedbackEventArgs e)
        {
            // see https://stackoverflow.com/questions/4505772/wpf-listbox-with-touch-inertia-pulls-down-entire-window
            e.Handled = true;
        }

        private void OnPanelSizeChanged(object sender, SizeChangedEventArgs e)
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

        private void OnStopGenerationClick(object sender, RoutedEventArgs e) => _viewModel.StopContentGeneration();

        private void OnStopPrintingClick(object sender, RoutedEventArgs e) => _viewModel.CancelPrinting();

        private void OnMemoryUsedClick(object sender, RoutedEventArgs e) => MessageBox.Show($"Memory: {DGCore.Utils.Tips.MemoryUsedInBytes:N0} байт");
    }
}
