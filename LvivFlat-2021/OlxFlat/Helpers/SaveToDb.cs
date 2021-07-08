using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using OlxFlat.Models;

namespace OlxFlat.Helpers
{
    public static class SaveToDb
    {
        #region ===============  OLX  ===================

        public static void OlxDataUpdate()
        {
            using (var conn = new SqlConnection(Settings.DbConnectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    // =======  OLX
                    cmd.CommandText = "DELETE a FROM Olx a INNER JOIN Buffer_OlxList b on a.Id = b.Id INNER JOIN Buffer_OlxDetails c on a.Id = c.Id";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "INSERT INTO [dbo].[Olx] ([Id],[Location],[Name],[Price],[Building],[Kind],[Wall],[Rooms],[Floor],[Storeys],[LastFloor]," +
                                      "[Size],[Kitchen],[Heating],[Layout],[State],[Bathroom],[Dated],[Dogovirna],[Description],[Realtor],[Private]," +
                                      "[NoCommission],[Change],[Cooperate],[Appliances],[Furniture],[Promoted],Href,ImageRef)" +
                                      "SELECT a.Id, a.Location, a.Name, b.[Price], b.[Building], b.[Kind], b.[Wall],b.[Rooms],b.[Floor],b.[Storeys],b.[LastFloor]," +
                                      "b.[Size],b.[Kitchen],b.[Heating],b.[Layout],b.[State],b.[Bathroom],a.[Dated],a.[Dogovirna],b.[Description],b.[Realtor]," +
                                      "b.[Private],b.[NoCommission],b.[Change],b.[Cooperate],b.[Appliances],b.[Furniture],a.[Promoted],a.Href,a.ImageRef " +
                                      "FROM Buffer_OlxList a inner join Buffer_OlxDetails b on a.Id=b.Id";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "update a set a.RCnt = b.cnt from olx a inner join (select realtor, count(*) cnt from olx group by realtor) b on a.realtor=b.realtor";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "UPDATE [dbLvivFlat2021].[dbo].[Olx] SET BadFlag=0 WHERE BadFlag<>0";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "UPDATE [dbLvivFlat2021].[dbo].[Olx] SET BadFlag=1 WHERE BadFlag=0 AND CHARINDEX(N'без ремонт', Description)>0";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "UPDATE [dbLvivFlat2021].[dbo].[Olx] SET BadFlag=2 WHERE BadFlag=0 AND CHARINDEX(N'0-цикл', Description)>0";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "UPDATE [dbLvivFlat2021].[dbo].[Olx] SET BadFlag=2 WHERE BadFlag=0 AND CHARINDEX(N'0 цикл', Description)>0";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "UPDATE [dbLvivFlat2021].[dbo].[Olx] SET BadFlag=2 WHERE BadFlag=0 AND CHARINDEX(N'0 - цикл', Description)>0";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "UPDATE [dbLvivFlat2021].[dbo].[Olx] SET BadFlag=3 WHERE BadFlag=0 AND CHARINDEX(N'Винники', Description)>0";
                    cmd.ExecuteNonQuery();

                    // ===== Olx_ImageRef
                    cmd.CommandText = "DELETE a FROM Olx_ImageRef a LEFT JOIN Olx c on a.Id = c.Id WHERE c.Id IS NULL";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "DELETE a FROM Olx_ImageRef a INNER JOIN (SELECT Id FROM Buffer_OlxDetails_ImageRef GROUP by Id) b on a.Id = b.Id INNER JOIN Olx c on a.Id = c.Id";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "INSERT INTO [Olx_ImageRef] ([Id],[SeqNo],[Href]) SELECT a.* FROM Buffer_OlxDetails_ImageRef a INNER JOIN Olx b on a.Id=b.Id";
                    cmd.ExecuteNonQuery();

                    // ===== Olx_Params
                    cmd.CommandText = "DELETE a FROM Olx_Params a LEFT JOIN Olx c on a.Id = c.Id WHERE c.Id IS NULL";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "DELETE a FROM Olx_Params a INNER JOIN (SELECT Id FROM Buffer_OlxDetails_Params GROUP by Id) b on a.Id = b.Id INNER JOIN Olx c on a.Id = c.Id";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "INSERT INTO [Olx_Params] ([Id],[Name],[Value]) SELECT a.* FROM Buffer_OlxDetails_Params a INNER JOIN Olx b on a.Id=b.Id";
                    cmd.ExecuteNonQuery();
                }
            }

        }

            #endregion

            #region ===============  OLX details  ==================

