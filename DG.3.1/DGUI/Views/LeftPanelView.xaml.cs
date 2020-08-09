using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DGCore.Menu;
using DGUI.ViewModels;

namespace DGUI.Views
{
    /// <summary>
    /// Interaction logic for LeftPanelView.xaml
    /// </summary>
    public partial class LeftPanelView
    {
        public LeftPanelView()
        {
            InitializeComponent();
        }

        private void LeftPanelView_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                var rootMenu = new RootMenu(DGCore.Misc.AppSettings.CONFIG_FILE_NAME);
                Menu.ItemsSource = rootMenu.Items;
            }
        }

        private void TreeViewItem_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var item = (TreeViewItem) sender;
            var mo = item.DataContext as MenuOption;
            if (mo != null)
            {
                try
                {
                    // Check on database error
                    var dd = ((MenuOption) item.DataContext).GetDataDefiniton();

                    var dgView = new DataGridView(mo);
                    AppViewModel.Instance.MwiContainer.HideLeftPanel();
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    e.Handled = true;
                }
            }
        }
    }

}
