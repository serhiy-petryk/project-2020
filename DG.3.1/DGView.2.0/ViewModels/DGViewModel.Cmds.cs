﻿using DGView.Views;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using DGCore.Common;
using WpfSpLib.Common;
using WpfSpLib.Controls;

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
            DGControl.FontSize += 2;
            if (RowViewMode == Enums.DGRowViewMode.OneRow)
                DGControl.RowHeight = DGControl.FontSize * 1.5 + 2;

            var cellInfo = DGControl.CurrentCell;
            foreach (var a1 in Data.Sorts)
            {
                Debug.Print($"Sort: {a1.PropertyDescriptor.Name}, {a1.SortDirection}");
            }
            for (var k = 0; k < Data.SortsOfGroups.Count; k++)
            {
                var group = Data.SortsOfGroups[k];
                foreach (var a1 in group)
                {
                    Debug.Print($"SortOfGroup: {k}, {a1.PropertyDescriptor.Name}, {a1.SortDirection}");
                }
            }
            /*var cellContent = cellInfo.Column.GetCellContent(cellInfo.Item);
            if (cellContent != null)
            {
                var cell = (DataGridCell)cellContent.Parent;
                cell.Tag = "1";
                var row = DataGridRow.GetRowContainingElement(cell);
                Debug.Print($"DG.CurrentCell: {row.GetIndex()}, {cell.Column.DisplayIndex}");
            }
            else
            {
                Debug.Print($"DG.CurrentCell: null");
            }*/
        }
        private void cmdClone(object p)
        {
            var mwiChild = Tips.GetVisualParents(DGControl).OfType<MwiChild>().FirstOrDefault();
            if (mwiChild == null) return;

            var dgView = new DataGridView();
            var child = new MwiChild
            {
                Title = mwiChild.Title,
                Content = dgView,
                Height = Math.Max(200.0, Window.GetWindow(mwiChild.MwiContainer).ActualHeight * 2 / 3)
            };
            var b = new Binding { Path = new PropertyPath("ActualThemeColor"), Source = mwiChild.MwiContainer, Converter = ColorHslBrush.Instance, ConverterParameter = "+45%:+0%:+0%" };
            child.SetBinding(MwiChild.ThemeColorProperty, b);

            mwiChild.MwiContainer.Children.Add(child);
            var settings = GetSettings();
            dgView.ViewModel.Bind(Data.UnderlyingData, LayoutId, StartUpParameters, LastAppliedLayoutName, settings);
        }
        private void cmdRequery(object p)
        {
            Data.RequeryData();
        }
    }
}
