using System.Windows;
using System.Windows.Controls;

namespace DGView.Views
{
    /// <summary>
    /// Interaction logic for FilterLine.xaml
    /// </summary>
    public partial class FilterLineView : UserControl
    {
        public DGCore.Filters.FilterLineBase FilterLine { get; }

        public FilterLineView()
        {
            InitializeComponent();
            DataContext = this;
        }
        public FilterLineView(DGCore.Filters.FilterLineBase filterLine): this()
        {
            FilterLine = filterLine;
        }

        private void DataGrid_OnUnloaded(object sender, RoutedEventArgs e)
        {
            // To prevent error: ''DeferRefresh' is not allowed during an AddNew or EditItem transaction.'
            ((DataGrid)sender).CommitEdit(DataGridEditingUnit.Row, true);
        }
    }
}
