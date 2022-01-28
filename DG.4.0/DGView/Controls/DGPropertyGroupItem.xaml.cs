using System.Windows.Controls;
using DGView.ViewModels;

namespace DGView.Controls
{
    /// <summary>
    /// Interaction logic for DGPropertyGroupItem.xaml
    /// </summary>
    public partial class DGPropertyGroupItem : UserControl
    {
        public DGPropertyGroupItemModel ViewModel => new DGPropertyGroupItemModel();

        public DGPropertyGroupItem()
        {
            InitializeComponent();
            DataContext = ViewModel;
        }
    }
}
