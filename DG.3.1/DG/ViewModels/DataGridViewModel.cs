using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using DG.Annotations;
using DG.Common;

namespace DG.ViewModels
{
    public class DataGridViewModel : INotifyPropertyChanged
    {
        public RelayCommand CmdToggleGrid { get; }

        public DataGridViewModel()
        {
            CmdToggleGrid = new RelayCommand((object o) =>
            {
                var dataGrid = (DataGrid)o;
                dataGrid.GridLinesVisibility = dataGrid.GridLinesVisibility == DataGridGridLinesVisibility.None
                    ? DataGridGridLinesVisibility.All
                    : DataGridGridLinesVisibility.None;
            });
        }
        //===========  INotifyPropertyChanged  ========== 
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
