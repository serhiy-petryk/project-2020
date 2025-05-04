using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using WpfSpLib.Common.ColorSpaces;
using WpfSpLib.Helpers;
using WpfSpLibDemo.Samples;

namespace WpfSpLibDemo.TestViews
{
    /// <summary>
    /// Interaction logic for DataGridHack.xaml
    /// </summary>
    public partial class DataGridHack : Window
    {
        #region ============== Properties/Events  ===================
        public static readonly DependencyProperty BaseHslProperty = DependencyProperty.Register("BaseHsl",
            typeof(HSL_Observable), typeof(DataGridHack), new FrameworkPropertyMetadata(null));
        public HSL_Observable BaseHsl
        {
            get => (HSL_Observable)GetValue(BaseHslProperty);
            set => SetValue(BaseHslProperty, value);
        }
        #endregion

        public IList<AuthorINotifyDataErrorInfo> DataINotifyDataErrorInfos { get; } = AuthorINotifyDataErrorInfo.Authors;
        public IList<AuthorIDataErrorInfo> DataIDataErrorInfos { get; } = AuthorIDataErrorInfo.Authors;
        public Author.Level[] EnumList { get; } = Enum.GetValues<Author.Level>();

        public IList<Author> Data { get; } = Author.Authors;

        public DataGridHack()
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

        private void TestDataGrid_OnBeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            var cell = DataGridHelper.GetDataGridCell((DataGrid)sender, e.Row, e.Column);
            var a1 = Validation.GetHasError(e.Row);
            var a2 = Validation.GetErrors(e.Row);
            cell.Dispatcher.BeginInvoke(() =>
            {
                var children = cell.GetVisualChildren().OfType<Control>().ToArray();
                foreach (var control in children)
                {
                    if (control is CheckBox || control is ComboBox)
                    {
                        if (control.VerticalAlignment != VerticalAlignment.Center)
                            control.VerticalAlignment = VerticalAlignment.Center;
                        if (control.VerticalContentAlignment != VerticalAlignment.Stretch)
                            control.VerticalContentAlignment = VerticalAlignment.Stretch;
                    }
                    else
                    {
                        if (control.VerticalAlignment != VerticalAlignment.Stretch)
                            control.VerticalAlignment = VerticalAlignment.Stretch;
                        if (control.VerticalContentAlignment != VerticalAlignment.Center)
                            control.VerticalContentAlignment = VerticalAlignment.Center;
                    }
                }
            }, DispatcherPriority.Normal);
        }

        private void DataGrid_Monochrome_OnBeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            var cell = DataGridHelper.GetDataGridCell((DataGrid)sender, e.Row, e.Column);
            // cell.BorderThickness = new Thickness(0);
            return;
            // Stretch cell to full height for Text datagrid column
            cell.Dispatcher.BeginInvoke(() =>
            {
                var children = cell.GetVisualChildren().OfType<Control>().ToArray();
                foreach (var control in children)
                {
                    if (control is TextBox)
                    {
                        if (control.VerticalAlignment != VerticalAlignment.Stretch)
                            control.VerticalAlignment = VerticalAlignment.Stretch;
                        if (control.VerticalContentAlignment != VerticalAlignment.Center)
                            control.VerticalContentAlignment = VerticalAlignment.Center;
                    }
                }
            }, DispatcherPriority.Normal);
        }

        private void DataGrid_Monochrome_OnLoadingRow(object sender, DataGridRowEventArgs e)
        {
            string rowHeaderText;
            if (e.Row.IsNewItem)
                rowHeaderText = "*"; // ((char)9654).ToString() + "★✶";
            else
                rowHeaderText = (e.Row.GetIndex() + 1).ToString("N0", LocalizationHelper.CurrentCulture);

            if (!Equals(e.Row.Header, rowHeaderText)) e.Row.Header = rowHeaderText;
        }

        private void ChangeIDataErrorInfoSupportButton_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (DataGrid dataGrid in this.GetVisualChildren().OfType<DataGrid>())
            {
                dataGrid.CommitEdit(DataGridEditingUnit.Row, true);

                if (Equals(dataGrid.ItemsSource, Data))
                {
                    dataGrid.ItemsSource = DataIDataErrorInfos;
                    DataTypeLabel.Content = "IDataErrorInfo";
                }
                else if (Equals(dataGrid.ItemsSource, DataIDataErrorInfos))
                {
                    dataGrid.ItemsSource = DataINotifyDataErrorInfos;
                    DataTypeLabel.Content = "INotifyDataErrorInfo";
                }
                else
                {
                    dataGrid.ItemsSource = Data;
                    DataTypeLabel.Content = null;
                }
            }
        }

        private void DataGridHack_OnClosing(object sender, CancelEventArgs e) => DataGridHelper.Control_OnClosing(this);

    }
}
