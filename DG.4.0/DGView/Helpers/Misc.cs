using System;
using System.Windows;
using System.Windows.Media;
using WpfSpLib.Controls;
using WpfSpLib.Themes;

namespace DGView.Helpers
{
    public static class Misc
    {
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
