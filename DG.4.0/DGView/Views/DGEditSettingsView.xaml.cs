using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DGCore.UserSettings;
using WpfSpLib.Helpers;

namespace DGView.Views
{
    /// <summary>
    /// Interaction logic for DGEditSettingsView.xaml
    /// </summary>
    public partial class DGEditSettingsView : UserControl
    {
        public ObservableCollection<Column> PropertiesData { get; }

        public DGEditSettingsView(DGV settings)
        {
            InitializeComponent();
            DataContext = this;
            PropertiesData = new ObservableCollection<Column>(settings.AllColumns);
        }

        #region =======  Drag/Drop event handlers ========
        private void PropertyList_OnPreviewMouseMove(object sender, MouseEventArgs e) => DragDropHelper.DragSource_OnPreviewMouseMove(sender, e, sender.GetType().Name);
        private void PropertyList_OnPreviewGiveFeedback(object sender, GiveFeedbackEventArgs e) => DragDropHelper.DragSource_OnPreviewGiveFeedback(sender, e);
        private void PropertyList_OnPreviewDragOver(object sender, DragEventArgs e) => DragDropHelper.DropTarget_OnPreviewDragOver(sender, e);
        private void PropertyList_OnPreviewDragEnter(object sender, DragEventArgs e) => DragDropHelper.DropTarget_OnPreviewDragOver(sender, e);
        private void PropertyList_OnPreviewDragLeave(object sender, DragEventArgs e) => DragDropHelper.DropTarget_OnPreviewDragLeave(sender, e);
        private void PropertyList_OnPreviewDrop(object sender, DragEventArgs e)
        {
            if (!DragDropHelper.Drag_Info.InsertIndex.HasValue || e.Effects != DragDropEffects.Copy) return;

            var sourceData = ((e.Data.GetData(sender.GetType().Name) as object[]) ?? new object[0]).OfType<Column>().ToArray();
            var insertIndex = DragDropHelper.Drag_Info.InsertIndex.Value + DragDropHelper.Drag_Info.FirstItemOffset;
            foreach (var item in sourceData)
            {
                var oldIndex = PropertiesData.IndexOf(item);
                if (oldIndex < insertIndex) insertIndex--;
                if (oldIndex != insertIndex)
                    PropertiesData.Move(oldIndex, insertIndex);
                insertIndex++;
            }
        }

        #endregion
    }
}
