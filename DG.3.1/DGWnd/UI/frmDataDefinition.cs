using System;
using System.Windows.Forms;

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
        _rootMenu = new DGCore.Menu.RootMenu(DGCore.Misc.AppSettings.CONFIG_FILE_NAME);
        if (!string.IsNullOrEmpty(_rootMenu.ApplicationTitle))
          (TopLevelControl ?? this).Text = _rootMenu.ApplicationTitle;
      }

      ucDataDefinition1.Bind(_rootMenu);
    }

  }
}