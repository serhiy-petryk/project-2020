﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using WpfSpLib.Common.ColorSpaces;
using WpfSpLib.Helpers;
using WpfSpLibDemo.Samples;

namespace WpfSpLibDemo.TestViews
{
    /// <summary>
    /// Interaction logic for DataGridTests.xaml
    /// </summary>
    public partial class DataGridTests : Window
    {
        #region ============== Properties/Events  ===================
        public static readonly DependencyProperty BaseHslProperty = DependencyProperty.Register("BaseHsl",
            typeof(HSL_Observable), typeof(DataGridTests), new FrameworkPropertyMetadata(null));
        public HSL_Observable BaseHsl
        {
            get => (HSL_Observable)GetValue(BaseHslProperty);
            set => SetValue(BaseHslProperty, value);
        }
        #endregion

        // Every datagrid must to have a separate items source collection, but items in collection may be the same
        public IList<AuthorIDataErrorInfo> Data1 { get; } = AuthorIDataErrorInfo.Authors;
        public IList<AuthorIDataErrorInfo> Data2 { get; } = AuthorIDataErrorInfo.Authors;
        public IList<AuthorIDataErrorInfo> Data3 { get; } = AuthorIDataErrorInfo.Authors;
        public IList<AuthorIDataErrorInfo> Data4 { get; } = AuthorIDataErrorInfo.Authors;
        public IList<AuthorIDataErrorInfo> Data5 { get; } = AuthorIDataErrorInfo.Authors;

        public Author.Level[] EnumList { get; } = Enum.GetValues<Author.Level>();

        public DataGridTests()
        {
            InitializeComponent();
            DataContext = this;
            var xhsl = new HSL(new RGB(0xB0 / 256.0, 0xC4 / 256.0, 0xDE / 256.0));//FFB0C4DE
            var hsl = new HSL(new RGB(0x78 / 256.0, 0xBC / 256.0, 0xFF / 256.0));//FF78BCFF
            BaseHsl = new HSL_Observable() { Hue = hsl.Hue, Saturation = hsl.Saturation, Lightness = hsl.Lightness };
        }

        private void DataGrid_OnThreeStateSorting(object sender, DataGridSortingEventArgs e) =>
            DataGridHelper.DataGrid_OnSorting((DataGrid)sender, e);

        private void ChangeHsl_OnClick(object sender, RoutedEventArgs e)
        {
            var hsl = BaseHsl;
            var a = (hsl.Hue + 30.0) % 360;
            hsl.Hue = a;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            // var currentCellInfo = DataGrid_Editable.CurrentCell;
            // var row = DataGrid_Editable.ItemContainerGenerator.ContainerFromItem(currentCellInfo.Item) as DataGridRow;
            // var cell = DataGridHelper.GetDataGridCell(DataGrid_Editable, row, currentCellInfo.Column);

            var dgCell = DataGridHelper.GetDataGridCell(DataGrid_InlineEditable.CurrentCell);
            var aa1 = dgCell.GetVisualChildren<DependencyObject>().ToArray();
        }

        private void DataGridTests_OnClosing(object sender, CancelEventArgs e) => DataGridHelper.Control_OnClosing(this);

        private void DataGrid_OnLoadingRow(object sender, DataGridRowEventArgs e)
        {
            string rowHeaderText;
            if (e.Row.IsNewItem)
                rowHeaderText = "*"; // ((char)9654).ToString() + "★✶";
            else
                rowHeaderText = (e.Row.GetIndex() + 1).ToString("N0", LocalizationHelper.CurrentCulture);

            if (!Equals(e.Row.Header, rowHeaderText)) e.Row.Header = rowHeaderText;
        }

        private void DataGrid_Editable_OnCurrentCellChanged(object sender, EventArgs e)
        {
            var dg = (DataGrid)sender;
            var item = dg.CurrentCell.Item;

            if (dg.CurrentCell.IsValid && !dg.CurrentCell.Column.IsReadOnly && item != null &&
                item.GetType().Name != "NamedObject")
            {
                dg.Dispatcher.BeginInvoke(() =>
                {
                    var cell = DataGridHelper.GetDataGridCell(dg.CurrentCell);
                    cell.SimulateMouseClick();
                }, DispatcherPriority.Normal);
            }
        }

        private void DataGrid_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key is not (Key.Left or Key.Up or Key.Right or Key.Down or Key.Home or Key.End or Key.PageUp
                or Key.PageDown or Key.Enter or Key.F2)) return;

            var dg = (DataGrid)sender;
            var cell = DataGridHelper.GetDataGridCell(dg.CurrentCell);
            if (dg.IsReadOnly || !cell.IsEditing ||
                Keyboard.Modifiers is not (ModifierKeys.None or ModifierKeys.Shift or ModifierKeys.Control)) return;

            var element = Keyboard.FocusedElement;
            if (element is TextBox textBox)
            {
                if (e.Key is Key.Up or Key.PageUp)
                {
                    var lineIndex = textBox.GetLineIndexFromCharacterIndex(textBox.CaretIndex);
                    if (lineIndex == 0 || textBox.SelectionLength == textBox.Text.Length)
                        dg.CommitEdit();
                }
                else if (e.Key is Key.Down or Key.PageDown or Key.Enter)
                {
                    var lineIndex = textBox.GetLineIndexFromCharacterIndex(textBox.CaretIndex);
                    if (lineIndex >= textBox.LineCount - 1 || textBox.SelectionLength == textBox.Text.Length)
                        dg.CommitEdit();
                }
                else if (e.Key is Key.F2)
                {
                    if (textBox.SelectionStart == 0 && textBox.SelectionLength == textBox.Text.Length)
                    {
                        // Unselect text
                        textBox.SelectionLength = 0;
                        textBox.SelectionStart = textBox.Text.Length;
                    }
                    else
                    {
                        // Select text
                        textBox.SelectionStart = 0;
                        textBox.SelectionLength = textBox.Text.Length;
                    }
                }
                else if (e.Key is Key.Left or Key.Home)
                {
                    if (textBox.CaretIndex == 0)
                        dg.CommitEdit();
                }
                else if (e.Key is Key.Right or Key.End)
                {
                    if (textBox.CaretIndex == textBox.Text.Length || (textBox.SelectionStart == 0 && textBox.SelectionLength == textBox.Text.Length))
                        dg.CommitEdit();
                }
            }
            else if (element is ComboBox comboBox)
            {
                if (Keyboard.Modifiers == ModifierKeys.None && e.Key != Key.F2 && !comboBox.IsDropDownOpen)
                    dg.CommitEdit();
            }
            else {}
        }
    }
}
