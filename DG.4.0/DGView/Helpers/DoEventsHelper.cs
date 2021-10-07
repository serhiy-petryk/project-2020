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
            DispatcherFrame nestedFrame = new DispatcherFrame();
            DispatcherOperation exitOperation = Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, _exitFrameCallback, nestedFrame);
            Dispatcher.PushFrame(nestedFrame);

            if (exitOperation.Status != DispatcherOperationStatus.Completed)
            {
                exitOperation.Abort();
            }
        }
    }
}
