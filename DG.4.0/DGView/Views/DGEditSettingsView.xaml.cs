using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using DGCore.PD;
using DGCore.UserSettings;
using DGView.ViewModels;
using WpfSpLib.Helpers;

namespace DGView.Views
{
    /// <summary>
    /// Interaction logic for DGEditSettingsView.xaml
    /// </summary>
    public partial class DGEditSettingsView : UserControl
    {
        public ObservableCollection<DGEditSettings_PropertyModel> PropertiesData { get; }

        #region =======  Quick Filter  =========
        private string _quickFilterText;
        public string QuickFilterText
        {
            get => _quickFilterText;
            set
            {
                if (!Equals(_quickFilterText, value))
                {
                    _quickFilterText = value;
                    SetFilter();
                }
            }
        }
        private void SetFilter()
        {
            var view = CollectionViewSource.GetDefaultView(PropertiesData);
            view.Filter += Filter;
            // DataGrid.SelectedItem = DataGrid.Items.OfType<object>().FirstOrDefault();
        }
        private bool Filter(object obj) => Helpers.Misc.SetFilter(((DGEditSettings_PropertyModel) obj).Name, QuickFilterText);
        #endregion

        public DGEditSettingsView(DGV settings, PropertyDescriptorCollection properties)
        {
            InitializeComponent();
            DataContext = this;
            PropertiesData = new ObservableCollection<DGEditSettings_PropertyModel>(settings.AllColumns.Select(o => new DGEditSettings_PropertyModel(o, settings, (IMemberDescriptor)properties[o.Id])));
            cbShowTotalRow.IsChecked = settings.ShowTotalRow;
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

            var sourceData = ((e.Data.GetData(sender.GetType().Name) as object[]) ?? new object[0]).OfType<DGEditSettings_PropertyModel>().ToArray();
            var insertIndex = DragDropHelper.Drag_Info.InsertIndex.Value + DragDropHelper.Drag_Info.FirstItemOffset;
            foreach (var item in sourceData)
            {
                var oldIndex = PropertyList.Items.IndexOf(item);
                if (oldIndex < insertIndex) insertIndex--;
                if (oldIndex != insertIndex)
                {
                    var offsetItem = (DGEditSettings_PropertyModel)PropertyList.Items[insertIndex];
                    var originalNewIndex = PropertiesData.IndexOf(offsetItem);
                    var originalOldIndex = PropertiesData.IndexOf(item);
                    PropertiesData.Move(originalOldIndex, originalNewIndex);
                }

                insertIndex++;
            }

            // Update row numeration
            var itemsHost = DragDropHelper.GetItemsHost(PropertyList);
            foreach (DataGridRow item in itemsHost.Children)
                PropertyList_OnLoadingRow(PropertyList, new DataGridRowEventArgs(item));
        }

        #endregion

        private void DataGridCell_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            return;
            var cell = sender as DataGridCell;
            var dataGrid = cell.GetVisualParents().OfType<DataGrid>().FirstOrDefault();
            if (cell.IsReadOnly || dataGrid == null) return;

            // see  MahApps.Metro.Controls.DataGridHelper.DataGridOnPreviewMouseLeftButtonDown
            var toggleButtons = cell.GetVisualChildren().OfType<ToggleButton>().ToArray();
            if (toggleButtons.Length == 1)
            {
                dataGrid.BeginEdit();
                toggleButtons[0].SetCurrentValue(ToggleButton.IsCheckedProperty, !toggleButtons[0].IsChecked);
                dataGrid.CommitEdit();
                e.Handled = true;
            }
            var textBlocks = cell.GetVisualChildren().OfType<TextBlock>().ToArray();
            if (textBlocks.Length == 1)
            {
                cell.Focus();
                dataGrid.BeginEdit();
                e.Handled = true;
            }
        }

        private void PropertyList_OnLoadingRow(object sender, DataGridRowEventArgs e)
        {
            var rowHeaderText = (e.Row.GetIndex() + 1).ToString("N0", LocalizationHelper.CurrentCulture);
            if (!Equals(e.Row.Header, rowHeaderText)) e.Row.Header = rowHeaderText;
        }
    }
}
