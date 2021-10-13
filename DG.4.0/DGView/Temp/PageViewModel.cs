using System;
using System.Printing;
using System.Windows;

namespace DGView.Temp
{
    public partial class PageViewModel: ICloneable
    {
        public PageSize[] AvailableSizes { get; set; }
        public PageSize Size { get; set; }
        public PageOrientation Orientation { get; set; }
        public Thickness Margins { get; set; }

        public object Clone() => new PageViewModel
            {AvailableSizes = AvailableSizes, Size = Size, Orientation = Orientation, Margins = Margins};
    }
}
