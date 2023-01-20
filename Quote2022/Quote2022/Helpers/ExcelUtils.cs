using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Quote2022.Models;

namespace Quote2022.Helpers
{
    public class ExcelUtils
    {
        public class StatisticsData
        {
            public string Title;
            public string Header1;
            public string Header2;
            public string Header3;
            public List<object[]> Table;
        }

        public static void SaveQuotesToExcel_NotNeed(IList<Quote> data, string fileName)
        {
            using (var excelPackage = new ExcelPackage())
            {
                var ws = excelPackage.Workbook.Worksheets.Add("Quotes");

                ws.Cells[1, 1].Value = "Quotes";
                ws.Cells[2, 1].Value = "Symbol";
                ws.Cells[2, 2].Value = "Date";
                ws.Cells[2, 3].Value = "Open";
                ws.Cells[2, 4].Value = "High";
                ws.Cells[2, 5].Value = "Low";
                ws.Cells[2, 6].Value = "Close";
                ws.Cells[2, 7].Value = "Volume";

                var row = 3;
                foreach (var q in data)
                {
                    ws.Cells[row, 1].Value = q.Symbol;
                    ws.Cells[row, 2].Value = q.Timed;
                    ws.Cells[row, 3].Value = q.Open;
                    ws.Cells[row, 4].Value = q.High;
                    ws.Cells[row, 5].Value = q.Low;
                    ws.Cells[row, 6].Value = q.Close;
                    ws.Cells[row, 7].Value = q.Volume;
                    row++;
                }

                excelPackage.SaveAs(new FileInfo(fileName));

            }
        }

