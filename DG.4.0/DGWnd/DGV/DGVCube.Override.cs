using System;
using System.Collections;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Windows.Forms;
using DGCore.UserSettings;
using Microsoft.VisualBasic;

namespace DGWnd.DGV
{
  public partial class DGVCube
  {
    protected override void OnCellMouseDown(DataGridViewCellMouseEventArgs e)
    {
      /*if (e.RowIndex == -1 && e.ColumnIndex >= 0 && this.Columns[e.ColumnIndex].SortMode == DataGridViewColumnSortMode.Automatic && this.CurrentCell != null) {
        this._lastActiveItem = this.Rows[this.CurrentRow.Index].DataBoundItem;
        this._lastActiveItemScreenOffset = this.CurrentRow.Index - this.FirstDisplayedScrollingRowIndex;
      }*/
      if (e.ColumnIndex == -1 && e.RowIndex >= 0)
      {        // row header click
        if (this.SelectionMode != DataGridViewSelectionMode.RowHeaderSelect) this.SelectionMode = DataGridViewSelectionMode.RowHeaderSelect;
      }
      else if (e.RowIndex == -1 && e.ColumnIndex >= 0)
      {        // column header click
        if (ModifierKeys == Keys.None)
        {
          if (this.SelectionMode != DataGridViewSelectionMode.RowHeaderSelect) this.SelectionMode = DataGridViewSelectionMode.RowHeaderSelect;
        }
        else
        {
          if (this.SelectionMode != DataGridViewSelectionMode.ColumnHeaderSelect) this.SelectionMode = DataGridViewSelectionMode.ColumnHeaderSelect;
        }
      }
      base.OnCellMouseDown(e);
    }

    protected override void OnCellDoubleClick(DataGridViewCellEventArgs e)
    {
      if (!(e.ColumnIndex == -1 || e.RowIndex == -1))
      {
        var cell = this[e.ColumnIndex, e.RowIndex];
        var column = this.Columns[cell.ColumnIndex];
        var columnName = column.DataPropertyName.ToUpper();
        var data = ((IList)this.DataSource)[e.RowIndex];
        if (data.GetType().Name != "DGVList_GroupItem`1")
        {
          var done = false;
          if (((IUserSettingProperties)this).SettingKey == "houses")
          {
            if (columnName == "COMMENT" || columnName == "YEAR" || columnName == "FLOORS" || columnName == "DEVELOPER")
            {
              done = true;
              EditCell(cell, data, "Houses");
            }
          }
          else if (((IUserSettingProperties)this).SettingKey == "domria")
          {
            if (columnName == "ID")
            {
              done = true;
              var url = @"https://dom.ria.com/uk/" + data.GetType().GetProperty("URL").GetValue(data).ToString();
              var sInfo = new ProcessStartInfo(url);
              Process.Start(sInfo);
            }
            else if (columnName == "COMMENT")
            {
              done = true;
              EditCell(cell, data, "DomRia");
            }
          }
          else if (((IUserSettingProperties)this).SettingKey == "olx")
          {
            if (columnName == "ID")
            {
              done = true;
              var url = data.GetType().GetProperty("HREF").GetValue(data).ToString();
              var sInfo = new ProcessStartInfo(url);
              Process.Start(sInfo);
            }
            else if (columnName == "COMMENT")
            {
              done = true;
              EditCell(cell, data, "Olx");
            }
          }
          else if (((IUserSettingProperties)this).SettingKey == "realestate2021")
          {
            if (columnName == "ID")
            {
              done = true;
              var url = @"https://www.real-estate.lviv.ua" + data.GetType().GetProperty("HREF").GetValue(data).ToString();
              var sInfo = new ProcessStartInfo(url);
              Process.Start(sInfo);
            }
            else if (columnName == "COMMENT")
            {
              done = true;
              var result = Interaction.InputBox("Enter comment text", "Cell edit", cell.Value?.ToString());
              EditCell(cell, data, "RealEstate");
            }
          }
          else if (((DGCore.UserSettings.IUserSettingProperties)this).SettingKey == "realestate")
          {
            if (columnName == "ID")
            {
              done = true;
              var url = data.GetType().GetProperty("URL").GetValue(data).ToString();
              var sInfo = new ProcessStartInfo(@"https://www.real-estate.lviv.ua" + url);
              Process.Start(sInfo);
            }
            else if (columnName == "COMMENT")
            {
              done = true;
              var result = Interaction.InputBox("Enter comment text", "Cell edit", cell.Value?.ToString());
              if (!string.IsNullOrEmpty(result))
              {
                result = result.Trim();
                if (string.IsNullOrWhiteSpace(result))
                  result = null;

                cell.Value = result;
                var id = data.GetType().GetProperty("ID").GetValue(data).ToString();
                using (var conn =
                  new SqlConnection("Data Source=localhost;Initial Catalog=dbAssessment;Integrated Security=True"))
                using (var cmd = new SqlCommand("UPDATE RealEstate SET comment=@comment where id=@id", conn))
                {
                  conn.Open();
                  cmd.Parameters.Add(new SqlParameter("comment", (object)result ?? DBNull.Value));
                  cmd.Parameters.Add(new SqlParameter("id", id));
                  cmd.ExecuteNonQuery();
                }
              }
            }
          }
          else if (((DGCore.UserSettings.IUserSettingProperties)this).SettingKey == "datasets")
          {
            if (columnName == "ID")
            {
              done = true;
              var id = data.GetType().GetProperty("ID").GetValue(data).ToString();
              var sInfo = new ProcessStartInfo(@"https://data.gov.ua/dataset/" + id);
              Process.Start(sInfo);
            }
            else if (columnName == "COMMENT")
            {
              done = true;
              var result = Interaction.InputBox("Enter comment text", "Cell edit", cell.Value?.ToString());
              if (!string.IsNullOrEmpty(result))
              {
                result = result.Trim();
                if (string.IsNullOrWhiteSpace(result))
                  result = null;

                cell.Value = result;
                var id = data.GetType().GetProperty("ID").GetValue(data).ToString();
                using (var conn =
                  new SqlConnection("Data Source=localhost;Initial Catalog=UOD;Integrated Security=True"))
                using (var cmd = new SqlCommand("UPDATE datasets SET comment=@comment where id=@id", conn))
                {
                  conn.Open();
                  cmd.Parameters.Add(new SqlParameter("comment", (object)result ?? DBNull.Value));
                  cmd.Parameters.Add(new SqlParameter("id", id));
                  cmd.ExecuteNonQuery();
                }
              }
            }
          }
          var fi = data.GetType().GetProperty(columnName);
          var cellData = fi == null ? null : fi.GetValue(data);
          if (!done && cellData is string s && s.StartsWith(@"https://"))
          {
            var sInfo = new ProcessStartInfo(s);
            Process.Start(sInfo);
          }
        }
      }
      base.OnCellDoubleClick(e);
    }

