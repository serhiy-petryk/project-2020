using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DGWnd.Utils;

namespace DGWnd.UI {
  public partial class frmSaveSetting : Form {
    public string SelectedSetting;

    private readonly DGCore.UserSettings.IUserSettingProperties _properties;
    private readonly string _lastAppliedLayoutName;
    private static readonly Pen _penForDelete = new Pen(Color.Red, 1);

    public frmSaveSetting() {
      InitializeComponent();
    }

    public frmSaveSetting(DGCore.UserSettings.IUserSettingProperties o, string lastAppliedLayoutName)
    {
      _properties = o;
      _lastAppliedLayoutName = lastAppliedLayoutName;
      InitializeComponent();
    }

    private void frmSaveSetting_Load(object sender, EventArgs e) {
      Utils.DGVClipboard.Clipboard_Attach(dataGridView1);
      var oo = DGCore.UserSettings.UserSettingsUtils.GetUserSettingDbObjects(_properties);
      dataGridView1.DataSource = oo;
      txtNewSetting.Text = _lastAppliedLayoutName;
      RefreshData();
    }

    private void RefreshData() {
      dataGridView1.Refresh();
      btnDelete.Enabled = dataGridView1.Rows.Count > 0;
      dataGridView1.Focus();
    }

    public bool Flag { get; set; }

    private void dataGridView1_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e) {
      var dgv = (DataGridView)sender;
      var r = dgv.Rows[e.RowIndex];
      var o = (DGCore.UserSettings.UserSettingsDbObject)r.DataBoundItem;
      foreach (DataGridViewCell cell in r.Cells)
      {
        if (!o.IsEditable)
          cell.ReadOnly = true;
        cell.Style.BackColor = cell.ReadOnly ? Color.WhiteSmoke : Color.LightCyan;
      }
    }

    private void btnOK_Click(object sender, EventArgs e) {
      var settingId = txtNewSetting.Text;
      var allowEdit = cbAllowEditToOthers.Checked;
      var allowView = cbAllowViewToOthers.Checked;
      if (SaveSetting(settingId, allowView, allowEdit)) {
        SelectedSetting=settingId;
        Close();
      }
    }

    private bool SaveSetting(string settingId, bool allowView, bool allowEdit) {
      if (string.IsNullOrEmpty(settingId)) {
        MessageBox.Show(@"Налаштування не може бути пустим", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return false;
      }
      DGVUtils.EndEdit((Control)_properties);
      return DGCore.UserSettings.UserSettingsUtils.SaveNewSetting(_properties, settingId, allowView, allowEdit);
    }

    private void btnSaveSettingChanges_Click(object sender, EventArgs e) {
      var result = DGCore.UserSettings.UserSettingsUtils.SaveChangedSettings((List<DGCore.UserSettings.UserSettingsDbObject>)dataGridView1.DataSource, _properties);
      if (result == 0) {
        MessageBox.Show(@"Не було змінено жодного налаштування", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
      }
      else if (result > 0) {
        MessageBox.Show($@"Було записано {result} змінених налаштуваннь", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        Flag = true;
        var cm = (CurrencyManager)BindingContext[dataGridView1.DataSource];
        var data = dataGridView1.DataSource;
        dataGridView1.DataSource = null;
        dataGridView1.DataSource = data;
      }
      else {
        MessageBox.Show(@"Помилка! Було спроба записати налаштування, для якого Ви не маєте права це робити", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e) {
      if (e.ColumnIndex<0 || e.RowIndex<0)
        return;

      var dgv = (DataGridView)sender;
      var r = dgv.Rows[e.RowIndex];
      var o = (DGCore.UserSettings.UserSettingsDbObject)r.DataBoundItem;
      var rowColor = (o.IsDeleted ? Color.Red : (o.IsChanged ? Color.Magenta : Color.Black));
      if (r.DefaultCellStyle.ForeColor!=rowColor)
        r.DefaultCellStyle.ForeColor=rowColor;
    }

    private void btnDelete_Click(object sender, EventArgs e) {
      if (dataGridView1.CurrentRow != null) {
        var o = (DGCore.UserSettings.UserSettingsDbObject)dataGridView1.CurrentRow.DataBoundItem;
        o.IsDeleted = !o.IsDeleted;
        RefreshData();
      }
    }

    private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e) {
      if (e.ColumnIndex < 0 || e.RowIndex < 0)
        return;

      var dgv = (DataGridView)sender;
      var r = dgv.Rows[e.RowIndex];
      var o = (DGCore.UserSettings.UserSettingsDbObject)r.DataBoundItem;
      if (o.IsDeleted) {
        e.Paint(e.CellBounds, e.PaintParts);  // This will paint the cell for you
        var height=(e.CellBounds.Top + e.CellBounds.Bottom) / 2;
        e.Graphics.DrawLine(_penForDelete, new Point(e.CellBounds.Left,height ), new Point(e.CellBounds.Right, height));
        e.Handled = true;
      }
    }

    private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e) {
      if (e.RowIndex < 0)
        return;

      var o = (DGCore.UserSettings.UserSettingsDbObject) dataGridView1.Rows[e.RowIndex].DataBoundItem;
      if (btnDelete.Enabled != o.IsEditable) btnDelete.Enabled = o.IsEditable;
    }
  }
}