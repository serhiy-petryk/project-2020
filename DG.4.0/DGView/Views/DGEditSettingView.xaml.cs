using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DGCore.UserSettings;
using WpfSpLib.Helpers;

namespace DGView.Views
{
    /// <summary>
    /// Interaction logic for DBEditSetting.xaml
    /// </summary>
    public partial class DGEditSettingView : UserControl
    {
        public ObservableCollection<Column> PropertiesData { get; }

        public DGEditSettingView(DGV settings)
        {
            InitializeComponent();
            DataContext = this;
            PropertiesData = new ObservableCollection<Column>(settings.AllColumns);
        }

        #region =======  Drag/Drop events handlers ========
        private void PropertyList_OnPreviewMouseMove(object sender, MouseEventArgs e) => DragDropHelper.DragSource_OnPreviewMouseMove(sender, e, sender.GetType().Name);
        private void PropertyList_OnPreviewGiveFeedback(object sender, GiveFeedbackEventArgs e) => DragDropHelper.DragSource_OnPreviewGiveFeedback(sender, e);
        private void PropertyList_OnPreviewDragOver(object sender, DragEventArgs e) => DragDropHelper.DropTarget_OnPreviewDragOver(sender, e);
        private void PropertyList_OnPreviewDragEnter(object sender, DragEventArgs e) => DragDropHelper.DropTarget_OnPreviewDragOver(sender, e);
        private void PropertyList_OnPreviewDragLeave(object sender, DragEventArgs e) => DragDropHelper.DropTarget_OnPreviewDragLeave(sender, e);
        private void PropertyList_OnPreviewDrop(object sender, DragEventArgs e) => DragDropHelper.DropTarget_OnPreviewDrop(sender, e, sender.GetType().Name);
        #endregion
    }
}
