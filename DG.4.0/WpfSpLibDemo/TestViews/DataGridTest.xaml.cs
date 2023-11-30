using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfSpLibDemo.Samples;

namespace WpfSpLibDemo.TestViews
{
    /// <summary>
    /// Interaction logic for DataGridTest.xaml
    /// </summary>
    public partial class DataGridTest : Window
    {

        public BindingList<FakeData> Data { get; } = new BindingList<FakeData>();
        public DataGridTest()
        {
            InitializeComponent();
            Grid.AutoGenerateColumns = true;
            Grid.ItemsSource = Data;
        }

        private void BtnGenerate_OnClick(object sender, RoutedEventArgs e)
        {
            Data.Clear();
            var cnt = Convert.ToInt32(ItemCount.Text);

            Data.RaiseListChangedEvents = false;
            for (var k=0; k<cnt;k++)
                Data.Add(new FakeData());
            Data.RaiseListChangedEvents = true;
            Data.ResetBindings();
        }
    }
}
