using System.Windows.Controls;

namespace DGView.Views
{
    /// <summary>
    /// Interaction logic for FilterLine.xaml
    /// </summary>
    public partial class FilterLineView : UserControl
    {
        private DGCore.Filters.FilterLineBase FilterLine { get; }

        public FilterLineView()
        {
            InitializeComponent();
        }
        public FilterLineView(DGCore.Filters.FilterLineBase filterLine): this()
        {
            FilterLine = filterLine;
            DataContext = FilterLine;
        }
    }
}
