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

        public string Id;
        public string Status;
        public string Address;
        public bool? HasLayout;
        public string HrefLayout;
        public string HrefOriginal; // Exists only for 6 items
        public string Amount;
        public string Price;
        public bool? Reliable;
        public string Finished;
        public string InProgress;
        public string Ranking; // 67 items have value
        public string DevName;
        public short DevYear;
        public short DevFinished;
        public short DevInProgress;
        public short DevInSale;
        public string Class;
        public string Houses;
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

        public VnHouseDetails(string fullFilename)
        {
            var content = File.ReadAllText(fullFilename, Encoding.UTF8);
            var fileDate = File.GetLastWriteTime(fullFilename).Date;

            var filename = Path.GetFileNameWithoutExtension(fullFilename).Split('_');
            if (filename.Length != 3)
                throw new Exception($"Error! VnHouseDetails constructor. Can't define Id for {filename}");
            Id = filename[2];

            var sections = content.Split(new[] {"class=\"section "}, StringSplitOptions.None);
            var section = sections[0];
            var a1 = section.IndexOf(">Планування<", StringComparison.InvariantCultureIgnoreCase);
            if (a1 > 0)
            {
                HasLayout = true;
                var a2 = section.Substring(0, a1).LastIndexOf("<a ", StringComparison.InvariantCultureIgnoreCase);
                var a3 = section.IndexOf("href=\"", a2, StringComparison.InvariantCultureIgnoreCase);
                var a4 = section.IndexOf("\"", a3 + 6, StringComparison.InvariantCultureIgnoreCase);
                HrefLayout = section.Substring(a3 + 6, a4 - a3 - 6);
            }

            for (var k = 1; k < sections.Length; k++)
            {
                section = sections[k];

                if (section.StartsWith("section--building-view-cover"))
                {
                    var i1 = section.IndexOf("class=\"building-view__rating\"", StringComparison.InvariantCultureIgnoreCase);
                    if (i1 > 0)
                    {
                        var i2 = section.IndexOf("class=\"building-view__rating-count\"", i1, StringComparison.InvariantCultureIgnoreCase);
                        var i3 = section.IndexOf(">", i2, StringComparison.InvariantCultureIgnoreCase);
                        var i4 = section.IndexOf("</", i3, StringComparison.InvariantCultureIgnoreCase);
                        var s1 = section.Substring(i3+1, i4-i3-1).Trim();
                        i3 = s1.LastIndexOf(' ');
                        var s2 = s1.Substring(0, i3);

                        i2 = section.IndexOf("class=\"building-view__rating-number\"", i4, StringComparison.InvariantCultureIgnoreCase);
                        i3 = section.IndexOf(">", i2, StringComparison.InvariantCultureIgnoreCase);
                        i4 = section.IndexOf("</", i3, StringComparison.InvariantCultureIgnoreCase);
                        s1 = section.Substring(i3 + 1, i4 - i3 - 1).Trim();
                        Ranking = $"{s1}({s2})";
                    }
                }

                else if (section.StartsWith("section--slider"))
                {
                    var i1 = section.IndexOf("Характеристики ЖК", StringComparison.InvariantCultureIgnoreCase);
                    if (i1 != -1)
                    {
                        i1 = section.IndexOf("<tr", i1, StringComparison.InvariantCultureIgnoreCase);
                        var i2 = section.IndexOf("</table", i1, StringComparison.InvariantCultureIgnoreCase);
                        var ss1 = section.Substring(i1, i2 - i1).Split(new[] {"</tr"}, StringSplitOptions.None);
                        for (var k1 = 0; k1 < ss1.Length - 1; k1++)
                        {
                            var ss2 = ss1[k1].Split(new[] {"</td"}, StringSplitOptions.None);
                            var i3 = ss2[0].IndexOf("<span", StringComparison.InvariantCultureIgnoreCase);
                            var i4 = ss2[0].IndexOf("</span", i3, StringComparison.InvariantCultureIgnoreCase);
                            var name = ss2[0].Substring(i3 + 6, i4 - i3 - 6).Trim();

                            i3 = ss2[1].IndexOf("<span", StringComparison.InvariantCultureIgnoreCase);
                            i4 = ss2[1].IndexOf("</span", i3, StringComparison.InvariantCultureIgnoreCase);
                            var value = ss2[1].Substring(i3 + 6, i4 - i3 - 6).Trim();

                            if (name == "Клас")
                                Class = value;
                            else if (name == "Кількість будинків")
                                Houses = value;
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
                                Address = value;

                        }
                        s3 = sr.ReadLine();
                    }
                }

                else if (section.StartsWith("section--map"))
                {
                    var i1 = section.IndexOf("building-view__status", StringComparison.InvariantCultureIgnoreCase);
                    var i2 = section.IndexOf(">", i1, StringComparison.InvariantCultureIgnoreCase);
                    var i3 = section.IndexOf("</", i1, StringComparison.InvariantCultureIgnoreCase);
                    Status = section.Substring(i2 + 1, i3 - i2 - 1).Trim();

                    i1 = section.IndexOf("complex-info--site", StringComparison.InvariantCultureIgnoreCase);
                    if (i1 > 0)
                    {
                        i2 = section.Substring(0, i1).LastIndexOf("<a ", StringComparison.InvariantCultureIgnoreCase);
                        i3 = section.IndexOf("href=\"", i2, StringComparison.InvariantCultureIgnoreCase);
                        var i4 = section.IndexOf("\"", i3 + 6, StringComparison.InvariantCultureIgnoreCase);
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
                        var i4 = section.IndexOf("</div>", i3, StringComparison.InvariantCultureIgnoreCase);
                        var s1 = section.Substring(i3 + 1, i4 - i3 - 1).Trim();
                        Amount = ConvertHTMLToPlainText.StripHTML(s1).Trim();
                    }

                    i1 = section.IndexOf("За м<sup>2</sup>", StringComparison.InvariantCultureIgnoreCase);
                    if (i1 > 0)
                    {
                        i2 = section.IndexOf("data-currency=\"usd\"", i1, StringComparison.InvariantCultureIgnoreCase);
                        i3 = section.IndexOf(">", i2, StringComparison.InvariantCultureIgnoreCase);
                        var i4 = section.IndexOf("</div>", i3, StringComparison.InvariantCultureIgnoreCase);
                        var s1 = section.Substring(i3 + 1, i4 - i3 - 1).Trim();
                        Price = ConvertHTMLToPlainText.StripHTML(s1).Trim();
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
                        i2 = section.IndexOf("text _mb-def", i1, StringComparison.InvariantCultureIgnoreCase);
                        i3 = section.IndexOf(">", i2, StringComparison.InvariantCultureIgnoreCase);
                        var i4 = section.IndexOf("</", i3, StringComparison.InvariantCultureIgnoreCase);
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
