using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DGCore.Common;
using DGCore.UserSettings;

namespace DGView.ViewModels
{
    public partial class DataGridViewModel
    {
        internal const string UserSettingsKind = "DGV_Setting";
        string IUserSettingProperties.SettingKind => UserSettingsKind;
        private string LayoutId { get; set; }
        string IUserSettingProperties.SettingKey => LayoutId;

        public DGV GetSettings()
        {
            // var font = new System.Drawing.Font;
            var o = new DGV
            {
                WhereFilter = ((IUserSettingSupport<List<Filter>>) Data.WhereFilter)?.GetSettings(),
                FilterByValue = ((IUserSettingSupport<List<Filter>>) Data.FilterByValue)?.GetSettings(),
                ShowTotalRow = Data.ShowTotalRow,
                ExpandedGroupLevel = Data.ExpandedGroupLevel,
                ShowGroupsOfUpperLevels = Data.ShowGroupsOfUpperLevels,
                // BaseFont = DGControl.FontFamily,
                IsGridVisible = DGControl.GridLinesVisibility == DataGridGridLinesVisibility.All,
                CellViewMode = Enums.DGCellViewMode.OneRow,
                // CellViewMode = this._CellViewMode,
                TextFastFilter = QuickFilterText
            };
            SaveColumnLayout(o);
            return o;
        }

        public DGV GetBlankSetting()
        {
            // Utils.Dgv.EndEdit(this);
            Data.ResetSettings();
            //Font = _startupFont;
            // CellBorderStyle = DataGridViewCellBorderStyle.Single; // For _IsGridVisible
            // CellViewMode = Enums.DGCellViewMode.OneRow;

            // For AllColumns
            //_allValidColumnNames = DGControl.Columns.Where(col => !string.IsNullOrEmpty(col.SortMemberPath) && !col.SortMemberPath.Contains('.'))
              //  .Select(col => col.SortMemberPath).ToList();

            ResizeColumnWidth(); // !!! Before SaveColumnInfo*/
            return ((IUserSettingSupport<DGV>) this).GetSettings();
        }

        public int _layoutCount = 0;
        public void ResizeColumnWidth()
        {
            this._layoutCount++;
            // this.AutoResizeColumns(this._CellViewMode == DGCore.Common.Enums.DGCellViewMode.NotSet ? DataGridViewAutoSizeColumnsMode.DisplayedCells : DataGridViewAutoSizeColumnsMode.ColumnHeader, false);
            // old this.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.ColumnHeader, true);
        }

        public void SetSetting(DGV settings)
        {
              Data.SetSettings(settings);
              CellViewMode = settings.CellViewMode;
              RestoreColumnLayout(settings);

              // Fixed bug (RefreshData()):
              // 2. Зміна записаних налаштувань для MastCoA.
              //  - default налаштування з групами
              //  - якщо міняємо налаштування, то збиваються колонки
            if (Data.UnderlyingData.IsDataReady)
            {
                Data.RefreshData();
            }

            // Invalidate(); // corrected bug - column header is blank after apply setting with new column
        }

        private void RestoreColumnLayout(DGV settingInfo)
        {
            if (Data.Groups.Count > 0)
                CreateGroupColumns();

            // ====================
            // Restore Columns order
            // ====================
            for (var i = settingInfo.AllColumns.Count - 1; i >= 0; i--)
            {
                var column = settingInfo.AllColumns[i];
                var k = Helpers.DataGridHelper.GetColumnIndexByPropertyName(DGControl, column.Id);
                if (k >= 0)
                {
                    var col = DGControl.Columns[k];
                    if (!column.IsHidden)
                        col.DisplayIndex = 0;

                    if (column.Width.HasValue && column.Width.Value > 0 && CellViewMode != Enums.DGCellViewMode.OneRow)
                        col.Width = column.Width.Value;
                    else
                        col.Width = DataGridLength.Auto;
                }
            }

            // ====================
            // Set ColumnVisibility
            // ====================
            foreach (var col in DGControl.Columns.OfType<DataGridBoundColumn>().Where(c => !string.IsNullOrEmpty(c.SortMemberPath)))
            {
                var settingColumn = settingInfo.AllColumns.FirstOrDefault(c => c.Id == col.SortMemberPath);
                if (settingColumn == null)
                    Helpers.DataGridHelper.SetColumnVisibility(col, false);
                else
                    Helpers.DataGridHelper.SetColumnVisibility(col, !settingColumn.IsHidden && Data.IsPropertyVisible(col.SortMemberPath));
            }

            // Set group columns visibility
            for (var k = 0; k < _groupColumns.Count; k++)
                Helpers.DataGridHelper.SetColumnVisibility(_groupColumns[k], Data.IsGroupColumnVisible(k));

            // Set GroupItemCount column visibility
            if (GroupItemCountColumn != null)
                Helpers.DataGridHelper.SetColumnVisibility(GroupItemCountColumn, Data.Groups.Count > 0);

            // ====================
            // Frozen columns
            // ====================
            var cntFrozen = 0;
            for (var i = 0; i < Data.Groups.Count; i++)
                _groupColumns[i].DisplayIndex = cntFrozen++;

            if (Data.Groups.Count > 0)
                GroupItemCountColumn.DisplayIndex = cntFrozen++;

            // Restore order of frozen columns
            foreach (var column in settingInfo.FrozenColumns)
            {
                var k = Helpers.DataGridHelper.GetColumnIndexByPropertyName(DGControl, column);
                if (k >= 0 && DGControl.Columns[k].Visibility == Visibility.Visible) // && !Columns[k].Frozen)
                    DGControl.Columns[k].DisplayIndex = cntFrozen++;
            }
            DGControl.FrozenColumnCount = cntFrozen;

            OnPropertiesChanged(nameof(IsGroupLevelButtonEnabled));
        }

