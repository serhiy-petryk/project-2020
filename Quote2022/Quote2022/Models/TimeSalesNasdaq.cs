using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace Quote2022.Models
{
    public class TimeSalesNasdaq
    {
        public static bool? IsEqual(Sale[] oo1, Sale[] oo2)
        {
            if (oo1 == null || oo2 == null) return null;
            if (oo1.Length != oo2.Length) return false;
            for (var k = 0; k < oo1.Length; k++)
            {
                if (!oo1[k].IsEqual(oo2[k]))
                    return false;
            }

            return true;
        }

        //========================================
        public Data data;
        public Status status;

        public bool CouldBeReload
        {
            get
            {
                if (status.bCodeMessage != null)
                    foreach (var eroor in status.bCodeMessage.Where(a => a.code != 1001))
                        return true;
                return false;
            }
        }

        public Sale[] GetSales()
        {
            if (data == null) return null;

            var sales = new Sale[data.rows.Length];
            for (var k = 0; k < sales.Length; k++)
                sales[k]=new Sale(data.rows[k]);
            return sales;
        }

        public class Sale
        {
            public TimeSpan Time;
            public float Price;
            public int Volume;

            private static CultureInfo usCulture = new CultureInfo("en-US");
            public Sale(DataRow row)
            {
                Time = TimeSpan.Parse(row.nlsTime, CultureInfo.InvariantCulture);
                Price = float.Parse(row.nlsPrice, NumberStyles.Any, usCulture);
                Volume = int.Parse(row.nlsShareVolume, NumberStyles.Any, usCulture);
            }

            public bool IsEqual(Sale o)
            {
                return TimeSpan.Equals(Time, o.Time) && float.Equals(Price, o.Price) && int.Equals(Volume, o.Volume);
            }

            public override string ToString() => Time.ToString() + " " + Price.ToString(usCulture) + " " + Volume.ToString();
        }

        public class Data
        {
            public string symbol;
            public int totalRecords;//	:	185
            public int offset;//	:	0
            public int limit;//	:	200000
            public DataRow headers;
            public DataRow[] rows;
            public TopTable topTable;
            public Description description;
            public string[] message;
        }

        public class DataRow
        {
            public string nlsTime;//	:	09:59:59
            public string nlsPrice;//	:	$ 148.66
            public string nlsShareVolume;//	:	100

        }
        public class TopTable
        {
            public TopTableRow headers;
            public TopTableRow[] rows;
        }

        public class TopTableRow
        {
            public string nlsVolume; //	:	0
            public string previousClose; //	:	$149.01
            // public decimal previousClose; //	:	$149.01
            public string todayHighLow; //	:	N/A
            public string fiftyTwoWeekHighLow; //	:	
        }
        public class Description
        {
            public string message;
            public string url;
        }
        public class Status
        {
            public int rCode;
            public CodeMessage[] bCodeMessage;
            public string developerMessage;
        }

        public class CodeMessage
        {
            public int code;
            public string errorMessage;
        }
    }

}
