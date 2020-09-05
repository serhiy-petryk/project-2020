using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using DGCore.Filters;

namespace DGView.Views
{
    /// <summary>
    /// Interaction logic for FilterLine.xaml
    /// </summary>
    public partial class FilterLineView : UserControl, INotifyPropertyChanged
    {
        public FilterLineBase FilterLine { get; }
        public FilterLineSubitemCollection EditableFilterLines { get; }

        public FilterLineView()
        {
            InitializeComponent();
            DataContext = this;
        }
        public FilterLineView(FilterLineBase filterLine): this()
        {
            FilterLine = filterLine;
            EditableFilterLines = (FilterLineSubitemCollection)filterLine.Items.Clone();
            RefreshUI();
        }

        private void DataGrid_OnUnloaded(object sender, RoutedEventArgs e)
        {
            // To prevent error: ''DeferRefresh' is not allowed during an AddNew or EditItem transaction.'
            ((DataGrid)sender).CommitEdit(DataGridEditingUnit.Row, true);
        }

        #region ============  INotifyPropertyChanged  ============
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertiesChanged(params string[] propertyNames)
        {
            foreach (var propertyName in propertyNames)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public void RefreshUI()
        {
            // OnPropertiesChanged(nameof(FilterLine));
        }
        #endregion


    }
}
