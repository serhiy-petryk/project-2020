using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DGWnd.Utils;

namespace DGWnd.UserControls {
  public partial class UC_ColumnSettings : UserControl {

    string _dragDropGroupID = "ColumnSettings_" + DGCore.Utils.Tips.GetUniqueNumber().ToString();
    List<DGCore.Misc.TotalLine> _totalLines;

    public UC_ColumnSettings() {
      InitializeComponent();
      Utils.DGVClipboard.Clipboard_Attach(this.dgvTotals);
    }
    private void PDC_ColumnSettings_Load(object sender, EventArgs e) {
      if (!DGCore.Utils.Tips.IsDesignMode) {
        this.clbAllColumns.DragDropGroup = _dragDropGroupID;
        this.clbGroups.DragDropGroup = _dragDropGroupID;
        this.clbSorts.DragDropGroup = _dragDropGroupID;
        this.lbFrozenColumns.DragDropGroup = _dragDropGroupID;

        DGVUtils.CreateComboColumnsForEnumerations(this.dgvTotals);
      }
    }

    public void Bind(DGCore.UserSettings.DGV settings, PropertyDescriptorCollection properties)
    {
      // Clear
      _totalLines = new List<DGCore.Misc.TotalLine>();// need to create new List (otherwise binding error);
      clbAllColumns.Items.Clear();
      clbGroups.Items.Clear();
      clbSorts.Items.Clear();
      lbFrozenColumns.Items.Clear();

      // Set column visibility
      foreach (var c in settings.AllColumns)
      {
        clbAllColumns.Items.Add(new Misc.CheckedListBoxItem(c.Id, properties[c.Id].DisplayName, c.IsHidden));
        if (c.IsHidden)
          clbAllColumns.SetItemChecked(clbAllColumns.Items.Count - 1, true);
      }

      // FrozenColumns
      settings.FrozenColumns.ForEach(c =>
      {
        var frozenCol = settings.AllColumns.FirstOrDefault(c1 => c1.Id == c);
        if (frozenCol != null)
          lbFrozenColumns.Items.Add(new Misc.CheckedListBoxItem(frozenCol.Id, properties[frozenCol.Id].DisplayName, frozenCol.IsHidden));
      });

      // Groups
      foreach (var group in settings.Groups)
      {
        var col = settings.AllColumns.FirstOrDefault(c => c.Id == group.Id);
        clbGroups.Items.Add(new Misc.CheckedListBoxItem(col.Id, properties[col.Id].DisplayName, col.IsHidden));
        if (group.SortDirection == ListSortDirection.Descending)
          clbGroups.SetItemChecked(clbGroups.Items.Count - 1, true);
      }

      //Sorts
      foreach (var sort in settings.Sorts)
      {
        var col = settings.AllColumns.FirstOrDefault(c => c.Id == sort.Id);
        clbSorts.Items.Add(new Misc.CheckedListBoxItem(col.Id, properties[col.Id].DisplayName, col.IsHidden));
        if (sort.SortDirection == ListSortDirection.Descending)
          clbSorts.SetItemChecked(clbSorts.Items.Count - 1, true);
      }

      // Totals
      _totalLines.AddRange(properties.Cast<PropertyDescriptor>()
        .Where(pd => pd.IsBrowsable && DGCore.Misc.TotalLine.IsTypeSupport(DGCore.Utils.Types.GetNotNullableType(pd.PropertyType)))
        .Select(pd => new DGCore.Misc.TotalLine(pd))); // Create total data source
      DGCore.Misc.TotalLine.ApplySettings(_totalLines, settings.TotalLines); // set statistic function & decimal places
      dgvTotals.DataSource = _totalLines;
      dgvTotals.Invalidate();

      // ShowTotalRow
      cbShowTotalRow.Checked = settings.ShowTotalRow;
    }

    public void ApplySettings(DGCore.UserSettings.DGV settings)
    {
      // Set column order && visibility
      var oldSettings = DGCore.Utils.Json.CloneJson(settings.AllColumns);
      settings.AllColumns.Clear();
      foreach (Misc.CheckedListBoxItem item in clbAllColumns.Items)
      {
        var oldItem = oldSettings.First(old => old.Id == item._id);
        oldItem.IsHidden = clbAllColumns.CheckedItems.Contains(item);
        settings.AllColumns.Add(oldItem);
      }

      // FrozenColumns
      settings.FrozenColumns = lbFrozenColumns.Items.Cast<Misc.CheckedListBoxItem>().Select(item => item._id).ToList();

      // Groups
      settings.Groups = clbGroups.Items.Cast<Misc.CheckedListBoxItem>().Select(item => new DGCore.UserSettings.Sorting
        {
          Id = item._id,
          SortDirection = clbGroups.CheckedItems.Contains(item) ? ListSortDirection.Descending : ListSortDirection.Ascending
        })
        .ToList();

      // Sorts
      settings.Sorts = clbSorts.Items.Cast<Misc.CheckedListBoxItem>().Select(item => new DGCore.UserSettings.Sorting
        {
          Id = item._id,
          SortDirection = clbSorts.CheckedItems.Contains(item) ? ListSortDirection.Descending : ListSortDirection.Ascending
        })
        .ToList();

      // Totals
      settings.TotalLines = _totalLines.Select(item => item.ToSettingsTotalLine()).ToList();

      // ShowTotalRow
      settings.ShowTotalRow = cbShowTotalRow.Checked;
    }

