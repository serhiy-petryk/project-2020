using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using DGCore.Helpers;
using DGView.Helpers;
using DGView.ViewModels;
using WpfSpLib.Controls;
using WpfSpLib.Effects;
using WpfSpLib.Helpers;

namespace DGView.Views
{
    /// <summary>
    /// Interaction logic for FindAndReplace.xaml
    /// </summary>
    public partial class FindTextView : UserControl, INotifyPropertyChanged
    {
        private static readonly CommandBinding _closeCommand = new CommandBinding(ApplicationCommands.Close, (s, e1) => ((FindTextView)s).Hide());

        public bool IsFindTextButtonEnabled => (FindWhat.Text ?? "") != "";

        private MwiChild _host;
        private DGViewModel _viewModel;
        public FindTextView(MwiChild host, DGViewModel viewModel)
        {
            InitializeComponent();
            DataContext = this;
            _host = host;
            _viewModel = viewModel;
            CommandBindings.Add(_closeCommand);
        }

        public void ToggleVisibility()
        {
            if (Parent == null)
            {
                var host = _host.GetInternalHost();
                var control = new ResizableControl
                {
                    Content = this,
                    LimitPositionToPanelBounds = true,
                    Resizable = false,
                    Focusable = false,
                    Visibility = Visibility.Hidden
                    // Opacity = 1,
                    // Background = Brushes.GreenYellow
                };
                CornerRadiusEffect.SetCornerRadius(control, CornerRadiusEffect.GetCornerRadius(this));
                control.CommandBindings.Add(new CommandBinding(ApplicationCommands.Close, (s, e1) => host.Children.Remove(control)));
                host.Children.Add(control);

                control.Dispatcher.BeginInvoke(new Action(Show), DispatcherPriority.Background);
            }
            else if (((FrameworkElement)Parent).Opacity < 0.1)
                Show();
            else
                Hide();
        }

        private async void Show()
        {
            // Background = new SolidColorBrush(WpfSpLib.Common.ColorSpaces.ColorUtils.GetOutlineColor(_host.ActualThemeColor));
            Background = new SolidColorBrush(_host.ActualThemeColor);
            var host = _host.GetInternalHost();
            var parent = (FrameworkElement)Parent;
            var left = Math.Max(0.0, (host.ActualWidth - parent.ActualWidth) / 2.0);
            var top = Math.Max(0.0, (host.ActualHeight - parent.ActualHeight) / 2.0);
            parent.Margin = new Thickness(left, top, 0, 0);
            parent.Visibility = Visibility.Visible;

            await Task.WhenAll(AnimationHelper.GetContentAnimations(parent, true));
            ControlHelper.SetFocus(this);
        }
        private async void Hide()
        {
            var parent = (FrameworkElement)Parent;
            await Task.WhenAll(AnimationHelper.GetContentAnimations(parent, false));
            _host.Focus();
        }

        public void Dispose()
        {
            _host = null;
            CommandBindings.Clear();
        }

