using System;
using System.Windows.Forms;

namespace DGWnd.Test {
  public partial class PropertyDescr : Form {
    public PropertyDescr() {
      InitializeComponent();
    }

    private void PropertyDescr_Load(object sender, EventArgs e) {
      this.propertyGrid1.SelectedObject = this;
    }

    private void button1_Click_1(object sender, EventArgs e) {
      ToolStrip toolStrip = null;
      foreach (Control control in this.propertyGrid1.Controls) {
        toolStrip = control as ToolStrip;
        if (toolStrip != null) {
          break;
        }
      }

      if (toolStrip != null) {
        ToolStripButton btn = new ToolStripButton();
        btn.Image = Properties.Resources.clock;
        btn.ImageTransparentColor = System.Drawing.Color.Magenta;
        btn.Name = "btnLoadData";
        //btn.Size = new System.Drawing.Size(123, 22);
        btn.Text = "Завантажити дані";
        btn.Click += new System.EventHandler(this.toolStripButton1_Click);
        toolStrip.Items.Add(btn);
      }


      /*ToolBar mobjToolbar =
                    (ToolBar)typeof(PropertyGrid).InvokeMember("mobjToolBar",
                                                        BindingFlags.GetField | BindingFlags.NonPublic |
                                                        BindingFlags.Instance,
                                                        null,
                                                        propertyGrid1,
                                                        null);
      ToolBarButton newButton = new ToolBarButton();
      //newButton.Image = new IconResourceHandle("Bulb.png");
      mobjToolbar.Buttons.Add(newButton);*/

    }

    private void toolStripButton1_Click(object sender, EventArgs e) {

    }
  }
}