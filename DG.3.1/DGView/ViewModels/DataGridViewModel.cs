using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using DGView.Common;
using DGView.Views;

namespace DGView.ViewModels
{
    public class DataGridViewModel : DependencyObject, INotifyPropertyChanged
    {
        public const bool AUTOGENERATE_COLUMNS = true;

        private readonly DataGridView _view;
        public DataGrid DGControl => _view.DataGrid;

        public RelayCommand CmdTest;
        public DataGridViewModel(DataGridView view)
        {
            _view = view;
            CmdTest = new RelayCommand((p) =>
            {
            });
        }

        //===========  INotifyPropertyChanged  =======================
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertiesChanged(params string[] propertyNames)
        {
            foreach (var propertyName in propertyNames)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
