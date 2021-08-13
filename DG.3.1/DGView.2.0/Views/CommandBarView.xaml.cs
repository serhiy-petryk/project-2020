using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DGView.ViewModels;
using WpfSpLib.Common;

namespace DGView.Views
{
    /// <summary>
    /// Interaction logic for CommandBarView.xaml
    /// </summary>
    public partial class CommandBarView : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty DGViewModelProperty = DependencyProperty.Register("DGViewModel", typeof(DataGridViewModel), typeof(CommandBarView), new FrameworkPropertyMetadata(null));
        public DataGridViewModel DGViewModel
        {
            get => (DataGridViewModel)GetValue(DGViewModelProperty);
            set => SetValue(DGViewModelProperty, value);
        }
        public DataGrid DGControl => DGViewModel?.DGControl;

        public CommandBarView()
        {
            InitializeComponent();

            CmdEditSetting = new RelayCommand(cmdEditSetting);
            CmdRowDisplayMode = new RelayCommand(cmdRowDisplayMode);
            CmdFastFilter = new RelayCommand(cmdFastFilter);
            CmdSortAsc = new RelayCommand(cmdSortAsc);
            CmdSortDesc = new RelayCommand(cmdSortDesc);
            CmdSortRemove = new RelayCommand(cmdSortRemove);

            DataContext = this;
        }

        #region ===========  INotifyPropertyChanged  ================

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertiesChanged(params string[] propertyNames)
        {
            foreach (var propertyName in propertyNames)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        private void CommandBarView_OnLoaded(object sender, RoutedEventArgs e)
        {
            var dgv = Tips.GetVisualParents(this).OfType<DataGridView>().FirstOrDefault();
            DGViewModel = dgv?.ViewModel;

            // CellViewModeComboBox
            /*CellViewModeComboBox.ItemsSource = CellViewModeClass.CellViewModeValues;
            CellViewModeComboBox.SelectedValue = CellViewModeComboBox.Items[1];
            CellViewModeComboBox.Width = ControlHelper.GetListWidth(CellViewModeComboBox);*/

            OnPropertiesChanged(nameof(DGControl));
        }
    }
}
