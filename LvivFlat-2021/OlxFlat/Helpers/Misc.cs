using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace OlxFlat.Helpers
{
    public static class Misc
    {
        internal const double LvivLat = 49.841976;
        internal const double LvivLong = 24.030865;

        public static void UpdateDistance(string tableName)
        {
            var data = new Dictionary<int, double>();
            using (var conn = new SqlConnection(Settings.DbConnectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $"select Id, Latitude, Longitude from {tableName} WHERE Distance IS NULL and Latitude IS NOT NULL and Longitude IS NOT NULL";
                    using (var rdr = cmd.ExecuteReader())
                        while (rdr.Read())
                        {
                            var lat = (decimal)rdr["Latitude"];
                            var lon = (decimal)rdr["Longitude"];
                            if (lat > 0.01m && lon > 0.01m)
                                data.Add((int)rdr["Id"], GetDistance(Convert.ToDouble(lat), Convert.ToDouble(lon)));
                        }

                    foreach (var kvp in data)
                    {
                        cmd.CommandText = $"UPDATE {tableName} SET Distance={Convert.ToInt32(kvp.Value * 1000)} WHERE Id={kvp.Key}";
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public static double GetDistance(double latitutde, double longitude)
        {
            double theDistance = (Math.Sin(DegreesToRadians(latitutde)) *
                                  Math.Sin(DegreesToRadians(LvivLat)) +
                                  Math.Cos(DegreesToRadians(latitutde)) *
                                  Math.Cos(DegreesToRadians(LvivLat)) *
                                  Math.Cos(DegreesToRadians(longitude - LvivLong)));

            return (RadiansToDegrees(Math.Acos(theDistance))) * 69.09 * 1.6093;
        }

        private static double CalcDistance(double latA, double longA, double latB, double longB)
        {

            double theDistance = (Math.Sin(DegreesToRadians(latA)) *
                                  Math.Sin(DegreesToRadians(latB)) +
                                  Math.Cos(DegreesToRadians(latA)) *
                                  Math.Cos(DegreesToRadians(latB)) *
                                  Math.Cos(DegreesToRadians(longA - longB)));

            return (RadiansToDegrees(Math.Acos(theDistance))) * 69.09 * 1.6093;
        }

        public static double DegreesToRadians(double d) => d * Math.PI / 180;
        public static double RadiansToDegrees(double d) => Convert.ToDouble(d) / Math.PI * 180;
    }
}
