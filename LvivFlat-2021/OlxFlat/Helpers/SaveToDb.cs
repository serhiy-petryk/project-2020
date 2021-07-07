﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using OlxFlat.Models;

namespace OlxFlat.Helpers
{
    public static class SaveToDb
    {
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
                data.Columns.Add(new DataColumn("Owner", typeof(string)));
                data.Columns.Add(new DataColumn("OwnerStarted", typeof(string)));
                data.Columns.Add(new DataColumn("Private", typeof(bool)));
                data.Columns.Add(new DataColumn("NoCommission", typeof(bool)));
                data.Columns.Add(new DataColumn("Change", typeof(bool)));
                data.Columns.Add(new DataColumn("Cooperate", typeof(bool)));
                data.Columns.Add(new DataColumn("Appliances", typeof(bool)));
                data.Columns.Add(new DataColumn("Furniture", typeof(bool)));

                foreach (var item in items)
                    data.Rows.Add(item.Id, item.Name, item.Price, item.Building, item.Kind, item.Wall, item.Rooms, item.Floor, item.Storeys,
                        item.LastFloor, item.Size, item.Kitchen, item.Heating, item.Layout, item.State, item.Bathroom,
                        item.Dated, item.Dogovirna, item.Description, item.Owner, item.OwnerStarted,
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
                data.Columns.Add(new DataColumn("Href", typeof(string)));

                foreach (var item in items)
                {
                    foreach (var href in item.ImageRefs)
                        data.Rows.Add(item.Id, href);
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
        #endregion
    }
}
