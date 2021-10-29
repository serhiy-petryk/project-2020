using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using OlxFlat.Helpers;

namespace OlxFlat.Models
{
    public class VnHouseDetails
    {
        private static CultureInfo _uaCulture = new CultureInfo("uk");

        private static Dictionary<string, string> _cityXref = new Dictionary<string, string>()
        {
            {"м. Брюховичі", "смт Брюховичі"}, {"м. Вінники", "м. Винники"}, {"м. Малехів", "с. Малехів"},
            {"м. Сокільники", "с. Сокільники"}, {"м. Солонка", "с. Солонка"},
            {"смт Івано-Франково", "смт Івано-Франкове"}
        };

        private static string GetAddress(string address)
        {
            if (address == null) return null;

            var i = address.IndexOf(',');
            if (i>0)
            {
                var key = address.Substring(0, i);
                if (_cityXref.ContainsKey(key))
                    return _cityXref[key] + address.Substring(i);
                return address;
            }

            if (_cityXref.ContainsKey(address)) return _cityXref[address];

            return address;
        }

        public string Id;
        public string Name;
        public string Status;
        public string City;
        public string Address;
        public bool? HasLayout;
        public string HrefLayout;
        public string HrefOriginal; // Exists only for 6 items
        public int? Amount;
        public int? PriceFrom;
        public int? PriceTo;
        public bool? Reliable;
        public string Finished;
        public string InProgress;
        public decimal? Rank; // 67 items have value
        public int? RankCount; // 67 items have value
        public string DevId;
        public string DevName;
        public short? DevYear;
        public short? DevFinished;
        public short? DevInProgress;
        public short? DevInSale;
        public string Class;
        public short? Houses;
        public string Floors;
        public string Technology;
        public string Walls;
        public string Warming;
        public string Heating;
        public string Height;
        public string Rooms;
        public string Flats;
        public string Size;
        public string Yard;
        public string Condition;
        public string Parking;
        public DateTime Dated;

