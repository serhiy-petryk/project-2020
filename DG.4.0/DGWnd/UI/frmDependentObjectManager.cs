using System;
using System.Windows.Forms;

namespace DGWnd.UI {
  public partial class frmDependentObjectManager : Form {
    public frmDependentObjectManager() {
      InitializeComponent();
    }

    private void frmDependentObjectManager_Load(object sender, EventArgs e) {
      DependentObjectManager_ObjectListChanged(null, null);
      DGCore.Misc.DependentObjectManager.ObjectListChanged += new EventHandler(DependentObjectManager_ObjectListChanged);
    }

    void DependentObjectManager_ObjectListChanged(object sender, EventArgs e) {
      if (this.txtLog.InvokeRequired) {
        this.Invoke((EventHandler)DependentObjectManager_ObjectListChanged, new object[] { sender, e });
        return;
      }
      this.txtLog.Text = DGCore.Misc.DependentObjectManager.GetStringPresentation();
    }

    private void frmDependentObjectManager_FormClosed(object sender, FormClosedEventArgs e) {
        DGCore.Misc.DependentObjectManager.ObjectListChanged -= new EventHandler(DependentObjectManager_ObjectListChanged);
    }
  }
}