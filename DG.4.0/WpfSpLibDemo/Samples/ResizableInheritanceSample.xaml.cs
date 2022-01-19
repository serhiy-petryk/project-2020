using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using WpfSpLib.Controls;
using WpfSpLib.Helpers;

namespace WpfSpLibDemo.Samples
{
    /// <summary>
    /// Interaction logic for ResizableInheritanceSample.xaml
    /// </summary>
    public partial class ResizableInheritanceSample : ResizableControl
    {
        static ResizableInheritanceSample()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ResizableInheritanceSample), new FrameworkPropertyMetadata(typeof(ResizableInheritanceSample)));
            FocusableProperty.OverrideMetadata(typeof(ResizableInheritanceSample), new FrameworkPropertyMetadata(true));
        }

        public ResizableInheritanceSample()
        {
            InitializeComponent();
            DataContext = this;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            MovingThumb = this.GetVisualChildren().OfType<Thumb>().FirstOrDefault(t => t.Name == MovingThumbName);
        }
    }
}
