using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DGCore.Helpers;
using DGView.Helpers;
using WpfSpLib.Common;
using WpfSpLib.Controls;
using static DGView.Controls.CustomDataGrid;

namespace DGView.ViewModels
{
    public partial class DGViewModel
    {
        public RelayCommand CmdSetSetting { get; private set; }
        public RelayCommand CmdEditSetting { get; private set; }
        public RelayCommand CmdRowDisplayMode { get; private set; }
        public RelayCommand CmdSetGroupLevel { get; private set; }
        public RelayCommand CmdSetSortAsc { get; private set; }
        public RelayCommand CmdSetSortDesc { get; private set; }
        public RelayCommand CmdClearSortings { get; private set; }
        public RelayCommand CmdSetFilterOnValue { get; private set; }
        public RelayCommand CmdClearFilterOnValue { get; private set; }
        public RelayCommand CmdSearch { get; private set; }
        public RelayCommand CmdClone { get; private set; }
        public RelayCommand CmdRequery { get; private set; }
        public RelayCommand CmdSaveAsExcelFile { get; private set; }
        public RelayCommand CmdSaveAsTextFile { get; private set; }

        private void InitCommands()
        {
            CmdSetSetting = new RelayCommand(cmdSetSetting);
            CmdEditSetting = new RelayCommand(cmdEditSetting);
            CmdRowDisplayMode = new RelayCommand(cmdRowDisplayMode);
            CmdSetGroupLevel = new RelayCommand(cmdSetGroupLevel);

            CmdSetSortAsc = new RelayCommand(cmdSetSortAsc);
            CmdSetSortDesc = new RelayCommand(cmdSetSortDesc);
            CmdClearSortings = new RelayCommand(cmdClearSortings);

            CmdSetFilterOnValue = new RelayCommand(cmdSetFilterOnValue);
            CmdClearFilterOnValue = new RelayCommand(cmdClearFilterOnValue);

            CmdSearch = new RelayCommand(cmdSearch);
            CmdClone = new RelayCommand(cmdClone);
            CmdRequery = new RelayCommand(cmdRequery);

            CmdSaveAsExcelFile = new RelayCommand(cmdSaveAsExcelFile);
            CmdSaveAsTextFile = new RelayCommand(cmdSaveAsTextFile);
        }

        private void cmdSetSetting(object p)
        {
            var newSetting = (string)p;
            DGCore.UserSettings.UserSettingsUtils.SetSetting(this, newSetting);
            LastAppliedLayoutName = newSetting;
        }
        private void cmdEditSetting(object p)
        {
            DialogMessage.ShowDialog($"cmdEditSetting: Not ready!", null, DialogMessage.DialogMessageIcon.Warning, new[] { "OK" });
        }
        private void cmdRowDisplayMode(object p)
        {
            var rowViewMode = (DGCore.Common.Enums.DGRowViewMode)Enum.Parse(typeof(DGCore.Common.Enums.DGRowViewMode), (string)p);
            RowViewMode = rowViewMode;
        }
        private void cmdSetGroupLevel(object p)
        {
            var i = (int?)p;
            Data.A_SetGroupLevel(i.HasValue ? Math.Abs(i.Value) : (int?)null, (i ?? 0) >= 0);
            SetColumnVisibility();
        }
        private void cmdSetSortAsc(object p)
        {
            if (IsSetFilterOnValueOrSortingEnable)
            {
                _lastCurrentCellInfo = DGControl.SelectedCells[0];
                Data.A_ApplySorting(_lastCurrentCellInfo.Column.SortMemberPath, _lastCurrentCellInfo.Item, ListSortDirection.Ascending);
            }
        }
        private void cmdSetSortDesc(object p)
        {
            if (IsSetFilterOnValueOrSortingEnable)
            {
                _lastCurrentCellInfo = DGControl.SelectedCells[0];
                Data.A_ApplySorting(_lastCurrentCellInfo.Column.SortMemberPath, _lastCurrentCellInfo.Item, ListSortDirection.Descending);
            }
        }
        private void cmdClearSortings(object p)
        {
            if (DGControl.SelectedCells.Count != 1) return;
            var cell = DGControl.SelectedCells[0];
            if (!cell.IsValid || cell.Item == null || string.IsNullOrEmpty(cell.Column.SortMemberPath)) return;

            _lastCurrentCellInfo = cell;
            Data.A_RemoveSorting(cell.Column.SortMemberPath, cell.Item);
        }
        private void cmdSetFilterOnValue(object p)
        {
            if (IsSetFilterOnValueOrSortingEnable)
            {
                _lastCurrentCellInfo = DGControl.SelectedCells[0];
                var pd = Data.Properties[_lastCurrentCellInfo.Column.SortMemberPath];
                var value = pd.GetValue(_lastCurrentCellInfo.Item);
                Data.A_SetByValueFilter(_lastCurrentCellInfo.Column.SortMemberPath, value);
            }
            OnPropertiesChanged(nameof(IsClearFilterOnValueEnable));
        }
        private void cmdClearFilterOnValue(object p)
        {
            if (DGControl.SelectedCells.Count == 1)
                _lastCurrentCellInfo = DGControl.SelectedCells[0];
            Data.A_ClearByValueFilter();
            OnPropertiesChanged(nameof(IsClearFilterOnValueEnable));
        }
        private void cmdSearch(object p)
        {
            var focusedControl = Keyboard.FocusedElement;
        }
        private void cmdClone(object p)
        {
            var mwiChild = Tips.GetVisualParents(DGControl).OfType<MwiChild>().FirstOrDefault();
            if (mwiChild == null) return;

            var dgView = CreateDataGrid(mwiChild.MwiContainer, mwiChild.Title);
            var settings = GetSettings();
            dgView.ViewModel.Bind(Data.UnderlyingData, LayoutId, StartUpParameters, LastAppliedLayoutName, settings);
        }
        private void cmdRequery(object p)
        {
            Data.RequeryData();
        }

