using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using DGWnd.Utils;

namespace DGWnd.UserControls {
  public partial class UC_DGVLayout : UserControl {

    private DGCore.UserSettings.DGV _settings;
    private DGCore.UserSettings.IUserSettingSupport<DGCore.UserSettings.DGV> _targetObject;

    public UC_DGVLayout() {
      InitializeComponent();
    }

    public void Bind(DGV.DGVCube dgv, DGCore.UserSettings.DGV settings)
    {
      _targetObject = dgv;
      _settings = settings;

      // May be whereFilter will repalce dgv.DataSource.WhereFilter
      // var whereFilter = new Filters.FilterList(dgv.DataSource.Properties);
      // ((UserSettings.IUserSettingSupport<List<UserSettings.Filter>>)whereFilter).SetSetting(settings.WhereFilter);

      ucColumnSettings.Bind(settings, dgv.DataSource.Properties);
      ucItemFilter.Bind(dgv.DataSource.WhereFilter, dgv.DataSource.UnderlyingData.GetData(false));
    }

    // ==========   Actions  =============
    private void btnApply_Click(object sender, EventArgs e) {
        DGVUtils.EndEdit(this);
      ucColumnSettings.ApplySettings(_settings);
      _settings.WhereFilter = ((DGCore.UserSettings.IUserSettingSupport<List<DGCore.UserSettings.Filter>>) ucItemFilter.FilterList).GetSettings();
      _targetObject.SetSetting(_settings);
    }

    private void btnClearFilter_Click(object sender, EventArgs e) {
      this.ucItemFilter.FilterList.ClearFilter();
      DGVUtils.Refresh(this);
    }

    private void btnItemsCode_Click(object sender, EventArgs e) {
      IEnumerable<DGCore.Filters.FilterLineBase> oo = this.ucItemFilter.FilterList;
      StringBuilder sb = new StringBuilder();
      foreach (DGCore.Filters.FilterLineBase o in oo) {
        if (!o.UniqueID.Contains(".")) {
          sb.Append("public " + DGCore.Utils.Types.GetTypeCSString(o.PropertyType) + " " + o.UniqueID + ";" + Environment.NewLine);
        }
      }
      Clipboard.SetText(sb.ToString());
      MessageBox.Show(@"Code for items is:" + Environment.NewLine + sb.ToString());
    }

  }
}
