using System;
using System.Collections.Generic;
using System.Globalization;
using OlxFlat.Helpers;

namespace OlxFlat.Models
{
    public class OlxList
    {
        private static CultureInfo _ruCulture = new CultureInfo("ru");

        public int Id;
        public int Price;
        public string Name;
        public string Location;
        public DateTime Dated;
        public bool Dogovirna;
        public bool Promoted;
        public string Href;
        public string ImageRef;

        public OlxList(string content, DateTime fileDate)
        {
            var i1 = content.IndexOf("</table>", StringComparison.InvariantCultureIgnoreCase);
            if (i1 < 1)
                throw new Exception("Trap!!!");

            var s2 = content.Substring(0, i1);
            Promoted = s2.IndexOf("offer promoted", StringComparison.InvariantCultureIgnoreCase) > 0;
            i1 = s2.IndexOf("data-id=\"", StringComparison.InvariantCultureIgnoreCase);

            if (i1 == -1)
                throw new Exception("Trap!!!");

            var sid = Parse.ParseString_Quotes(s2, i1 + 7);
            if (sid.Length != 9)
                throw new Exception("Trap!!! Bad id");

            Id = int.Parse(sid);

            i1 = s2.IndexOf("href=\"", i1 + 7, StringComparison.InvariantCultureIgnoreCase);
            Href = Parse.ParseString_Quotes(s2, i1 + 4).Trim();

            i1 = s2.IndexOf("src=\"", i1 + 7, StringComparison.InvariantCultureIgnoreCase);
            ImageRef = Parse.ParseString_Quotes(s2, i1 + 3).Trim();

            i1 = s2.IndexOf("href=\"", i1 + 7, StringComparison.InvariantCultureIgnoreCase);
            var href2 = Parse.ParseString_Quotes(s2, i1 + 4).Trim();

            i1 = s2.IndexOf("<strong>", i1 + 7, StringComparison.InvariantCultureIgnoreCase);
            var i2 = s2.IndexOf("</strong>", i1 + 7, StringComparison.InvariantCultureIgnoreCase);
            var name = s2.Substring(i1 + 8, i2 - i1 - 8);
            Name = System.Web.HttpUtility.HtmlDecode(name).Trim();

            i1 = s2.IndexOf("class=\"price\"", i2 + 7, StringComparison.InvariantCultureIgnoreCase);
            i1 = s2.IndexOf("<strong>", i1 + 7, StringComparison.InvariantCultureIgnoreCase);
            i2 = s2.IndexOf("</strong>", i1 + 7, StringComparison.InvariantCultureIgnoreCase);
            var sPrice = s2.Substring(i1 + 8, i2 - i1 - 8).Trim();
            if (!sPrice.EndsWith("$"))
                throw new Exception("Trap! Price");
            var price = double.Parse(sPrice.Replace("$", "").Replace(" ", ""), CultureInfo.InvariantCulture);
            Price = Convert.ToInt32(price);

            var i11 = s2.IndexOf("location-filled", i2 + 7, StringComparison.InvariantCultureIgnoreCase);
            Dogovirna = s2.Substring(i1, i11 - i1).IndexOf("оговорная", StringComparison.InvariantCultureIgnoreCase) > 0;

            i1 = s2.IndexOf("</i>", i11 + 7, StringComparison.InvariantCultureIgnoreCase);
            i2 = s2.IndexOf("</span>", i1 + 4, StringComparison.InvariantCultureIgnoreCase);
            Location = s2.Substring(i1 + 4, i2 - i1 - 4).Replace("Львов, ", "").Trim().Substring(0, 5);

            i1 = s2.IndexOf("clock", i2 + 7, StringComparison.InvariantCultureIgnoreCase);
            i1 = s2.IndexOf("</i>", i1 + 7, StringComparison.InvariantCultureIgnoreCase);
            i2 = s2.IndexOf("</span>", i1 + 4, StringComparison.InvariantCultureIgnoreCase);
            var clock = s2.Substring(i1 + 4, i2 - i1 - 4).Trim();
            var created = fileDate;
            if (clock.StartsWith("Сегодня"))
            {
                var a1 = TimeSpan.Parse(clock.Substring(7).Trim(), _ruCulture);
                created = created.Add(a1);
            }
            else if (clock.StartsWith("Вчера"))
            {
                var a1 = TimeSpan.Parse(clock.Substring(5).Trim(), _ruCulture);
                created = created.Add(a1).AddDays(-1);
            }
            else
            {
                var aa1 = clock.Split(' ');
                if (aa1.Length != 3)
                    throw new Exception("Trap! Clocck");
                created = DateTime.Parse(clock, _ruCulture);
            }

            Dated = created;

            if (Href != href2)
                throw new Exception("Trap! Different href");
        }

        public void CheckEquality(OlxList otherItem)
        {
            if (Id != otherItem.Id)
                throw new Exception($"OlxList: Bad check equality - Id! Id: {Id}");
            if (Price != otherItem.Price)
                throw new Exception($"OlxList: Bad check equality - Price! Id: {Id}");
            if (Name != otherItem.Name)
                throw new Exception($"OlxList: Bad check equality - Description! Id: {Id}");
            if (Location != otherItem.Location)
                throw new Exception($"OlxList: Bad check equality - Location! Id: {Id}");
            if (Dated != otherItem.Dated)
                throw new Exception($"OlxList: Bad check equality - Created! Id: {Id}");
            if (Dogovirna != otherItem.Dogovirna)
                throw new Exception($"OlxList: Bad check equality - Dogovirna! Id: {Id}");
            if (Promoted != otherItem.Promoted)
                throw new Exception($"OlxList: Bad check equality - Promoted! Id: {Id}");
            var ss1 = Href.Split('#');
            var ss2 = otherItem.Href.Split('#');
            if (ss1[0] != ss2[0])
                throw new Exception($"OlxList: Bad check equality - Href! Id: {Id}");
            //if (ImageRef != otherItem.ImageRef)
              //  throw new Exception($"OlxList: Bad check equality - ImageRef! Id: {Id}");
        }
    }
}
