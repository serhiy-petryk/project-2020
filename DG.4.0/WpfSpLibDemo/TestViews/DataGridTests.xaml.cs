using System;
using System.Collections.Generic;
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
        public IList<Author> Data { get; } = Author.Authors;
        public Author.Level[] EnumList { get; } = Enum.GetValues<Author.Level>();

        public DataGridTests()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void DataGrid_OnThreeStateSorting(object sender, DataGridSortingEventArgs e) =>
            DataGridHelper.DataGrid_OnSorting((DataGrid)sender, e);
    }
}
