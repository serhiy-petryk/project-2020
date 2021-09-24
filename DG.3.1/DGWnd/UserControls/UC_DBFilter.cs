using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace DGWnd.UserControls {
  public partial class UC_DBFilter : UserControl, /*Utils.ISettingTripleSupport,*/ DGCore.UserSettings.IUserSettingSupport<List<DGCore.UserSettings.Filter>>
  {

    string _settingKey;
    Action _actionApply;
    string _lastAppliedLayoutName;

    public UC_DBFilter() {
      InitializeComponent();
    }

    public void Bind(DGCore.Filters.FilterList newFilterList, string settingKey, Action actionApply, ICollection dataSource) {
      _settingKey = settingKey;
      this._actionApply = actionApply;
      this.btnApply.Visible = this._actionApply != null;
      this.sepApply.Visible = this._actionApply != null;
      this.ucFilter.Bind(newFilterList, dataSource);
            // Utils.SettingsTriple.Init(this, null);
      DGCore.UserSettings.UserSettingsUtils.Init(this, null);
//      Utils.SettingsTriple.Init(this, true, new ToolStripItem[] { this.btnSettingsDelete, this.btnSettingsLoad, this.btnSettingsSave, this.cbSettingList });
      this.ucFilter.Refresh();// Refrseh StringPresentation label
    }

    //============   Settings  ==================
    public string SettingKind => "DGV_DBFilter";
    public string SettingKey => _settingKey;

    List<DGCore.UserSettings.Filter> DGCore.UserSettings.IUserSettingSupport<List<DGCore.UserSettings.Filter>>.GetSettings() =>
      ((DGCore.UserSettings.IUserSettingSupport<List<DGCore.UserSettings.Filter>>)ucFilter.FilterList).GetSettings();
    List<DGCore.UserSettings.Filter> DGCore.UserSettings.IUserSettingSupport<List<DGCore.UserSettings.Filter>>.GetBlankSetting() =>
      ((DGCore.UserSettings.IUserSettingSupport<List<DGCore.UserSettings.Filter>>)ucFilter.FilterList).GetBlankSetting();

    void DGCore.UserSettings.IUserSettingSupport<List<DGCore.UserSettings.Filter>>.SetSetting(List<DGCore.UserSettings.Filter> settings) => 
      ((DGCore.UserSettings.IUserSettingSupport<List<DGCore.UserSettings.Filter>>) ucFilter.FilterList).SetSetting(settings);

//=================================

    public DGCore.Filters.FilterList FilterList => ucFilter.FilterList;

    private void btnApply_Click(object sender, EventArgs e) {
      DGCore.Utils.DGV.EndEdit(this);
      _actionApply?.Invoke();
    }

    private void btnClear_Click(object sender, EventArgs e) {
      this.ucFilter.FilterList.ClearFilter();
      DGCore.Utils.DGV.Refresh(this);
    }

    private void btnItemsCode_Click(object sender, EventArgs e) {
      IEnumerable<DGCore.Filters.FilterLineBase> oo = this.ucFilter.FilterList;
      StringBuilder sb=new StringBuilder();
      foreach (DGCore.Filters.FilterLineBase o in oo) {
        if (o.UniqueID.IndexOf(".", StringComparison.Ordinal) == -1) {
          string s =o.DisplayName;
          if (!string.IsNullOrEmpty(s) && s != o.UniqueID) {
            sb.Append("[DisplayName(\"" + s +"\")]"+ Environment.NewLine);
          }
          if (!string.IsNullOrEmpty(o.Description)) {
            sb.Append("[Description(\"" + o.Description + "\")]" + Environment.NewLine);
          }
          DGCore.Filters.FilterLine_Database o1 = o as DGCore.Filters.FilterLine_Database;
          if (o1 != null && !string.IsNullOrEmpty(o1._dbColumn.DisplayFormat)) {
            sb.Append("[BO_DbColumn(null, \"" + o1._dbColumn.DisplayFormat + "\", null)] " + Environment.NewLine);
          }
          if (o1 != null && !string.IsNullOrEmpty(o1._dbColumn.DbMasterSql)) {
            sb.Append("public object of type (" + o1._dbColumn.DbMasterSql + ") " + o.UniqueID + ";" + Environment.NewLine + Environment.NewLine);
          }
          else {
            sb.Append("public " + DGCore.Utils.Types.GetTypeCSString(o.PropertyType) + " " + o.UniqueID + ";" + Environment.NewLine + Environment.NewLine);
          }
        }
      }
      MessageBox.Show($@"Code for items is: {Environment.NewLine + sb}");
      Clipboard.SetText(sb.ToString());
    }

    private void btnSelectLayout_Click(object sender, EventArgs e) {
        DGCore.Utils.DGV.EndEdit(this);
      using (var frm = new UI.frmSelectSetting(this, _lastAppliedLayoutName)) {
        var x = frm.ShowDialog();
        if (!string.IsNullOrEmpty(frm.SelectedSetting)) {
            DGCore.UserSettings.UserSettingsUtils.SetSetting(this, frm.SelectedSetting);
          _lastAppliedLayoutName = frm.SelectedSetting;
          ucFilter.Refresh();
        }
      }
    }
    private void btnSelectLayout_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e) {
        DGCore.UserSettings.UserSettingsUtils.SetSetting(this, e.ClickedItem.Text);
      _lastAppliedLayoutName = e.ClickedItem.Text;
      DGCore.Utils.DGV.Refresh(this);
    }
    private void btnSelectLayout_DropDownOpening(object sender, EventArgs e) {
      var keys = DGCore.UserSettings.UserSettingsUtils.GetKeysFromDb(this);
      var items = new List<ToolStripMenuItem>();
      foreach (var s in keys) 
        items.Add(new ToolStripMenuItem(s));
      btnSelectLayout.DropDownItems.Clear();
      btnSelectLayout.DropDownItems.AddRange(items.ToArray());
    }

    private void btnSaveLayout_Click(object sender, EventArgs e) {
      //Utils.Dgv.EndEdit(this.ucFilter.dgvFilter);
      using (var frm = new UI.frmSaveSetting(this, _lastAppliedLayoutName)) {
        var x = frm.ShowDialog();
        if (!string.IsNullOrEmpty(frm.SelectedSetting)) {
          _lastAppliedLayoutName = frm.SelectedSetting;
        }
      }

    }
  }
}
