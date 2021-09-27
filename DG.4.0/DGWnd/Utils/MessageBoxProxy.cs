using System;
using System.Windows.Forms;
using DGCore.Common;

namespace DGWnd.Utils
{
    public class MessageBoxProxy : IMessageBox
    {
        //Dictionary<object, object>
        public Enums.MessageBoxResult Show(string message, string caption, Enums.MessageBoxButtons buttons, Enums.MessageBoxIcon icon)
        {
            Enum.TryParse<MessageBoxButtons>(buttons.ToString(), out var buttons2);
            Enum.TryParse<MessageBoxIcon>(icon.ToString(), out var icon2);
            
            var result2 = MessageBox.Show(message, caption, buttons2, icon2);
            
            Enum.TryParse<Enums.MessageBoxResult>(result2.ToString(), out var result);
            return result;
        }
    }
}
