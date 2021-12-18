using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DGCore.Common;
using DGCore.Helpers;
using DGCore.PD;

namespace DGWnd.Utils
{
    public static class Tips
    {
        public static readonly TypeConverter ByteArrayToBitmapConverter = new ImageConverter();

        public static DGCellValueFormatter GetDGCellValueFormatter(DataGridViewColumn column)
        {
            var pdc = DGVUtils.GetInternalPropertyDescriptorCollection(column.DataGridView);
            var propertyDescriptor = pdc[column.DataPropertyName];
            if (column.Name == "#group_ItemCount")
                return new DGCellValueFormatter(new PropertyDescriptorForGroupItemCount());
            return new DGCellValueFormatter((IMemberDescriptor)propertyDescriptor);
        }

        public static void ExitApplication()
        {
            if (System.Windows.Forms.Application.MessageLoop) // WinForms app
                System.Windows.Forms.Application.Exit();
            else // Console app
                Environment.Exit(1);
        }

        public static ContentAlignment? ConvertAlignment(Enums.Alignment? source)
        {
            switch (source)
            {
                case Enums.Alignment.Left: return ContentAlignment.MiddleLeft;
                case Enums.Alignment.Right: return ContentAlignment.MiddleRight;
                case Enums.Alignment.Center: return ContentAlignment.MiddleCenter;
            }

            return null;
        }
    }
}
