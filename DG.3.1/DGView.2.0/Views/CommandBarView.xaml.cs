using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfSpLib.Common;

namespace DGView.Views
{
    /// <summary>
    /// Interaction logic for CommandBarView.xaml
    /// </summary>
    public partial class CommandBarView : UserControl// , INotifyPropertyChanged
    {
        public CommandBarView()
        {
            InitializeComponent();
        }

        private void CommandBarView_OnLoaded(object sender, RoutedEventArgs e)
        {
            var dgv = this.GetVisualParents().OfType<DataGridView>().FirstOrDefault();
            DataContext = dgv?.ViewModel;
        }

        #region ===========  INotifyPropertyChanged  ================
       /* public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertiesChanged(params string[] propertyNames)
        {
            foreach (var propertyName in propertyNames)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }*/
        #endregion

    }
}
