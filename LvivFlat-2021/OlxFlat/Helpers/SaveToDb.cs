using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using OlxFlat.Models;
using RealEstateFlat.Models;

namespace OlxFlat.Helpers
{
    public static class SaveToDb
    {
        private static string[] _zeroStates = {
            "Нульовий цикл", "нулевой цикл", "нульовому цикл", "0-й цикл", "О-й цикл", "0-цикл", "0 цикл", "0 - цикл",
            "\"О\"-цикл", "0-му цикл", "0-ий цикл", "О цикл", "О-цикл", "0 -цикл", "0ий цикл", "о- цикл", "Нульвий цикл",
            "0цикл", "0 –ий цикл", "О -Цикл", "0- цикл"
        };

        #region ===============  RealEstate  =================

        public static void RealEstateDataUpdate()
        {
            using (var conn = new SqlConnection(Settings.DbConnectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE from RealEstate where Id in (SELECT a.Id FROM Buffer_RealEstateList AS a INNER JOIN Buffer_RealEstateDetails AS b ON a.Id = b.Id)";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "INSERT INTO [dbo].[RealEstate] ([Id],[Comment],[District],[Building],[Wall],[State],[Amount],[Rooms],[Size],[Living],[Kitchen]," +
                                      "[Floor],[Floors],[Address],[Dated],[Description],[Height],[Balconies],[Private],[Href],[Latitude],[Longitude],[VIP],[NotFound],[RealtorId],[Realtor]) " +
                                      @"SELECT a.Id, iif(b.Moved <> 0, '- Moved', iif(b.NotFound <> 0, '- NotFound', null)) comment, a.District, a.Building, b.Wall, b.State, " +
                                      @"ISNULL(b.Amount, a.Amount) AS Amount, ISNULL(b.Rooms, a.Rooms) AS Rooms, ISNULL(b.Size, a.Size) AS Size, ISNULL(b.Living, a.Living) AS Living, " +
                                      @"ISNULL(b.Kitchen, a.Kitchen) AS Kitchen, ISNULL(b.Floor, a.Floor) AS Floor, ISNULL(b.Floors, a.Floors) AS Floors, a.Address, " +
                                      @"ISNULL(b.Dated, a.Dated) AS Dated, b.Description, b.Height, b.Balconies, a.Private, a.Href, a.Latitude, a.Longitude, a.VIP, b.NotFound, b.RealtorId, b.Realtor " +
                                      @"FROM Buffer_RealEstateList AS a INNER JOIN Buffer_RealEstateDetails AS b ON a.Id = b.Id";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "UPDATE RealEstate SET LastFloor = IIF(Floor = Floors, 1, 0)";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "update a set a.RCnt = b.cnt from RealEstate a inner join (select RealtorId, count(*) cnt from RealEstate group by RealtorId) b on a.realtorId=b.realtorId";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "UPDATE RealEstate SET BadFlag=0 WHERE BadFlag<>0";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "UPDATE RealEstate SET BadFlag=1 WHERE BadFlag=0 AND CHARINDEX(N'без ремонт', Description)>0";
                    cmd.ExecuteNonQuery();

                    foreach (var s in _zeroStates)
                    {
                        cmd.CommandText = $"UPDATE RealEstate SET BadFlag=2 WHERE BadFlag=0 AND CHARINDEX(N'{s}', Description)>0";
                        cmd.ExecuteNonQuery();
                    }

                    cmd.CommandText = "UPDATE RealEstate SET BadFlag=3 WHERE BadFlag=0 AND CHARINDEX(N'Винник', Description)>0";
                    cmd.ExecuteNonQuery();

                    //update realestate set comment='- old' where dated<DATEADD(month, -1, getdate()) and comment is null;
                }
            }
        }

        #endregion

        #region ===============  RealEstate details  ==================
        public static void RealEstateDetails_Save(IEnumerable<RealEstateDetails> items)
        {
            using (var data = new DataTable())
            {
                data.Columns.Add(new DataColumn("Id", typeof(int)));
                data.Columns.Add(new DataColumn("Amount", typeof(int)));
                data.Columns.Add(new DataColumn("Building", typeof(string)));
                data.Columns.Add(new DataColumn("Wall", typeof(string)));
                data.Columns.Add(new DataColumn("State", typeof(string)));
                data.Columns.Add(new DataColumn("Rooms", typeof(int)));
                data.Columns.Add(new DataColumn("Size", typeof(decimal)));
                data.Columns.Add(new DataColumn("Living", typeof(decimal)));
                data.Columns.Add(new DataColumn("Kitchen", typeof(decimal)));
                data.Columns.Add(new DataColumn("Floor", typeof(int)));
                data.Columns.Add(new DataColumn("Floors", typeof(int)));
                data.Columns.Add(new DataColumn("Height", typeof(string)));
                data.Columns.Add(new DataColumn("Balconies", typeof(int)));
                data.Columns.Add(new DataColumn("Dated", typeof(DateTime)));
                data.Columns.Add(new DataColumn("Description", typeof(string)));
                data.Columns.Add(new DataColumn("NotFound", typeof(bool)));
                data.Columns.Add(new DataColumn("Moved", typeof(bool)));
                data.Columns.Add(new DataColumn("RealtorId", typeof(int)));
                data.Columns.Add(new DataColumn("Realtor", typeof(string)));

                foreach (var item in items)
                {
                    data.Rows.Add(item.Id, item.Amount, item.Building, item.Wall, item.State, item.Rooms, item.Size,
                        item.Living, item.Kitchen, item.Floor, item.Floors, item.Height, item.Balconies, item.Dated,
                        item.Description, item.NotFound, item.Moved, item.RealtorId, item.Realtor);
                }

                using (var conn = new SqlConnection(Settings.DbConnectionString))
                using (var cmd = conn.CreateCommand())
                {
                    conn.Open();

                    cmd.CommandText = "DELETE from [Buffer_RealEstateDetails]";
                    cmd.ExecuteNonQuery();

                    using (var sbc = new SqlBulkCopy(conn))
                    {
                        sbc.DestinationTableName = "Buffer_RealEstateDetails";
                        sbc.WriteToServer(data);
                        sbc.Close();
                    }
                }
            }
        }
        #endregion

        #region ===============  RealEstate list  ==================
        public static void RealEstateList_Save(IEnumerable<RealEstateList> items)
        {
            using (var data = new DataTable())
            {
                data.Columns.Add(new DataColumn("Id", typeof(int)));
                data.Columns.Add(new DataColumn("District", typeof(string)));
                data.Columns.Add(new DataColumn("Building", typeof(string)));
                data.Columns.Add(new DataColumn("Amount", typeof(int)));
                data.Columns.Add(new DataColumn("Rooms", typeof(int)));
                data.Columns.Add(new DataColumn("Size", typeof(int)));
                data.Columns.Add(new DataColumn("Living", typeof(int)));
                data.Columns.Add(new DataColumn("Kitchen", typeof(int)));
                data.Columns.Add(new DataColumn("Floor", typeof(int)));
                data.Columns.Add(new DataColumn("Floors", typeof(int)));
                data.Columns.Add(new DataColumn("Address", typeof(string)));
                data.Columns.Add(new DataColumn("Dated", typeof(DateTime)));
                data.Columns.Add(new DataColumn("Private", typeof(bool)));
                data.Columns.Add(new DataColumn("Href", typeof(string)));
                data.Columns.Add(new DataColumn("Latitude", typeof(decimal)));
                data.Columns.Add(new DataColumn("Longitude", typeof(decimal)));
                data.Columns.Add(new DataColumn("VIP", typeof(bool)));
                foreach (var item in items)
                    data.Rows.Add(item.Id, item.District, item.Building, item.Amount, item.Rooms, item.Size,
                        item.Living, item.Kitchen, item.Floor, item.Floors, item.Address, item.Dated, item.Private,
                        item.Href, item.Latitude, item.Longitude, item.VIP);


                using (var conn = new SqlConnection(Settings.DbConnectionString))
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "DELETE from [Buffer_RealEstateList]";
                        cmd.ExecuteNonQuery();
                    }

                    using (var sbc = new SqlBulkCopy(conn))
                    {
                        sbc.DestinationTableName = "Buffer_RealEstateList";
                        sbc.WriteToServer(data);
                        sbc.Close();
                    }
                }
            }
        }
        #endregion