    private void EditCell(DataGridViewCell cell, object data, string tableName)
    {
      var propertyName = cell.DataGridView.Columns[cell.ColumnIndex].DataPropertyName;
      var result = Interaction.InputBox($"Enter {propertyName} text of value", "Cell editing", cell.Value?.ToString());
      if (!string.IsNullOrEmpty(result))
      {
        result = result.Trim();
        if (string.IsNullOrWhiteSpace(result))
          result = null;

        cell.Value = result;
        var id = data.GetType().GetProperty("ID").GetValue(data).ToString();
        using (var conn = new SqlConnection("Data Source=localhost;Initial Catalog=dbLvivFlat2021;Integrated Security=True"))
        using (var cmd = new SqlCommand($"UPDATE [{tableName}] SET {propertyName}=@value where id=@id", conn))
        {
          conn.Open();
          cmd.Parameters.Add(new SqlParameter("value", (object)result ?? DBNull.Value));
          cmd.Parameters.Add(new SqlParameter("id", id));
          cmd.ExecuteNonQuery();
        }
      }
    }


    protected override void OnDataError(bool displayErrorDialogIfNoHandler, DataGridViewDataErrorEventArgs e)
    {
      var cell = this[e.ColumnIndex, e.RowIndex];
      cell.ToolTipText = $"Data error!!!\nContext: {e.Context.ToString()}\nMessage: {e.Exception.Message}\nStack trace: {e.Exception.StackTrace.Trim()}";
      // ToDo: remove tooltip when no error
      // base.OnDataError(displayErrorDialogIfNoHandler, e);
    }

