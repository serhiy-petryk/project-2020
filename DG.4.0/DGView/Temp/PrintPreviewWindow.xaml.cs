using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
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
        private readonly IPrintContentGenerator _printContentGenerator;

        public PrintPreviewWindow(IPrintContentGenerator printContentGenerator)
        {
            InitializeComponent();
            _printContentGenerator = printContentGenerator;
            _viewModel = new PrintPreviewViewModel(this);
            DataContext = _viewModel;

            if (DesignerProperties.GetIsInDesignMode(this)) return;

            if (DocumentViewer.Template.FindName("PrintSelector", DocumentViewer) is Control printSelector)
                printSelector.Width = PrintPreviewViewModel.Printers.Max(p => ControlHelper.MeasureString(p.PrintQueue.FullName, printSelector).Width) + 28.0;

            Closed += (sender, args) =>
            {
                if (_printContentGenerator != null)
                    _printContentGenerator.StopGeneration = true;
            };

            Dispatcher.BeginInvoke(new Action(GenerateContent), DispatcherPriority.ApplicationIdle);
        }

        private void GenerateContent()
        {
            if (_printContentGenerator != null)
            {
                var fixedDoc = new FixedDocument();
                DocumentViewer.Document = fixedDoc;
                _viewModel.GenerateContent( fixedDoc, _printContentGenerator);
            }
        }

        #region ========  Test methods  ===========

        private void OnTestClick(object sender, RoutedEventArgs e)
        {
            GenerateContent();
        }

        #endregion

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

        private void OnStopGenerationClick(object sender, RoutedEventArgs e)
        {
            // Debug.Print($"StopGeneration");
            _printContentGenerator.StopGeneration = true;
        }
    }
}
