using System.Collections;
using System.Windows.Controls;

namespace DGView.Views
{
    /// <summary>
    /// Interaction logic for FilterGrid.xaml
    /// </summary>
    public partial class FilterGrid : UserControl
    {
        public DGCore.Filters.FilterList FilterList { get; private set; }
        private ICollection _dataSource;

        public FilterGrid()
        {
            InitializeComponent();
        }

        public void Bind(DGCore.Filters.FilterList filterList, ICollection dataSource)
        {
            FilterList = filterList;
            _dataSource = dataSource;
        }
    }
}
