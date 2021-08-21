using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DGView.ViewModels
{
    public partial class DataGridViewModel
    {
        private void SetColumnVisibility()
        {
            foreach (var col in DGControl.Columns.OfType<DataGridBoundColumn>().Where(c => !string.IsNullOrEmpty(c.SortMemberPath)))
            {
                var visible = Data.IsPropertyVisible(col.SortMemberPath);
                if (col.Visibility == Visibility.Visible && !visible)
                    col.Visibility = Visibility.Hidden;
                else if (col.Visibility != Visibility.Visible && visible)
                    col.Visibility = Visibility.Visible;
            }

            // Set group columns visibility
            foreach (var c in _groupColumns.Where((c, index) => Data.IsGroupColumnVisible(index) != (c.Visibility == Visibility.Visible)))
                c.Visibility = c.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;

        }
    }
}
