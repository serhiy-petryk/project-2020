using System.Collections;
using System.Linq;
using System.Windows.Controls;
using Common;

namespace DGView.Views
{
    /// <summary>
    /// Interaction logic for FilterLine.xaml
    /// </summary>
    public partial class FilterLineView : UserControl
    {
        public DGCore.Filters.FilterLineBase FilterLine { get; }

        public IEnumerable xPossibleOperands =>
            Enums.FilterOperandTypeConverter.GetPossibleOperands(FilterLine.PropertyType, FilterLine.PropertyCanBeNull)
                .Select(item => new
                {
                    ID = item,
                    Description = new Enums.FilterOperandTypeConverter().ConvertTo(null, null, item, typeof(string))
                });
        public IEnumerable PossibleOperands =>
            Enums.FilterOperandTypeConverter.GetPossibleOperands(FilterLine.PropertyType, FilterLine.PropertyCanBeNull);

        public FilterLineView()
        {
            InitializeComponent();
        }
        public FilterLineView(DGCore.Filters.FilterLineBase filterLine): this()
        {
            FilterLine = filterLine;
            DataContext = this;
        }
    }
}
