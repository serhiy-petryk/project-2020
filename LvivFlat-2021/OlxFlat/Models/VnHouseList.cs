using System;
using System.Collections.Generic;
using System.IO;
using OlxFlat.Helpers;

namespace OlxFlat.Models
{
    public class VnHouseList
    {

        public string Id;
        public string Name;
        public string City;
        public string Address;
        public string Price;
        public string Status;
        public string Class;
        public string Type;
        public bool? Recommended;
        public bool? GoodPrice;

        public VnHouseList (string content)
        {
            var i1 = content.IndexOf("\"building-card__title\"", StringComparison.InvariantCultureIgnoreCase);
            var i2 = content.Substring(0, i1).LastIndexOf(@"//vn.com.ua/ua/complex/", StringComparison.InvariantCultureIgnoreCase);
            var i3 = content.IndexOf("\"", i2 + 1, StringComparison.InvariantCultureIgnoreCase);
            Id = content.Substring(i2 + 23, i3 - i2 - 23).Trim();

            i2 = content.IndexOf(">", i1, StringComparison.InvariantCultureIgnoreCase);
            i3 = content.IndexOf("</", i2, StringComparison.InvariantCultureIgnoreCase);
            Name = content.Substring(i2 + 1, i3 - i2 - 1).Trim();

            i1 = content.IndexOf("building-card__address", i3, StringComparison.InvariantCultureIgnoreCase);
            i2 = content.IndexOf(">", i1, StringComparison.InvariantCultureIgnoreCase);
            i3 = content.IndexOf("</", i2, StringComparison.InvariantCultureIgnoreCase);
            Address = content.Substring(i2 + 1, i3 - i2 - 1).Trim();
            i1 = Address.IndexOf(",", StringComparison.InvariantCultureIgnoreCase);
            City = i1>0 ? Address.Substring(0, i1).Trim() : Address;

            i1 = content.IndexOf("building-card__prices", i3, StringComparison.InvariantCultureIgnoreCase);
            i1 = content.IndexOf(">", i1, StringComparison.InvariantCultureIgnoreCase);
            i2 = content.IndexOf("building-card__footer", i1, StringComparison.InvariantCultureIgnoreCase);
            var s1 = content.Substring(i1+1, i2 - i1-1);
            i3 = s1.LastIndexOf("</", StringComparison.InvariantCultureIgnoreCase);
            var s11 = s1.Substring(0, i3);
            var s12 = ConvertHTMLToPlainText.StripHTML(s11);
            var sr = new StringReader(s12);
            var s2 = sr.ReadLine();
            var ss1 = new List<string>();
            while (s2 != null)
            {
                if (!string.IsNullOrEmpty(s2) && !s2.Contains("грн"))
                    ss1.Add(s2.Trim());
                s2 = sr.ReadLine();
            }

            if (ss1.Count > 0)
                Price = string.Join(" ", ss1.ToArray());

            i1 = content.IndexOf(@"building-card-infographic/", i2, StringComparison.InvariantCultureIgnoreCase);
            while (i1 > 0)
            {
                i2 = content.IndexOf(".", i1, StringComparison.InvariantCultureIgnoreCase);
                var type = content.Substring(i1 + 26, i2 - i1 - 26).Trim();

                i1 = content.IndexOf("building-card__infographic-name", i2, StringComparison.InvariantCultureIgnoreCase);
                i2 = content.IndexOf(">", i1, StringComparison.InvariantCultureIgnoreCase);
                i3 = content.IndexOf("</", i2, StringComparison.InvariantCultureIgnoreCase);
                var name = content.Substring(i2 + 1, i3 - i2 - 1).Trim();

                if (!string.IsNullOrEmpty(name))
                {
                    if (type == "status")
                        Status = name;
                    else if (type == "class")
                        Class = name;
                    else if (type == "type")
                        Type = name;
                    else if (type == "reliability")
                        Recommended = true;
                    else
                        throw new Exception($"Error in VnHouse constructor. '{type}' type doesn't defined");
                }

                i1 = content.IndexOf(@"building-card-infographic/", i3, StringComparison.InvariantCultureIgnoreCase);
            }

            if (content.IndexOf(">Відмінна ціна</", StringComparison.InvariantCultureIgnoreCase) > 0)
                GoodPrice = true;
        }
    }
}
