using System;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DGView.Views
{
    /// <summary>
    /// Interaction logic for FilterGrid.xaml
    /// </summary>
    public partial class FilterGrid : UserControl, INotifyPropertyChanged
    {
        public DGCore.Filters.FilterList FilterList { get; private set; }
        // public string FilterStringPresentation => FilterList?.GetStringPresentation();

        private ICollection _dataSource;

        public FilterGrid()
        {
            InitializeComponent();
            DataContext = this;
        }

        public void Bind(DGCore.Filters.FilterList filterList, ICollection dataSource)
        {
            FilterList = filterList;
            _dataSource = dataSource;
            RefreshUI();
        }

        #region =========  Event handlers  ===========
        private void OnFilterEditPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var cell = (DataGridCell)sender;
            var filterLine = cell.DataContext as DGCore.Filters.FilterLineBase;
            var view = new FilterLineView(filterLine);
            var height = Math.Max(200, Window.GetWindow(this).ActualHeight * 2 / 3);
            Common.Tips.ShowMwiChildDialog(view, "Dialog", new Size(double.NaN, height));
        }
        #endregion

        #region ============  INotifyPropertyChanged  ============
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertiesChanged(params string[] propertyNames)
        {
            foreach (var propertyName in propertyNames)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public void RefreshUI()
        {
            OnPropertiesChanged(nameof(FilterList));
        }
        #endregion
    }
}
