using System.Linq;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace DGView.Controls
{
    public static class DropDownButtonHelper
    {
        public static ContextMenu OpenDropDownMenu(object sender)
        {
            if (sender is ToggleButton button && Equals(button.IsChecked, true))
            {
                var cm = button.Tag as ContextMenu ?? button.Resources.Values.OfType<ContextMenu>().FirstOrDefault();
                if (cm != null && !cm.IsOpen) // ContextMenu may be already opened (?bug (binding mode=TwoWay=>twice event call when value changed), see SplitButtonStyle)
                {
                    if (cm.PlacementTarget == null)
                    {
                        cm.PlacementTarget = button;
                        cm.Placement = PlacementMode.Bottom;
                        cm.Closed += (senderClosed, eClosed) => ((ToggleButton)sender).IsChecked = false;
                    }
                    cm.IsOpen = true;
                    return cm;
                }
            }
            return null;
        }
    }
}