        private void CreateGroupColumns()
        {
            if (GroupItemCountColumn == null)
            {
                GroupItemCountColumn = View.Resources["GroupItemCountColumn"] as DataGridColumn;
                DGControl.Columns.Add(GroupItemCountColumn);
            }

            // Create new group columns if neccessary
            while (Data.Groups.Count > _groupColumns.Count)
            {
                var groupColumn = new DataGridTextColumn
                {
                    IsReadOnly = true,
                    Header = (_groupColumns.Count + 1).ToString(),
                    CanUserResize = true, //Resizable = DataGridViewTriState.False,
                    CanUserSort = false, //SortMode = DataGridViewColumnSortMode.NotSortable,
                };
                DGControl.Columns.Add(groupColumn);
                _groupColumns.Add(groupColumn);
            }
        }

        private void SaveColumnLayout(DGV settings)
        {
            var cols = Helpers.DataGridHelper.GetColumnsInDisplayOrder(DGControl, false);

            // Set columns for default settings
            foreach (var c in cols)
                if (!string.IsNullOrEmpty(c.SortMemberPath))
                {
                    settings.AllColumns.Add(new Column
                    {
                        Id = c.SortMemberPath,
                        DisplayName = Properties[c.SortMemberPath].DisplayName,
                        IsHidden = c.Visibility != Visibility.Visible,
                        Width = System.Convert.ToInt32(c.ActualWidth)
                    });

                    if (c.IsFrozen)
                        settings.FrozenColumns.Add(c.SortMemberPath);
                }

            settings.Groups.AddRange(Data.Groups.Select(
                e => new Sorting { Id = e.PropertyDescriptor.Name, SortDirection = e.SortDirection }));

            settings.Sorts.AddRange(Data.Sorts.Select(
                e => new Sorting { Id = e.PropertyDescriptor.Name, SortDirection = e.SortDirection }));

            Data.SortsOfGroups.ForEach(e1 =>
            {
                var list = e1.Select(e2 => new Sorting { Id = e2.PropertyDescriptor.Name, SortDirection = e2.SortDirection })
                    .ToList();
                settings.SortsOfGroup.Add(list);
            });

            foreach (var totalLine in Data.TotalLines.Where(tl => tl.TotalFunction != DGCore.Common.Enums.TotalFunction.None))
                settings.TotalLines.Add(new TotalLine
                {
                    Id = totalLine.Id,
                    DecimalPlaces = totalLine.DecimalPlaces,
                    TotalFunction = totalLine.TotalFunction
                });
        }

        private void SetCellElementStyleAndWidth()
        {
            foreach (var col in DGControl.Columns.OfType<DataGridTextColumn>())
            {
                var p = Properties.OfType<PropertyDescriptor>().FirstOrDefault(p1 => p1.Name == col.SortMemberPath);
                if (p != null)
                {
                    var alignment = Helpers.DataGridHelper.GetDefaultColumnAlignment(p.PropertyType);
                    if (alignment != null)
                    {
                        var wrap = _cellViewMode == Enums.DGCellViewMode.WordWrap ? "Wrap" : "NoWrap";
                        var styleName = $"CellStyle_{wrap}{alignment}";
                        var style = View.Resources[styleName] as Style;
                        col.ElementStyle = style;
                    }
                }
            }
        }
    }
}
