using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfSpLib.Common;
using WpfSpLib.Controls;

namespace DGView.Helpers
{
    public static class Misc
    {
        public static void OpenDGDialog(DataGrid dataGrid, FrameworkElement dialogView, string title)
        {
            var owner = dataGrid.GetVisualParents().OfType<MwiChild>().FirstOrDefault();
            var host = owner.GetDialogHost();
            var height = Math.Max(200, Window.GetWindow(host).ActualHeight * 2 / 3);
            var width = Math.Max(200, Window.GetWindow(host).ActualWidth * 2 / 3);
            OpenMwiDialog(dialogView, title, (child, adorner) =>
            {
                child.Height = height;
                child.Width = width;
                child.Theme = owner?.ActualTheme;
                child.ThemeColor = owner?.ActualThemeColor;
            });
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
