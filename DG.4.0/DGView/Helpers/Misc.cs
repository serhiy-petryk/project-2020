using System.Windows;
using System.Windows.Media;
using WpfSpLib.Controls;
using WpfSpLib.Themes;

namespace DGView.Helpers
{
    public static class Misc
    {
        public static void OpenDialog(FrameworkElement dialogContent, string title, Size size, MwiThemeInfo theme, Color? themeColor, MwiChild.Buttons visibleButtons = MwiChild.Buttons.Close | MwiChild.Buttons.Maximize, bool resizable = true)
        {
            var content = new MwiChild
            {
                Content = dialogContent,
                LimitPositionToPanelBounds = true,
                VisibleButtons = visibleButtons,
                Title = title,
                Width = size.Width,
                Height = size.Height,
                IsActive = true,
                Theme = theme,
                ThemeColor = themeColor,
                Resizable = resizable
            };
            // var adorner = new DialogAdorner(_owner.DialogHost) { CloseOnClickBackground = true };
            var adorner = new DialogAdorner(null) { CloseOnClickBackground = true };
            adorner.ShowContentDialog(content);
        }
    }
}
