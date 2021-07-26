using System;
using System.Collections.Generic;
using System.Globalization;
using OlxFlat;

namespace RealEstateFlat.Models
{
    public class RealEstateList
    {
        private static CultureInfo _uaCulture = new CultureInfo("uk");

        private static Dictionary<string, string> _districts = new Dictionary<string, string>
        {
            {"Сихівський район", "Сихів"}, {"Галицький район", "Галич"}, {"Шевченківський район", "Шевчен"},
            {"Франківський район", "Франк"}, {"Личаківський район", "Личак"}, {"Залізничний район", "Залізн"}
        };
        //==============================
        public int Id;
        public string District;
        public string Building;
        public int Amount;
        public int Rooms;
        public int Size;
        public int? Living;
        public int? Kitchen;
        public int Floor;
        public int Floors;
        public string Address;
        public DateTime Dated;
        public bool? Private;
        public string Href;
        public decimal Latitude;
        public decimal Longitude;
        public bool? VIP;

        public RealEstateList(string content, DateTime fileDate)
        {
            var i1 = content.IndexOf("pagination", StringComparison.InvariantCultureIgnoreCase);
            if (i1 > 0)
                content = content.Substring(0, i1);

            i1 = content.IndexOf("data-id=\"", StringComparison.InvariantCultureIgnoreCase);
            var i2 = content.IndexOf("\"", i1+9, StringComparison.InvariantCultureIgnoreCase);
            Id =int.Parse(content.Substring(i1 + 9, i2 - i1-9));

            i1 = content.IndexOf("data-lat=\"", StringComparison.InvariantCultureIgnoreCase);
            i2 = content.IndexOf("\"", i1 + 10, StringComparison.InvariantCultureIgnoreCase);
            Latitude = decimal.Parse(content.Substring(i1 + 10, i2 - i1 - 10), CultureInfo.InstalledUICulture);

            i1 = content.IndexOf("data-lng=\"", StringComparison.InvariantCultureIgnoreCase);
            i2 = content.IndexOf("\"", i1 + 10, StringComparison.InvariantCultureIgnoreCase);
            Longitude = decimal.Parse(content.Substring(i1 + 10, i2 - i1 - 10), CultureInfo.InstalledUICulture);

            i1 = content.IndexOf("href=\"", i2, StringComparison.InvariantCultureIgnoreCase);
            i2 = content.IndexOf("\"", i1 + 6, StringComparison.InvariantCultureIgnoreCase);
            Href = content.Substring(i1 + 6, i2 - i1 - 6);

            i1 = content.IndexOf(">VIP</", i2, StringComparison.InvariantCultureIgnoreCase);
            if (i1 > 0) VIP = true;

            i1 = content.IndexOf("object-listing-price", i2, StringComparison.InvariantCultureIgnoreCase);
            i2 = content.IndexOf("</", i1 + 20, StringComparison.InvariantCultureIgnoreCase);
            var i21 = content.LastIndexOf(">", i2, StringComparison.InvariantCultureIgnoreCase);
            var s2 = content.Substring(i21 + 1, i2 - i21 - 1).Trim();
            Amount = int.Parse(s2, NumberStyles.AllowThousands, _uaCulture);

            var i3 = content.IndexOf("</", i2 + 2, StringComparison.InvariantCultureIgnoreCase);
            var i31 = content.LastIndexOf(">", i3, StringComparison.InvariantCultureIgnoreCase);
            var currency = content.Substring(i31 + 1, i3 - i31 - 1).Trim();
            if (currency == "$"){}
            else if (currency == "грн") 
                Amount =Convert.ToInt32(Amount / Settings._usRate);
            else if (currency == "евро") 
                Amount = Convert.ToInt32(Amount * Settings._euroRate);
            else 
                throw new Exception("Trap!!!");

            i2 = content.IndexOf("<span ", i3, StringComparison.InvariantCultureIgnoreCase);
            while (i2 > 0)
            {
                i21 = content.LastIndexOf(">", i2+5, StringComparison.InvariantCultureIgnoreCase);
                i3 = content.IndexOf(">", i2 + 5, StringComparison.InvariantCultureIgnoreCase);
                i31 = content.IndexOf("</", i2 + 5, StringComparison.InvariantCultureIgnoreCase);
                var kind = content.Substring(i3 + 1, i31 - i3 - 1).Trim();
                if (kind == "кімн")
                    Rooms = int.Parse(content.Substring(i21 + 1, i2 - i21 - 1).Trim());
                else if (kind == "кв.м")
                {
                    var ss1 = content.Substring(i21 + 1, i2 - i21 - 1).Trim().Split('/');
                    Size = int.Parse(ss1[0].Trim());
                    if (ss1.Length == 3)
                        Living = int.Parse(ss1[1].Trim());
                    if (ss1.Length > 1)
                        Kitchen = int.Parse(ss1[ss1.Length - 1].Trim());
                }
                else if (kind == "пов")
                {
                    var ss1 = content.Substring(i21 + 1, i2 - i21 - 1).Trim().Split('/');
                    Floor = int.Parse(ss1[0].Trim());
                    Floors = int.Parse(ss1[1].Trim());
                }
                else
                    throw new Exception("Trap!!!");

                i2 = content.IndexOf("<span ", i2+5, StringComparison.InvariantCultureIgnoreCase);
            }

            i1 = content.LastIndexOf("object-listing-address", StringComparison.InvariantCultureIgnoreCase);
            i2 = content.IndexOf(">", i1 + 20, StringComparison.InvariantCultureIgnoreCase);
            i21 = content.IndexOf("</", i1 + 20, StringComparison.InvariantCultureIgnoreCase);
            var s1 = content.Substring(i2 + 1, i21 - i2 - 1).Trim();
            var ss2 = content.Substring(i2 + 1, i21 - i2 - 1).Trim().Split(new []{ "Львів," }, StringSplitOptions.None);
            for (var k = 0; k < ss2.Length; k++) ss2[k] = ss2[k].Trim();
            if (ss2[0].EndsWith(","))
                ss2[0] = ss2[0].Substring(0, ss2[0].Length - 1).Trim();
            Address = System.Web.HttpUtility.HtmlDecode(ss2[0]).Trim();
            District = _districts[ss2[1]];

            i1 = content.IndexOf("object-listing-text", i21, StringComparison.InvariantCultureIgnoreCase);
            i2 = content.IndexOf(">", i1 + 20, StringComparison.InvariantCultureIgnoreCase);
            i21 = content.IndexOf("</", i1 + 20, StringComparison.InvariantCultureIgnoreCase);
            s2 = content.Substring(i2 + 1, i21 - i2 - 1).Trim();
            if (s2.StartsWith("Продаж квартири,"))
                Building = s2.Substring(16).Trim();
            else if (s2.StartsWith("Продаж квартири")) { }
            else
                throw new Exception("Trap!!!");

            i1 = content.IndexOf("object-listing-text", i21, StringComparison.InvariantCultureIgnoreCase);
            i2 = content.IndexOf(">", i1 + 20, StringComparison.InvariantCultureIgnoreCase);
            i21 = content.IndexOf("</", i1 + 20, StringComparison.InvariantCultureIgnoreCase);
            ss2 = content.Substring(i2 + 1, i21 - i2 - 1).Trim().Split('•');
            for (var k = 0; k < ss2.Length; k++) ss2[k] = ss2[k].Trim();
            if (ss2[0].EndsWith("днів на сайті"))
            {
                var days = int.Parse(ss2[0].Substring(0, ss2[0].Length - 13).Trim());
                Dated = fileDate.AddDays(-days);
            }
            else
                throw new Exception("Trap!!!");

            if (ss2[1] == "від агента" || ss2[1] == "from.") { }
            else if (ss2[1] == "від власника") Private = true;
            else
                throw new Exception("Trap!!!");
        }

