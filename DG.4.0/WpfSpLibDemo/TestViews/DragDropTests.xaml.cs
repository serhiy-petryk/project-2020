using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using WpfSpLib.Helpers;
using WpfSpLibDemo.Samples;

namespace WpfSpLibDemo.TestViews
{
    /// <summary>
    /// Interaction logic for DragDropTests.xaml
    /// </summary>
    public partial class DragDropTests : Window
    {
        public DragDropTests()
        {
            InitializeComponent();
            DataContext = this;

            view1.ItemsSource = MyTask.CreateTasks();
            view2.ItemsSource = new ObservableCollection<MyTask>();
        }
        private void View1_OnPreviewMouseMove(object sender, MouseEventArgs e) => DragDropHelper.DragSource_OnPreviewMouseMove(sender, e);
        private void View1_OnPreviewGiveFeedback(object sender, GiveFeedbackEventArgs e) => DragDropHelper.DragSource_OnPreviewGiveFeedback(sender, e);
        private void View1_OnPreviewDragOver(object sender, DragEventArgs e) => DragDropHelper.DropTarget_OnPreviewDragOver(sender, e);
        private void View1_OnPreviewDragEnter(object sender, DragEventArgs e) => DragDropHelper.DropTarget_OnPreviewDragOver(sender, e);
        private void View1_OnPreviewDragLeave(object sender, DragEventArgs e) => DragDropHelper.DropTarget_OnPreviewDragLeave(sender, e);
        private void View1_OnDrop(object sender, DragEventArgs e) => DragDropHelper.DropTarget_OnDrop(sender, e);
        private void View2_OnDrop(object sender, DragEventArgs e) => DragDropHelper.DropTarget_OnDrop(sender, e);
    }
}
