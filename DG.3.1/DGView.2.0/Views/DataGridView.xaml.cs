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
    }
}
