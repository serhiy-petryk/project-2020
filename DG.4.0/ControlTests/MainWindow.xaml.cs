using System.Windows;
using System.Windows.Media;
using ControlTests.Styles.WpfOffice;

namespace ControlTests
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var accentBrush = TryFindResource("AccentColorBrush") as SolidColorBrush;
            if (accentBrush != null) accentBrush.Color.CreateAccentColors();
        }

    }
}
