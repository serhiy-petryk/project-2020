using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace DGView.Temp
{
    public class DGPrintContentGenerator : IPrintContentGenerator
    {
        public bool StopGeneration { get; set; }

        public DGPrintContentGenerator(IList items, DataGridColumn[] columns)
        {

        }

        public void GenerateContent(FixedDocument document, Thickness margins)
        {
        }
    }
}