        //        DataGridViewCell _lastFindCell = null;
        DataGridCellInfo _lastFindCell = new DataGridCellInfo();
        private void OnFindButtonClick(object sender, RoutedEventArgs e)
        {
            var _findCell = new DataGridCellInfo();
            if (_lastFindCell != _viewModel.DGControl.CurrentCell)
                _lastFindCell = new DataGridCellInfo();

            if (FindInAllTable.IsChecked ?? false)
            {
                var columns = _viewModel.DGControl.Columns.Where(c => c.Visibility == Visibility.Visible).OrderBy(c => c.DisplayIndex).ToArray();
                _findCell = sp_FindInTableOrColumn(columns);
                if (_findCell.IsValid)
                {
                    // var cell = DGHelper.GetDataGridCell(_findCell);
                    _viewModel.DGControl.SelectedCells.Clear();
                    _viewModel.DGControl.SelectedCells.Add(_findCell);
                    _viewModel.DGControl.ScrollIntoView(_findCell.Item, _findCell.Column);
                }
                else
                {
                    MessageBox.Show("Текст більше не знайдено");
                    _lastFindCell = new DataGridCellInfo();
                }
            }
            else if (FindInColumn.IsChecked ?? false)
            {
                _findCell = sp_FindInTableOrColumn(new[] { DGHelper.GetActiveCellInfo(_viewModel.DGControl).Column });
                if (_findCell.IsValid)
                {
                    // var cell = DGHelper.GetDataGridCell(_findCell);
                    _viewModel.DGControl.SelectedCells.Clear();
                    _viewModel.DGControl.SelectedCells.Add(_findCell);
                    _viewModel.DGControl.ScrollIntoView(_findCell.Item, _findCell.Column);
                }
                else
                {
                    MessageBox.Show("Текст більше не знайдено");
                    _lastFindCell = new DataGridCellInfo();
                }
            }
            else if (FindInSelection.IsChecked ?? false)
            {
                _findCell = sp_FindInSelection();
                if (_findCell.IsValid)
                {
                    // var cell = DGHelper.GetDataGridCell(_findCell);
                    _viewModel.DGControl.SelectedCells.Clear();
                    _viewModel.DGControl.SelectedCells.Add(_findCell);
                    _viewModel.DGControl.ScrollIntoView(_findCell.Item, _findCell.Column);
                }
                else
                {
                    MessageBox.Show("Текст більше не знайдено");
                    _lastFindCell = new DataGridCellInfo();
                }
            }

            /*DataGridViewCell FindCell = null;
            if (_lastFindCell != this._dgv.CurrentCell)
            {
                _lastFindCell = null;
            }
            if (this.rbAllTable1.Checked)
            {
                FindCell = sp_FindInTable();
                //        if (FindCell != null) _dgv.CurrentCell = FindCell;
                if (FindCell != null)
                {
                    if (FindCell == _lastFindCell)
                    {
                        MessageBox.Show("Текст більше не знайдено");
                    }
                    else
                    {
                        DGVUtils.SetNewCurrentCell(_dgv, FindCell);
                        _lastFindCell = FindCell;
                    }
                }
            }
            else if (this.rbSelection1.Checked)
            {
                FindCell = sp_FindInSelection();
                if (FindCell != null)
                {
                    DataGridViewSelectedCellCollection selectedCells = this._dgv.SelectedCells;
                    DGVUtils.SetNewCurrentCell(_dgv, FindCell);
                    //          _dgv.CurrentCell = FindCell;
                    foreach (DataGridViewCell cell in selectedCells) cell.Selected = true;
                }
            }
            else if (this.rbActiveColumn1.Checked)
            {
                FindCell = sp_FindInColumn();
                //        if (FindCell != null) _dgv.CurrentCell = FindCell;
                if (FindCell != null) DGVUtils.SetNewCurrentCell(_dgv, FindCell);
            }
            if (FindCell == null)
            {
                MessageBox.Show("Текст не знайдено");
            }*/
        }

        DataGridCellInfo sp_FindInSelection()
        {
            // Search criteria
            var findWhat = FindWhat.Text;
            var matchCase = MatchCase.IsChecked ?? false;
            var matchCell = MatchCell.IsChecked ?? false;
            var findUp = FindUp.IsChecked ?? false;
            var searchMethod = (Use.IsChecked ?? false) ? cbUse.SelectedIndex : -1;

            var properties = new List<PropertyDescriptor>();
            /*var helpers = DGHelper.GetColumnHelpers(columns, _viewModel.Properties, properties).Where(h =>
                !(h.NotNullableValueType == typeof(byte[]) || h.NotNullableValueType == typeof(bool))).ToArray();
            var getters = properties.Select(p => new DGCellValueFormatter(p).StringForFindTextGetter).ToArray();
            var items = _viewModel.DGControl.Items;*/
            return new DataGridCellInfo();
        }

