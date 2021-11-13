// see https://docs.microsoft.com/ru-ru/dotnet/api/system.windows.threading.dispatcherframe?view=windowsdesktop-5.0
using System.Windows.Threading;

namespace DGView.Helpers
{
    internal class DoEventsHelper
    {
        private static readonly DispatcherOperationCallback _exitFrameCallback = (state) =>
        {
            ((DispatcherFrame)state).Continue = false;
            return null;
        };

        internal static void DoEvents()
        {
            var nestedFrame = new DispatcherFrame();
            var exitOperation = Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, _exitFrameCallback, nestedFrame);
            Dispatcher.PushFrame(nestedFrame);

            if (exitOperation.Status != DispatcherOperationStatus.Completed)
                exitOperation.Abort();
        }
    }
}
