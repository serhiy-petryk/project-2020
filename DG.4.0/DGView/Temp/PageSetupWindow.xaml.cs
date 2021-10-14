using System;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using WpfSpLib.Helpers;

namespace DGView.Temp
{
    /// <summary>
    /// Interaction logic for PageSetupWindow.xaml
    /// </summary>
    public partial class PageSetupWindow : Window
    {
        private PageViewModel _viewModel;
        public PageSetupWindow(PageViewModel pageViewModel)
        {
            InitializeComponent();
            _viewModel = (PageViewModel)pageViewModel.Clone();
            DataContext = _viewModel;

            Dispatcher.BeginInvoke(new Action(() =>
            {
                PageSizeSelector.Width = _viewModel.AvailableSizes.Max(size => ControlHelper.MeasureString(size.Name, PageSizeSelector).Width) + 28.0;
            }), DispatcherPriority.Background);

        }
    }
}
