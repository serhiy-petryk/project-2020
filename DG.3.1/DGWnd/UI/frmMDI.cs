using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace DGWnd.UI {
  public partial class frmMDI : Form {

//    private MenuManagers.EditMenuManagerTS m_oEditMenuManager = null;
    private int wndCnt = 0;

    public frmMDI() {
      InitializeComponent();
      //    m_oEditMenuManager = new MenuManagers.EditMenuManagerTS();
      //  m_oEditMenuManager.ConnectMenus(this.mnuEdit);
    }

    #region *********************  Menu ***************************
    private void mnuWHorizontal_Click(object sender, EventArgs e) {
      this.LayoutMdi(MdiLayout.TileHorizontal);
    }

    private void mnuWVertical_Click(object sender, EventArgs e) {
      this.LayoutMdi(MdiLayout.TileVertical);
    }

    private void mnuWArrange_Click(object sender, EventArgs e) {
      this.LayoutMdi(MdiLayout.ArrangeIcons);
    }

    private void mnuWCascade_Click(object sender, EventArgs e) {
      this.LayoutMdi(MdiLayout.Cascade);
    }

    /// **************** End menu *********************
    #endregion **************** End Menu *****************

    private void frmMDI_Load(object sender, EventArgs e) {
      AttachNewChildForm(new UI.frmDataDefinition());
    }

    # region ******************** Procs ********************
    private void CheckChildForms() {
      for (int i = 0; i < this.stripWnds.Items.Count; i++) {
        ToolStripItem item = this.stripWnds.Items[i];
        if (item.Name.StartsWith("TSB")) {
          int wndID = int.Parse(item.Name.Substring(3));
          bool flag = false;
          foreach (Form frm in this.MdiChildren) {//circle by form collection
            if (frm.Handle.ToInt32() == wndID) { // form exist
              flag = true; break;
            }
          }
          if (!flag) { // Remove items
            if (i > 1) {
              this.stripWnds.Items.RemoveAt(i - 1);
              this.stripWnds.Items.RemoveAt(i - 1);
              i -= 2;
            }
            else {
              this.stripWnds.Items.RemoveAt(i);
              this.stripWnds.Items.RemoveAt(i);
              i--;
            }
          } // end of remove items
        }
      }
    }

    public void AttachNewChildForm(Form frm) {
      this.wndCnt++;
      if (this.stripWnds.Items.Count != 0) { // add separator
        ToolStripSeparator x1 = new ToolStripSeparator();
        this.stripWnds.Items.Add(x1);
      }
      // add button
      ToolStripSplitButton x2 = new ToolStripSplitButton(frm.Text);
      x2.Name = "SB" + this.wndCnt.ToString();
      x2.Image = null;
      x2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
      x2.Click += new System.EventHandler(this.TabStripButtonClick);
      x2.Margin = new System.Windows.Forms.Padding(2, 1, 0, 2);
      x2.DropDownOpening += new System.EventHandler(this.TabStripButtonDeleteWnd);
      this.stripWnds.Items.Add(x2);
      // Form
      frm.Tag = "SB" + wndCnt.ToString();
      frm.MdiParent = this;
//      frm.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ChildClosed);
      frm.Disposed += new EventHandler(this.ChildClosed);
      frm.Activated += new System.EventHandler(this.ChildActivate);
      frm.Show();
      CheckVisibility(this.MdiChildren.Length);
    }

    private void ChildClosed(object sender, System.EventArgs e) {
      System.Windows.Forms.Form x = (System.Windows.Forms.Form)sender;
      x.Activated -= new System.EventHandler(this.ChildActivate);
      x.Disposed -= new EventHandler(this.ChildClosed);
      string id = x.Tag as String;
      x.Tag = null;
      if (id != null) {
        ToolStripItem item = this.stripWnds.Items[id];
        if (item != null) {
          int i = this.stripWnds.Items.IndexOf(item);
          if (i > 0) i--;
          this.stripWnds.Items.RemoveAt(i);
          if (this.stripWnds.Items.Count > 0) this.stripWnds.Items.RemoveAt(i); // Check of last element
        }
      }
      CheckVisibility(this.MdiChildren.Length - 1);
      if (this.Visible) {// free memory of previous form by open new blank form
        Form frm = new Form();
        frm.MdiParent = this;
        frm.Show();
        frm.Dispose();
      }
    }

    static Font _normalWndFont=null;
    static Font _activeWndFont=null;
    private void ChildActivate(object sender, System.EventArgs e) {
      System.Windows.Forms.Form x = (System.Windows.Forms.Form)sender;
      string id = x.Tag as String;
      foreach (ToolStripItem item in this.stripWnds.Items) {
        if (item.GetType().Name == "ToolStripSplitButton" && item.Name.StartsWith("SB")) {
          if (_normalWndFont == null) {
            _normalWndFont = item.Font;
            _activeWndFont = new Font(_normalWndFont, FontStyle.Italic);
          }
          if (item.Name == id) {// active window
            if (item.Font != _activeWndFont) {
              item.Font = _activeWndFont;
              item.ForeColor = Color.Blue;
            }
          }
          else {
            if (item.Font != _normalWndFont) {
              item.Font = _normalWndFont;
              item.ForeColor = Color.Black;
            }
          }
        }
//          item.BackColor = (item.Name == id ? Color.WhiteSmoke : this.BackColor);
      }
    }

    private void TabStripButtonClick(object sender, EventArgs e) {
      ToolStripSplitButton item = (ToolStripSplitButton)sender;
      foreach (System.Windows.Forms.Form x1 in this.MdiChildren) {
        string frmID = (string)x1.Tag;
        if (frmID == item.Name) {
          x1.Activate(); break;
        }
      }
    }

    private void TabStripButtonDeleteWnd(object sender, EventArgs e) {
      ToolStripSplitButton item = (ToolStripSplitButton)sender;
      foreach (System.Windows.Forms.Form x1 in this.MdiChildren) {
        string frmID = (string)x1.Tag;
        if (frmID == item.Name) {
          x1.Close(); break;
        }
      }
    }

    private void CheckVisibility(int wndCount) {
      return;
      if (wndCount <= 0) { // no child windows
        if (this.mnuWindow.Enabled) this.mnuWindow.Enabled = false;
        if (this.stripWnds.Visible) this.stripWnds.Visible = false;
      }
      else {
        if (!this.mnuWindow.Enabled) this.mnuWindow.Enabled = true;
        if (!this.stripWnds.Visible) this.stripWnds.Visible = true;
      }
    }

    private void mainMenu_ItemAdded(object sender, ToolStripItemEventArgs e) {
      e.Item.Overflow = ToolStripItemOverflow.AsNeeded;
    }

#endregion    ///////////////////////////////  END ///////////////////////////

    private void btnDataDefinitionList_Click(object sender, EventArgs e) => AttachNewChildForm(new UI.frmDataDefinition());
    private void mnuExit_Click(object sender, EventArgs e) => Application.Exit();
    private void btnMemoryInUsed_Click(object sender, EventArgs e) => MessageBox.Show($@"Програма займає {DGCore.Utils.Tips.MemoryUsedInBytes:N0} байт памяті");
    private void btnDependentObjectManager_Click(object sender, EventArgs e) => AttachNewChildForm(new UI.frmDependentObjectManager());
    private void btnLog_Click(object sender, EventArgs e) => AttachNewChildForm(new UI.frmLog());
    private void btnWebDownloader_Click(object sender, EventArgs e) => AttachNewChildForm(new WebDownloader.frmWebDownloader());

    private void btnClearSqlCache_Click(object sender, EventArgs e)
    {
      using (var conn = new SqlConnection("Data Source=localhost;Initial Catalog=dbOneSAP_DW;Integrated Security=True"))
      using (var cmd = conn.CreateCommand())
      {
        conn.Open();
        cmd.CommandText = "CHECKPOINT";
        cmd.ExecuteNonQuery();
        cmd.CommandText = "DBCC DROPCLEANBUFFERS";
        cmd.ExecuteNonQuery();
      }
      MessageBox.Show(@"Sql Cache was cleared");
    }
  }
}