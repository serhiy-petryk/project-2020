using System.ComponentModel;

namespace DGCore.Utils
{
    public interface IDGColumnHelper
    {
        PropertyDescriptor PropertyDescriptor { get; }
        bool IsValid { get; }
        object GetFormattedValueFromItem(object item, bool clipboardMode);
        bool Contains(object item, string searchString);
        // only for Window DG: void GetColumnSize(Graphics g, Font font, IEnumerable<object> items, out float colWidth, out float rowHeight, List<float> rowHeights);
    }
}
