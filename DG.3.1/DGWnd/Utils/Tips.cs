using System;
using System.Drawing;
using DGCore.Common;

namespace DGWnd.Utils
{
    public static class Tips
    {
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
