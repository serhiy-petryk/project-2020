using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using OfficeOpenXml.Table;

namespace Quote2022.Helpers
{
    public class ExcelTestEPPlus
    {
        public static void AAXmlTable(Dictionary<string, List<object[]>> context)
        {
            const int rowOffset = 1;
            var fileName = Settings.MinuteYahooLogFolder + "TestEPPlus.xlsx";
            if (File.Exists(fileName))
                File.Delete(fileName);

            using (var excelPackage = new ExcelPackage(new FileInfo(fileName)))
            {
                foreach (var kvp in context)
                {
                    var ws = excelPackage.Workbook.Worksheets.Add(kvp.Key);
                    ws.Cells[rowOffset + 1, 1].Value = kvp.Value[0][0];
                    ws.Cells[rowOffset + 2, 1].Value = kvp.Value[1][0];

                    for (var k1 = 2; k1 < kvp.Value.Count; k1++)
                    {
                        var oo = kvp.Value[k1];
                        for (var k2 = 0; k2 < oo.Length; k2++)
                        {
                            ws.Cells[k1 + rowOffset + 1, k2 + 1].Value = oo[k2];
                            if (oo[k2] is TimeSpan ts)
                            {
                                ws.Cells[k1 + rowOffset + 1, k2 + 1].Style.Numberformat.Format = "h:mm";
                            }
                        }
                    }

                    // var cell = ws.Cells[4, 1];
                    //ws.Cells.Max(c=>c.)

                    using (var rg = ws.Cells[4, 1, ws.Dimension.Rows + 1, ws.Dimension.Columns])
                    {
                        //Ading a table to a Range
                        var tbl = ws.Tables.Add(rg, kvp.Key);
                        tbl.ShowTotal = false;
                        tbl.TableStyle = TableStyles.Medium27;
                    }

                    for (var k1 = 2; k1 < kvp.Value.Count; k1++)
                    {
                        var oo = kvp.Value[k1];
                        for (var k2 = 0; k2 < oo.Length; k2++)
                        {
                            ws.Cells[k1 + rowOffset + 1, k2 + 1].Value = oo[k2];
                            if (oo[k2] is TimeSpan ts)
                            {
                                ws.Cells[k1 + rowOffset + 1, k2 + 1].Style.Numberformat.Format = "h:mm";
                            }
                        }
                    }
                }

                excelPackage.Save();
            }

        }
    }
}
