using System;
using System.Globalization;
using System.Printing;
using System.Windows;

namespace DGView.Temp
{
    public partial class PageViewModel: ICloneable
    {
        public enum MeasurementSystem { Metric, US }
        public static MeasurementSystem CurrentMeasurementSystem { get; set; } = RegionInfo.CurrentRegion.IsMetric ? MeasurementSystem.Metric : MeasurementSystem.US;

        public PageSize[] AvailableSizes { get; }
        public PageSize Size { get; set; }
        public PageOrientation Orientation { get; set; }
        public Thickness Margins { get; set; }

        public PageViewModel(PageSize[] availableSizes)
        {
            AvailableSizes = availableSizes;
            if (AvailableSizes.Length > 0)
                Size = AvailableSizes[0];
        }

        public object Clone() => new PageViewModel(AvailableSizes) {Size = Size, Orientation = Orientation, Margins = Margins};
    }
}
