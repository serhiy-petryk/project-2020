using System;
using System.Windows.Forms;

namespace DGWnd.UI {
  public partial class frmSelectSetting : Form {
    public string SelectedSetting;

    private DGCore.UserSettings.IUserSettingProperties _properties;
    private readonly string _lastAppliedLayoutName;

    public frmSelectSetting() {
      InitializeComponent();
    }

    public frmSelectSetting(DGCore.UserSettings.IUserSettingProperties properties, string lastAppliedLayoutName)
    {
      _properties = properties;
      _lastAppliedLayoutName = lastAppliedLayoutName;
      InitializeComponent();
    }

    private void frmSaveSetting_Load(object sender, EventArgs e) {
      Utils.DGVClipboard.Clipboard_Attach(dataGridView1);
      var oo = DGCore.UserSettings.UserSettingsUtils.GetUserSettingDbObjects(_properties);
      dataGridView1.DataSource = oo;
      dataGridView1.Focus();
      btnApply.Enabled = dataGridView1.CurrentRow != null;
    }

    private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e) {
      if (e.RowIndex < 0)
        return;

      var o = (DGCore.UserSettings.UserSettingsDbObject)dataGridView1.Rows[e.RowIndex].DataBoundItem;
      SelectedSetting = o.SettingId;
      Close();
    }

    private void btnApply_Click(object sender, EventArgs e) {
      if (dataGridView1.CurrentRow == null) {
        MessageBox.Show(@"Не можливо визначити активний рядок", @"Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
      else {
        var o = (DGCore.UserSettings.UserSettingsDbObject)dataGridView1.CurrentRow.DataBoundItem;
        SelectedSetting = o.SettingId;
        Close();
      }
    }

  }
}