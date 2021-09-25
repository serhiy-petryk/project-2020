using System;
using DGCore.Common;
using WpfSpLib.Controls;

namespace DGView.Helpers
{
    public class MessageBoxProxy : IMessageBox
    {
        public Enums.MessageBoxResult Show(string message, string caption, Enums.MessageBoxButtons buttons, Enums.MessageBoxIcon icon)
        {
            string[] buttons2 = null;
            switch (buttons)
            {
                case Enums.MessageBoxButtons.OK:
                    buttons2 = new[] { "OK" };
                    break;
                case Enums.MessageBoxButtons.OKCancel:
                    buttons2 = new[] { "OK", "Cancel" };
                    break;
                case Enums.MessageBoxButtons.YesNoCancel:
                    buttons2 = new[] { "Yes", "No", "Cancel" };
                    break;
                case Enums.MessageBoxButtons.YesNo:
                    buttons2 = new[] { "Yes", "No" };
                    break;
            }

            DialogMessage.DialogMessageIcon? icon2 = null;
            if (Enum.TryParse<DialogMessage.DialogMessageIcon>(icon.ToString(), out var icon21))
                icon2 = icon21;

            var result2 = DialogMessage.ShowDialog(message, caption, icon2, buttons2);

            if (Enum.TryParse<Enums.MessageBoxResult>(result2, out var result))
                return result;
            return Enums.MessageBoxResult.None;
        }
    }
}
