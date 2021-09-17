using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using DGCore.Common;
using DGCore.UserSettings;
using DGView.Controls;
using WpfSpLib.Helpers;

namespace DGView.ViewModels
{
    public partial class DGViewModel
    {
        internal const string UserSettingsKind = "DGV_Setting";
        string IUserSettingProperties.SettingKind => UserSettingsKind;
        private string LayoutId { get; set; }
        string IUserSettingProperties.SettingKey => LayoutId;

        public DGV GetSettings()
        {
            Debug.Print($"GetSettings");
            // var font = new System.Drawing.Font;
            var o = new DGV
            {
                WhereFilter = ((IUserSettingSupport<List<Filter>>) Data.WhereFilter)?.GetSettings(),
                FilterByValue = ((IUserSettingSupport<List<Filter>>) Data.FilterByValue)?.GetSettings(),
                ShowTotalRow = Data.ShowTotalRow,
                ExpandedGroupLevel = Data.ExpandedGroupLevel,
                ShowGroupsOfUpperLevels = Data.ShowGroupsOfUpperLevels,
                // BaseFont = DGControl.FontFamily,
                IsGridVisible = IsGridLinesVisible,
                RowViewMode = RowViewMode,
                // RowViewMode = this._RowViewMode,
                TextFastFilter = QuickFilterText
            };
            SaveColumnLayout(o);
            return o;
        }

        public DGV GetBlankSetting()
        {
            Debug.Print($"GetBlankSetting");
            // Utils.Dgv.EndEdit(this);
            Data.ResetSettings();
            //Font = _startupFont;
            // CellBorderStyle = DataGridViewCellBorderStyle.Single; // For _IsGridVisible
            // RowViewMode = Enums.DGRowViewMode.OneRow;

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
            // this.AutoResizeColumns(this._RowViewMode == DGCore.Common.Enums.DGRowViewMode.NotSet ? DataGridViewAutoSizeColumnsMode.DisplayedCells : DataGridViewAutoSizeColumnsMode.ColumnHeader, false);
            // old this.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.ColumnHeader, true);
        }

