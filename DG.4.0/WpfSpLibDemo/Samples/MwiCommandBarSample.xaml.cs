using System.Linq;
using System.Windows;
using System.Windows.Media;
using WpfSpLib.Controls;
using WpfSpLib.Helpers;

namespace WpfSpLibDemo.Samples
{
    /// <summary>
    /// Interaction logic for CommandBarExample.xaml
    /// </summary>
    public partial class MwiCommandBarSample
    {
        public MwiCommandBarSample()
        {
            InitializeComponent();
        }

        private void OnTestButtonClick(object sender, RoutedEventArgs e)
        {
            var a1 = ((DependencyObject) sender).GetVisualParents().OfType<MwiStartupDemo>().FirstOrDefault();
            var a2 = a1.Content as MwiChild;
            a2.Background = Brushes.Green;

        }
    }
}
