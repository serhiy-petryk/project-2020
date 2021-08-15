using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using DGView.ViewModels;
using WpfSpLib.Helpers;

namespace DGView.Views
{
    /// <summary>
    /// Interaction logic for DataGridView.xaml
    /// </summary>
    public partial class DataGridView : UserControl
    {
        public DataGridViewModel ViewModel => (DataGridViewModel)DataContext;

        public DataGridView()
        {
            InitializeComponent();
            Unloaded += OnUnloaded;
            DataContext = new DataGridViewModel(this);
        }

        public void OnUnloaded(object sender, RoutedEventArgs e)
        {
            if (this.IsElementDisposing())
            {
                DataGrid.ItemsSource = null;
                DataGrid.Columns.Clear();
                ViewModel.Dispose();
            }
        }

        private void DataGrid_OnLoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
            if (e.Row.IsSelected)
                Debug.Print($"Selected: {e.Row.Header}");
        }

        private void TempDataGrid_OnSelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            Debug.Print($"DataGrid_OnSelectedCellsChanged: {e}");
        }

        private void TempDataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Debug.Print($"DataGrid_OnSelectionChanged: {e}");
        }
    }
}
