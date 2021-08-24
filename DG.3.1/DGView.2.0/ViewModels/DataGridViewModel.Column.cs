using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using DGCore.UserSettings;

namespace DGView.ViewModels
{
    public partial class DataGridViewModel
    {
        internal DataGridColumn GroupItemCountColumn = null;
        private List<DataGridTextColumn> _groupColumns = new List<DataGridTextColumn>();

        private void SetColumnVisibility(DGV settingInfo)
        {
            foreach (var col in DGControl.Columns.OfType<DataGridBoundColumn>().Where(c => !string.IsNullOrEmpty(c.SortMemberPath)))
            {
                var settingColumn = settingInfo.AllColumns.FirstOrDefault(c => c.Id == col.SortMemberPath);
                if (settingColumn == null)
                    Helpers.DataGridHelper.SetColumnVisibility(col, false);
                else
                    Helpers.DataGridHelper.SetColumnVisibility(col, !settingColumn.IsHidden && Data.IsPropertyVisible(col.SortMemberPath));
            }

            // Set group columns visibility
            for (var k=0; k<_groupColumns.Count; k++)
                Helpers.DataGridHelper.SetColumnVisibility(_groupColumns[k], Data.IsGroupColumnVisible(k));

            // Set GroupItemCount column visibility
            if (GroupItemCountColumn != null)
                Helpers.DataGridHelper.SetColumnVisibility(GroupItemCountColumn, Data.Groups.Count > 0);
        }

        private void SetColumnOrder()
        {

        }

    }
}