        DataGridCellInfo sp_FindInTableOrColumn(DataGridColumn[] columns)
        {
            // Search criteria
            var findWhat = FindWhat.Text;
            var matchCase = MatchCase.IsChecked ?? false;
            var matchCell = MatchCell.IsChecked ?? false;
            var findUp = FindUp.IsChecked ?? false;
            var searchMethod = (Use.IsChecked ?? false) ? cbUse.SelectedIndex : -1;

            var properties = new List<PropertyDescriptor>();
            var helpers = DGHelper.GetColumnHelpers(columns, _viewModel.Properties, properties).Where(h => !(h.NotNullableValueType == typeof(byte[]) || h.NotNullableValueType == typeof(bool))).ToArray();
            var getters = properties.Select(p => new DGCellValueFormatter(p).StringForFindTextGetter).ToArray();
            var items = _viewModel.DGControl.Items;

            if (findUp)
            {
                var startItemIndex = items.Count - 1;
                var startColumn = helpers.Length - 1;
                var activeCell = DGHelper.GetActiveCellInfo(_viewModel.DGControl);
                if (activeCell.IsValid)
                {
                    startItemIndex = items.IndexOf(activeCell.Item);
                    var startHelper = helpers.Select((h, index) => new { Item = h, Index = index }).FirstOrDefault(h => h.Item.ColumnDisplayIndex < activeCell.Column.DisplayIndex);
                    if (startHelper != null)
                        startColumn = startHelper.Index;
                    else
                        startItemIndex--;
                }

                for (var rowIndex = startItemIndex; rowIndex >= 0; rowIndex--)
                {
                    var firstColumn = rowIndex == startItemIndex ? startColumn : helpers.Length - 1;
                    var item = items[rowIndex];
                    for (var columnIndex = firstColumn; columnIndex >= 0; columnIndex--)
                    {
                        if (sp_FindBase(getters[columnIndex](item), findWhat, matchCase, matchCell, searchMethod))
                            return new DataGridCellInfo(item, columns.First(c => c.DisplayIndex == helpers[columnIndex].ColumnDisplayIndex));
                    }
                }
            }
            else
            {
                var startItemIndex = 0;
                var startColumn = 0;
                var activeCell = DGHelper.GetActiveCellInfo(_viewModel.DGControl);
                if (activeCell.IsValid)
                {
                    startItemIndex = items.IndexOf(activeCell.Item);
                    var startHelper = helpers.Select((h, index) => new { Item = h, Index = index }).FirstOrDefault(h => h.Item.ColumnDisplayIndex > activeCell.Column.DisplayIndex);
                    if (startHelper != null)
                        startColumn = startHelper.Index;
                    else
                        startItemIndex++;
                }

                for (var rowIndex = startItemIndex; rowIndex < items.Count; rowIndex++)
                {
                    var firstColumn = rowIndex == startItemIndex ? startColumn : 0;
                    var item = items[rowIndex];
                    for (var columnIndex = firstColumn; columnIndex < helpers.Length; columnIndex++)
                    {
                        if (sp_FindBase(getters[columnIndex](item), findWhat, matchCase, matchCell, searchMethod))
                            return new DataGridCellInfo(item, columns.First(c => c.DisplayIndex == helpers[columnIndex].ColumnDisplayIndex));
                    }
                }
            }
            return new DataGridCellInfo();

            /*if (_dgv.CurrentCell == null) return null;

            // Search criterions
            String sFindWhat = this.FindWhatTextBox1.Text;
            bool bMatchCase = this.MatchCaseCheckBox1.Checked;
            bool bMatchCell = this.MatchCellCheckBox1.Checked;
            bool bSearchUp = this.SearchUpCheckBox1.Checked;
            int iSearchMethod = -1; // No regular repression or wildcard
            if (this.UseCheckBox1.Checked)
            {
                iSearchMethod = this.UseComboBox1.SelectedIndex;
            }

            DataGridViewColumn[] cols = DGVUtils.GetColumnsInDisplayOrder(this._dgv, true);
            Utils.DGVColumnHelper[] colHelpers = new Utils.DGVColumnHelper[cols.Length];
            for (int i = 0; i < cols.Length; i++) colHelpers[i] = new Utils.DGVColumnHelper(cols[i]);

            // Start of search            
            int iSearchStartRow = _dgv.CurrentCell.RowIndex;
            int iLastRowNumber = _dgv.Rows.Count;
            if (iLastRowNumber > 0 && _dgv.Rows[iLastRowNumber - 1].IsNewRow) iLastRowNumber--;
            if (iLastRowNumber < 0) return null;

            // Find startup column number
            int iSearchStartColumn = -1;
            for (int i = 0; i < cols.Length; i++)
            {
                if (this._dgv.CurrentCell.ColumnIndex == cols[i].Index)
                {
                    iSearchStartColumn = i;
                    break;
                }
            }
            if (iSearchStartColumn < 0) return null;

            int iRowIndex = iSearchStartRow;
            int iColIndex = iSearchStartColumn;
            IList data = (IList)ListBindingHelper.GetList(this._dgv.DataSource, this._dgv.DataMember);
            do
            {
                // Get next Column & Row numbers
                if (bSearchUp) iColIndex--;
                else iColIndex++;
                if (iColIndex >= cols.Length)
                {
                    iColIndex = 0;
                    iRowIndex++;
                }
                else if (iColIndex < 0)
                {
                    iColIndex = cols.Length - 1;
                    iRowIndex--;
                }
                if (iRowIndex >= iLastRowNumber) iRowIndex = 0;
                else if (iRowIndex < 0) iRowIndex = iLastRowNumber - 1;
                // find value
                if (sp_FindBase(colHelpers[iColIndex].GetFormattedValueFromItem(data[iRowIndex], false), sFindWhat, bMatchCase, bMatchCell, iSearchMethod))
                {
                    return _dgv[cols[iColIndex].Index, iRowIndex];
                }
            } while (!(iRowIndex == iSearchStartRow && iColIndex == iSearchStartColumn));
            return null;*/
        }