        public void CheckDuplicate(RealEstateList item)
        {
            if (!Equals(District, item.District))
                throw new Exception($"Check district. Id: {Id}");
            if (!Equals(Building, item.Building))
                throw new Exception($"Check Building. Id: {Id}");
            if (!Equals(Amount, item.Amount))
                throw new Exception($"Check Amount. Id: {Id}");
            if (!Equals(Rooms, item.Rooms))
                throw new Exception($"Check Rooms. Id: {Id}");
            if (!Equals(Size, item.Size))
                throw new Exception($"Check Size. Id: {Id}");
            if (!Equals(Living, item.Living))
                throw new Exception($"Check Living. Id: {Id}");
            if (!Equals(Kitchen, item.Kitchen))
                throw new Exception($"Check Kitchen. Id: {Id}");
            if (!Equals(Floor, item.Floor))
                throw new Exception($"Check Floor. Id: {Id}");
            if (!Equals(Floors, item.Floors))
                throw new Exception($"Check Floors. Id: {Id}");
            if (!Equals(Address, item.Address))
                throw new Exception($"Check Address. Id: {Id}");
            if (!Equals(Dated, item.Dated))
                throw new Exception($"Check Dated. Id: {Id}");
            if (!Equals(Private, item.Private))
                throw new Exception($"Check Private. Id: {Id}");
            if (!Equals(Href, item.Href))
                throw new Exception($"Check Href. Id: {Id}");
            if (!Equals(Latitude, item.Latitude))
                throw new Exception($"Check Latitude. Id: {Id}");
            if (!Equals(Longitude, item.Longitude))
                throw new Exception($"Check Longitude. Id: {Id}");
            //if (!Equals(VIP, item.VIP))
              //  throw new Exception($"Check VIP. Id: {Id}");
        }
    }
}
