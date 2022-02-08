using System.Windows.Threading;

namespace WpfSpLib.Helpers
{
    class DoEventsHelper
    {
        private static readonly DispatcherOperationCallback _exitFrameCallback = (state) =>
        {
            ((DispatcherFrame)state).Continue = false;
            return null;
        };

        internal static void DoEvents(DispatcherPriority priority)
        {
            var nestedFrame = new DispatcherFrame();
            var exitOperation = Dispatcher.CurrentDispatcher.BeginInvoke(priority, _exitFrameCallback, nestedFrame);
            Dispatcher.PushFrame(nestedFrame);

            if (exitOperation.Status != DispatcherOperationStatus.Completed)
                exitOperation.Abort();
        }
    }
}
