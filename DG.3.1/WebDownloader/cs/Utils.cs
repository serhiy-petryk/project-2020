using System.Diagnostics;
using System.Windows.Forms;

namespace WebDownloader {
  class csUtils {

    public static void MDIAttachForm(Form childForm) {
      foreach (Form frm in Application.OpenForms) {
        if (frm.GetType().Name == "frmMDI")
        {
          var mi = frm.GetType().GetMethod("AttachNewChildForm");
          mi.Invoke(frm, new object[] {childForm});
          // ((frmMDI)frm).AttachNewChildForm(childForm);
          return;
        }
      }
      childForm.Show(); // Not child form
    }

    public static Process AsynRunCmd(string cmdLine) {
      ProcessStartInfo psi = new ProcessStartInfo("cmd.exe", "/C " + cmdLine);
      psi.WorkingDirectory = csIni.pathExe;
      psi.CreateNoWindow = true;
      psi.UseShellExecute = false;
      return Process.Start(psi);
    }
  }
}
