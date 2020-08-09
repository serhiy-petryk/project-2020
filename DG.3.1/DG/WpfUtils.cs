using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DG
{
    public static class WpfUtils
    {
        public static async void RunInBackgroundSync(Action code)
        {
            if (Thread.CurrentThread.IsBackground)
                code.Invoke();
            else
                await Task.Run(code);
            // (new Task(code)).RunSynchronously();
            /*{
              var t = new Thread(() =>
              {
                Thread.CurrentThread.IsBackground = true;
                code.Invoke();
              });
              t.Start();
            }*/
        }

        public static void WpfUIThreadSync(this ContentControl @this, Action code)
        {
            if (!@this.Dispatcher.CheckAccess())
                @this.Dispatcher.Invoke(code);
            else
                code.Invoke();
        }

    }
}
