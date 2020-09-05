// ToDo:
// 1. Clone values
// 2. Copy & paste for grid
// 3. MwiChild + Disposable => close
// 4. Button => monochrome or flat style

using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DGCore.Filters;
using DGView.Mwi;

namespace DGView.Views
{
    /// <summary>
    /// Interaction logic for FilterLine.xaml
    /// </summary>
    public partial class FilterLineView : UserControl
    {
        public FilterLineBase FilterLine { get; }
        public FilterLineSubitemCollection Clone_FilterLines { get; }
        public bool Clone_Not { get; set; }

        private MwiChild ParentWindow => Common.Tips.GetVisualParents(this).OfType<MwiChild>().FirstOrDefault();

        public FilterLineView()
        {
            InitializeComponent();
            DataContext = this;
        }
        public FilterLineView(FilterLineBase filterLine) : this()
        {
            FilterLine = filterLine;
            Clone_FilterLines = (FilterLineSubitemCollection)filterLine.Items.Clone();
            Clone_Not = FilterLine.Not;
        }

        #region ==========  Event handlers  ==========
        private void DataGrid_OnUnloaded(object sender, RoutedEventArgs e)
        {
            // To prevent error: ''DeferRefresh' is not allowed during an AddNew or EditItem transaction.'
            ((DataGrid)sender).CommitEdit(DataGridEditingUnit.Row, true);
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            FilterLine.Items.Clear();
            foreach (var item in Clone_FilterLines.Where(a => a.IsValid))
                FilterLine.Items.Add(new FilterLineSubitem { Owner = item.Owner, FilterOperand = item.FilterOperand, Value1 = item.Value1, Value2 = item.Value2 });
            FilterLine.Not = Clone_Not;
            CloseButton_OnClick(sender, e);
        }

        private void ClearButton_OnClick(object sender, RoutedEventArgs e)
        {
            Clone_FilterLines.Clear();
        }

        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            ParentWindow?.CmdClose.Execute(null);
        }
        #endregion
    }
}
