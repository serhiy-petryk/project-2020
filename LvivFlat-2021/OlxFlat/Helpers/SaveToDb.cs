using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using OlxFlat.Models;

namespace OlxFlat.Helpers
{
    public static class SaveToDb
    {
        // ===============  OLX  ==============
        public static void OlxList_Save(Dictionary<int, OlxList> items)
        {
            using (var data = new DataTable())
            {
                data.Columns.Add(new DataColumn("id", typeof(int)));
                data.Columns.Add(new DataColumn("price", typeof(int)));
                data.Columns.Add(new DataColumn("description", typeof(string)));
                data.Columns.Add(new DataColumn("location", typeof(string)));
                data.Columns.Add(new DataColumn("created", typeof(DateTime)));
                data.Columns.Add(new DataColumn("dogovirna", typeof(bool)));
                data.Columns.Add(new DataColumn("promoted", typeof(bool)));
                data.Columns.Add(new DataColumn("href", typeof(string)));
                data.Columns.Add(new DataColumn("imageref", typeof(string)));

                foreach (var item in items.Values)
                    data.Rows.Add(item.Id, item.Price, item.Description, item.Location, item.Created, item.Dogovirna,
                        item.Promoted, item.Href, item.ImageRef);


                using (var conn = new SqlConnection(Settings.DbConnectionString))
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "DELETE from [Buffer_OlxList]";
                        cmd.ExecuteNonQuery();
                    }

                    using (var sbc = new SqlBulkCopy(conn))
                    {
                        sbc.DestinationTableName = "Buffer_OlxList";
                        sbc.WriteToServer(data);
                        sbc.Close();
                    }


                    /*using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "UPDATE a set days=b.days " +
                                          "from realestate a " +
                                          "inner join tmp_RealEstate b on a.id= b.id ";
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = "INSERT INTO[dbo].[RealEstate] ([id],[url],[image],[title]," +
                                          "[address],[terms],[price],[agentType],[days],[rooms],[square],[floor],[dated]) " +
                                          "select a.[id], a.[url], a.[image], a.[title], a.[address], a.[terms], a.[price]," +
                                          "a.[agentType], a.[days], a.[rooms], a.[square], a.[floor], a.dated " +
                                          "from tmp_realestate a " +
                                          "left join RealEstate b on a.id= b.id " +
                                          "where b.id is null";
                        cmd.ExecuteNonQuery();

                    }*/
                }
            }
        }
    }
}
