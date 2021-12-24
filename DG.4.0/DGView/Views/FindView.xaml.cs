using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using WpfSpLib.Controls;
using WpfSpLib.Helpers;

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
            var parents = WpfSpLib.Common.Tips.GetVisualParents(findView).ToArray();//.OfType<MwiChild>().FirstOrDefault();
            parents.OfType<MwiChild>().FirstOrDefault()?.Focus();

            var viewParent = parents.OfType<ResizableControl>().FirstOrDefault();
            await Task.WhenAll(AnimationHelper.GetContentAnimations(viewParent, false));

            ((Panel)viewParent.Parent).Children.Remove(viewParent);
        });

        public FindView()
        {
            InitializeComponent();
            CommandBindings.Add(_closeCommand);
        }

    }
}
