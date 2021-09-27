using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
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
      StringBuilder sb = new StringBuilder();
      Dictionary<object, List<IComponent>> data = DGCore.Misc.DependentObjectManager.LinksData;
      foreach (KeyValuePair<object, List<IComponent>> kvp in data) {
        sb.Append(GetObjectPresentation(kvp.Key)+ Environment.NewLine);
        foreach (IComponent o in kvp.Value) {
          sb.Append("\t" + GetObjectPresentation(o) + Environment.NewLine);
        }
        sb.Append(Environment.NewLine);
      }
      this.txtLog.Text = sb.ToString();
    }

    private string GetObjectPresentation(object o) {
//      return o.GetType().Name + "; " + o.ToString();
      return o.ToString();
    }

    private void frmDependentObjectManager_FormClosed(object sender, FormClosedEventArgs e) {
        DGCore.Misc.DependentObjectManager.ObjectListChanged -= new EventHandler(DependentObjectManager_ObjectListChanged);
    }
  }
}