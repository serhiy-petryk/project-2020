using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Windows.Controls;

namespace DGView.Views
{
    /// <summary>
    /// Interaction logic for DbFilterView.xaml
    /// </summary>
    public partial class DbFilterView : UserControl
    {

        public string StringPresentation => "ToDo: StringPresentation"; //     public Filters.FilterList FilterList => ucFilter.FilterList;

        public DbFilterView()
        {
            InitializeComponent();
        }

        public void Bind(DGCore.Filters.FilterList newFilterList, string settingKey, Action actionApply, ICollection dataSource)
        {

        }
    }
}
