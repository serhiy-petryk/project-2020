using System.Collections.Generic;
using System.Windows.Controls;
using DGCore.Common;
using DGCore.UserSettings;

namespace DGView.ViewModels
{
    public partial class DataGridViewModel
    {
        private List<string> _allValidColumnNames = new List<string>();

        DataGridColumn _groupItemCountColumn = null;

        //=========================
        internal const string UserSettingsKind = "DGV_Setting";
        string IUserSettingProperties.SettingKind => UserSettingsKind;
        private string LayoutId { get; set; }
        string IUserSettingProperties.SettingKey => LayoutId;

        /*DGCore.UserSettings.DGV DGCore.UserSettings.IUserSettingSupport<DGCore.UserSettings.DGV>.GetSettings()
        {
            DGCore.Utils.Dgv.EndEdit(this);
            var o = new DGCore.UserSettings.DGV
            {
                WhereFilter = ((DGCore.UserSettings.IUserSettingSupport<List<DGCore.UserSettings.Filter>>)DataSource.WhereFilter)?.GetSettings(),
                FilterByValue = ((DGCore.UserSettings.IUserSettingSupport<List<DGCore.UserSettings.Filter>>)DataSource.FilterByValue)?.GetSettings(),
                ShowTotalRow = DataSource.ShowTotalRow,
                ExpandedGroupLevel = DataSource.ExpandedGroupLevel,
                ShowGroupsOfUpperLevels = DataSource.ShowGroupsOfUpperLevels,
                BaseFont = this.Font,
                IsGridVisible = this._IsGridVisible,
                CellViewMode = this._CellViewMode,
                TextFastFilter = DataSource.TextFastFilter
            };
            ApplyColumnLayout(o);
            return o;
        }

        DGCore.UserSettings.DGV DGCore.UserSettings.IUserSettingSupport<DGCore.UserSettings.DGV>.GetBlankSetting()
        {
            DGCore.Utils.Dgv.EndEdit(this);
            DataSource.ResetSettings();
            Font = _startupFont;
            CellBorderStyle = DataGridViewCellBorderStyle.Single; // For _IsGridVisible
            _CellViewMode = DGCore.Common.Enums.DGCellViewMode.OneRow;

            // For AllColumns
            _allValidColumnNames = Columns.Cast<DataGridViewColumn>()
              .Where(col => !string.IsNullOrEmpty(col.DataPropertyName) && !col.DataPropertyName.Contains('.'))
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

            if (settings.BaseFont != null)
                this.Font = settings.BaseFont;
            _IsGridVisible = settings.IsGridVisible;

            _CellViewMode = settings.CellViewMode;

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

        private void ApplyColumnLayout(DGCore.UserSettings.DGV settings)
        {
            var cols = DGCore.Utils.Dgv.GetColumnsInDisplayOrder(this, false);

            // Set columns for default settings
            foreach (var c in cols)
                if (!string.IsNullOrEmpty(c.DataPropertyName))
                {
                    settings.AllColumns.Add(new DGCore.UserSettings.Column
                    {
                        Id = c.DataPropertyName,
                        DisplayName = DataSource.Properties[c.DataPropertyName].DisplayName,
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
                var k = DGCore.Utils.Dgv.GetColumnIndexByPropertyName(this, column.Id);
                if (k >= 0)
                {
                    var col = Columns[k];
                    if (!column.IsHidden)
                    {
                        _allValidColumnNames.Add(col.DataPropertyName);
                        col.DisplayIndex = 0;
                    }

                    //var visible = !column.IsHidden && DataSource.IsPropertyVisible(column.Id); // on Startup DataSource.IsPropertyVisible == false for all columns
                    //if (col.Visible != visible)
                      //col.Visible = visible;
                    if (col.Visible == column.IsHidden)
                        col.Visible = !column.IsHidden;
                    if (column.Width.HasValue && column.Width.Value > 0)
                        col.Width = column.Width.Value;
                }
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
                var k = DGCore.Utils.Dgv.GetColumnIndexByPropertyName(this, column);
                if (k >= 0 && Columns[k].Visible && !Columns[k].Frozen)
                {
                    Columns[k].DisplayIndex = cntFrozen++;
                    Columns[k].Frozen = true;
                }
            }
        }*/

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
            // ApplyColumnLayout(o);
            return o;
        }

        public DGV GetBlankSetting()
        {
            // Utils.Dgv.EndEdit(this);
            /*DataSource.ResetSettings();
            Font = _startupFont;
            CellBorderStyle = DataGridViewCellBorderStyle.Single; // For _IsGridVisible
            _CellViewMode = Enums.DGCellViewMode.OneRow;

            // For AllColumns
            _allValidColumnNames = Columns.Cast<DataGridViewColumn>()
                .Where(col => !string.IsNullOrEmpty(col.DataPropertyName) && !col.DataPropertyName.Contains('.'))
                .Select(col => col.DataPropertyName).ToList();

            ResizeColumnWidth(); // !!! Before SaveColumnInfo*/
            return ((IUserSettingSupport<DGV>) this).GetSettings();
        }

        public void SetSetting(DGV settings)
        {
            Data.SetSettings(settings);

            RestoreColumnLayout(settings);

            if (Data.UnderlyingData.IsDataReady)
            {
                // UI.frmLog.AddEntry("Settings Before RefreshData " + Columns.Cast<DataGridViewColumn>().Count(c => c.Visible));
                Data.RefreshData();
            }

            /*
            if (Visible)
                Visible = false;

            DataSource.SetSettings(settings);

            if (settings.BaseFont != null)
                this.Font = settings.BaseFont;
            _IsGridVisible = settings.IsGridVisible;

            _CellViewMode = settings.CellViewMode;

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

            // Invalidate(); // corrected bug - column header is blank after apply setting with new column*/
        }

