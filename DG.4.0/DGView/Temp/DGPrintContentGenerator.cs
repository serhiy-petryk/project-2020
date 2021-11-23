using System;
using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using WpfSpLib.Helpers;

namespace DGView.Temp
{
    public class DGPrintContentGenerator : IPrintContentGenerator
    {
        public bool StopGeneration { get; set; }

        private CultureInfo _currentCulture => LocalizationHelper.CurrentCulture;

        private IList _items;
        private DataGridColumn[] _columns;
        private string _title;
        private string _subTitle;

        private Size _pageSize;
        private Thickness _pageMargins;

        public DGPrintContentGenerator(IList items, DataGridColumn[] columns, string title, string[] subHeaders)
        {
            _items = items;
            _columns = columns;
            _title = title;
            if (subHeaders != null)
                _subTitle = string.Join(Environment.NewLine, subHeaders);
        }

        public void GenerateContent(FixedDocument document, Thickness margins)
        {
            _pageSize = document.DocumentPaginator.PageSize;
            _pageMargins = margins;

        }
    }
}