        public void SetSetting(DGV settings)
        {
            Debug.Print($"SetSetting: {settings}");

            if (settings.Groups.Count != _groupColumns.Count)
                ManageGroupColumns(settings.Groups.Count);

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
                var colCnt = 0;
                for (var k = 0; k < _groupColumns.Count; k++)
                {
                    if (_groupColumns[k].DisplayIndex != colCnt++)
                        _groupColumns[k].DisplayIndex = colCnt-1;
                }

                if (GroupItemCountColumn != null && GroupItemCountColumn.DisplayIndex != colCnt++)
                    GroupItemCountColumn.DisplayIndex = colCnt - 1;

                for (var k = 0; k < _columns.Count; k++)
                {
                    var col = _columns[k];
                    var dgCol = DGControl.Columns.First(c => c.SortMemberPath == col.Id);
                    if (dgCol.DisplayIndex != colCnt++)
                        dgCol.DisplayIndex = colCnt - 1;
                }

                DGControl.FrozenColumnCount = _groupColumns.Count + _frozenColumns.Count + (_groupColumns.Count > 0 ? 1 : 0);
            }));

            SetColumnVisibility();

            Data.SetSettings(settings);
            RowViewMode = settings.RowViewMode;

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

        internal void SetColumnVisibility()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                // Set group columns visibility
                for (var k = 0; k < _groupColumns.Count; k++)
                    Helpers.DataGridHelper.SetColumnVisibility(_groupColumns[k], Data.IsGroupColumnVisible(k));

                // Set GroupItemCount column visibility
                if (GroupItemCountColumn != null)
                    Helpers.DataGridHelper.SetColumnVisibility(GroupItemCountColumn, Data.Groups.Count > 0);

                foreach (var dgCol in DGControl.Columns.OfType<DataGridBoundColumn>().Where(c => !string.IsNullOrEmpty(c.SortMemberPath)))
                {
                    var col = _columns.FirstOrDefault(c => c.Id == dgCol.SortMemberPath);
                    if (col == null)
                        Helpers.DataGridHelper.SetColumnVisibility(dgCol, false);
                    else
                        Helpers.DataGridHelper.SetColumnVisibility(dgCol, !col.IsHidden && Data.IsPropertyVisible(dgCol.SortMemberPath));
                }
            }));
        }

        private void ManageGroupColumns(int groupCount)
        {
            Debug.Print($"CreateGroupColumns: {groupCount}");

            if (GroupItemCountColumn == null)
            {
                GroupItemCountColumn = View.Resources["GroupItemCountColumn"] as DataGridColumn;
                DGControl.Columns.Insert(0, GroupItemCountColumn);
            }

            // Create new group columns if neccessary
            while (groupCount > _groupColumns.Count)
            {
                var template = TemplateGenerator.CreateDataTemplate(() =>
                    {
                        var viewbox = new Viewbox
                        {
                            Child = new Path {Data = Geometry.Empty, Fill = DGControl.Foreground}
                        };
                        var borderDot = new Grid
                        {
                            Height = 1, Width = 1, Background = CustomDataGrid.GroupBorderBrush,
                            HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Bottom
                        };
                        var grid = new Grid();
                        grid.Children.Add(viewbox);
                        grid.Children.Add(borderDot);
                        grid.Children.Add(new Grid {Background = Brushes.White, Opacity = 0.01});
                        return grid;
                    }
                );

                //var a = _groupColumns.Count % (DataGridView._groupBrushes.Length - 1) + 1;
                //var brush = DataGridView._groupBrushes[a];
                //var cellStyle = new Style(typeof(DataGridCell));
                //cellStyle.Setters.Add(new Setter(Control.BackgroundProperty, brush));

                var headerStyle = View.Resources["DataGridGroupColumnHeaderStyle"] as Style;
                var groupColumn = new DataGridTemplateColumn()
                {
                    IsReadOnly = true,
                    CanUserResize = false,
                    CanUserSort = false,
                    CellTemplate = template,
                    HeaderStyle = headerStyle,
                    Width = _groupColumns.Count == 0 ? 21 : 20
                    //CellStyle = cellStyle
                };
                DGControl.Columns.Insert(_groupColumns.Count, groupColumn);
                _groupColumns.Add(groupColumn);
            }

            // Remove unnecessary columns
            while (groupCount < _groupColumns.Count)
            {
                DGControl.Columns.Remove(_groupColumns[_groupColumns.Count - 1]);
                _groupColumns.RemoveAt(_groupColumns.Count - 1);
            }
        }

        private void SaveColumnLayout(DGV settings)
        {
            Debug.Print($"SaveColumnLayout: {settings}");

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

            settings.Groups.AddRange(Data.Groups.Select(e => new Sorting { Id = e.PropertyDescriptor.Name, SortDirection = e.SortDirection }));

            settings.Sorts.AddRange(Data.Sorts.Select(e => new Sorting { Id = e.PropertyDescriptor.Name, SortDirection = e.SortDirection }));

            Data.SortsOfGroups.ForEach(e1 =>
            {
                var list = e1.Select(e2 => new Sorting { Id = e2.PropertyDescriptor.Name, SortDirection = e2.SortDirection }).ToList();
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
            Debug.Print($"SetCellElementStyleAndWidth");

            foreach (var col in DGControl.Columns.OfType<DataGridTextColumn>())
            {
                var p = Properties.OfType<PropertyDescriptor>().FirstOrDefault(p1 => p1.Name == col.SortMemberPath);
                if (p != null)
                {
                    var alignment = Helpers.DataGridHelper.GetDefaultColumnAlignment(p.PropertyType);
                    if (alignment != null)
                    {
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            var styleName = $"CellStyle_{RowViewMode}{alignment}";
                            var style = View.Resources[styleName] as Style;
                            col.ElementStyle = style;
                            if (RowViewMode == Enums.DGRowViewMode.OneRow)
                                DGControl.RowHeight = DGControl.FontSize * 1.5 + 2;
                            else if (!double.IsNaN(DGControl.RowHeight))
                                DGControl.RowHeight = double.NaN;
                        }), DispatcherPriority.ContextIdle);
                    }
                }
            }
        }
    }
}
