using System;
using System.Text;
using System.Windows.Forms;
using DGCore.Common;
using DGWnd.Utils;

namespace DGWnd.UI {
  public partial class frmDataDefinition : Form
  {
    private static DGCore.Menu.RootMenu _rootMenu;

    public frmDataDefinition()
    {
      InitializeComponent();
    }

    private void frmDataDefinition_Load(object sender, EventArgs e) {
        if (_rootMenu == null)
        {
            try
            {
                _rootMenu = new DGCore.Menu.RootMenu();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                if (ex is LoadJsonConfigException loadException)
                {
                    sb.AppendLine(Localization.GetMessage("LoadConfigError1", loadException.FileName));
                    if (loadException.LineNumber.HasValue)
                        sb.AppendLine(Localization.GetMessage("LoadConfigError2", loadException.LineNumber, loadException.Position));
                    sb.AppendLine(null);
                    sb.AppendLine(Localization.GetMessage("LoadConfigError3"));
                    sb.AppendLine(loadException.Message);
                }
                else
                    sb.AppendLine(ex.Message);
                MessageBox.Show(sb.ToString(), null, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Tips.ExitApplication();
                return;
            }

            if (!string.IsNullOrEmpty(_rootMenu.ApplicationTitle))
                (TopLevelControl ?? this).Text = _rootMenu.ApplicationTitle;
        }

        ucDataDefinition1.Bind(_rootMenu);
    }

  }
}