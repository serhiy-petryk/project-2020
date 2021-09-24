using System;

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
    }
}