        public static void SaveStatisticsToExcel(Dictionary<string, StatisticsData> data, string fileName)
        {
            // ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var excelPackage = new ExcelPackage())
            {
                foreach (var kvp in data)
                {
                    if (kvp.Value.Table.Count == 0)
                        continue;

                    var columnCount = kvp.Value.Table[0].Length;

                    var ws = excelPackage.Workbook.Worksheets.Add(kvp.Key);

                    //Add table
                    using (var rg = ws.Cells[5, 1, kvp.Value.Table.Count + 4, columnCount])
                    {
                        var tbl = ws.Tables.Add(rg, kvp.Key);
                    }

                    // Wrap text for table headers
                    ws.Row(5).Style.WrapText = true;

                    for (var k1 = 0; k1 < kvp.Value.Table.Count; k1++)
                    {
                        var oo = kvp.Value.Table[k1];
                        for (var k2 = 0; k2 < oo.Length; k2++)
                        {
                            ws.Cells[k1 + 5, k2 + 1].Value = oo[k2];
                            if (oo[k2] is TimeSpan ts)
                                ws.Cells[k1 + 5, k2 + 1].Style.Numberformat.Format = "h:mm";
                            else if (oo[k2] is DateTime dt)
                                ws.Cells[k1 + 5, k2 + 1].Style.Numberformat.Format = "yyyy-MM-dd";
                        }
                    }

                    // Clear header and set color of separation column
                    ws.Cells[5, columnCount - 10].Value = " ";
                    using (var rg2 = ws.Cells[5, columnCount - 10, kvp.Value.Table.Count + 4, columnCount - 10])
                    {
                        rg2.Style.Fill.PatternType = ExcelFillStyle.Solid; ;
                        rg2.Style.Fill.BackgroundColor.SetColor(Color.Yellow);
                    }

                    ws.Cells.AutoFitColumns();

                    ws.Column(columnCount - 10).Width = 0.7 * 12.0 / 7.0;
                    //ws.Column(columnCount - 11).Width = 8.5 + 5.0/7.0;
                    //ws.Column(columnCount - 12).Width = 8.5 + 5.0 / 7.0;
                    //ws.Column(columnCount - 13).Width = 8.5 + 5.0 / 7.0;
                    //ws.Column(columnCount - 14).Width = 8.5 + 5.0 / 7.0;
                    //ws.Column(columnCount - 15).Width = 8.9 + 5.0 / 7.0;
                    // ws.Column(columnCount - 16).Width = 8.9 + 5.0 / 7.0;
                    ws.Column(columnCount - 17).Width = 8.0 + 5.0 / 7.0;
                    ws.Column(columnCount - 18).Width = 8.0 + 5.0 / 7.0;
                    ws.Column(columnCount - 19).Width = 7.5 + 5.0 / 7.0;
                    ws.Column(columnCount - 20).Width = 7.5 + 5.0 / 7.0;
                    //ws.Column(columnCount - 21).Width = 11.0 + 5.0 / 7.0;
                    //ws.Column(columnCount - 22).Width = 11.0 + 5.0 / 7.0;
                    //ws.Column(columnCount - 23).Width = 11.0 + 5.0 / 7.0;
                    //ws.Column(columnCount - 24).Width = 11.0 + 5.0 / 7.0;
                    //ws.Column(columnCount - 25).Width = 11.0 + 5.0 / 7.0;
                    ws.Column(columnCount - 26).Width = 5.0 + 5.0 / 7.0;
                    ws.Column(columnCount - 27).Width = 5.0 + 5.0 / 7.0;
                    ws.Column(columnCount - 28).Width = 6.4 + 5.0 / 7.0;
                    // ws.Column(columnCount - 29).Width = 11.3 + 5.0 / 7.0;
                    ws.Column(columnCount - 30).Width = 5.5 + 5.0 / 7.0;

                    if (!string.IsNullOrEmpty(kvp.Value.Title))
                        ws.Cells[1, 1].Value = kvp.Value.Title;
                    if (!string.IsNullOrEmpty(kvp.Value.Header1))
                        ws.Cells[2, 1].Value = kvp.Value.Header1;
                    if (!string.IsNullOrEmpty(kvp.Value.Header2))
                        ws.Cells[3, 1].Value = kvp.Value.Header2;
                    if (!string.IsNullOrEmpty(kvp.Value.Header3))
                        ws.Cells[4, 1].Value = kvp.Value.Header3;

                    // Save total values
                    ws.Cells[1, columnCount - 22, 4, columnCount - 22].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    ws.Cells[1, columnCount - 21, 4, columnCount - 20].Style.Font.Bold = true;

                    ws.Cells[1, columnCount - 22].Value = $"{kvp.Value.Table[0][columnCount - 30]}, max/min/avg):";
                    ws.Cells[1, columnCount - 21].Formula = $"SUBTOTAL(104,{kvp.Key}[{kvp.Value.Table[0][columnCount - 30]}])";
                    ws.Cells[1, columnCount - 20].Formula = $"SUBTOTAL(105,{kvp.Key}[{kvp.Value.Table[0][columnCount - 30]}])";
                    ws.Cells[1, columnCount - 19].Formula = $"SUBTOTAL(101,{kvp.Key}[{kvp.Value.Table[0][columnCount - 30]}])";

                    ws.Cells[2, columnCount - 22].Value = $"{kvp.Value.Table[0][columnCount - 26]}, max/min/avg):";
                    ws.Cells[2, columnCount - 21].Formula = $"SUBTOTAL(104,{kvp.Key}[{kvp.Value.Table[0][columnCount - 26]}])";
                    ws.Cells[2, columnCount - 20].Formula = $"SUBTOTAL(105,{kvp.Key}[{kvp.Value.Table[0][columnCount - 26]}])";
                    ws.Cells[2, columnCount - 19].Formula = $"SUBTOTAL(101,{kvp.Key}[{kvp.Value.Table[0][columnCount - 26]}])";

                    ws.Cells[3, columnCount - 22].Value = $"{kvp.Value.Table[0][columnCount - 25]}, max/min/avg):";
                    ws.Cells[3, columnCount - 21].Formula = $"SUBTOTAL(104,{kvp.Key}[{kvp.Value.Table[0][columnCount - 25]}])";
                    ws.Cells[3, columnCount - 20].Formula = $"SUBTOTAL(105,{kvp.Key}[{kvp.Value.Table[0][columnCount - 25]}])";
                    ws.Cells[3, columnCount - 19].Formula = $"SUBTOTAL(101,{kvp.Key}[{kvp.Value.Table[0][columnCount - 25]}])";

                    ws.Cells[4, columnCount - 22].Value = $"{kvp.Value.Table[0][columnCount - 24]}, max/min/avg):";
                    ws.Cells[4, columnCount - 21].Formula = $"SUBTOTAL(104,{kvp.Key}[{kvp.Value.Table[0][columnCount - 24]}])";
                    ws.Cells[4, columnCount - 20].Formula = $"SUBTOTAL(105,{kvp.Key}[{kvp.Value.Table[0][columnCount - 24]}])";
                    ws.Cells[4, columnCount - 19].Formula = $"SUBTOTAL(101,{kvp.Key}[{kvp.Value.Table[0][columnCount - 24]}])";

                    ws.View.FreezePanes(5, columnCount - 30);

                    // ws.Calculate();
                }
                excelPackage.SaveAs(new FileInfo(fileName));
            }
        }
    }
}