        public VnHouseDetails(string fullFilename)
        {
            var content = File.ReadAllText(fullFilename, Encoding.UTF8);
            Dated = File.GetLastWriteTime(fullFilename);

            var filename = Path.GetFileNameWithoutExtension(fullFilename).Split('_');
            if (filename.Length != 3)
                throw new Exception($"Error! VnHouseDetails constructor. Can't define Id for {filename}");
            Id = filename[2];

            var sections = content.Split(new[] {"class=\"section "}, StringSplitOptions.None);
            var section = sections[0];

            int i2, i3, i4;
            var i1 = section.IndexOf(">Планування<", StringComparison.InvariantCultureIgnoreCase);
            if (i1 > 0)
            {
                HasLayout = true;
                i2 = section.Substring(0, i1).LastIndexOf("<a ", StringComparison.InvariantCultureIgnoreCase);
                i3 = section.IndexOf("href=\"", i2, StringComparison.InvariantCultureIgnoreCase);
                i4 = section.IndexOf("\"", i3 + 6, StringComparison.InvariantCultureIgnoreCase);
                HrefLayout = section.Substring(i3 + 6, i4 - i3 - 6);
            }

            i1 = section.LastIndexOf("control-panel__title", StringComparison.InvariantCultureIgnoreCase);
            i2 = section.IndexOf(">", i1, StringComparison.InvariantCultureIgnoreCase);
            i3 = section.IndexOf("</", i2, StringComparison.InvariantCultureIgnoreCase);
            Name = section.Substring(i2 + 1, i3 - i2 - 1).Trim();

            for (var k = 1; k < sections.Length; k++)
            {
                section = sections[k];

                if (section.StartsWith("section--building-view-cover"))
                {
                    i1 = section.IndexOf("class=\"building-view__rating\"", StringComparison.InvariantCultureIgnoreCase);
                    if (i1 > 0)
                    {
                        i2 = section.IndexOf("class=\"building-view__rating-count\"", i1, StringComparison.InvariantCultureIgnoreCase);
                        i3 = section.IndexOf(">", i2, StringComparison.InvariantCultureIgnoreCase);
                        i4 = section.IndexOf("</", i3, StringComparison.InvariantCultureIgnoreCase);
                        var s1 = section.Substring(i3+1, i4-i3-1).Trim();
                        i3 = s1.LastIndexOf(' ');
                        var s2 = s1.Substring(0, i3);

                        i2 = section.IndexOf("class=\"building-view__rating-number\"", i4, StringComparison.InvariantCultureIgnoreCase);
                        i3 = section.IndexOf(">", i2, StringComparison.InvariantCultureIgnoreCase);
                        i4 = section.IndexOf("</", i3, StringComparison.InvariantCultureIgnoreCase);
                        s1 = section.Substring(i3 + 1, i4 - i3 - 1).Trim();
                        Rank = decimal.Parse(s1, CultureInfo.InvariantCulture);
                        RankCount = int.Parse(s2);
                    }
                }

                if (section.StartsWith("section--slider") || (section.StartsWith("section--building-view-cover") && !sections[k+1].StartsWith("section--slider")))
                {
                    i1 = section.IndexOf("Характеристики ЖК", StringComparison.InvariantCultureIgnoreCase);
                    if (i1 != -1)
                    {
                        i1 = section.IndexOf("<tr", i1, StringComparison.InvariantCultureIgnoreCase);
                        i2 = section.IndexOf("</table", i1, StringComparison.InvariantCultureIgnoreCase);
                        var ss1 = section.Substring(i1, i2 - i1).Split(new[] {"</tr"}, StringSplitOptions.None);
                        for (var k1 = 0; k1 < ss1.Length - 1; k1++)
                        {
                            var ss2 = ss1[k1].Split(new[] {"</td"}, StringSplitOptions.None);
                            i3 = ss2[0].IndexOf("<span", StringComparison.InvariantCultureIgnoreCase);
                            i4 = ss2[0].IndexOf("</span", i3, StringComparison.InvariantCultureIgnoreCase);
                            var name = ss2[0].Substring(i3 + 6, i4 - i3 - 6).Trim();

                            i3 = ss2[1].IndexOf("<span", StringComparison.InvariantCultureIgnoreCase);
                            i4 = ss2[1].IndexOf("</span", i3, StringComparison.InvariantCultureIgnoreCase);
                            var value = ss2[1].Substring(i3 + 6, i4 - i3 - 6).Trim();

                            if (name == "Клас")
                                Class = value;
                            else if (name == "Кількість будинків")
                                Houses = short.Parse(value);
                            else if (name == "Поверховість")
                                Floors = value;
                            else if (name == "Технологія будівництва")
                                Technology = value;
                            else if (name == "Стіни")
                                Walls = value;
                            else if (name == "Утеплення")
                                Warming = value;
                            else if (name == "Опалення")
                                Heating = value;
                            else if (name == "Висота стелі")
                                Height = value;
                            else if (name == "Кімнатність квартир")
                                Rooms = value.Replace(" ", "");
                            else if (name == "Кількість квартир")
                                Flats = value;
                            else if (name == "Площа квартир")
                                Size = ConvertHTMLToPlainText.StripHTML(value).Trim();
                            else if (name == "Територія")
                                Yard = value;
                            else if (name == "Стан квартири")
                                Condition = value;
                            else if (name == "Паркінг")
                                Parking = value;
                            else
                                throw new Exception($"Error! Невідома характеристика для {filename}: {name}");
                        }
                    }

                    i1 = section.IndexOf(">Адреса<", StringComparison.InvariantCultureIgnoreCase);
                    var i21 = section.Substring(0, i1).LastIndexOf("<table", StringComparison.InvariantCultureIgnoreCase);
                    var i31 = section.IndexOf("</table", i21, StringComparison.InvariantCultureIgnoreCase);
                    var s1 = section.Substring(i21, i31 - i21 + 8);
                    var s2 = ConvertHTMLToPlainText.StripHTML(s1).Trim();
                    var sr = new StringReader(s2);
                    var s3 = sr.ReadLine();
                    while (s3 != null)
                    {
                        if (!string.IsNullOrEmpty(s3))
                        {
                            s3 = s3.Trim();
                            i21 = s3.IndexOf('\t');
                            var key = s3.Substring(0, i21).Trim();
                            var value = s3.Substring(i21 + 1).Trim();
                            if (key == "Адреса")
                            {
                                Address = GetAddress(value);
                                i21 = Address.IndexOf(',');
                                if (i21 < 0)
                                    City = Address;
                                else
                                    City = Address.Substring(0, i21).Trim();
                            }

                        }
                        s3 = sr.ReadLine();
                    }
                }

                if (section.StartsWith("section--map") || (section.StartsWith("section--slider") && !sections[k + 1].StartsWith("section--map")))
                {
                    i1 = section.IndexOf("building-view__status", StringComparison.InvariantCultureIgnoreCase);
                    i2 = section.IndexOf(">", i1, StringComparison.InvariantCultureIgnoreCase);
                    i3 = section.IndexOf("</", i1, StringComparison.InvariantCultureIgnoreCase);
                    Status = section.Substring(i2 + 1, i3 - i2 - 1).Trim();

                    i1 = section.IndexOf("complex-info--site", StringComparison.InvariantCultureIgnoreCase);
                    if (i1 > 0)
                    {
                        i2 = section.Substring(0, i1).LastIndexOf("<a ", StringComparison.InvariantCultureIgnoreCase);
                        i3 = section.IndexOf("href=\"", i2, StringComparison.InvariantCultureIgnoreCase);
                        i4 = section.IndexOf("\"", i3 + 6, StringComparison.InvariantCultureIgnoreCase);
                        var s1 = section.Substring(i3 + 6, i4 - i3 - 6);
                        var ss1 = s1.Split('?');
                        if (ss1.Length != 3)
                            throw new Exception($"Error! VnHouseDetails constructor. Check site href for {Id}: {s1}");
                        HrefOriginal = ss1[1];
                    }

                    // i1 = section.IndexOf(">Ціни від забудовника<", StringComparison.InvariantCultureIgnoreCase);
                    i1 = section.IndexOf(">За квартиру<", StringComparison.InvariantCultureIgnoreCase);
                    if (i1 > 0)
                    {
                        i2 = section.IndexOf("data-currency=\"usd\"", i1, StringComparison.InvariantCultureIgnoreCase);
                        i3 = section.IndexOf(">", i2, StringComparison.InvariantCultureIgnoreCase);
                        i4 = section.IndexOf("</div>", i3, StringComparison.InvariantCultureIgnoreCase);
                        var s1 = section.Substring(i3 + 1, i4 - i3 - 1).Trim();
                        var amt = ConvertHTMLToPlainText.StripHTML(s1).Trim();
                        if (!string.IsNullOrEmpty(amt))
                        {
                            if (amt.EndsWith("$"))
                                amt = amt.Substring(0, amt.Length - 1).Trim();
                            if (amt.StartsWith("від"))
                                amt = amt.Substring(3).Trim();
                            Amount = int.Parse(amt.Replace(" ", ""));
                        }
                    }

                    i1 = section.IndexOf("За м<sup>2</sup>", StringComparison.InvariantCultureIgnoreCase);
                    if (i1 > 0)
                    {
                        i2 = section.IndexOf("data-currency=\"usd\"", i1, StringComparison.InvariantCultureIgnoreCase);
                        i3 = section.IndexOf(">", i2, StringComparison.InvariantCultureIgnoreCase);
                        i4 = section.IndexOf("</div>", i3, StringComparison.InvariantCultureIgnoreCase);
                        var s1 = section.Substring(i3 + 1, i4 - i3 - 1).Trim();
                        var price = ConvertHTMLToPlainText.StripHTML(s1).Trim();
                        if (!string.IsNullOrEmpty(price))
                        {
                            if (price.EndsWith("$"))
                                price = price.Substring(0, price.Length - 1).Trim();
                            if (price.StartsWith("від"))
                            {
                                price = price.Substring(3).Trim();
                                PriceFrom = int.Parse(price.Replace(" ", ""));
                            }
                            else
                            {
                                var prices = price.Split('-');
                                if (prices.Length!=2)
                                    throw new Exception($"Error! VnHouseDetails constructor. Check 'price' values for {Id}: {price}");
                                PriceFrom = int.Parse(prices[0].Replace(" ", ""));
                                PriceTo = int.Parse(prices[1].Replace(" ", ""));
                            }
                        }
                    }

                    var ss2 = section.Split(new[] {"media-item__desc"}, StringSplitOptions.None);
                    var finished = new List<string>();
                    var inProgress = new List<string>();
                    for (var k1 = 1; k1 < ss2.Length; k1++)
                    {
                        var s1 = ss2[k1];
                        i1 = s1.IndexOf(">", StringComparison.InvariantCultureIgnoreCase);
                        i2 = s1.IndexOf("</div", StringComparison.InvariantCultureIgnoreCase);
                        var s2 = s1.Substring(i1 + 1, i2 - i1 - 1).Trim();
                        if (s2 == "Надійний <br> забудовник")
                            Reliable = true;
                        else if (s2.Contains("20"))
                        {
                            if (s2.Contains("введено в експлуатацію в"))
                                finished.Add(s2.Replace("введено в експлуатацію в", "").Trim());
                            else
                                inProgress.Add(s2);
                        }
                        else if (s2.Contains("введено в експлуатацію"))
                            finished.Add(s2.Replace("введено в експлуатацію", "готово"));
                        else if (s2.Contains("в проекті"))
                            inProgress.Add(s2);
                        else if (s2.Contains("будівництво заморожене"))
                            inProgress.Add(s2.Replace("будівництво заморожене", "заморожено"));
                        else if (s2.Contains("дата введення в експлуатацію не заявлена"))
                            inProgress.Add(s2.Replace("дата введення в експлуатацію не заявлена", "не заявлено"));
                        else if (s2.Contains("дата ведення в експлуатацію не заявлена"))
                            inProgress.Add(s2.Replace("дата ведення в експлуатацію не заявлена", "не заявлено"));
                        else
                            throw new Exception($"Error! VnHouseDetails constructor. Check 'media-item__desc' class for {Id}: {s2}");
                    }
                    if (finished.Count > 0)
                        Finished = string.Join("; ", finished.Select(a => a.Replace("  ", " ")));
                    if (inProgress.Count > 0)
                        InProgress = string.Join("; ", inProgress.Select(a => a.Replace("  ", " ")));

                    i1 = section.IndexOf("Перейти на сторінку забудовника", StringComparison.InvariantCultureIgnoreCase);
                    if (i1 > 0)
                    {
                        i2 = section.IndexOf("href=\"", i1, StringComparison.InvariantCultureIgnoreCase);
                        i3 = section.IndexOf("\"", i2 + 6, StringComparison.InvariantCultureIgnoreCase);
                        i4 = section.Substring(0, i3).LastIndexOf("/", i3, StringComparison.InvariantCultureIgnoreCase);
                        DevId = section.Substring(i4 + 1, i3 - i4 - 1).Trim();

                        i2 = section.IndexOf("text _mb-def", i1, StringComparison.InvariantCultureIgnoreCase);
                        i3 = section.IndexOf(">", i2, StringComparison.InvariantCultureIgnoreCase);
                        i4 = section.IndexOf("</", i3, StringComparison.InvariantCultureIgnoreCase);
                        DevName = section.Substring(i3 + 1, i4 - i3 - 1).Trim();

                        i2 = section.IndexOf("<table", i4, StringComparison.InvariantCultureIgnoreCase);
                        if (i2 > 0)
                        {
                            i3 = section.IndexOf("</table", i2, StringComparison.InvariantCultureIgnoreCase);
                            ss2 = section.Substring(i2, i3 - i2).Split(new[] {"</tr>"}, StringSplitOptions.None);

                            foreach (var s2 in ss2.Where(s3 => !string.IsNullOrEmpty(s3.Trim())))
                            {
                                var ss3 = s2.Split(new[] {"</td>"}, StringSplitOptions.None);
                                i4 = ss3[0].LastIndexOf(">", StringComparison.InvariantCultureIgnoreCase);
                                var key = ss3[0].Substring(i4 + 1).Trim();
                                i4 = ss3[1].LastIndexOf(">", StringComparison.InvariantCultureIgnoreCase);
                                var value = ss3[1].Substring(i4 + 1).Trim();
                                if (key == "Рік заснування")
                                    DevYear = short.Parse(value);
                                else if (key == "Здано ЖК")
                                    DevFinished = short.Parse(value);
                                else if (key == "Будується ЖК")
                                    DevInProgress = short.Parse(value);
                                else if (key == "В продажі ЖК")
                                    DevInSale = short.Parse(value);
                                else
                                    throw new Exception($"Error! VnHouseDetails constructor. Check developer parameter usagefor {Id}: {key}");
                            }
                        }
                    }
                }

                else if (section.StartsWith("section--news-cards")){}
                else if (section.StartsWith("section--building-cards")){}
                else {}
            }
        }
    }
}