            public static void OlxDetails_Save(IList<OlxDetails> items)
        {
            using (var data = new DataTable())
            {
                data.Columns.Add(new DataColumn("id", typeof(int)));
                data.Columns.Add(new DataColumn("Name", typeof(string)));
                data.Columns.Add(new DataColumn("price", typeof(int)));
                data.Columns.Add(new DataColumn("Building", typeof(string)));
                data.Columns.Add(new DataColumn("Kind", typeof(string)));
                data.Columns.Add(new DataColumn("Wall", typeof(string)));
                data.Columns.Add(new DataColumn("Rooms", typeof(decimal)));
                data.Columns.Add(new DataColumn("Floor", typeof(decimal)));
                data.Columns.Add(new DataColumn("Storeys", typeof(decimal)));
                data.Columns.Add(new DataColumn("LastFloor", typeof(byte)));
                data.Columns.Add(new DataColumn("Size", typeof(decimal)));
                data.Columns.Add(new DataColumn("Kitchen", typeof(decimal)));
                data.Columns.Add(new DataColumn("Heating", typeof(string)));
                data.Columns.Add(new DataColumn("Layout", typeof(string)));
                data.Columns.Add(new DataColumn("State", typeof(string)));
                data.Columns.Add(new DataColumn("Bathroom", typeof(string)));
                data.Columns.Add(new DataColumn("dated", typeof(DateTime)));
                data.Columns.Add(new DataColumn("dogovirna", typeof(bool)));
                data.Columns.Add(new DataColumn("description", typeof(string)));
                data.Columns.Add(new DataColumn("Realtor", typeof(string)));
                data.Columns.Add(new DataColumn("Private", typeof(bool)));
                data.Columns.Add(new DataColumn("NoCommission", typeof(bool)));
                data.Columns.Add(new DataColumn("Change", typeof(bool)));
                data.Columns.Add(new DataColumn("Cooperate", typeof(bool)));
                data.Columns.Add(new DataColumn("Appliances", typeof(bool)));
                data.Columns.Add(new DataColumn("Furniture", typeof(bool)));

                foreach (var item in items)
                    data.Rows.Add(item.Id, item.Name, item.Price, item.Building, item.Kind, item.Wall, item.Rooms, item.Floor, item.Storeys,
                        item.LastFloor, item.Size, item.Kitchen, item.Heating, item.Layout, item.State, item.Bathroom,
                        item.Dated, item.Dogovirna, item.Description, item.Realtor, 
                        item.Private, item.NoCommission, item.Change, item.Cooperate, item.Appliances, item.Furniture);

                using (var conn = new SqlConnection(Settings.DbConnectionString))
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "DELETE from [Buffer_OlxDetails]";
                        cmd.ExecuteNonQuery();
                    }

                    using (var sbc = new SqlBulkCopy(conn))
                    {
                        sbc.DestinationTableName = "Buffer_OlxDetails";
                        sbc.WriteToServer(data);
                        sbc.Close();
                    }
                }
            }

            using (var data = new DataTable())
            {
                data.Columns.Add(new DataColumn("id", typeof(int)));
                data.Columns.Add(new DataColumn("Name", typeof(string)));
                data.Columns.Add(new DataColumn("Value", typeof(string)));

                foreach (var item in items)
                {
                    foreach (var pp in item.Parameters)
                        data.Rows.Add(item.Id, pp.Item1, pp.Item2);
                }
                //=============
                using (var conn = new SqlConnection(Settings.DbConnectionString))
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "DELETE from [Buffer_OlxDetails_Params]";
                        cmd.ExecuteNonQuery();
                    }

                    using (var sbc = new SqlBulkCopy(conn))
                    {
                        sbc.DestinationTableName = "Buffer_OlxDetails_Params";
                        sbc.WriteToServer(data);
                        sbc.Close();
                    }
                }
            }

            //=============
            using (var data = new DataTable())
            {
                data.Columns.Add(new DataColumn("id", typeof(int)));
                data.Columns.Add(new DataColumn("seqno", typeof(int)));
                data.Columns.Add(new DataColumn("Href", typeof(string)));

                foreach (var item in items)
                {
                    var seqno = 0;
                    foreach (var href in item.ImageRefs)
                        data.Rows.Add(item.Id, seqno++, href);
                }

                using (var conn = new SqlConnection(Settings.DbConnectionString))
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "DELETE from [Buffer_OlxDetails_ImageRef]";
                        cmd.ExecuteNonQuery();
                    }

                    using (var sbc = new SqlBulkCopy(conn))
                    {
                        sbc.DestinationTableName = "Buffer_OlxDetails_ImageRef";
                        sbc.WriteToServer(data);
                        sbc.Close();
                    }
                }
            }

        }
        #endregion


        #region ===============  OLX list  ==================
        public static void OlxList_Save(IEnumerable<OlxList> items)
        {
            using (var data = new DataTable())
            {
                data.Columns.Add(new DataColumn("id", typeof(int)));
                data.Columns.Add(new DataColumn("price", typeof(int)));
                data.Columns.Add(new DataColumn("name", typeof(string)));
                data.Columns.Add(new DataColumn("location", typeof(string)));
                data.Columns.Add(new DataColumn("dated", typeof(DateTime)));
                data.Columns.Add(new DataColumn("dogovirna", typeof(bool)));
                data.Columns.Add(new DataColumn("promoted", typeof(bool)));
                data.Columns.Add(new DataColumn("href", typeof(string)));
                data.Columns.Add(new DataColumn("imageref", typeof(string)));

                foreach (var item in items)
                    data.Rows.Add(item.Id, item.Price, item.Name, item.Location, item.Dated, item.Dogovirna,
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
                }
            }
        }
        #endregion
    }
}
