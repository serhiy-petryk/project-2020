using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfSpLib.Controls;

namespace DGView.Views
{
    /// <summary>
    /// Interaction logic for FindAndReplace.xaml
    /// </summary>
    public partial class FindView : UserControl
    {
        private static readonly CommandBinding _closeCommand = new CommandBinding(ApplicationCommands.Close, async (s, e1) =>
        {
            var findView = (FindView)s;
            var a1 = WpfSpLib.Common.Tips.GetVisualParents(findView).OfType<MwiChild>().FirstOrDefault();
            a1?.Focus();

            FrameworkElement parent = findView;
            while (parent != null)
            {
                if (parent.Parent is Panel panel)
                {
                    panel.Children.Remove(parent);
                    break;
                }
                parent = parent.Parent as FrameworkElement;
            }
        });

        public FindView()
        {
            InitializeComponent();
            CommandBindings.Add(_closeCommand);
        }

    }
}
