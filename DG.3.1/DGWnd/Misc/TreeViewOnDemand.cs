using System;
using System.Collections;
using System.Linq;
using System.Windows.Forms;

namespace DGWnd.Misc
{

  //============================================
  public class TreeViewOnDemand : TreeView
  {

    private Func<object, bool> _fnIsFolder;
    private Func<object, IEnumerable> _fnGetChilds;
    private Func<object, int> _fnGetImageIndex;

    public TreeViewOnDemand()
    {
      this.HideSelection = false; // Show selected item
      // this.LabelEdit = true;
      // this.AfterLabelEdit += TreeViewOnDemand_AfterLabelEdit;
    }

    /*private void TreeViewOnDemand_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
    {
      var oldLabel = e.Node.Text;
      var newLabel = e.Label;
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
      base.OnKeyDown(e);

      var node = this.SelectedNode;
      if (e.KeyCode == Keys.F2 && this.LabelEdit && node != null && !node.IsEditing)
        node.BeginEdit();
    }*/

    public void _Bind(IEnumerable rootItems, Func<object, bool> fnIsFolder, Func<object, IEnumerable> fnGetChilds, Func<object, int> fnGetImageIndex)
    {
      _fnIsFolder = fnIsFolder;
      _fnGetChilds = fnGetChilds;
      _fnGetImageIndex = fnGetImageIndex;

      this.Nodes.Clear();
      foreach (var o in rootItems)
      {
        this.Nodes.Add(CreateNode(o));
      }
      if (this.Nodes.Count > 0) this.SelectedNode = this.Nodes[0];
    }

    private TreeNode CreateNode(object o)
    {
      var node = new TreeNode();
      if (o == null) return node;

      node.Tag = o;
      node.Text = o.ToString();

      if (_fnIsFolder(o))
      {
        var childs = _fnGetChilds(o);
        if (childs != null && childs.Cast<object>().Any()) //Not empty submenu 
          node.Nodes.Add("");
      }
      if (_fnGetImageIndex != null)
      {
        node.ImageIndex = _fnGetImageIndex(node.Tag);
        node.SelectedImageIndex = node.ImageIndex;
      }
      return node;
    }

    // ===============   Override section  =================
    protected override void OnBeforeExpand(TreeViewCancelEventArgs e)
    {
      base.OnBeforeExpand(e);

      var o = e.Node.Tag;
      if (e.Node.Nodes.Count == 1 && e.Node.Nodes[0].Tag == null && _fnIsFolder(o))
      {
        // fill blank folder
        e.Node.Nodes.Clear();
        var childs = _fnGetChilds(o);
        e.Node.Nodes.AddRange((from object x in childs select this.CreateNode(x)).ToArray());
      }
      // Application.DoEvents();
    }
  }
}
