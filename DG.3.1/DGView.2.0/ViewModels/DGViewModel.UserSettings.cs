using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DGCore.Common;
using DGCore.UserSettings;

namespace DGView.ViewModels
{
    public partial class DGViewModel
    {
        internal const string UserSettingsKind = "DGV_Setting";
        string IUserSettingProperties.SettingKind => UserSettingsKind;
        string IUserSettingProperties.SettingKey => DataDefinition.SettingID;

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
                CellViewMode = CellViewMode,
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
            // Prepare columns list
            _columns.Clear();
            _columns.AddRange(settings.AllColumns);
            for (var k = 0; k < _columns.Count; k++)
            {
                var dgCol = DGControl.Columns.FirstOrDefault(c => c.SortMemberPath == _columns[k].Id);
                if (dgCol == null)
                    _columns.RemoveAt(k--);
            }
            foreach (var col in DGControl.Columns.Where(c => !string.IsNullOrEmpty(c.SortMemberPath)))
            {
                var c1 = _columns.FirstOrDefault(c => c.Id == col.SortMemberPath);
                if (c1 == null)
                    _columns.Add(new Column() { Id = col.SortMemberPath, DisplayName = col.SortMemberPath.Replace(".", "^"), IsHidden = true });
            }

            _frozenColumns.Clear();
            _frozenColumns.AddRange(settings.FrozenColumns);

            // ToDo: reorder columns according frozen columns

            // ====================
            // Prepare start column layout : restore columns order & width
            // ====================
            Dispatcher.BeginInvoke(new Action(() =>
            {
                for (var k = _columns.Count - 1; k >= 0; k--)
                {
                    var col = _columns[k];
                    var dgCol = DGControl.Columns.First(c => c.SortMemberPath == col.Id);
                    dgCol.DisplayIndex = 0;

                    if (col.Width.HasValue && col.Width.Value > 0 && CellViewMode != Enums.DGCellViewMode.OneRow)
                        dgCol.Width = col.Width.Value;
                    else
                        dgCol.Width = DataGridLength.Auto;
                }
            }));

            Data.SetSettings(settings);
            CellViewMode = settings.CellViewMode;
            DGControl.GridLinesVisibility = settings.IsGridVisible ? DataGridGridLinesVisibility.All : DataGridGridLinesVisibility.None;

            OnPropertiesChanged(nameof(IsGroupLevelButtonEnabled));

            // Fixed bug (RefreshData()):
            // 2. Зміна записаних налаштувань для MastCoA.
            //  - default налаштування з групами
            //  - якщо міняємо налаштування, то збиваються колонки
            if (Data.UnderlyingData.IsDataReady)
                Data.RefreshData();
            // else
               // await Task.Factory.StartNew(() => Data.UnderlyingData.GetData(false));

            // Invalidate(); // corrected bug - column header is blank after apply setting with new column
        }

        private void RestoreColumnLayout(DGV settingInfo)
        {
            if (Data.Groups.Count > 0)
                CreateGroupColumns();

            // ====================
            // Set ColumnVisibility
            // ====================
            foreach (var dgCol in DGControl.Columns.OfType<DataGridBoundColumn>().Where(c => !string.IsNullOrEmpty(c.SortMemberPath)))
            {
                var col = _columns.FirstOrDefault(c => c.Id == dgCol.SortMemberPath);
                if (col == null)
                    Helpers.DataGridHelper.SetColumnVisibility(dgCol, false);
                else
                    Helpers.DataGridHelper.SetColumnVisibility(dgCol, !col.IsHidden && Data.IsPropertyVisible(dgCol.SortMemberPath));
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
            var cntFrozen = _frozenColumns.Count;
            for (var k = Data.Groups.Count - 1; k >= 0; k--)
            {
                _groupColumns[k].DisplayIndex = 0;
                cntFrozen++;
            }

            if (Data.Groups.Count > 0)
            {
                GroupItemCountColumn.DisplayIndex = 0;
                cntFrozen++;
            }

            DGControl.FrozenColumnCount = cntFrozen;
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
            // Columns in display order
            var cols = DGControl.Columns.OrderBy(c => c.DisplayIndex).ToArray();

            // Set columns for default settings
            foreach (var c in cols)
                if (!string.IsNullOrEmpty(c.SortMemberPath))
                {
                    settings.AllColumns.Add(new Column
                    {
                        Id = c.SortMemberPath,
                        DisplayName = Properties[c.SortMemberPath].DisplayName,
                        IsHidden = c.Visibility != Visibility.Visible,
                        Width = c.Width == DataGridLength.Auto ? (int?)null : System.Convert.ToInt32(c.ActualWidth)
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