        #region ===============  DomRia  ===================
        public static Dictionary<int, object> DomRia_GetExistingIds()
        {
            var existingIds = new Dictionary<int, object>();
            using (var conn = new SqlConnection(Settings.DbConnectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "select Id from DomRia";
                    using (var rdr = cmd.ExecuteReader())
                        while (rdr.Read())
                            existingIds.Add((int)rdr["Id"], null);
                }
            }

            return existingIds;
        }

        public static void DomRiaDetails_Save(IEnumerable<DomRiaDetails> items)
        {
            using (var data = new DataTable())
            {
                data.Columns.Add(new DataColumn("Id", typeof(int)));
                data.Columns.Add(new DataColumn("Comment", typeof(string)));
                data.Columns.Add(new DataColumn("District", typeof(string)));
                data.Columns.Add(new DataColumn("Description", typeof(string)));
                data.Columns.Add(new DataColumn("Price", typeof(int)));
                data.Columns.Add(new DataColumn("Building", typeof(string)));
                data.Columns.Add(new DataColumn("Heating", typeof(string)));
                data.Columns.Add(new DataColumn("Wall", typeof(string)));
                data.Columns.Add(new DataColumn("Rooms", typeof(int)));
                data.Columns.Add(new DataColumn("Floor", typeof(int)));
                data.Columns.Add(new DataColumn("Storeys", typeof(int)));
                data.Columns.Add(new DataColumn("LastFloor", typeof(byte)));
                data.Columns.Add(new DataColumn("Size", typeof(decimal)));
                data.Columns.Add(new DataColumn("Kitchen", typeof(decimal)));
                data.Columns.Add(new DataColumn("Living", typeof(decimal)));
                data.Columns.Add(new DataColumn("Dated", typeof(DateTime)));
                data.Columns.Add(new DataColumn("Inspected", typeof(DateTime)));
                data.Columns.Add(new DataColumn("Url", typeof(string)));
                data.Columns.Add(new DataColumn("Realtor", typeof(string)));
                data.Columns.Add(new DataColumn("Realtor_ok", typeof(bool)));
                data.Columns.Add(new DataColumn("Street", typeof(string)));
                data.Columns.Add(new DataColumn("BuildingNo", typeof(string)));
                data.Columns.Add(new DataColumn("Deleted", typeof(int)));
                data.Columns.Add(new DataColumn("Obmin", typeof(string)));
                data.Columns.Add(new DataColumn("Torg", typeof(bool)));
                data.Columns.Add(new DataColumn("Rozstrochka", typeof(bool)));
                data.Columns.Add(new DataColumn("Dogovirna", typeof(bool)));
                data.Columns.Add(new DataColumn("Propozycia", typeof(string)));
                data.Columns.Add(new DataColumn("Longitude", typeof(double)));
                data.Columns.Add(new DataColumn("Latitude", typeof(double)));

                var existingIds = DomRia_GetExistingIds();
                using (var conn = new SqlConnection(Settings.DbConnectionString))
                {
                    conn.Open();
                    foreach (var item in items)
                        if (!existingIds.ContainsKey(item.realty_id))
                        {
                            data.Rows.Add(item.realty_id, null, item.district_name_uk, item.Description, item.Price,
                                item.Building, item.Heating, item.wall_type_uk, item.rooms_count, item.floor, item.floors_count,
                                item.LastFloor, item.total_square_meters, item.kitchen_square_meters, item.living_square_meters,
                                item.publishing_date, item.inspected_at, item.beautiful_url, item.user_id, item.realtorVerified,
                                item.street_name_uk, item.building_number_str, item.delete_reason, item.Obmin, item.Torg,
                                item.Rozstrochka, item.Dogovirna, item.Propozycia, item.longitude, item.latitude);
                        }

                    using (var sbc = new SqlBulkCopy(conn))
                    {
                        sbc.DestinationTableName = "DomRia";
                        sbc.WriteToServer(data);
                        sbc.Close();
                    }
                }
            }
        }
        #endregion

