using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Excel;

namespace Quote2022.Helpers
{
    class ExcelTestXml
    {
        public static void AAXmlTable(Dictionary<string, List<object[]>> context)
        {
            const int rowOffset = 1;

            var wb = new XLWorkbook();

            foreach (var kvp in context)
            {
                //Adding a worksheet
                var ws = wb.Worksheets.Add(kvp.Key);

                ws.Cell(rowOffset + 1, 1).Value = kvp.Value[0][0];
                ws.Cell(rowOffset + 2, 1).Value = kvp.Value[1][0];

                for (var k1 = 2; k1 < kvp.Value.Count; k1++)
                {
                    //                    var row = sheet.CreateRow(rowOffset + k1);
                    //if (k1==2)
                    // row.Height = 5000;

                    var oo = kvp.Value[k1];
                    for (var k2 = 0; k2 < oo.Length; k2++)
                    {
                        ws.Cell(k1 + rowOffset + 1, k2 + 1).Value = oo[k2];
                    }
                }

                // Create table
                var colCount = ws.ColumnsUsed().Count();
                var range = ws.Range(4, 1, ws.RowsUsed().Count() + 1, colCount + 1);
                var table = range.CreateTable(kvp.Key);
                table.ShowTotalsRow = false;

                // Set styles
                ws.Row(rowOffset + 3).Style.Font.Bold = true;
                ws.Row(rowOffset + 3).Style.Alignment.WrapText = true;
                ws.Row(rowOffset + 3).Height = 30;
                ws.Columns("V").Style.Fill.BackgroundColor = XLColor.Yellow;
                ws.SheetView.Freeze(3, colCount - 30);
                ws.Columns(2, ws.ColumnCount() - 1).AdjustToContents();

                // Set column width
                ws.Column(colCount - 28).Width = 11.3;
                ws.Column(colCount - 26).Width = 5.8;
                ws.Column(colCount - 25).Width = 5.8;
                ws.Column(colCount - 24).Width = 11.3;
                ws.Column(colCount - 23).Width = 11.3;
                ws.Column(colCount - 22).Width = 11.3;
                ws.Column(colCount - 21).Width = 11.3;
                ws.Column(colCount - 20).Width = 11.3;
                ws.Column(colCount - 19).Width = 7.5;
                ws.Column(colCount - 18).Width = 7.5;
                ws.Column(colCount - 17).Width = 8;
                ws.Column(colCount - 16).Width = 8;
                ws.Column(colCount - 15).Width = 8.9;
                ws.Column(colCount - 14).Width = 8.9;
                ws.Column(colCount - 13).Width = 8.5;
                ws.Column(colCount - 12).Width = 8.5;
                ws.Column(colCount - 11).Width = 8.5;
                ws.Column(colCount - 10).Width = 8.5;
                ws.Column(colCount - 9).Width = 1;
            }

            wb.SaveAs(Settings.MinuteYahooLogFolder + "TestXml.xlsx");

        }
        public static void AAXmlNoTable(Dictionary<string, List<object[]>> context)
        {
            const int rowOffset = 1;

            var wb = new XLWorkbook();

            foreach (var kvp in context)
            {
                //Adding a worksheet
                var ws = wb.Worksheets.Add(kvp.Key);

                ws.Cell(rowOffset + 1, 1).Value = kvp.Value[0][0];
                ws.Cell(rowOffset + 2, 1).Value = kvp.Value[1][0];

                for (var k1 = 2; k1 < kvp.Value.Count; k1++)
                {
                    //                    var row = sheet.CreateRow(rowOffset + k1);
                    //if (k1==2)
                    // row.Height = 5000;

                    var oo = kvp.Value[k1];
                    for (var k2 = 0; k2 < oo.Length; k2++)
                    {
                        ws.Cell(k1 + rowOffset + 1, k2 + 1).Value = oo[k2];
                    }
                }

                ws.Row(rowOffset + 3).Style.Font.Bold = true;
                ws.Row(rowOffset + 3).Style.Alignment.WrapText = true;
                ws.Row(rowOffset + 3).Height = 30;
                ws.Columns(2, ws.ColumnCount() - 1).AdjustToContents();

                var colCount = ws.ColumnsUsed().Count();
                // ws.Row(rowOffset+3).AdjustToContents();
                ws.Column(colCount - 28).Width = 11.3;
                ws.Column(colCount - 26).Width = 5.8;
                ws.Column(colCount - 25).Width = 5.8;
                ws.Column(colCount - 24).Width = 11.3;
                ws.Column(colCount - 23).Width = 11.3;
                ws.Column(colCount - 22).Width = 11.3;
                ws.Column(colCount - 21).Width = 11.3;
                ws.Column(colCount - 20).Width = 11.3;
                ws.Column(colCount - 19).Width = 7.5;
                ws.Column(colCount - 18).Width = 7.5;
                ws.Column(colCount - 17).Width = 8;
                ws.Column(colCount - 16).Width = 8;
                ws.Column(colCount - 15).Width = 8.9;
                ws.Column(colCount - 14).Width = 8.9;
                ws.Column(colCount - 13).Width = 8.5;
                ws.Column(colCount - 12).Width = 8.5;
                ws.Column(colCount - 11).Width = 8.5;
                ws.Column(colCount - 10).Width = 8.5;
                ws.Column(colCount - 9).Width = 1;

                ws.Columns("V").Style.Fill.BackgroundColor = XLColor.Yellow;

                ws.SheetView.Freeze(3, colCount - 30);
            }

            wb.SaveAs(Settings.MinuteYahooLogFolder + "TestXml.xlsx");

        }
    }
}
