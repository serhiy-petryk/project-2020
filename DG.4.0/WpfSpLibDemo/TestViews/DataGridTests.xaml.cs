using System.Windows;
using System.Windows.Controls;
using WpfSpLib.Helpers;
using WpfSpLibDemo.Samples;

namespace WpfSpLibDemo.TestViews
{
    /// <summary>
    /// Interaction logic for DataGridTests.xaml
    /// </summary>
    public partial class DataGridTests : Window
    {
        public DataGridTests()
        {
            InitializeComponent();
            DataContext = this;
            TestDataGrid1.ItemsSource = Author.Authors;
            TestDataGrid2.ItemsSource = Author.Authors;
            TestDataGrid3.ItemsSource = Author.Authors;
        }

        private void DataGrid_OnThreeStateSorting(object sender, DataGridSortingEventArgs e) =>
            DataGridHelper.DataGrid_OnSorting((DataGrid)sender, e);
    }
}
