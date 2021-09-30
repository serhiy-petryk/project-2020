using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DGWnd.Utils;

namespace DGWnd.UserControls {
  public partial class UCDataDefinition : UserControl {

    public UCDataDefinition() {
      InitializeComponent();
      PropertyGrid_AdjustToolStrip();
    }

    public void Bind(DGCore.Menu.SubMenu root)
    {
      this.treeView._Bind(root.Items, o => (o is DGCore.Menu.SubMenu), o => ((DGCore.Menu.SubMenu) o).Items, o => (o is DGCore.Menu.SubMenu ? 0 : 1));
    }

    void ActionProcedure()
    {
      var mo = this.treeView.SelectedNode.Tag as DGCore.Menu.MenuOption;
      var dd = mo?.GetDataDefiniton();
      if (dd == null) return;

      DGVUtils.EndEdit(this);
      UI.frmDGV frm = new UI.frmDGV {Text = dd._description};
      Control mainForm = this.TopLevelControl;
      if (mainForm is UI.frmMDI) 
        ((UI.frmMDI)mainForm).AttachNewChildForm(frm);
      else 
        frm.Show();// First: Show; then: bind  
     
      frm.Bind(dd.GetDataSource(frm.dgv), dd.SettingID, GetParameterPresentationString(), this.cbDataSettingName.Text, null);
    }

    private string GetParameterPresentationString() {
      if (this.ucFilterDB != null && this.ucFilterDB.Visible) {
        return this.ucFilterDB.FilterList.StringPresentation;
      }
      else if (this.pg.Visible) {
          DGCore.Sql.ParameterCollection parameters = (DGCore.Sql.ParameterCollection)this.pg.SelectedObject;
        return parameters.GetStringPresentation();
      }
      return null;
    }

    private void pg_PropertyValueChanged(object s, PropertyValueChangedEventArgs e) {
        DGCore.Sql.ParameterCollection sqlParams = (DGCore.Sql.ParameterCollection)this.pg.SelectedObject;
      sqlParams.AfterValueChanged(e.ChangedItem.PropertyDescriptor.Name, e.OldValue, e.ChangedItem.Value);
      this.SetError(sqlParams.GetError());
/*      Misc.DataDefiniton dd = (Misc.DataDefiniton)this.pg.SelectedObject;
      dd.DbParameters.AfterValueChanged(e.ChangedItem.PropertyDescriptor.Name, e.OldValue, e.ChangedItem.Value);
      this.SetError(dd.DbParameters.GetError());*/
    }

    void PropertyGrid_AdjustToolStrip() {
      ToolStrip ts = null;
      foreach (Control c in this.pg.Controls) {
        if (c is ToolStrip) { ts = (ToolStrip) c; break; }
      }
      if (ts != null) {
        foreach (ToolStripItem x in ts.Items) x.Visible = false;
        ToolStripButton btn = new ToolStripButton();
        btn.Image = Properties.Resources.clock;
        btn.ImageTransparentColor = System.Drawing.Color.Magenta;
        btn.Name = "btnLoadData";
//        btn.Size = new System.Drawing.Size(123, 22);
        btn.Text = @"Завантажити дані";
        btn.Click += new System.EventHandler(this.PropertyGrid_ClickDataLoad);
        ts.Items.Add(btn);
      }
    }
    private void PropertyGrid_ClickDataLoad(object sender, EventArgs e) {
      ActionProcedure();
    }

    void SetError(string errorText) {
      this.lblError.Text=errorText;
      if (string.IsNullOrEmpty(errorText)) {
        this.tlpParameter.RowStyles[3].SizeType = SizeType.Absolute;
        this.tlpParameter.RowStyles[3].Height = 0;
      }
      else {
        this.lblError.Visible = true;
        this.tlpParameter.RowStyles[3].SizeType = SizeType.AutoSize;
      }
    }

    private void treeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e) {
      if (e.Button == MouseButtons.Right) {
        if (!(e.Node.Tag is DGCore.Menu.SubMenu)) {
          this.treeView.SelectedNode = e.Node;
          cmTreeView.Show(treeView.PointToScreen(new Point(e.X, e.Y)));
        }
      }
    }

    private void miTree_Open_Click(object sender, EventArgs e) {
      ActionProcedure();
    }

    private void miTree_Settings_Click(object sender, EventArgs e) {
      var mo = this.treeView.SelectedNode.Tag as DGCore.Menu.MenuOption;
      var dataDefiniton = mo?.GetDataDefiniton();
      DGCore.DB.DbSchemaTable st = dataDefiniton._dbCmd.GetSchemaTable();
    }

    private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
    {
      try
      {
        this.cbDataSettingName.Text = null;
        e.Node.ForeColor = System.Drawing.Color.Black;
        var mo = this.treeView.SelectedNode.Tag as DGCore.Menu.MenuOption;
        var dataDefiniton = mo?.GetDataDefiniton();
        if (dataDefiniton == null)
        {
          this.tlpParameter.Visible = false;
          this.pg.Visible = false;
          this.ucFilterDB.Visible = false;
          this.lblError.Visible = false;
        }
        else
        {
          this.tlpParameter.Visible = true;
          var userSettingProperties =
            new DGCore.UserSettings.FakeUserSettingProperties
            {
              SettingKind = DGV.DGVCube.UserSettingsKind,
              SettingKey = dataDefiniton.SettingID
            };
          if (string.IsNullOrEmpty(userSettingProperties.SettingKey))
          {
            this.lblDataSettingName.Visible = false;
            this.cbDataSettingName.Visible = false;
          }
          else
          {
            this.lblDataSettingName.Visible = true;
            this.cbDataSettingName.Visible = true;
            List<string> settingNames = DGCore.UserSettings.UserSettingsUtils.GetKeysFromDb(userSettingProperties);
            this.cbDataSettingName.Items.Clear();
            this.cbDataSettingName.Items.AddRange(settingNames.ToArray());
          }
          DGCore.Sql.ParameterCollection parameters = dataDefiniton.DbParameters;
          if (parameters == null || parameters._parameters.Count == 0)
          {
            this.pg.Visible = false;
            this.lblError.Text = null;
            DGCore.Filters.FilterList fo = dataDefiniton.WhereFilter;
            this.ucFilterDB.Visible = fo != null;
            if (fo != null) this.ucFilterDB.Bind(fo, dataDefiniton.SettingID, ActionProcedure, null);
            this.tlpParameter.RowStyles[2].Height = 100F;
            this.tlpParameter.RowStyles[4].Height = 0F;
          }
          else
          {
            this.SetError(dataDefiniton.DbParameters.GetError());
            this.ucFilterDB.Visible = false;
            this.pg.Visible = true;
            this.pg.SelectedObject = parameters;
            this.tlpParameter.RowStyles[2].Height = 0;
            this.tlpParameter.RowStyles[4].Height = 100;
            //           GetPropertyGridEntries();
          }
        }
      }
      catch (Exception ex)
      {
        e.Node.ForeColor = System.Drawing.Color.Red;
        MessageBox.Show(ex.ToString(), @"Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
      // Refresh filter/parameter label text
      lblFilter.Text = pg.Visible ? @"Параметри запиту" : (ucFilterDB.Visible ? @"Фільтр даних" : null);
    }

    private void treeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
    {
      var node = this.treeView.SelectedNode;
      if (node == e.Node)
        ActionProcedure();
    }
  }
}
