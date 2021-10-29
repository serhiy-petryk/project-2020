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
            "0цикл", "0 –ий цикл", "О -Цикл", "0- цикл", "0-й цикл", "переуступ"
        };

        private static string[] _isCO =
        {
            "Централізоване опал", "Центральне опал", "Опалення центр", "бойлер", " будинковий лічил",
            " ЦО,", ",ЦО,", ".ЦО,", (char) 13 + "ЦО,",
            " ЦО.", ",ЦО.", ".ЦО.", (char) 13 + "ЦО.",
            " ЦО;", ",ЦО;", ".ЦО;", (char) 13 + "ЦО;",
            " ЦО ", ",ЦО ", ".ЦО ", (char) 13 + "ЦО "
        };

        #region ===============  VN  ===================
        public static void VnDataUpdate()
        {
            using (var conn = new SqlConnection(Settings.DbConnectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandTimeout = 150;
                    cmd.CommandText = "UPDATE a SET Name = b.Name, Count = b.Count, Year = b.Year, Finished = b.Finished, InProgress = b.InProgress, InSale = b.InSale, "
                                      +"Rank = b.Rank, RankCount = b.RankCount, Dated = b.Dated FROM VN_Developers AS a INNER JOIN vBuffer_VN_Developers AS b ON a.Id = b.Id ";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "INSERT INTO [dbo].[VN_Developers] (Id, Name, Count, Year, Finished, InProgress, InSale, Rank, RankCount, Dated) " +
                                      "SELECT a.Id, a.Name, a.Count, a.Year, a.Finished, a.InProgress, a.InSale, a.Rank, a.RankCount, a.Dated " +
                                      "FROM vBuffer_VN_Developers a LEFT JOIN VN_Developers b on a.Id=b.Id WHERE b.Id IS NULL";
                    cmd.ExecuteNonQuery();
                }
            }
        }

        #endregion

        #region ===============  VN House list  ==================
        public static void VN_House_Details_Save(IEnumerable<VnHouseDetails> items)
        {
            using (var data = new DataTable())
            {
                data.Columns.Add(new DataColumn("Id", typeof(string)));
                data.Columns.Add(new DataColumn("Name", typeof(string)));
                data.Columns.Add(new DataColumn("Status", typeof(string)));
                data.Columns.Add(new DataColumn("City", typeof(string)));
                data.Columns.Add(new DataColumn("Address", typeof(string)));
                data.Columns.Add(new DataColumn("HasLayout", typeof(bool)));
                data.Columns.Add(new DataColumn("Amount", typeof(int)));
                data.Columns.Add(new DataColumn("Price1", typeof(int)));
                data.Columns.Add(new DataColumn("Price2", typeof(int)));
                data.Columns.Add(new DataColumn("Reliable", typeof(bool)));
                data.Columns.Add(new DataColumn("Finished", typeof(string)));
                data.Columns.Add(new DataColumn("InProgress", typeof(string)));
                data.Columns.Add(new DataColumn("Rank", typeof(decimal)));
                data.Columns.Add(new DataColumn("RankCount", typeof(int)));
                data.Columns.Add(new DataColumn("DevId", typeof(string)));
                data.Columns.Add(new DataColumn("DevName", typeof(string)));
                data.Columns.Add(new DataColumn("DevYear", typeof(short)));
                data.Columns.Add(new DataColumn("DevFinished", typeof(short)));
                data.Columns.Add(new DataColumn("DevInProgress", typeof(short)));
                data.Columns.Add(new DataColumn("DevInSale", typeof(short)));
                data.Columns.Add(new DataColumn("Class", typeof(string)));
                data.Columns.Add(new DataColumn("Houses", typeof(short)));
                data.Columns.Add(new DataColumn("Floors", typeof(string)));
                data.Columns.Add(new DataColumn("Technology", typeof(string)));
                data.Columns.Add(new DataColumn("Walls", typeof(string)));
                data.Columns.Add(new DataColumn("Warming", typeof(string)));
                data.Columns.Add(new DataColumn("Heating", typeof(string)));
                data.Columns.Add(new DataColumn("Height", typeof(string)));
                data.Columns.Add(new DataColumn("Rooms", typeof(string)));
                data.Columns.Add(new DataColumn("Flats", typeof(string)));
                data.Columns.Add(new DataColumn("Size", typeof(string)));
                data.Columns.Add(new DataColumn("Yard", typeof(string)));
                data.Columns.Add(new DataColumn("Condition", typeof(string)));
                data.Columns.Add(new DataColumn("Parking", typeof(string)));
                data.Columns.Add(new DataColumn("Dated", typeof(DateTime)));

                foreach (var item in items)
                {
                    data.Rows.Add(item.Id, item.Name, item.Status, item.City, item.Address, item.HasLayout, item.Amount,
                        item.Price1, item.Price2, item.Reliable, item.Finished, item.InProgress, item.Rank,
                        item.RankCount, item.DevId, item.DevName, item.DevYear, item.DevFinished, item.DevInProgress,
                        item.DevInSale, item.Class, item.Houses, item.Floors, item.Technology, item.Walls, item.Warming,
                        item.Heating, item.Height, item.Rooms, item.Flats, item.Size, item.Yard, item.Condition,
                        item.Parking, item.Dated);
                }

                using (var conn = new SqlConnection(Settings.DbConnectionString))
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandTimeout = 150;
                    conn.Open();

                    cmd.CommandText = "DELETE from [Buffer_VN_House_Details]";
                    cmd.ExecuteNonQuery();

                    using (var sbc = new SqlBulkCopy(conn))
                    {
                        sbc.BulkCopyTimeout = 300;
                        sbc.DestinationTableName = "Buffer_VN_House_Details";
                        sbc.WriteToServer(data);
                        sbc.Close();
                    }
                }
            }
        }
        #endregion

        #region ===============  VN House list  ==================
        public static void VN_House_List_Save(IEnumerable<VnHouseList> items)
        {
            using (var data = new DataTable())
            {
                data.Columns.Add(new DataColumn("Id", typeof(string)));
                data.Columns.Add(new DataColumn("Name", typeof(string)));
                data.Columns.Add(new DataColumn("City", typeof(string)));
                data.Columns.Add(new DataColumn("Address", typeof(string)));
                data.Columns.Add(new DataColumn("Price", typeof(string)));
                data.Columns.Add(new DataColumn("Status", typeof(string)));
                data.Columns.Add(new DataColumn("Class", typeof(string)));
                data.Columns.Add(new DataColumn("Type", typeof(string)));
                data.Columns.Add(new DataColumn("Recommended", typeof(bool)));
                data.Columns.Add(new DataColumn("GoodPrice", typeof(bool)));
                foreach (var item in items)
                    data.Rows.Add(item.Id, item.Name, item.City, item.Address, item.Price, item.Status, item.Class,
                        item.Type, item.Recommended, item.GoodPrice);


                using (var conn = new SqlConnection(Settings.DbConnectionString))
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandTimeout = 150;
                        cmd.CommandText = "DELETE from [Buffer_VN_House_List]";
                        cmd.ExecuteNonQuery();
                    }

                    using (var sbc = new SqlBulkCopy(conn))
                    {
                        sbc.BulkCopyTimeout = 150;
                        sbc.DestinationTableName = "Buffer_VN_House_List";
                        sbc.WriteToServer(data);
                        sbc.Close();
                    }
                }
            }
        }
        #endregion

        #region ===============  RealEstate  =================

        public static void RealEstateDataUpdate()
        {
            using (var conn = new SqlConnection(Settings.DbConnectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandTimeout = 150;
                    cmd.CommandText = "INSERT INTO [dbo].[RealEstate] ([Id],[Comment],[District],[Building],[Wall],[State],[Amount],[FirstAmount],[Rooms],[Size],[Living],[Kitchen]," +
                                      "[Floor],[Floors],[Address],[Dated],[Description],[Height],[Balconies],[Private],[Href],[Latitude],[Longitude],[VIP],[NotFound],[RealtorId],[Realtor]) " +
                                      @"SELECT a.Id, iif(b.Moved <> 0, '-- Moved', iif(b.NotFound <> 0, '-- NotFound', null)) comment, a.District, a.Building, b.Wall, b.State, " +
                                      @"ISNULL(b.Amount, a.Amount) AS Amount, ISNULL(b.Amount, a.Amount) AS FirstAmount, ISNULL(b.Rooms, a.Rooms) AS Rooms, ISNULL(b.Size, a.Size) AS Size, ISNULL(b.Living, a.Living) AS Living, " +
                                      @"ISNULL(b.Kitchen, a.Kitchen) AS Kitchen, ISNULL(b.Floor, a.Floor) AS Floor, ISNULL(b.Floors, a.Floors) AS Floors, a.Address, " +
                                      @"ISNULL(b.Dated, a.Dated) AS Dated, b.Description, b.Height, b.Balconies, a.Private, a.Href, a.Latitude, a.Longitude, a.VIP, b.NotFound, b.RealtorId, b.Realtor " +
                                      @"FROM Buffer_RealEstateList AS a INNER JOIN Buffer_RealEstateDetails AS b ON a.Id = b.Id";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "UPDATE a SET Amount=b.Amount FROM RealEstate a INNER JOIN Buffer_RealEstateList b ON a.id=b.id WHERE a.Amount<>b.Amount";
                    cmd.ExecuteNonQuery();
                }

                SetBadFlag("RealEstate");
                SetIsCO("RealEstate");
                SetIsPanel("RealEstate");

                Misc.UpdateDistance("RealEstate");
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
                    cmd.CommandTimeout = 150;
                    conn.Open();

                    cmd.CommandText = "DELETE from [Buffer_RealEstateDetails]";
                    cmd.ExecuteNonQuery();

                    using (var sbc = new SqlBulkCopy(conn))
                    {
                        sbc.BulkCopyTimeout = 300;
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
                        cmd.CommandTimeout = 150;
                        cmd.CommandText = "DELETE from [Buffer_RealEstateList]";
                        cmd.ExecuteNonQuery();
                    }

                    using (var sbc = new SqlBulkCopy(conn))
                    {
                        sbc.BulkCopyTimeout = 150;
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
                    cmd.CommandTimeout = 150;
                    cmd.CommandText = "select Id from DomRia";
                    using (var rdr = cmd.ExecuteReader())
                        while (rdr.Read())
                            existingIds.Add((int)rdr["Id"], null);
                }
            }

            return existingIds;
        }

        public static void DomRiaDataUpdate()
        {
            using (var conn = new SqlConnection(Settings.DbConnectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandTimeout = 150;
                    /*cmd.CommandText = "UPDATE DomRia SET Missing=1 WHERE Missing<>1";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "UPDATE a SET Missing=0 FROM domria a inner join Buffer_DomRiaDetails b on a.Id=b.Id";
                    cmd.ExecuteNonQuery();*/

                    // cmd.CommandText = "UPDATE a SET Amount=b.Amount FROM domria a inner join Buffer_DomRiaDetails b on a.Id=b.Id WHERE a.Amount<>b.Amount";
                    // cmd.ExecuteNonQuery();

                    cmd.CommandText = "INSERT INTO DomRia ([Id],[Comment],[District],[Description],[Amount],[Building],[Heating],[Wall],[Rooms],[Floor],[Floors],[LastFloor]," +
                                      "[Size],[Kitchen],[Living],[Dated],[Address],[Inspected],[Url],[Realtor],[RealtorVerified],[Street],[BuildingNo],[Deleted],[Obmin],[Torg],"+
                                      "[Rozstrochka],[Dogovirna],[Propozycia],[Longitude],[Latitude],[RealtorName],[AgencyId],[AgencyName],[AgencyType],[AgencyUserId],"+
                                      "[BuildId],[BuildName],[DeveloperId],[FlatCount],[BuildPrice],[BuildUserId],[BuildStreet],[BuildReady],[BuildClass],[BuildClassText],"+
                                      "[BuildEnd],[BuildHostName],[FirstAmount]) " +
                                      "SELECT a.[Id],null,a.[District],a.[Description],a.[Amount],a.[Building],a.[Heating],a.[Wall],a.[Rooms],a.[Floor],a.[Floors],a.[LastFloor]," +
                                      "a.[Size],a.[Kitchen],a.[Living],a.[Dated],a.[Address],a.[Inspected],a.[Url],a.[Realtor],a.[RealtorVerified],a.[Street],a.[BuildingNo],a.[Deleted],a.[Obmin],a.[Torg]," +
                                      "a.[Rozstrochka],a.[Dogovirna],a.[Propozycia],a.[Longitude],a.[Latitude],a.[RealtorName],a.[AgencyId],a.[AgencyName],a.[AgencyType],a.[AgencyUserId]," +
                                      "a.[BuildId],a.[BuildName],a.[DeveloperId],a.[FlatCount],a.[BuildPrice],a.[BuildUserId],a.[BuildStreet],a.[BuildReady],a.[BuildClass],a.[BuildClassText]," +
                                      "a.[BuildEnd],a.[BuildHostName],a.[Amount] " +
                                      "FROM Buffer_DomRiaDetails a LEFT JOIN DomRia b ON a.Id=b.Id WHERE b.Id IS NULL";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "UPDATE a SET RealtorName=b.RealtorName from DomRia a INNER JOIN (SELECT Realtor, max(isnull(RealtorName,'')) RealtorName " +
                                      "FROM DomRia WHERE RealtorName is not null GROUP BY Realtor) b on a.Realtor=b.Realtor WHERE a.RealtorName is null";
                    cmd.ExecuteNonQuery();
                }

                SetBadFlag("DomRia");
                Misc.UpdateDistance("DomRia");
            }
        }

        public static void DomRiaDetails_Save(IEnumerable<DomRiaDetails> items)
        {
            using (var data = new DataTable())
            {
                data.Columns.Add(new DataColumn("Id", typeof(int)));
                data.Columns.Add(new DataColumn("District", typeof(string)));
                data.Columns.Add(new DataColumn("Description", typeof(string)));
                data.Columns.Add(new DataColumn("Amount", typeof(int)));
                data.Columns.Add(new DataColumn("Building", typeof(string)));
                data.Columns.Add(new DataColumn("Heating", typeof(string)));
                data.Columns.Add(new DataColumn("Wall", typeof(string)));
                data.Columns.Add(new DataColumn("Rooms", typeof(int)));
                data.Columns.Add(new DataColumn("Floor", typeof(int)));
                data.Columns.Add(new DataColumn("Floors", typeof(int)));
                data.Columns.Add(new DataColumn("LastFloor", typeof(byte)));
                data.Columns.Add(new DataColumn("Size", typeof(decimal)));
                data.Columns.Add(new DataColumn("Kitchen", typeof(decimal)));
                data.Columns.Add(new DataColumn("Living", typeof(decimal)));
                data.Columns.Add(new DataColumn("Dated", typeof(DateTime)));
                data.Columns.Add(new DataColumn("Address", typeof(string)));
                data.Columns.Add(new DataColumn("Inspected", typeof(DateTime)));
                data.Columns.Add(new DataColumn("Url", typeof(string)));
                data.Columns.Add(new DataColumn("Realtor", typeof(string)));
                data.Columns.Add(new DataColumn("RealtorName", typeof(string)));
                data.Columns.Add(new DataColumn("RealtorVerified", typeof(bool)));
                data.Columns.Add(new DataColumn("Street", typeof(string)));
                data.Columns.Add(new DataColumn("BuildingNo", typeof(string)));
                data.Columns.Add(new DataColumn("Deleted", typeof(int)));
                data.Columns.Add(new DataColumn("Obmin", typeof(string)));
                data.Columns.Add(new DataColumn("Torg", typeof(bool)));
                data.Columns.Add(new DataColumn("Rozstrochka", typeof(bool)));
                data.Columns.Add(new DataColumn("Dogovirna", typeof(bool)));
                data.Columns.Add(new DataColumn("Propozycia", typeof(string)));
                data.Columns.Add(new DataColumn("Longitude", typeof(decimal)));
                data.Columns.Add(new DataColumn("Latitude", typeof(decimal)));
                data.Columns.Add(new DataColumn("AgencyId", typeof(int)));
                data.Columns.Add(new DataColumn("AgencyName", typeof(string)));
                data.Columns.Add(new DataColumn("AgencyType", typeof(int)));
                data.Columns.Add(new DataColumn("AgencyUserId", typeof(int)));
                data.Columns.Add(new DataColumn("BuildId", typeof(int)));
                data.Columns.Add(new DataColumn("BuildName", typeof(string)));
                data.Columns.Add(new DataColumn("DeveloperId", typeof(int)));
                data.Columns.Add(new DataColumn("FlatCount", typeof(int)));
                data.Columns.Add(new DataColumn("BuildPrice", typeof(int)));
                data.Columns.Add(new DataColumn("BuildUserId", typeof(int)));
                data.Columns.Add(new DataColumn("NoLegalStatus", typeof(int)));
                data.Columns.Add(new DataColumn("BuildStreet", typeof(string)));
                data.Columns.Add(new DataColumn("BuildReady", typeof(bool)));
                data.Columns.Add(new DataColumn("BuildClass", typeof(int)));
                data.Columns.Add(new DataColumn("BuildClassText", typeof(string)));
                data.Columns.Add(new DataColumn("BuildEnd", typeof(DateTime)));
                data.Columns.Add(new DataColumn("BuildHostName", typeof(string)));

                foreach (var item in items)
                {
                    data.Rows.Add(item.realty_id, item.district_name_uk, item.Description, item.Amount, item.Building,
                        item.Heating, item.wall_type_uk, item.rooms_count, item.floor, item.floors_count,
                        item.LastFloor, item.total_square_meters, item.kitchen_square_meters, item.living_square_meters,
                        item.publishing_date, item.Address, item.inspected_at, item.beautiful_url, item.user_id,
                        item.realtorName, item.realtorVerified, item.street_name_uk, item.building_number_str,
                        item.delete_reason, item.Obmin, item.Torg, item.Rozstrochka, item.Dogovirna, item.Propozycia,
                        item.longitude, item.latitude, item.agencyId, item.agencyName, item.agencyType,
                        item.agencyUserId, item.buildId, item.buildName, item.developerId, item.flatCount,
                        item.buildPrice, item.buildUserId, item.noLegalStatus, item.buildStreet, item.buildReady,
                        item.buildClass, item.buildClassText, item.buildEnd, item.buildHostName);
                }

                using (var conn = new SqlConnection(Settings.DbConnectionString))
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandTimeout = 150;
                        cmd.CommandText = "DELETE FROM Buffer_DomRiaDetails";
                        cmd.ExecuteNonQuery();
                    }
                    using (var sbc = new SqlBulkCopy(conn))
                    {
                        sbc.BulkCopyTimeout = 150;
                        sbc.DestinationTableName = "Buffer_DomRiaDetails";
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
                    cmd.CommandTimeout = 150;
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
                        cmd.CommandTimeout = 150;
                        cmd.CommandText = "DELETE from [Buffer_OlxDetails]";
                        cmd.ExecuteNonQuery();
                    }

                    using (var sbc = new SqlBulkCopy(conn))
                    {
                        sbc.BulkCopyTimeout = 150;
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
                        cmd.CommandTimeout = 150;
                        cmd.CommandText = "DELETE from [Buffer_OlxDetails_Params]";
                        cmd.ExecuteNonQuery();
                    }

                    using (var sbc = new SqlBulkCopy(conn))
                    {
                        sbc.BulkCopyTimeout = 150;
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
                        cmd.CommandTimeout = 150;
                        cmd.CommandText = "DELETE from [Buffer_OlxDetails_ImageRef]";
                        cmd.ExecuteNonQuery();
                    }

                    using (var sbc = new SqlBulkCopy(conn))
                    {
                        sbc.BulkCopyTimeout = 150;
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
                        cmd.CommandTimeout = 150;
                        cmd.CommandText = "DELETE from [Buffer_OlxList]";
                        cmd.ExecuteNonQuery();
                    }

                    using (var sbc = new SqlBulkCopy(conn))
                    {
                        sbc.BulkCopyTimeout = 150;
                        sbc.DestinationTableName = "Buffer_OlxList";
                        sbc.WriteToServer(data);
                        sbc.Close();
                    }
                }
            }
        }
        #endregion

        private static void SetBadFlag(string tableName)
        {
            using (var conn = new SqlConnection(Settings.DbConnectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandTimeout = 300;
                    cmd.CommandText = $"UPDATE {tableName} SET Bad=null WHERE Bad is NOT NULL";
                    cmd.ExecuteNonQuery();

                    string s1 = null;
                    foreach (var s in _zeroStates)
                        s1 += $" OR CHARINDEX(N'{s}', Description)>0";
                    cmd.CommandText = $"UPDATE {tableName} SET Bad='0 цикл' WHERE Bad IS NULL AND ({s1.Substring(3)})";
                    cmd.ExecuteNonQuery();

                    /*foreach (var s in _zeroStates)
                    {
                        cmd.CommandText = $"UPDATE {tableName} SET Bad='0 цикл' WHERE Bad IS NULL AND CHARINDEX(N'{s}', Description)>0";
                        cmd.ExecuteNonQuery();
                    }*/

                    cmd.CommandText = $"UPDATE {tableName} SET Bad='без ремонт' WHERE Bad IS NULL AND CHARINDEX(N'без ремонт', Description)>0";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = $"UPDATE {tableName} SET Bad='Винники' WHERE Bad IS NULL AND CHARINDEX(N'Винники', Description)>0";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = $"UPDATE {tableName} SET Bad='2022' WHERE Bad IS NULL AND CHARINDEX(N'2022', Description)>0";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = $"UPDATE {tableName} SET Bad='2023' WHERE Bad IS NULL AND CHARINDEX(N'2023', Description)>0";
                    cmd.ExecuteNonQuery();
                }
            }
        }
        private static void SetIsCO(string tableName)
        {
            using (var conn = new SqlConnection(Settings.DbConnectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandTimeout = 150;
                    cmd.CommandText = $"UPDATE {tableName} SET IsCO=null WHERE IsCO is NOT NULL";
                    cmd.ExecuteNonQuery();

                    string s1 = null;
                    foreach (var s in _isCO)
                        s1 += $" OR CHARINDEX(N'{s}', Description)>0";

                    cmd.CommandText = $"UPDATE {tableName} SET IsCO=1 WHERE IsCO IS NULL AND ({s1.Substring(3)})";
                    cmd.ExecuteNonQuery();
                }
            }
        }
        private static void SetIsPanel(string tableName)
        {
            using (var conn = new SqlConnection(Settings.DbConnectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandTimeout = 150;
                    cmd.CommandText = $"UPDATE {tableName} SET IsPanel=null WHERE IsPanel is NOT NULL";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = $"UPDATE {tableName} SET IsPanel = 1 where WALL=N'панель' AND (Building=N'Чешка' OR Building=N'Хрущовка')";
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
