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
        public PageViewModel ViewModel { get; }
        public PageSetupWindow(PageViewModel pageViewModel)
        {
            InitializeComponent();
            ViewModel = (PageViewModel)pageViewModel.Clone();
            DataContext = ViewModel;
            PageSizeSelector.Width = ViewModel.AvailableSizes.Max(size => ControlHelper.MeasureString(size.Name, PageSizeSelector).Width) + 28.0;
        }
    }
}
