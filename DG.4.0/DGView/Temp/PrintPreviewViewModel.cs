using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DGView.Temp
{
    public class PrintPreviewViewModel: INotifyPropertyChanged
    {
        private static readonly LocalPrintServer _printServer = new LocalPrintServer();
        // private ComboBox _printerComboBox;
        public Printer[] Printers { get; } = new LocalPrintServer().GetPrintQueues().Select(p => new Printer(p)).ToArray();
        public PrintQueue CurrentPrinter { get; set; } //=> _printerComboBox?.SelectedItem as PrintQueue;
        public PrintCapabilities CurrentPrintCapabilities { get; private set; }

        private int _savedPages = -3;

        public int SavedPages
        {
            get => _savedPages;
            set
            {
                _savedPages = value;
                OnPropertiesChanged(nameof(SavedPages));
            }
        }

        public PrintPreviewViewModel()
        {

        }

        #region ===========  INotifyPropertyChanged  ==============
        public event PropertyChangedEventHandler PropertyChanged;

        internal void OnPropertiesChanged(params string[] propertyNames)
        {
            foreach (var propertyName in propertyNames)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion


        #region ==========  Subclasses  ============
        public class Printer
        {
            public PrintQueue PrintQueue { get; }

            public Printer(PrintQueue printQueue)
            {
                PrintQueue = printQueue;
            }
        }
        #endregion
    }
}
