using System.Collections;
using System.Windows.Controls;

namespace DGView.Views
{
    /// <summary>
    /// Interaction logic for FilterLine.xaml
    /// </summary>
    public partial class FilterLineView : UserControl
    {
        public DGCore.Filters.FilterLineBase FilterLine { get; }

        public IEnumerable PossibleOperands => DGCore.Common.Enums.FilterOperandTypeConverter.GetPossibleOperands(FilterLine.PropertyType, FilterLine.PropertyCanBeNull);

        public FilterLineView()
        {
            InitializeComponent();
            DataContext = this;
        }
        public FilterLineView(DGCore.Filters.FilterLineBase filterLine): this()
        {
            FilterLine = filterLine;
        }
    }
}