        #region ===============  OLX  ===================

        public static void OlxDataUpdate()
        {
            using (var conn = new SqlConnection(Settings.DbConnectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
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

                    foreach (var s in _zeroStates)
                    {
                        cmd.CommandText = $"UPDATE Olx SET BadFlag=2 WHERE BadFlag=0 AND CHARINDEX(N'{s}', Description)>0";
                        cmd.ExecuteNonQuery();
                    }

                    cmd.CommandText = "UPDATE [dbLvivFlat2021].[dbo].[Olx] SET BadFlag=3 WHERE BadFlag=0 AND CHARINDEX(N'Винник', Description)>0";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "UPDATE [dbLvivFlat2021].[dbo].[Olx] SET GoodFlag=0 WHERE GoodFlag<>0";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "UPDATE [dbLvivFlat2021].[dbo].[Olx] SET GoodFlag=1 WHERE GoodFlag=0 AND (charindex(N'індив', Description)>0 OR charindex(N'индив', Description)>0)";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "UPDATE [dbLvivFlat2021].[dbo].[Olx] SET GoodFlag=2 WHERE GoodFlag=0 AND (charindex(N'кондиц', Description)>0 OR Id in (SELECT distinct ID from Olx_Params WHERE charindex(N'кондиц', VALUE)>0))";
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
