using System.Windows.Controls;
using DGView.ViewModels;

namespace DGView.Controls
{
    /// <summary>
    /// Interaction logic for DGPropertyGroupItem.xaml
    /// </summary>
    public partial class DGPropertyGroupItem : UserControl
    {
        public xxDGProperty_GroupItemModel ViewModel { get; set; } = new xxDGProperty_GroupItemModel();

        public DGPropertyGroupItem()
        {
            InitializeComponent();
            DataContext = ViewModel;
            // DGPropertyGroupItemElement.ViewModel = ViewModel.NextGroup;
        }
    }
}