        private bool sp_FindBase(object cellValue, string findString, bool bMatchCase, bool bMatchCell, int iSearchMethod)
        {
            if (!(cellValue is string)) return false;
            var searchString = ((string)cellValue).Replace((char)160, (char)32);
            // Regular string search
            if (iSearchMethod == -1)
            {
                // Match Cell
                if (bMatchCell)
                {
                    if (bMatchCase)
                        return string.Equals(findString, searchString, StringComparison.Ordinal);
                    else
                        return string.Equals(findString, searchString, StringComparison.OrdinalIgnoreCase);
                }
                // No Match Cell
                else
                {
                    if (bMatchCase) return searchString.IndexOf(findString, StringComparison.Ordinal) >= 0;
                    else return searchString.IndexOf(findString, StringComparison.OrdinalIgnoreCase) >= 0;
                    //if (bMatchCase) return FastIndexOf(SearchString, FindString) >= 0; 
                    //else return FastIndexOf(SearchString.ToLower(), FindString.ToLower()) >= 0;
                }
            }
            else
            {
                // Regular Expression
                var RegexPattern = findString;
                // Wildcards
                if (iSearchMethod == 1)
                {
                    // Convert wildcard to regex:
                    RegexPattern = "^" + System.Text.RegularExpressions.Regex.Escape(findString).Replace("\\*", ".*").Replace("\\?", ".") + "$";
                }
                System.Text.RegularExpressions.RegexOptions strCompare = System.Text.RegularExpressions.RegexOptions.None;
                if (!bMatchCase)
                {
                    strCompare = System.Text.RegularExpressions.RegexOptions.IgnoreCase;
                }
                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(RegexPattern, strCompare);
                return regex.IsMatch(searchString);
            }
        }

        #region ===========  INotifyPropertyChanged  ==============
        public event PropertyChangedEventHandler PropertyChanged;
        internal void OnPropertiesChanged(params string[] propertyNames)
        {
            foreach (var propertyName in propertyNames)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion


        private void FindWhat_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            OnPropertiesChanged(nameof(IsFindTextButtonEnabled));
        }
    }
}
