using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DGWnd.Utils;

namespace DGWnd.DGV
{

  public partial class DGVCube: DGCore.UserSettings.IUserSettingSupport<DGCore.UserSettings.DGV>
  {
    internal const string UserSettingsKind = "DGV_Setting";
    string DGCore.UserSettings.IUserSettingProperties.SettingKind => UserSettingsKind;
    string DGCore.UserSettings.IUserSettingProperties.SettingKey => _layoutID;

    DGCore.UserSettings.DGV DGCore.UserSettings.IUserSettingSupport<DGCore.UserSettings.DGV>.GetSettings()
    {
      DGVUtils.EndEdit(this);
      var fontConverter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(Font));
      var o = new DGCore.UserSettings.DGV
      {
        WhereFilter = ((DGCore.UserSettings.IUserSettingSupport<List<DGCore.UserSettings.Filter>>)DataSource.WhereFilter)?.GetSettings(),
        FilterByValue = ((DGCore.UserSettings.IUserSettingSupport<List<DGCore.UserSettings.Filter>>)DataSource.FilterByValue)?.GetSettings(),
        ShowTotalRow = DataSource.ShowTotalRow,
        ExpandedGroupLevel = DataSource.ExpandedGroupLevel,
        ShowGroupsOfUpperLevels = DataSource.ShowGroupsOfUpperLevels,
        BaseFont = Font == null ? null : fontConverter.ConvertToInvariantString(Font),
        IsGridVisible = _IsGridVisible,
        RowViewMode = _RowViewMode,
        TextFastFilter = DataSource.TextFastFilter
      };
      SaveColumnLayout(o);
      return o;
    }

    DGCore.UserSettings.DGV DGCore.UserSettings.IUserSettingSupport<DGCore.UserSettings.DGV>.GetBlankSetting()
    {
        DGVUtils.EndEdit(this);
      DataSource.ResetSettings();
      Font = _startupFont;
      CellBorderStyle = DataGridViewCellBorderStyle.Single; // For _IsGridVisible
      _RowViewMode = DGCore.Common.Enums.DGRowViewMode.OneRow;

      // For AllColumns
      _allValidColumnNames = Columns.Cast<DataGridViewColumn>()
        .Where(col => !string.IsNullOrEmpty(col.DataPropertyName) && !col.DataPropertyName.Contains(DGCore.Common.Constants.MDelimiter))
        .Select(col => col.DataPropertyName).ToList();

      ResizeColumnWidth(); // !!! Before SaveColumnInfo
      return ((DGCore.UserSettings.IUserSettingSupport<DGCore.UserSettings.DGV>)this).GetSettings();
    }

    void DGCore.UserSettings.IUserSettingSupport<DGCore.UserSettings.DGV>.SetSetting(DGCore.UserSettings.DGV settings)
    {
      // TopLevelControl?.SuspendLayout();
      // SuspendLayout();
      if (Visible)
        Visible = false;

      DataSource.SetSettings(settings);

      if (!string.IsNullOrEmpty(settings.BaseFont))
      {
          var converter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(Font));
          Font = (Font)converter.ConvertFromInvariantString(settings.BaseFont);
      }
      _IsGridVisible = settings.IsGridVisible;
      _RowViewMode = settings.RowViewMode;

      RestoreColumnLayout(settings);

      // Fixed bug (RefreshData()):
      // 2. Зміна записаних налаштувань для MastCoA.
      //  - default налаштування з групами
      //  - якщо міняємо налаштування, то збиваються колонки
      if (DataSource.UnderlyingData.IsDataReady)
      {
        // UI.frmLog.AddEntry("Settings Before RefreshData " + Columns.Cast<DataGridViewColumn>().Count(c => c.Visible));
        DataSource.RefreshData();
      }