    /*public override void Sort(DataGridViewColumn dataGridViewColumn, ListSortDirection direction)
    {
      // "CurrentCell can not be set to invisible cell" Error occurs when sorted change from Descending to unsorted in group mode
      try
      {
        base.Sort(dataGridViewColumn, direction);
      }
      catch (Exception ex) { }
    }*/

    protected override void OnKeyDown(KeyEventArgs e)
    {
      if (e.Control && (e.KeyCode == Keys.F))
      {
        Find_OpenForm();
        e.Handled = true;
      }
      base.OnKeyDown(e);
    }

    protected override bool ProcessDataGridViewKey(KeyEventArgs e)
    {
      if (e.Control && (e.KeyCode == Keys.C || e.KeyCode == Keys.Insert))
      {
        // Copy to Clipboard
        Utils.DGVClipboard.Clipboard_CopyByObjectsAsText(this);
        return true;
      }

      if (e.KeyCode == Keys.Delete)
      {// Delete rows
       /*if (this.AllowUserToDeleteRows && this.EditingControl == null && (
         this.SelectionMode == DataGridViewSelectionMode.FullRowSelect ||
         this.SelectionMode == DataGridViewSelectionMode.RowHeaderSelect)) {

     //          ((IDGVList)this.DataSource).Items_Delete();
     //        return true;

         List<int> rowNos = new List<int>();
         List<int> uncommittedRowNos = new List<int>();

         foreach (DataGridViewRow r in this.SelectedRows) {
           if (r.IsNewRow || (_lastActiveRow == r && _lastItem == null)) uncommittedRowNos.Add(r.Index);
           else rowNos.Add(r.Index);
         }

         ((IDGVList)this.DataSource).Items_Delete(rowNos, uncommittedRowNos);
         return true;
       }*/
      }
      else if (e.KeyCode == Keys.Escape)
      {// VS sets IsCurrentRowDirty to false after Escape pressed independed from does DataArray support CancelEdit or not
        if (this.CurrentCell != null && !this.CurrentCell.IsInEditMode)
        {
          this.CancelEdit();
          // and then we need to call standard procedure to set IsCurrentRowDirty into false
        }
      }
      return base.ProcessDataGridViewKey(e);
    }

    protected override void OnCellClick(DataGridViewCellEventArgs e)
    {
      base.OnCellClick(e);
      if (e.ColumnIndex > 0 && e.RowIndex >= 0 && this._visibleGroupNumberStart.HasValue)
      {
          DGCore.DGVList.IDGVList_GroupItem o = this.Rows[e.RowIndex].DataBoundItem as DGCore.DGVList.IDGVList_GroupItem;
        if (o != null && o.Level > 0 && this.Columns[e.ColumnIndex] == this._groupColumns[o.Level - 1])
        {
          // EndEdit();
          SuspendLayout();// ??? Does not work: flipped
          // Save item position
          _lastActiveItem = o;
          _lastActiveItemScreenOffset = CurrentRow.Index - FirstDisplayedScrollingRowIndex;

          DataSource.ItemExpandedChanged(e.RowIndex);

          // Restore position
          RestorePositionOfLastActiveItem();
          ResumeLayout(true);
        }
      }
    }

    /* Doesnot work == column selection (use shirt+mouse click); comment at 2015-04-19
    protected override void OnCellDoubleClick(DataGridViewCellEventArgs e) {
      if (e.ColumnIndex >= 0 && e.RowIndex == -1) {
        //      this.Columns[e.ColumnIndex].Selected = !this.Columns[e.ColumnIndex].Selected;
        this.Columns[e.ColumnIndex].Selected = true;
      }
      //      base.OnCellDoubleClick(e);
    }*/

    protected override void OnFontChanged(EventArgs e)
    {
      this.RowTemplate.Height = Convert.ToInt32(this.Font.GetHeight()) + 6;
      base.OnFontChanged(e);
      if (Columns.Count > 0)
        ResizeColumnWidth();
    }

    protected override void OnColumnWidthChanged(DataGridViewColumnEventArgs e)
    {
      this._layoutCount++;
      base.OnColumnWidthChanged(e);
    }

    protected override void OnCellEnter(DataGridViewCellEventArgs e)
    {
      if (_isOnCellEnterActive)
      {
        GetCellDataStatus(e.ColumnIndex, e.RowIndex);
        base.OnCellEnter(e);
        RefreshSortColumnGlyphs();
      }
    }

  }

}