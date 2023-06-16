using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;

namespace DGWnd.Test
{
  public class SettingsReplacement
  {
    public static void Test1()
    {
      List<Settings> data=new List<Settings>();
      using (var conn = new SqlConnection("initial catalog=dbOneSAP_DW;Pooling=false;Data Source=localhost;Integrated Security=SSPI;Encrypt=false"))
      using (var cmd = new SqlCommand("SELECT * from DGV_SettingsTriple", conn))
      {
        conn.Open();
        using (var rdr = cmd.ExecuteReader())
          while (rdr.Read())
          {
            var o = new Settings(rdr);
            if (o.Kind == "DGV_Setting")
            {
              
            }
            data.Add(o);
          }
      }
    }

    public class Settings
    {
      public string Kind;
      public string Key;
      public string Id;
      public byte[] Data;

      public Settings(IDataRecord r)
      {
        Kind = r.GetString(0);
        Key = r.GetString(1);
        Id = r.GetString(2);
        Data = (byte[])r.GetValue(3);
      }
    }

  }
}