      // Invalidate(); // corrected bug - column header is blank after apply setting with new column
    }

    private void SaveColumnLayout(DGCore.UserSettings.DGV settings)
    {
      var cols = DGVUtils.GetColumnsInDisplayOrder(this, false);

      // Set columns for default settings
      foreach (var c in cols)
        if (!string.IsNullOrEmpty(c.DataPropertyName))
        {
          settings.AllColumns.Add(new DGCore.UserSettings.Column
          {
            Id = c.DataPropertyName,
            // DisplayName = DataSource.Properties[c.DataPropertyName].DisplayName,
            IsHidden = !_allValidColumnNames?.Contains(c.DataPropertyName) ?? !c.Visible,
            Width = c.Width
          });

          if (c.Frozen)
            settings.FrozenColumns.Add(c.DataPropertyName);
        }

      settings.Groups.AddRange(DataSource.Groups.Select(
        e => new DGCore.UserSettings.Sorting { Id = e.PropertyDescriptor.Name, SortDirection = e.SortDirection }));

      settings.Sorts.AddRange(DataSource.Sorts.Select(
        e => new DGCore.UserSettings.Sorting { Id = e.PropertyDescriptor.Name, SortDirection = e.SortDirection }));

      DataSource.SortsOfGroups.ForEach(e1 =>
      {
        var list = e1.Select(e2 => new DGCore.UserSettings.Sorting { Id = e2.PropertyDescriptor.Name, SortDirection = e2.SortDirection })
          .ToList();
        settings.SortsOfGroup.Add(list);
      });

      foreach (var totalLine in DataSource.TotalLines.Where(tl => tl.TotalFunction != DGCore.Common.Enums.TotalFunction.None))
        settings.TotalLines.Add(new DGCore.UserSettings.TotalLine
        {
          Id = totalLine.Id,
          DecimalPlaces = totalLine.DecimalPlaces,
          TotalFunction = totalLine.TotalFunction
        });
    }

    private void RestoreColumnLayout(DGCore.UserSettings.DGV settingInfo)
    {
      _isOnCellEnterActive = false;
      _allValidColumnNames.Clear();
      _SetGroupColumns();

      // Unfroze columns
      for (var i = 0; i < Columns.Count; i++)
        if (Columns[i].Frozen)
          Columns[i].Frozen = false;

      // Restore Columns order
      for (var i = (settingInfo.AllColumns.Count - 1); i >= 0; i--)
      {
        var column = settingInfo.AllColumns[i];
        var k = DGVUtils.GetColumnIndexByPropertyName(this, column.Id);
        if (k >= 0)
        {
          var col = Columns[k];
          if (!column.IsHidden)
          {
            _allValidColumnNames.Add(col.DataPropertyName);
            col.DisplayIndex = 0;
          }

          /*var visible = !column.IsHidden && DataSource.IsPropertyVisible(column.Id); // on Startup DataSource.IsPropertyVisible == false for all columns
          if (col.Visible != visible)
            col.Visible = visible;*/
          if (col.Visible == column.IsHidden)
            col.Visible = !column.IsHidden;
          if (column.Width.HasValue && column.Width.Value > 0)
            col.Width = column.Width.Value;
        }
      }

      // Hide column which doesn't exist in SettingInfo
      foreach (var column in Columns.OfType<DataGridViewColumn>().Where(c=> !string.IsNullOrEmpty(c.DataPropertyName)))
      {
        var infoColumn = settingInfo.AllColumns.FirstOrDefault(c => c.Id == column.DataPropertyName);
        if (infoColumn == null)
          column.Visible = false;
      }

      var cntFrozen = 0;
      // Image group columns: Restore order and freeze
      //      for (int i = (this._groups.Count - 1); i >= 0; i--) {
      for (var i = 0; i < DataSource.Groups.Count; i++)
      {
        _groupColumns[i].DisplayIndex = cntFrozen++;
        _groupColumns[i].Frozen = true;
        if (_groupColumns[i].Visible != DataSource.IsGroupColumnVisible(i))
          _groupColumns[i].Visible = !_groupColumns[i].Visible;
      }
      // Set itemcount group column
      if (_groupItemCountColumn.Visible != (DataSource.Groups.Count > 0))
        _groupItemCountColumn.Visible = !_groupItemCountColumn.Visible;

      if (DataSource.Groups.Count > 0)
      {
        _groupItemCountColumn.DisplayIndex = cntFrozen++;
        _groupItemCountColumn.Frozen = true;
      }

      // Restore order of frozen columns
      foreach (var column in settingInfo.FrozenColumns)
      {
        var k = DGVUtils.GetColumnIndexByPropertyName(this, column);
        if (k >= 0 && Columns[k].Visible && !Columns[k].Frozen)
        {
          Columns[k].DisplayIndex = cntFrozen++;
          Columns[k].Frozen = true;
        }
      }
      _isOnCellEnterActive = true;
    }

  }
}
