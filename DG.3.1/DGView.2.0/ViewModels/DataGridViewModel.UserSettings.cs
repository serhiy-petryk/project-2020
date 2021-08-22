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
            //if (Visible)
              //  Visible = false;

              Data.SetSettings(settings);

//              if (settings.BaseFont != null)
  //                this.Font = settings.BaseFont;
    //          _IsGridVisible = settings.IsGridVisible;

              CellViewMode = settings.CellViewMode;

              RestoreColumnLayout(settings);

              // Fixed bug (RefreshData()):
              // 2. Зміна записаних налаштувань для MastCoA.
              //  - default налаштування з групами
              //  - якщо міняємо налаштування, то збиваються колонки
            if (Data.UnderlyingData.IsDataReady)
            {
                // UI.frmLog.AddEntry("Settings Before RefreshData " + Columns.Cast<DataGridViewColumn>().Count(c => c.Visible));
                Data.RefreshData();
            }

            // Invalidate(); // corrected bug - column header is blank after apply setting with new column
        }

        private void RestoreColumnLayout(DGV settingInfo)
        {
            // _allValidColumnNames.Clear();
            SetGroupColumns();

            // Unfroze columns
            /*for (var i = 0; i < DGControl.Columns.Count; i++)
                if (DGControl.Columns[i].IsFrozen)
                    DGControl.Columns[i].IsFrozen = false;*/

            // Restore Columns order
            for (var i = settingInfo.AllColumns.Count - 1; i >= 0; i--)
            {
                var column = settingInfo.AllColumns[i];
                var k = Helpers.DataGridHelper.GetColumnIndexByPropertyName(DGControl, column.Id);
                if (k >= 0)
                {
                    var col = DGControl.Columns[k];
                    if (!column.IsHidden)
                    {
                        // _allValidColumnNames.Add(col.DataPropertyName);
                        // _allValidColumnNames.Add(col.SortMemberPath);
                        col.DisplayIndex = 0;
                    }

                    //var visible = !column.IsHidden && DataSource.IsPropertyVisible(column.Id); // on Startup DataSource.IsPropertyVisible == false for all columns
                    //if (col.Visible != visible)
                    //col.Visible = visible;
                    /*if (col.Visible == column.IsHidden)
                        col.Visible = !column.IsHidden;*/
                    Helpers.DataGridHelper.SetColumnVisibility(col, !column.IsHidden);


                    if (column.Width.HasValue && column.Width.Value > 0 && CellViewMode != Enums.DGCellViewMode.OneRow)
                        col.Width = column.Width.Value;
                    else
                        col.Width = DataGridLength.Auto;

                }
                else
                {
                    // throw new Exception("Trap!! RestoreColumnLayout");
                }
            }

            var cntFrozen = 0;
            // Image group columns: Restore order and freeze
            //      for (int i = (this._groups.Count - 1); i >= 0; i--) {
            for (var i = 0; i < Data.Groups.Count; i++)
            {
                _groupColumns[i].DisplayIndex = cntFrozen++;
                // _groupColumns[i].Frozen = true;
                //if (_groupColumns[i].Visible != DataSource.IsGroupColumnVisible(i))
                //  _groupColumns[i].Visible = !_groupColumns[i].Visible;
                Helpers.DataGridHelper.SetColumnVisibility(_groupColumns[i], Data.IsGroupColumnVisible(i));
            }
            // Set itemcount group column
            //if (_groupItemCountColumn.Visible != (DataSource.Groups.Count > 0))
              //  _groupItemCountColumn.Visible = !_groupItemCountColumn.Visible;
              Helpers.DataGridHelper.SetColumnVisibility(GroupItemCountColumn, Data.Groups.Count > 0);

            if (Data.Groups.Count > 0)
            {
                GroupItemCountColumn.DisplayIndex = cntFrozen++;
                // _groupItemCountColumn.Frozen = true;
            }

            // Restore order of frozen columns
            foreach (var column in settingInfo.FrozenColumns)
            {
                var k = Helpers.DataGridHelper.GetColumnIndexByPropertyName(DGControl, column);
                //if (k >= 0 && Columns[k].Visible && !Columns[k].Frozen)
//                {
  //                  Columns[k].DisplayIndex = cntFrozen++;
    //                Columns[k].Frozen = true;
      //          }
                if (k >= 0 && DGControl.Columns[k].Visibility == Visibility.Visible) // && !Columns[k].Frozen)
                {
                    DGControl.Columns[k].DisplayIndex = cntFrozen++;
                    // Columns[k].Frozen = true;
                }
            }
            DGControl.FrozenColumnCount = cntFrozen;
        }

        private void SetGroupColumns()
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
                    // AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                    // Name = "#group_" + this._groupColumns.Count,
                    // DefaultCellStyle = { NullValue = null }
                };
                DGControl.Columns.Add(groupColumn);
                _groupColumns.Add(groupColumn);
                /*if (_groupColumns.Count >= _groupPens.Count)
                {
                    // Need add new pen
                    var penNo = (_groupColumns.Count - 1) % (_defaultGroupPens.Length - 1) + 1;
                    _groupPens.Add(_defaultGroupPens[penNo]);
                }*/
            }
            // clear Column header backcolor
            /*foreach (DataGridViewColumn c in this.Columns)
            {
                c.HeaderCell.Style.BackColor = new Color();
            }*/

            // Set visible for group Columns and order for data fgroup columns
            for (int i = 0; i < this._groupColumns.Count; i++)
            {
                if (i < Data.Groups.Count)
                {
                    _groupColumns[i].Visibility = Data.ShowGroupsOfUpperLevels || i >= (Data.ExpandedGroupLevel - 1) ? Visibility.Visible : Visibility.Collapsed;
                    //          DataGridViewColumn dataColumn = this._groupColumns[i].Tag as DataGridViewColumn;
                      //                  if (dataColumn == null || (dataColumn.DataPropertyName != this._groups[i].PropertyDescriptor.Name)) {
                        //                  int k = Utils.DGV.GetColumnIndexByPropertyName(this, this._groups[i].PropertyDescriptor.Name);
                          //                if (k >= 0) {
                            //                dataColumn = this.Columns[k];
                              //              this._groupColumns[i].Tag = dataColumn;
                                //          }
                                  //      }
                    /*foreach (DataGridViewColumn c in this.Columns)
                    {
                        if (c.DataPropertyName == DataSource.Groups[i].PropertyDescriptor.Name)
                        {
                            if (c.HeaderCell.Style.BackColor != this._groupPens[i + 1].Color) c.HeaderCell.Style.BackColor = this._groupPens[i + 1].Color;
                        }
                    }*/
                    //          if (dataColumn != null && dataColumn.HeaderCell.Style.BackColor != this._groupPens[i + 1].Color) dataColumn.HeaderCell.Style.BackColor = this._groupPens[i + 1].Color;
                    /*if (this._groupColumns[i].Width != (this.Font.Height + 7))
                        this._groupColumns[i].Width = this.Font.Height + 7;// difference is from 7(Font=16pt) to 9(Font=9pt) pixels

                    if (this._groupColumns[i].DefaultCellStyle.BackColor != this._groupPens[i + 1].Color)
                        this._groupColumns[i].DefaultCellStyle.BackColor = this._groupPens[i + 1].Color;*/
                }
                else
                {// blank GroupColumn
                    this._groupColumns[i].Visibility = Visibility.Collapsed;
                    // this._groupColumns[i].Tag = null;// Clear dataColumn in tag of groupcolumn
                }
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
                        DisplayName = Data.Properties[c.SortMemberPath].DisplayName,
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
            foreach (var col in DGControl.Columns.OfType<DataGridBoundColumn>())
            {
                var p = Data.Properties.OfType<PropertyDescriptor>().FirstOrDefault(p1 => p1.Name == col.SortMemberPath);
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