        private void cmdSaveAsExcelFile(object p)
        {
            DataGridHelper.GetSelectedArea(DGControl, out var objectsToSave, out var columns);

            // var properties = new List<PropertyDescriptorForDataGridColumn>();
            var columnDescriptions = new List<DataGridColumnDescription>();
            foreach (var column in columns)
            {
                if (!string.IsNullOrEmpty(column.SortMemberPath))
                    columnDescriptions.Add(new DataGridColumnDescription(Properties[column.SortMemberPath]));
                else if (column.HeaderStringFormat.StartsWith("Group_")) { }
                else if (column.HeaderStringFormat == "GroupItemCountColumn")
                    columnDescriptions.Add(new DataGridColumnDescription((string)Application.Current.Resources["Loc:DGV.GroupItemCountColumnHeader"]));
                else
                    throw new Exception("Trap!!!");
            }

            var filename = $"DGV_{LayoutId}.{DGCore.Utils.ExcelApp.GetDefaultExtension()}";

            var title = DGControl.GetVisualParents().OfType<MwiChild>().First().Title;
            var groupColumnNames = Data.Groups.Select(g=> g.PropertyDescriptor.Name).ToList();
            var groupColors = new int[_groupColumns.Count + 1];
            for (var k = 0; k <= _groupColumns.Count; k++)
            {
                var c = _groupBrushes[k == 0 ? 0 : ((k - 1) % (_groupBrushes.Length - 1)) + 1].Color;
                groupColors[k] = DGCore.Utils.ExcelApp.GetExcelColor(c.R, c.G, c.B);
            }
            SaveData.SaveAndOpenDataToXlsFile(filename, title, GetSubheaders_ExcelAndPrint(), objectsToSave, columnDescriptions.ToArray(), groupColumnNames, groupColors);
        }

        private string[] GetSubheaders_ExcelAndPrint()
        {
            List<string> subHeaders = new List<string>();
            if (!string.IsNullOrEmpty(LastAppliedLayoutName)) subHeaders.Add("Останнє налаштування: " + LastAppliedLayoutName);
            if (!string.IsNullOrEmpty(StartUpParameters)) subHeaders.Add("Початкові параметри: " + StartUpParameters);
            var s1 = Data.WhereFilter.StringPresentation;
            if (!string.IsNullOrEmpty(s1)) subHeaders.Add("Фільтр даних: " + s1);
            if (Data.FilterByValue != null)
            {
                s1 = Data.FilterByValue.StringPresentation;
                if (!string.IsNullOrEmpty(s1)) subHeaders.Add("Фільтр по виразу клітинки: " + s1);
            }
            s1 = Data.TextFastFilter;
            if (!string.IsNullOrEmpty(s1)) subHeaders.Add("Текст швидкого фільтру: " + s1);
            return subHeaders.ToArray();
        }

        private void cmdSaveAsTextFile(object p)
        {
            DataGridHelper.GetSelectedArea(DGControl, out var objectsToSave, out var columns);

            var properties = new PropertyDescriptor[columns.Length];
            for (var k = 0; k < columns.Length; k++)
            {
                var column = columns[k];
                if (!string.IsNullOrEmpty(column.SortMemberPath))
                    properties[k] = Properties[column.SortMemberPath];
                else if (column.HeaderStringFormat.StartsWith("Group_"))
                    properties[k] = new DGCore.Helpers.PropertyDescriptorForDataGridGroupColumn(int.Parse(column.HeaderStringFormat.Substring(6)));
                else if (column.HeaderStringFormat == "GroupItemCountColumn")
                    properties[k] = new DGCore.Helpers.PropertyDescriptorForDataGridGroupColumn(
                        (string)Application.Current.Resources["Loc:DGV.GroupItemCountColumnHeader"]);
                else
                    throw new Exception("Trap!!!");
            }

            var filename = $"DGV_{LayoutId}.txt";
            DGCore.Helpers.SaveData.SaveAndOpenDataToTextFile(filename, objectsToSave, properties);
        }
    }
}
