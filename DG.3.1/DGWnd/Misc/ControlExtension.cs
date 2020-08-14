// see https://stackoverflow.com/questions/661561/how-do-i-update-the-gui-from-another-thread

using System;
using System.Windows.Forms;

namespace DGWnd.Misc
{
    public static class ControlExtensions
    {
        /// <summary>
        /// Executes the Action asynchronously on the UI thread, does not block execution on the calling thread.
        /// </summary>
        /// <param name="this"></param>
        /// <param name="code"></param>
        public static void UIThreadAsync(this Control @this, Action code)
        {
            if (@this.InvokeRequired)
                @this.BeginInvoke(code);
            else
                code.Invoke();
        }
        public static void UIThreadSync(this Control @this, Action code)
        {
            if (@this.InvokeRequired)
                @this.Invoke(code);
            else
                code.Invoke();
        }
    }
}