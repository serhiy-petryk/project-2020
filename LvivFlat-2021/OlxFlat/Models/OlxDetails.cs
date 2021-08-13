using OlxFlat.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace OlxFlat.Models
{
    public class OlxDetails
    {
        public static Dictionary<string, int> _param = new Dictionary<string, int>();
        internal static int _olxMax1 = 0;
        internal static int _olxMax2 = 0;
        internal static int _olxMax3 = 0;
        internal static int _olxMax4 = 0;
        internal static int _olxMax5 = 0;
        internal static int _olxMax6 = 0;

        private static CultureInfo _ruCulture = new CultureInfo("ru");

        public int Id;
        public string Name;
        public int Price;
        public DateTime Dated;
        public bool Dogovirna;
        public string Description;
        public List<string> ImageRefs = new List<string>();
        public List<(string, string)> Parameters = new List<(string, string)>();
        public string Realtor;

        public bool Private;
        public bool NoCommission;
        public bool Change;
        public bool Cooperate;
        public bool Appliances;
        public decimal Rooms;
        public bool? Furniture;
        public decimal Size;
        public decimal Kitchen;
        public string Heating;
        public string Layout;
        public string State;
        public string Bathroom;
        public string Building;
        public string Kind;
        public string Wall;
        public decimal Floor;
        public decimal Storeys;
        public bool LastFloor => Floor == Storeys;

        private bool? _bussiness;

        public OlxDetails(string filename)
        {
            var s = File.ReadAllText(filename, Encoding.UTF8);
            var fileDate = File.GetLastWriteTime(filename).Date;

            var ss1 = Path.GetFileNameWithoutExtension(filename).Split('_');
            Id = int.Parse(ss1[ss1.Length - 1]);

            ss1 = s.Split(new[] { "\"adPhotos-swiperSlide\"" }, StringSplitOptions.None);
            for (var k1 = 1; k1 < ss1.Length; k1++)
            {
                var a1 = ss1[k1].IndexOf("img data-src=", StringComparison.InvariantCultureIgnoreCase);
                if (a1 == -1)
                    a1 = ss1[k1].IndexOf("img src=", StringComparison.InvariantCultureIgnoreCase);
                if (a1 == -1)
                        throw new Exception("Trap! OlxDetails. Image");
                var imageRef = Parse.ParseString_Quotes(ss1[k1], a1 + 7);
                if (!imageRef.Contains("image;s="))
                    throw new Exception("Trap! OlxDetails. Image");
                ImageRefs.Add(imageRef.Trim());
            }

            var s1 = ss1[ss1.Length - 1];
            var i1 = s1.IndexOf("ad-posted-at", StringComparison.InvariantCultureIgnoreCase);
            var sDated = Parse.ParseString_Braces(s1, i1 + 7).Trim();
            if (sDated.StartsWith("Сегодня"))
                Dated = fileDate;
            else
                Dated = DateTime.Parse(sDated, _ruCulture);

            i1 = s1.IndexOf("\"ad_title\"", i1, StringComparison.InvariantCultureIgnoreCase);
            Name = System.Web.HttpUtility.HtmlDecode(Parse.ParseString_Braces(s1, i1+7)).Trim();

            i1 = s1.IndexOf("\"ad-price-container\"", i1, StringComparison.InvariantCultureIgnoreCase);
            i1 = s1.IndexOf("<h3 ", i1, StringComparison.InvariantCultureIgnoreCase);
            var s2 = Parse.ParseString_Braces(s1, i1 + 7);
            var sPrice = s2.Replace("<!-- -->", "").Replace(" ", "").Trim();

            var rate = 1.0;
            if (sPrice.EndsWith("грн."))
            {
                sPrice = sPrice.Replace("грн.", "");
                rate = Settings._usRate;
            }
            else if (sPrice.EndsWith("$"))
                sPrice = sPrice.Replace("$", "");
            else if (sPrice.EndsWith("€"))
            {
                rate = 1 / Settings._euroRate;
                sPrice = sPrice.Replace("€", "");
            }
            else
                throw new Exception("Trap! Olx details.Price");
            var price = double.Parse(sPrice, CultureInfo.InvariantCulture) / rate;
            Price = Convert.ToInt32(price);

            var i2 = s1.IndexOf("\"negotiable-label\"", i1, StringComparison.InvariantCultureIgnoreCase);
            Dogovirna = i2 > 0;

            i2 = s1.IndexOf("\"css-xl6fe0-Text eu5v0x0\"", i1, StringComparison.InvariantCultureIgnoreCase);
            while (i2 > 0)
            {
                i1 = i2;
                var s3 = Parse.ParseString_Braces(s1, i1).Trim();

                if (string.Equals(s3, "Без комиссии", StringComparison.InvariantCultureIgnoreCase))
                    NoCommission = true;
                else if (string.Equals(s3, "Бизнес", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (_bussiness.HasValue)
                        throw new Exception("Trap! Olx param details. Bussiness");
                    _bussiness = true;

                }
                else if (string.Equals(s3, "Частное лицо", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (_bussiness.HasValue)
                        throw new Exception("Trap! Olx param details. Частное лицо");
                    _bussiness = false;
                    Private = true;
                }
                else if (string.Equals(s3, "Возможность обмена", StringComparison.InvariantCultureIgnoreCase))
                    Change = true;
                else if (string.Equals(s3, "Готов сотрудничать с риэлторами", StringComparison.InvariantCultureIgnoreCase))
                    Cooperate = true;
                else
                {
                    var i4 = s3.IndexOf(':');
                    var p1 = s3.Substring(0, i4).Trim();
                    var p2 = s3.Substring(i4 + 1).Trim();

                    if (p1 == "Бытовая техника")
                    {
                        if (!p2.StartsWith("Без бытовой техники"))
                            Appliances = true;
                        Parameters.Add((p1, p2));
                    }
                    else if (p1 == "Количество комнат")
                    {
                        var i5 = p2.IndexOf("комн", StringComparison.InvariantCultureIgnoreCase);
                        if (i5>0)
                            p2 = p2.Substring(0, i5).Trim();
                        Rooms = decimal.Parse(p2, CultureInfo.InvariantCulture);
                    }
                    else if (p1 == "Меблирование")
                        Furniture = p2 == "Да";
                    else if (p1 == "Общая площадь")
                    {
                        if (!p2.EndsWith("м²"))
                            throw new Exception("Trap! Olx param details. Общая площадь");
                        Size = decimal.Parse(p2.Substring(0, p2.Length - 2).Replace(" ", "").Trim(), CultureInfo.InvariantCulture);
                    }
                    else if (p1 == "Отопление")
                        Heating = p2.Replace("Индивидуальное", "Индив.").Replace("Собственная", "Собств.").Trim();
                    else if (p1 == "Планировка")
                        Layout = p2;
                    else if (p1 == "Площадь кухни")
                    {
                        if (!p2.EndsWith("м²"))
                            throw new Exception("Trap! Olx param details. Площадь кухни");
                        Kitchen = decimal.Parse(p2.Substring(0, p2.Length - 2).Replace(" ", "").Trim(), CultureInfo.InvariantCulture);
                    }
                    else if (p1 == "Ремонт")
                        State = p2;
                    else if (p1 == "Санузел")
                        Bathroom = p2;
                    else if (p1 == "Тип дома")
                        Building = p2.Replace("Жилой фонд ", "").Replace("На этапе строительства", "Строится").Trim();
                    else if (p1 == "Тип объекта")
                        Kind = p2.Replace("Часть квартиры", "Часть кв.").Trim();
                    else if (p1 == "Тип стен")
                        Wall = p2.Replace("Шлакоблочный", "Шлакоблок").Trim();
                    else if (p1 == "Этаж")
                    {
                        if (p2 == "38 000") p2 = "3";
                        Floor = decimal.Parse(p2, CultureInfo.InvariantCulture);
                    }
                    else if (p1 == "Этажность")
                        Storeys = decimal.Parse(p2, CultureInfo.InvariantCulture);
                    else if (p1 == "Инфраструктура (до 500 метров)" || p1 == "Коммуникации" || p1 == "Комфорт" || p1 == "Ландшафт (до 1 км.)" || p1 == "Мультимедиа")
                        Parameters.Add((p1, p2));
                    else
                        throw new Exception("Trap! Olx param details. Общая площадь");
                }
                i2 = s1.IndexOf("\"css-xl6fe0-Text eu5v0x0\"", i1 + 10, StringComparison.InvariantCultureIgnoreCase);
            }

            // Statistics
            /*foreach (var p in Parameters)
            {
                var ss2 = p.Split(':');
                var a1 = ss2.Length == 1 ? "_" + ss2[0].Trim() : ss2[0];
                if (!_param.ContainsKey(a1))
                    _param.Add(a1,0);
                _param[a1]++;
            }*/

            i1 = s1.IndexOf("\"ad_description\"", i1, StringComparison.InvariantCultureIgnoreCase);
            i2 = s1.IndexOf("<div ", i1 + 10, StringComparison.InvariantCultureIgnoreCase);
            i2 = s1.IndexOf(">", i2 + 3, StringComparison.InvariantCultureIgnoreCase);
            var i3 = s1.IndexOf("</div", i2 + 10, StringComparison.InvariantCultureIgnoreCase);
            s2 = s1.Substring(i2 + 1, i3 - i2 - 1);
            var ss3 = s2.Split(new[] { "<br />" }, StringSplitOptions.RemoveEmptyEntries);
            for (var k2 = 0; k2 < ss3.Length; k2++)
                ss3[k2] = ss3[k2].Trim();
            Description = string.Join(Environment.NewLine, ss3);
            i1 = i3;

            i2 = s1.IndexOf("\"css-owpmn2-Text eu5v0x0\"", i1, StringComparison.InvariantCultureIgnoreCase);
            if (i2 < 0)
                i2 = s1.IndexOf("\"css-u8mbra-Text eu5v0x0\"", i1, StringComparison.InvariantCultureIgnoreCase);
            Realtor = Parse.ParseString_Braces(s1, i2 + 10);

            i1 = s1.IndexOf("\"css-1bafgv4-Text eu5v0x0\"", i2, StringComparison.InvariantCultureIgnoreCase);
            var ownerStarted = Parse.ParseString_Braces(s1, i1 + 10).Trim();
            if (!ownerStarted.StartsWith("на OLX с"))
                throw new Exception("Trap! Olx details. OwnerStarted");
            var s4 = DateTime.Parse(ownerStarted.Substring(8).Trim(), _ruCulture);
            Realtor += ":" + s4.ToString("yy-MM");

            // Statistics
            if (Name.Length > _olxMax1) _olxMax1 = Name.Length;
            if (Description.Length > _olxMax2) _olxMax2 = Description.Length;
            if (Realtor.Length > _olxMax3) _olxMax3 = Realtor.Length;
            foreach (var o in ImageRefs)
                if (o.Length > _olxMax5) _olxMax5 = o.Length;
            foreach (var o in Parameters)
                if (o.Item2 != null && o.Item2.Length > _olxMax6) _olxMax6 = o.Item2.Length;
        }
    }
}
