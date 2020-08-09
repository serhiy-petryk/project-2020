using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace DG.UI
{
    /// <summary>
    /// Interaction logic for MainWindowSort.xaml
    /// </summary>
    public partial class frmDG
    {
        private void Do_CellViewMode(object sender, SelectionChangedEventArgs e)
        {
        }
        private void Do_FastFilter(object sender, TextChangedEventArgs e)
        {
        }
        private void Do_ThemesChanged(object sender, SelectionChangedEventArgs e)
        {
            // Process with delay because there is a bug: 'Schemes' combobox не деактивується після спрацювання - не змінюється колір
            var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(50) };
            timer.Tick += (timerSender, args) =>
            {
                timer.Stop();
                (e.AddedItems[0] as Themes.ThemeInfo)?.ApplyTheme();
            };
            timer.Start();
        }

        private void GridLinesVisibilityChanged(object sender, EventArgs e) => Btn_ToggleGridVisibility.Content =
            Resources[DataGrid.GridLinesVisibility == DataGridGridLinesVisibility.None ? "Icon_GridOn" : "Icon_GridOff"];

        private void Do_MemoryInUse(object sender, RoutedEventArgs e) => MessageBox.Show("Memory: " + DGCore.Utils.Tips.MemoryUsedInBytes.ToString("N0"));
    }
}