        private void RestoreColumnLayout(DGCore.UserSettings.DGV settingInfo)
        {
            _allValidColumnNames.Clear();
            _SetGroupColumns();

            // Unfroze columns
            /*for (var i = 0; i < Columns.Count; i++)
                if (Columns[i].Frozen)
                    Columns[i].Frozen = false;

            // Restore Columns order
            for (var i = (settingInfo.AllColumns.Count - 1); i >= 0; i--)
            {
                var column = settingInfo.AllColumns[i];
                var k = DGCore.Utils.Dgv.GetColumnIndexByPropertyName(this, column.Id);
                if (k >= 0)
                {
                    var col = Columns[k];
                    if (!column.IsHidden)
                    {
                        _allValidColumnNames.Add(col.DataPropertyName);
                        col.DisplayIndex = 0;
                    }

                    //var visible = !column.IsHidden && DataSource.IsPropertyVisible(column.Id); // on Startup DataSource.IsPropertyVisible == false for all columns
                    //if (col.Visible != visible)
                    //col.Visible = visible;
                    if (col.Visible == column.IsHidden)
                        col.Visible = !column.IsHidden;
                    if (column.Width.HasValue && column.Width.Value > 0)
                        col.Width = column.Width.Value;
                }
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
                var k = DGCore.Utils.Dgv.GetColumnIndexByPropertyName(this, column);
                if (k >= 0 && Columns[k].Visible && !Columns[k].Frozen)
                {
                    Columns[k].DisplayIndex = cntFrozen++;
                    Columns[k].Frozen = true;
                }
            }*/
        }

        private void _SetGroupColumns()
        {
            if (_groupItemCountColumn == null)
            {
                _groupItemCountColumn = new DataGridTextColumn
                {
                    /*Name = "#group_ItemCount",
                    Header = @"К-сть елементів",
                    IsReadOnly = true,
                    // Resizable = DataGridViewTriState.True,
                    CanUserResize = true,
                    DefaultCellStyle = { NullValue = null, Alignment = DataGridViewContentAlignment.MiddleCenter, Format = "N0" },
                    CellTemplate = { Style = { Alignment = DataGridViewContentAlignment.MiddleCenter } },
                    ValueType = typeof(int),
                    SortMode = DataGridViewColumnSortMode.NotSortable*/
                };
                // Columns.Add(_groupItemCountColumn);
            }
            // Create new group columns if neccessary
            /*while (DataSource.Groups.Count > _groupColumns.Count)
            {
                var groupColumn = new DataGridViewTextBoxColumn
                {
                    ReadOnly = true,
                    HeaderText = (_groupColumns.Count + 1).ToString(),
                    Resizable = DataGridViewTriState.False,
                    SortMode = DataGridViewColumnSortMode.NotSortable,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                    Name = "#group_" + this._groupColumns.Count,
                    DefaultCellStyle = { NullValue = null }
                };
                Columns.Add(groupColumn);
                _groupColumns.Add(groupColumn);
                if (_groupColumns.Count >= _groupPens.Count)
                {
                    // Need add new pen
                    var penNo = (_groupColumns.Count - 1) % (_defaultGroupPens.Length - 1) + 1;
                    _groupPens.Add(_defaultGroupPens[penNo]);
                }
            }
            // clear Column header backcolor
            foreach (DataGridViewColumn c in this.Columns)
            {
                c.HeaderCell.Style.BackColor = new Color();
            }

            // Set visible for group Columns and order for data fgroup columns
            for (int i = 0; i < this._groupColumns.Count; i++)
            {
                if (i < DataSource.Groups.Count)
                {
                    this._groupColumns[i].Visible = DataSource.ShowGroupsOfUpperLevels || i >= (DataSource.ExpandedGroupLevel - 1);
                    //          DataGridViewColumn dataColumn = this._groupColumns[i].Tag as DataGridViewColumn;
                      //                  if (dataColumn == null || (dataColumn.DataPropertyName != this._groups[i].PropertyDescriptor.Name)) {
                        //                  int k = Utils.DGV.GetColumnIndexByPropertyName(this, this._groups[i].PropertyDescriptor.Name);
                          //                if (k >= 0) {
                            //                dataColumn = this.Columns[k];
                              //              this._groupColumns[i].Tag = dataColumn;
                                //          }
                                  //      }
                    foreach (DataGridViewColumn c in this.Columns)
                    {
                        if (c.DataPropertyName == DataSource.Groups[i].PropertyDescriptor.Name)
                        {
                            if (c.HeaderCell.Style.BackColor != this._groupPens[i + 1].Color) c.HeaderCell.Style.BackColor = this._groupPens[i + 1].Color;
                        }
                    }
                    //          if (dataColumn != null && dataColumn.HeaderCell.Style.BackColor != this._groupPens[i + 1].Color) dataColumn.HeaderCell.Style.BackColor = this._groupPens[i + 1].Color;
                    if (this._groupColumns[i].Width != (this.Font.Height + 7))
                        this._groupColumns[i].Width = this.Font.Height + 7;// difference is from 7(Font=16pt) to 9(Font=9pt) pixels

                    if (this._groupColumns[i].DefaultCellStyle.BackColor != this._groupPens[i + 1].Color)
                        this._groupColumns[i].DefaultCellStyle.BackColor = this._groupPens[i + 1].Color;
                }
                else
                {// blank GroupColumn
                    this._groupColumns[i].Visible = false;
                    // this._groupColumns[i].Tag = null;// Clear dataColumn in tag of groupcolumn
                }
            }*/

        }
    }
}
