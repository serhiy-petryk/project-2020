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

            if (FindInAllTable.IsChecked ?? false)
                _findCell = sp_FindInTableOrColumn(_viewModel.DGControl.Columns.Where(c => c.Visibility == Visibility.Visible).OrderBy(c => c.DisplayIndex).ToArray());
            else if (FindInColumn.IsChecked ?? false)
                _findCell = sp_FindInTableOrColumn(new[] {DGHelper.GetActiveCellInfo(_viewModel.DGControl).Column});
            else if (FindInSelection.IsChecked ?? false)
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

        DataGridCellInfo sp_FindInSelection()
        {
            // Search criteria
            var findWhat = FindWhat.Text;
            var matchCase = MatchCase.IsChecked ?? false;
            var matchCell = MatchCell.IsChecked ?? false;
            var findUp = FindUp.IsChecked ?? false;
            var searchMethod = (Use.IsChecked ?? false) ? cbUse.SelectedIndex : -1;

            var items = _viewModel.DGControl.Items;
            var selectedCells = findUp
                ? _viewModel.DGControl.SelectedCells.OrderByDescending(c => items.IndexOf(c.Item))
                    .ThenByDescending(c => c.Column.DisplayIndex)
                : _viewModel.DGControl.SelectedCells.OrderBy(c => items.IndexOf(c.Item))
                    .ThenBy(c => c.Column.DisplayIndex);

            var properties = new List<PropertyDescriptor>();
            var helpers = DGHelper.GetColumnHelpers(_viewModel.DGControl.Columns.Where(c=> c.Visibility == Visibility.Visible).ToArray(), _viewModel.Properties, properties).Where(h =>
                !(h.NotNullableValueType == typeof(byte[]) || h.NotNullableValueType == typeof(bool))).ToArray();

            var getters = new Func<object, string>[_viewModel.DGControl.Columns.Count];
            for (var i=0; i<helpers.Length;i++)
                getters[helpers[i].ColumnDisplayIndex] = new DGCellValueFormatter(properties[i]).StringForFindTextGetter;

            foreach (var cell in selectedCells)
            {
                var getter = getters[cell.Column.DisplayIndex];
                if (getter != null && sp_FindBase(getter(cell.Item), findWhat, matchCase, matchCell, searchMethod))
                    return cell;
            }
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
