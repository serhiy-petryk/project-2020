using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace DGView.Temp
{
    /// <summary>
    /// Interaction logic for PageSetupWindow.xaml
    /// </summary>
    public partial class PageSetupWindow : Window
    {
        public PageViewModel ViewModel { get; }
        public PageSetupWindow(PageViewModel pageViewModel)
        {
            InitializeComponent();
            ViewModel = pageViewModel.GetPageSetupModel(PageArea, PrintingArea);
            DataContext = ViewModel;
            Dispatcher.BeginInvoke(new Action(() => ViewModel.UpdateUI(MarginContainer.ActualHeight)));
        }

        private void OnPageSizeSelectorMouseEnter(object sender, MouseEventArgs e)
        {
            var btn = (ToggleButton) sender;
            var txtBlock = WpfSpLib.Common.Tips.GetVisualChildren(btn).OfType<TextBlock>().FirstOrDefault();
            if (txtBlock != null)
            {
                if (!string.IsNullOrEmpty(txtBlock.Text) && WpfSpLib.Common.Tips.IsTextTrimmed(txtBlock))
                    ToolTipService.SetToolTip(btn, txtBlock.Text);
                else
                    ToolTipService.SetToolTip(btn, null);
            }
        }

        private void OnPrintingAreaSizeChanged(object sender, SizeChangedEventArgs e)
        {
            ViewModel.UpdateUiBySlider(MarginContainer.ActualHeight, PrintingArea);
        }
    }
}