    private void ListBox_Dropped(object sender, ThirdParty.Oli.DroppedEventArgs e) {
      ListBox lbTarget = (ListBox)e.Target;
      ListBox lbSource = (ListBox)e.Source;
      if (lbSource == lbTarget) {
        if (lbTarget == lbFrozenColumns) ReoderAllColumnsListAccordingToFrozenColumns();
        return; // Reoder items == do not need to change something
      }
      if (lbTarget != this.clbAllColumns) {
        // Remove dublicates in target
        foreach (object o in e.DroppedItems) {
          for (int i = 0; i < lbTarget.Items.Count; i++) {
            if (lbTarget.Items[i].Equals(o) && !lbTarget.SelectedIndices.Contains(i)) {
              lbTarget.Items.RemoveAt(i--);
            }
          }
        }
        // Remove dublicates in Sorts and Groups
        if (lbTarget == this.clbGroups) {
          foreach (object o in this.clbGroups.Items) {
            for (int i = 0; i < this.clbSorts.Items.Count; i++) {
//              if (this.clbSorts.Items[i] == o) this.clbSorts.Items.RemoveAt(i--);
              if (Object.Equals(this.clbSorts.Items[i], o)) this.clbSorts.Items.RemoveAt(i--);
            }
          }
          // Add to frozen columns list
          foreach (object o1 in e.DroppedItems) {
            bool flag = false;
            foreach (object o2 in this.lbFrozenColumns.Items) {
              if (o1.Equals(o2)) { flag = true; break; }
            }
            if (!flag) { this.lbFrozenColumns.Items.Add(o1); }
          }
        }
        if (lbTarget == this.clbSorts) {
          foreach (object o in this.clbSorts.Items) {
            for (int i = 0; i < this.clbGroups.Items.Count; i++) {
//              if (this.clbGroups.Items[i] == o) this.clbGroups.Items.RemoveAt(i--);
              if (Object.Equals(this.clbGroups.Items[i],o)) this.clbGroups.Items.RemoveAt(i--);
            }
          }
        }
        // Remove from groups => remove from Frozen columns
        if (lbSource == this.clbGroups && lbTarget!=this.lbFrozenColumns) {
          foreach (object o1 in e.DroppedItems) {
            foreach (object o2 in this.lbFrozenColumns.Items) {
              if (o1.Equals(o2)) { this.lbFrozenColumns.Items.Remove(o2); break; }
            }
          }
        }
      }
      else {
        // Remove new items in cblAllColumns
        for (int i = (lbTarget.SelectedIndices.Count - 1); i >= 0; i--) {
          lbTarget.Items.RemoveAt(lbTarget.SelectedIndices[i]);
        }
        // Remove in source
        foreach (object o in e.DroppedItems) lbSource.Items.Remove(o);
        // Remove from groups => remove from Frozen columns
        if (lbSource == this.clbGroups) {
          foreach (object o1 in e.DroppedItems) {
            foreach (object o2 in this.lbFrozenColumns.Items) {
              if (o1.Equals(o2)) { this.lbFrozenColumns.Items.Remove(o2); break; }
            }
          }
        }
      }
/*      bool flagRemove = (lbSource != this.clbAllColumns);
      if (flagRemove && (lbSource==this.lbFrozenColumns && lbTarget != this.clbAllColumns)) flagRemove = false;
      if (flagRemove) {
        // Remove in source
        foreach (object o in e.DroppedItems) lbSource.Items.Remove(o);
      }*/
      if (lbTarget == this.lbFrozenColumns || lbSource==this.lbFrozenColumns) {
        ReoderAllColumnsListAccordingToFrozenColumns();
      }
    }

    void ReoderAllColumnsListAccordingToFrozenColumns() {
      // Frozen columns must be first in column list
      // Reoder frozen columns from AllColumnsList
      for (int i = (this.lbFrozenColumns.Items.Count - 1); i >= 0; i--) {
        object item = this.lbFrozenColumns.Items[i];
        this.clbAllColumns.Items.Remove(item);
        this.clbAllColumns.Items.Insert(0, item);
      }
    }

    private void dgV_ReoderColumnsAndRows1_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e) {
      DataGridViewRow r = this.dgvTotals.Rows[e.RowIndex];
      // Set backColor for cells (editable: LightCyan; readonly: WhiteSmoke);
      foreach (DataGridViewCell cell in r.Cells) {
        if (cell.ReadOnly) {
          if (cell.Style.BackColor != Color.WhiteSmoke) cell.Style.BackColor = Color.WhiteSmoke;
        }
        else {
          if (cell.Style.BackColor != Color.LightCyan)cell.Style.BackColor = Color.LightCyan;
        }
      }
    }

    private void ItemCheckChanges(object sender, ItemCheckEventArgs e) {
      var item = (Misc.CheckedListBoxItem)((CheckedListBox)sender).Items[e.Index];
      item._checkState = e.NewValue== CheckState.Checked;
    }
  }
}
