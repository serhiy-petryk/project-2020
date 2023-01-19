using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Quote2022.Helpers
{
    public class ExcelTestEPPlus
    {
        public static void AAXmlTable(Dictionary<string, List<object[]>> context)
        {
            const int rowOffset = 1;
            var fileName = Settings.MinuteYahooLogFolder + "TestEPPlus.xlsx";
            // if (File.Exists(fileName))
               //  File.Delete(fileName);

            // ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var excelPackage = new ExcelPackage())
            {
                foreach (var kvp in context)
                {
                    var ws = excelPackage.Workbook.Worksheets.Add(kvp.Key);
                    using (var rg = ws.Cells[4, 1, kvp.Value.Count + 1, kvp.Value[2].Length])
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
                        }
                    }

                    // Clear header and set color of separation column
                    ws.Cells[4, ws.Dimension.Columns - 10].Value = " ";
                    using (var rg2 = ws.Cells[4, ws.Dimension.Columns - 10, kvp.Value.Count + 1, ws.Dimension.Columns - 10])
                    {
                        rg2.Style.Fill.PatternType = ExcelFillStyle.Solid; ;
                        rg2.Style.Fill.BackgroundColor.SetColor(Color.Yellow);
                    }

                    ws.Cells.AutoFitColumns();

                    ws.Column(ws.Dimension.Columns - 10).Width = 1 * 12.0 / 7.0;
                    /*ws.Column(ws.Dimension.Columns - 11).Width = 8.5 + 5.0/7.0;
                    ws.Column(ws.Dimension.Columns - 12).Width = 8.5 + 5.0 / 7.0;
                    ws.Column(ws.Dimension.Columns - 13).Width = 8.5 + 5.0 / 7.0;
                    ws.Column(ws.Dimension.Columns - 14).Width = 8.5 + 5.0 / 7.0;
                    ws.Column(ws.Dimension.Columns - 15).Width = 8.9 + 5.0 / 7.0;
                    ws.Column(ws.Dimension.Columns - 16).Width = 8.9 + 5.0 / 7.0;*/
                    ws.Column(ws.Dimension.Columns - 17).Width = 8.0 + 5.0 / 7.0;
                    ws.Column(ws.Dimension.Columns - 18).Width = 8.0 + 5.0 / 7.0;
                    ws.Column(ws.Dimension.Columns - 19).Width = 7.5 + 5.0 / 7.0;
                    ws.Column(ws.Dimension.Columns - 20).Width = 7.5 + 5.0 / 7.0;
                    /*ws.Column(ws.Dimension.Columns - 21).Width = 11.0 + 5.0 / 7.0;
                    ws.Column(ws.Dimension.Columns - 22).Width = 11.0 + 5.0 / 7.0;
                    ws.Column(ws.Dimension.Columns - 23).Width = 11.0 + 5.0 / 7.0;
                    ws.Column(ws.Dimension.Columns - 24).Width = 11.0 + 5.0 / 7.0;
                    ws.Column(ws.Dimension.Columns - 25).Width = 11.0 + 5.0 / 7.0;*/
                    ws.Column(ws.Dimension.Columns - 26).Width = 5.0 + 5.0 / 7.0;
                    ws.Column(ws.Dimension.Columns - 27).Width = 5.0 + 5.0 / 7.0;
                    ws.Column(ws.Dimension.Columns - 28).Width = 6.4 + 5.0 / 7.0;
                    // ws.Column(ws.Dimension.Columns - 29).Width = 11.3 + 5.0 / 7.0;
                    ws.Column(ws.Dimension.Columns - 30).Width = 5.5 + 5.0 / 7.0;

                    ws.Cells[rowOffset + 1, 1].Value = kvp.Value[0][0];
                    ws.Cells[rowOffset + 2, 1].Value = kvp.Value[1][0];

                    ws.View.FreezePanes(5, ws.Dimension.Columns - 30);
                }
                excelPackage.SaveAs(new FileInfo(fileName));
            }
        }
    }
}
