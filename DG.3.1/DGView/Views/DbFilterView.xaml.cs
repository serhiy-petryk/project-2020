using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace DGView.Views
{
    /// <summary>
    /// Interaction logic for DbFilterView.xaml
    /// </summary>
    public partial class DbFilterView : UserControl
    {

        public DbFilterView()
        {
            InitializeComponent();
        }

        public void Bind(DGCore.Filters.FilterList newFilterList, string settingKey, Action actionApply, ICollection dataSource)
        {

        }

        private void LoadData_OnClick(object sender, RoutedEventArgs e)
        {
        }

        private void OpenSettingButton_OnChecked(object sender, RoutedEventArgs e)
        {
        }

        private void ClearFilter_OnClick(object sender, RoutedEventArgs e)
        {
        }
    }
}
