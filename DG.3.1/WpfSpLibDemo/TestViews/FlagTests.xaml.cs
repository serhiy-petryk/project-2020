using System.Windows;

namespace WpfSpLibDemo.TestViews
{
    /// <summary>
    /// Interaction logic for FlagTests.xaml
    /// </summary>
    public partial class FlagTests
    {

        public FlagTests()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void OnChangeSizeClick(object sender, RoutedEventArgs e)
        {
            // AA.Width = AA.ActualWidth * 1.2;
        }
    }
}
