using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
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
            if (data.Count == 0)
                throw new Exception("SaveStatisticsToExcel error. No data");

            // ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var excelPackage = new ExcelPackage())
            {
                // var wsSummary = data.Count > 1 ? excelPackage.Workbook.Worksheets.Add("Summary") : null;
                var wsSummary = excelPackage.Workbook.Worksheets.Add("Summary");

                wsSummary.Cells["A1"].Value = "Summary";
                wsSummary.Cells["A1"].Style.Font.Bold = true;
                wsSummary.Cells["A1"].Style.Font.Size = 12;
                wsSummary.Cells["A2"].Value = "Sheet";
                wsSummary.Cells["A2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                wsSummary.Cells["A2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                wsSummary.Cells["A2:A3"].Merge = true;
                wsSummary.Cells["B2:D2"].Merge = true;
                wsSummary.Cells["E2:G2"].Merge = true;
                wsSummary.Cells["H2:J2"].Merge = true;
                wsSummary.Cells["K2:M2"].Merge = true;
                wsSummary.Cells["B2"].Value = "BuyK";
                wsSummary.Cells["E2"].Value = "SellK";
                wsSummary.Cells["H2"].Value = "(BuyK+SellK)/2";
                wsSummary.Cells["K2"].Value = "(Open/CL-1),%";
                wsSummary.Cells["B3"].Value = "Max";
                wsSummary.Cells["C3"].Value = "Min";
                wsSummary.Cells["D3"].Value = "Avg";
                wsSummary.Cells["E3"].Value = "Max";
                wsSummary.Cells["F3"].Value = "Min";
                wsSummary.Cells["G3"].Value = "Avg";
                wsSummary.Cells["H3"].Value = "Max";
                wsSummary.Cells["I3"].Value = "Min";
                wsSummary.Cells["J3"].Value = "Avg";
                wsSummary.Cells["K3"].Value = "Max";
                wsSummary.Cells["L3"].Value = "Min";
                wsSummary.Cells["M3"].Value = "Avg";
                wsSummary.Cells["B2:M3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                //wsSummary.Column(5).Width = 0.5 * 12.0 / 7.0;
                //wsSummary.Column(9).Width = 0.5 * 12.0 / 7.0;
                //wsSummary.Column(13).Width = 0.5 * 12.0 / 7.0;
                var summaryRow = 4;

                foreach (var kvp in data)
                {
                    var columnCount = kvp.Value.Table[0].Length;

                    var ws = excelPackage.Workbook.Worksheets.Add(kvp.Key);

                    if (kvp.Value.Table.Count == 0)
                        continue;

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
                    ws.Column(columnCount - 11).Width = 8.5 + 5.0/7.0;
                    ws.Column(columnCount - 12).Width = 8.5 + 5.0 / 7.0;
                    ws.Column(columnCount - 13).Width = 8.5 + 5.0 / 7.0;
                    ws.Column(columnCount - 14).Width = 8.5 + 5.0 / 7.0;
                    ws.Column(columnCount - 15).Width = 8.9 + 5.0 / 7.0;
                    ws.Column(columnCount - 16).Width = 8.9 + 5.0 / 7.0;
                    ws.Column(columnCount - 17).Width = 8.0 + 5.0 / 7.0;
                    ws.Column(columnCount - 18).Width = 8.0 + 5.0 / 7.0;
                    ws.Column(columnCount - 19).Width = 7.5 + 5.0 / 7.0;
                    ws.Column(columnCount - 20).Width = 7.5 + 5.0 / 7.0;
                    ws.Column(columnCount - 21).Width = 8.5 + 5.0 / 7.0;
                    ws.Column(columnCount - 22).Width = 8.5 + 5.0 / 7.0;
                    ws.Column(columnCount - 23).Width = 8.5 + 5.0 / 7.0;
                    ws.Column(columnCount - 24).Width = 8.5 + 5.0 / 7.0;
                    ws.Column(columnCount - 25).Width = 8.5 + 5.0 / 7.0;
                    ws.Column(columnCount - 26).Width = 5.0 + 5.0 / 7.0;
                    ws.Column(columnCount - 27).Width = 5.0 + 5.0 / 7.0;
                    ws.Column(columnCount - 28).Width = 5.0 + 5.0 / 7.0;
                    ws.Column(columnCount - 29).Width = 8.5 + 5.0 / 7.0;
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

                    ws.View.FreezePanes(6, columnCount - 30);

                    if (wsSummary != null)
                    {
                        wsSummary.Cells[summaryRow, 1].Value = kvp.Key;
                        wsSummary.Cells[summaryRow, 2].Formula = $"{kvp.Key}!{OfficeOpenXml.ExcelCellAddress.GetColumnLetter(columnCount - 21)}2";
                        wsSummary.Cells[summaryRow, 3].Formula = $"{kvp.Key}!{OfficeOpenXml.ExcelCellAddress.GetColumnLetter(columnCount - 20)}2";
                        wsSummary.Cells[summaryRow, 4].Formula = $"{kvp.Key}!{OfficeOpenXml.ExcelCellAddress.GetColumnLetter(columnCount - 19)}2";

                        wsSummary.Cells[summaryRow, 5].Formula = $"{kvp.Key}!{OfficeOpenXml.ExcelCellAddress.GetColumnLetter(columnCount - 21)}3";
                        wsSummary.Cells[summaryRow, 6].Formula = $"{kvp.Key}!{OfficeOpenXml.ExcelCellAddress.GetColumnLetter(columnCount - 20)}3";
                        wsSummary.Cells[summaryRow, 7].Formula = $"{kvp.Key}!{OfficeOpenXml.ExcelCellAddress.GetColumnLetter(columnCount - 19)}3";

                        wsSummary.Cells[summaryRow, 8].Formula = $"{kvp.Key}!{OfficeOpenXml.ExcelCellAddress.GetColumnLetter(columnCount - 21)}4";
                        wsSummary.Cells[summaryRow, 9].Formula = $"{kvp.Key}!{OfficeOpenXml.ExcelCellAddress.GetColumnLetter(columnCount - 20)}4";
                        wsSummary.Cells[summaryRow, 10].Formula = $"{kvp.Key}!{OfficeOpenXml.ExcelCellAddress.GetColumnLetter(columnCount - 19)}4";

                        wsSummary.Cells[summaryRow, 11].Formula = $"{kvp.Key}!{OfficeOpenXml.ExcelCellAddress.GetColumnLetter(columnCount - 21)}1";
                        wsSummary.Cells[summaryRow, 12].Formula = $"{kvp.Key}!{OfficeOpenXml.ExcelCellAddress.GetColumnLetter(columnCount - 20)}1";
                        wsSummary.Cells[summaryRow, 13].Formula = $"{kvp.Key}!{OfficeOpenXml.ExcelCellAddress.GetColumnLetter(columnCount - 19)}1";

                        summaryRow++;
                    }

                    // ws.Calculate();
                }


                wsSummary.Cells[$"A{summaryRow}"].Value = "Max";
                wsSummary.Cells[$"A{summaryRow+1}"].Value = "Min";
                wsSummary.Cells[$"A{summaryRow+2}"].Value = "Avg";

                for (var k1 = 0; k1 < 4; k1++)
                for (var k2 = 0; k2 < 3; k2++)
                {
                    var a = OfficeOpenXml.ExcelCellAddress.GetColumnLetter(k1 * 3 + k2 + 2);
                    wsSummary.Cells[$"{a}{summaryRow}"].Formula = $"MAX({a}4:{a}{summaryRow - 1})";
                    wsSummary.Cells[$"{a}{summaryRow + 1}"].Formula = $"MIN({a}4:{a}{summaryRow - 1})";
                    wsSummary.Cells[$"{a}{summaryRow + 2}"].Formula = $"AVERAGE({a}4:{a}{summaryRow - 1})";
                }

                wsSummary.Cells[summaryRow, 1, summaryRow + 2, 13].Style.Font.Bold = true;

                wsSummary.Cells[4, 1, summaryRow - 1, 13].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                SetBorder(wsSummary.Cells["A2:M3"]);
                wsSummary.Cells[2, 2, summaryRow + 2, 2].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                wsSummary.Cells[2, 5, summaryRow + 2, 5].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                wsSummary.Cells[2, 8, summaryRow + 2, 8].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                wsSummary.Cells[2, 11, summaryRow + 2, 11].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                wsSummary.Column(1).AutoFit();

                excelPackage.SaveAs(new FileInfo(fileName));
            }

            void SetBorder(ExcelRange cells)
            {
                cells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                cells.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            }
        }
    }
}
