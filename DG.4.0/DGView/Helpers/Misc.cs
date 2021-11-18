using System;
using System.Linq;
using System.Windows;
using WpfSpLib.Common;
using WpfSpLib.Controls;

namespace DGView.Helpers
{
    public static class Misc
    {
        public static FrameworkElement GetDialogHost(UIElement element)
        {
            var parentMwiChild = element.GetVisualParents().OfType<MwiChild>().LastOrDefault(a => a != element);
            if (parentMwiChild?.Template.FindName("ContentBorder", parentMwiChild) is FrameworkElement fe)
                return fe;
            return parentMwiChild?.DialogHost;
        }

        public static void OpenMwiDialog(FrameworkElement dialogContent, string title, Action<MwiChild, DialogAdorner> beforeShowDialogAction)
        {
            var width = dialogContent.Width;
            var height = dialogContent.Height;
            if (!double.IsNaN(dialogContent.Width)) dialogContent.Width = double.NaN;
            if (!double.IsNaN(dialogContent.Height)) dialogContent.Height = double.NaN;

            var content = new MwiChild
            {
                Content = dialogContent,
                LimitPositionToPanelBounds = true,
                Title = title,
                VisibleButtons = MwiChild.Buttons.Close | MwiChild.Buttons.Maximize,
                IsActive = true
            };
            
            // Migrate valid width/height values from dialogContent to host
            if (!double.IsNaN(width)) content.Width = width;
            if (!double.IsNaN(height)) content.Height = height;

            // var adorner = new DialogAdorner(_owner.DialogHost) { CloseOnClickBackground = true };
            var adorner = new DialogAdorner(null) { CloseOnClickBackground = true };
            beforeShowDialogAction?.Invoke(content, adorner);
            adorner.ShowContentDialog(content);
        }
    }
}
