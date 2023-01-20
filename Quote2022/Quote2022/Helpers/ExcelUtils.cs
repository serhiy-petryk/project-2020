using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Quote2022.Helpers
{
    public class ExcelUtils
    {
        public static void SaveStatisticsToExcel(Dictionary<string, List<object[]>> context, string fileName)
        {
            const int rowOffset = 2;
            // var fileName = Settings.MinuteYahooLogFolder + "TestEPPlus.xlsx";

            // ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var excelPackage = new ExcelPackage())
            {
                foreach (var kvp in context)
                {
                    var columnCount = kvp.Value[2].Length;

                    var ws = excelPackage.Workbook.Worksheets.Add(kvp.Key);

                    //Add table
                    using (var rg = ws.Cells[rowOffset + 3, 1, kvp.Value.Count + 1, columnCount + 1])
                    {
                        var tbl = ws.Tables.Add(rg, kvp.Key);
                    }

                    // Wrap text for table headers
                    ws.Row(rowOffset + 3).Style.WrapText = true;

                    for (var k1 = 2; k1 < kvp.Value.Count; k1++)
                    {
                        var oo = kvp.Value[k1];
                        for (var k2 = 0; k2 < oo.Length; k2++)
                        {
                            ws.Cells[k1 + rowOffset + 1, k2 + 1].Value = oo[k2];
                            if (oo[k2] is TimeSpan ts)
                                ws.Cells[k1 + rowOffset + 1, k2 + 1].Style.Numberformat.Format = "h:mm";
                            else if (oo[k2] is DateTime dt)
                                ws.Cells[k1 + rowOffset + 1, k2 + 1].Style.Numberformat.Format = "yyyy-MM-dd";
                        }
                    }

                    // Clear header and set color of separation column
                    ws.Cells[rowOffset + 3, columnCount - 10].Value = " ";
                    using (var rg2 = ws.Cells[rowOffset + 3, columnCount - 10, kvp.Value.Count + 1, columnCount - 10])
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

                    ws.Cells[rowOffset + 1, 1].Value = kvp.Value[0][0];
                    ws.Cells[rowOffset + 2, 1].Value = kvp.Value[1][0];

                    // Save total values
                    ws.Cells[1, columnCount - 22, 4, columnCount - 22].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    ws.Cells[1, columnCount - 21, 4, columnCount - 20].Style.Font.Bold = true;

                    ws.Cells[1, columnCount - 22].Value = $"{kvp.Value[2][columnCount - 30]}, max/min/avg):";
                    ws.Cells[1, columnCount - 21].Formula = $"SUBTOTAL(104,{kvp.Key}[{kvp.Value[2][columnCount - 30]}])";
                    ws.Cells[1, columnCount - 20].Formula = $"SUBTOTAL(105,{kvp.Key}[{kvp.Value[2][columnCount - 30]}])";
                    ws.Cells[1, columnCount - 19].Formula = $"SUBTOTAL(101,{kvp.Key}[{kvp.Value[2][columnCount - 30]}])";

                    ws.Cells[2, columnCount - 22].Value = $"{kvp.Value[2][columnCount - 26]}, max/min/avg):";
                    ws.Cells[2, columnCount - 21].Formula = $"SUBTOTAL(104,{kvp.Key}[{kvp.Value[2][columnCount - 26]}])";
                    ws.Cells[2, columnCount - 20].Formula = $"SUBTOTAL(105,{kvp.Key}[{kvp.Value[2][columnCount - 26]}])";
                    ws.Cells[2, columnCount - 19].Formula = $"SUBTOTAL(101,{kvp.Key}[{kvp.Value[2][columnCount - 26]}])";

                    ws.Cells[3, columnCount - 22].Value = $"{kvp.Value[2][columnCount - 25]}, max/min/avg):";
                    ws.Cells[3, columnCount - 21].Formula = $"SUBTOTAL(104,{kvp.Key}[{kvp.Value[2][columnCount - 25]}])";
                    ws.Cells[3, columnCount - 20].Formula = $"SUBTOTAL(105,{kvp.Key}[{kvp.Value[2][columnCount - 25]}])";
                    ws.Cells[3, columnCount - 19].Formula = $"SUBTOTAL(101,{kvp.Key}[{kvp.Value[2][columnCount - 25]}])";

                    ws.Cells[4, columnCount - 22].Value = $"{kvp.Value[2][columnCount - 24]}, max/min/avg):";
                    ws.Cells[4, columnCount - 21].Formula = $"SUBTOTAL(104,{kvp.Key}[{kvp.Value[2][columnCount - 24]}])";
                    ws.Cells[4, columnCount - 20].Formula = $"SUBTOTAL(105,{kvp.Key}[{kvp.Value[2][columnCount - 24]}])";
                    ws.Cells[4, columnCount - 19].Formula = $"SUBTOTAL(101,{kvp.Key}[{kvp.Value[2][columnCount - 24]}])";

                    ws.View.FreezePanes(5, columnCount - 30);

                    ws.Calculate();
                }
                excelPackage.Workbook.FullCalcOnLoad = false;
                excelPackage.SaveAs(new FileInfo(fileName));
            }
        }
    }
}
