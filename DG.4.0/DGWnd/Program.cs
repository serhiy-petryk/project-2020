using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using DGCore.Helpers;
using DGWnd.Misc;
using DGWnd.Utils;

namespace DGWnd {
  public class Program {

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main() {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);

      // Test.SettingsReplacement.Test1();
      // Application.Run(new Test.PropertyDescr());

      DGCore.Common.Shared.MessageBoxProxy = new MessageBoxProxy();
      TypeDescriptor.AddAttributes(typeof(CheckState), new TypeConverterAttribute(typeof(CheckStateConverter)));

      try
      {
        Application.Run(new UI.frmMDI());
      }
      catch (Exception ex)
      {
        MessageBox.Show($@"ERROR!{Environment.NewLine}{ex}");
      }
    }

    static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
    {
      if (e.Exception is System.Data.ConstraintException)
      {
        var thread = (Thread)sender;
        System.Data.ConstraintException e1 = (System.Data.ConstraintException)e.Exception;
        MessageBox.Show(e.Exception.ToString());
      }
      else
      {
        MessageBox.Show(e.Exception.ToString());
        //Application.Exit();
      }
    }

  }
}
