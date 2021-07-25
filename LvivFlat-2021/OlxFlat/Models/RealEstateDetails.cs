using System;
using System.Globalization;
using System.IO;
using System.Text;
using OlxFlat.Helpers;

namespace OlxFlat.Models
{
    public class RealEstateDetails
    {
        private static CultureInfo _uaCulture = new CultureInfo("uk");

        public int Id;
        public int? Amount;
        public string Building;
        public string Wall;
        public string State;
        public int? Rooms;
        public decimal? Size;
        public decimal? Living;
        public decimal? Kitchen;
        public int? Floor;
        public int? Floors;
        public string Height;
        public int? Balconies;
        public DateTime? Dated;
        public string Description;
        public bool NotFound;

        public int? RealtorId;
        public string Realtor;

        public RealEstateDetails(string filename)
        {
            var s = File.ReadAllText(filename, Encoding.UTF8);
            var fileDate = File.GetLastWriteTime(filename).Date;

            var ss1 = Path.GetFileNameWithoutExtension(filename).Split('_');
            Id = int.Parse(ss1[ss1.Length - 1]);
            if (s == "NotFound")
            {
                NotFound = true;
                return;
            }

            var k1 = s.IndexOf("<div class=\"mb-3\">", StringComparison.InvariantCultureIgnoreCase);
            var k2 = s.IndexOf("</div>", k1 + 18, StringComparison.InvariantCultureIgnoreCase);
            Description = ConvertHTMLToPlainText.StripHTML(s.Substring(k1 + 18, k2 - k1 - 18)).Replace("\r\r", "\r").Trim();

            ss1 = s.Substring(k2).Split(new[] {"<li class=\"col-sm-6 col-dense-left\">"}, StringSplitOptions.None);
            for (var k = 1; k < ss1.Length; k++)
            {
                k1 = ss1[k].IndexOf("</li>", StringComparison.InvariantCultureIgnoreCase);
                var s1 = ss1[k].Substring(0, k1).Trim();
                k1 = s1.IndexOf(":", StringComparison.InvariantCultureIgnoreCase);
                var name = s1.Substring(0, k1).TrimEnd();
                var value = s1.Substring(k1 + 1).Trim();

                if (name == "Ціна")
                {
                }
                else if (name == "Ціна $")
                {
                    if (!value.EndsWith("$"))
                        throw new Exception($"RealEstateDetails. 'Ціна $' value error: '{value}'");
                    if (value.StartsWith("від"))
                        value = value.Substring(3).Trim();
                    Amount = int.Parse(value.Substring(0, value.Length - 1).Trim(), NumberStyles.AllowThousands, _uaCulture);
                }
                else if (name == "Кімнат")
                    Rooms = int.Parse(value);
                else if (name == "Загальна площа")
                {
                    if (!value.EndsWith("кв.м"))
                        throw new Exception($"RealEstateDetails. 'Загальна площа' value error: '{value}'");
                    if (value.StartsWith("від"))
                        value = value.Substring(3).Trim();
                    Size = decimal.Parse(value.Substring(0, value.Length - 4).Trim(), CultureInfo.InvariantCulture);
                }
                else if (name == "Житлова площа")
                {
                    if (!value.EndsWith("кв.м"))
                        throw new Exception($"RealEstateDetails. 'Загальна Житлова площа' value error: '{value}'");
                    if (value.StartsWith("від"))
                        value = value.Substring(3).Trim();
                    Living = decimal.Parse(value.Substring(0, value.Length - 4).Trim(), CultureInfo.InvariantCulture);
                }
                else if (name == "Площа кухні")
                {
                    if (!value.EndsWith("кв.м"))
                        throw new Exception($"RealEstateDetails. 'Площа кухні' value error: '{value}'");
                    if (value.StartsWith("від"))
                        value = value.Substring(3).Trim();
                    Kitchen = decimal.Parse(value.Substring(0, value.Length - 4).Trim(), CultureInfo.InvariantCulture);
                }
                else if (name == "Поверх")
                    Floor = int.Parse(value);
                else if (name == "Поверховість")
                    Floors = int.Parse(value);
                else if (name == "Висота стелі")
                {
                    if (!value.EndsWith("м"))
                        throw new Exception($"RealEstateDetails. 'Висота стелі' value error: '{value}'");
                    Height = value.Substring(0, value.Length - 1).Trim();
                    /*value = value.Substring(0, value.Length - 1).Trim();
                    if (value.EndsWith("м."))
                        Height = decimal.Parse(value.Substring(0, value.Length - 2).Trim(), CultureInfo.InvariantCulture);
                    else if (value.EndsWith("м"))
                        Height = decimal.Parse(value.Substring(0, value.Length - 1).Trim(), CultureInfo.InvariantCulture);
                    else if (value.EndsWith("|") || value.EndsWith("і"))
                        Height = decimal.Parse(value.Substring(0, value.Length - 1).Trim(), CultureInfo.InvariantCulture) / 100m;
                    else
                    {
                        value = value.Replace("/", ".").Replace("-", ".");
                        Height = decimal.Parse(value, CultureInfo.InvariantCulture);
                    }*/
                }
                else if (name == "Днів на сайті") { }
                else if (name == "Оновлено")
                    Dated = DateTime.Parse(value, _uaCulture);
                else if (name == "Код") { }
                else if (name == "Матеріал стін")
                    Wall = value;
                else if (name == "Стан")
                    State = value;
                else if (name == "Тип будівлі")
                    Building = value;
                else if (name == "Балконів")
                    Balconies = int.Parse(value);
                else
                    throw new Exception($"RealEstateDetails. Unknown parameter name: {name}");
            }

            var s2 = ss1[ss1.Length - 1];
            k1 = s2.IndexOf("href=\"/profile/", StringComparison.InvariantCultureIgnoreCase);
            if (k1 > 0) // realtor data may be obsolete
            {
                k2 = s2.IndexOf("/", k1 + 16, StringComparison.InvariantCultureIgnoreCase);
                RealtorId = int.Parse(s2.Substring(k1 + 15, k2 - k1 - 15));

                k1 = s2.IndexOf("href=\"/profile/", k2, StringComparison.InvariantCultureIgnoreCase);
                k1 = s2.IndexOf(">", k1 + 15, StringComparison.InvariantCultureIgnoreCase);
                k2 = s2.IndexOf("</", k1, StringComparison.InvariantCultureIgnoreCase);
                Realtor = System.Web.HttpUtility.HtmlDecode(s2.Substring(k1 + 1, k2 - k1 - 1)).Trim();
            }
        }
    }
}
