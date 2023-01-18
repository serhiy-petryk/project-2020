using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.PropertyGridInternal;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Quote2022.Models;

namespace Quote2022.Helpers
{
    class ExcelTest
    {
        public static void AATable(Dictionary<string, List<object[]>> context)
        {
            const int rowOffset = 1;

            IWorkbook workbook = new XSSFWorkbook();
            IDataFormat format = workbook.CreateDataFormat();

            var boldFont = workbook.CreateFont();
            boldFont.IsBold = true;

            ICellStyle separatorStyle = workbook.CreateCellStyle();
            separatorStyle.FillForegroundColor = IndexedColors.Yellow.Index;
            separatorStyle.FillPattern = FillPattern.SolidForeground;

            foreach (var kvp in context)
            {
                var sheet = (XSSFSheet)workbook.CreateSheet(kvp.Key);

                var headerRow = sheet.CreateRow(rowOffset);
                var headerCell = headerRow.CreateCell(0);
                headerCell.SetCellValue((string)kvp.Value[0][0]);
                headerRow = sheet.CreateRow(rowOffset+1);
                headerCell = headerRow.CreateCell(0);
                headerCell.SetCellValue((string)kvp.Value[1][0]);

                var table = sheet.CreateTable();
                // table.IsHasTotalsRow = false;
                table.Name = "tbl" + kvp.Key;
                var ctTable = table.GetCTTable();
                ctTable.tableColumns.count = 33;
                ctTable.headerRowCount = 1;


                ctTable.id = 1;
                // table.IsHasTotalsRow = false;
                table.DisplayName = "Table1";
                // table.SetCellReferences(new NPOI.SS.Util.AreaReference("A1:C5", NPOI.SS.SpreadsheetVersion.EXCEL2007));
                // table.CreateColumn(null, 0);

                // table.Style.IsShowColumnStripes = false;
                // table.Style.IsShowRowStripes = true;



                int? separateColumnNo = null;
                for (var k1 = 2; k1 < kvp.Value.Count; k1++)
                {
                    var row = sheet.CreateRow(rowOffset + k1);
                    //if (k1==2)
                    // row.Height = 5000;

                    var oo = kvp.Value[k1];
                    for (var k2 = 0; k2 < oo.Length; k2++)
                    {
                        var cell = row.CreateCell(k2);
                        var value = oo[k2];
                        if (value == null)
                            cell.SetCellValue((string)null);
                        else if (value is string s)
                            cell.SetCellValue(s);
                        else if (value is int a1)
                            cell.SetCellValue(a1);
                        else if (value is float a2)
                            cell.SetCellValue(a2);
                        else if (value is double a3)
                            cell.SetCellValue(a3);
                        else if (value is DateTime dt)
                            cell.SetCellValue(dt);
                        else if (value is TimeSpan ts)
                        {
                            var day = new TimeSpan(1, 0, 0, 0);
                            var tsValue = Convert.ToDouble(ts.Ticks) / Convert.ToDouble(day.Ticks);
                            SetValueAndFormat(workbook, cell, tsValue, format.GetFormat("h:mm"));
                        }
                        else
                            throw new Exception("Add data type for Excel");

                        if (k1 == 2)
                        {
                            cell.CellStyle = workbook.CreateCellStyle();
                            cell.CellStyle.WrapText = true;
                            // cell.CellStyle.Alignment.
                            cell.CellStyle.SetFont(boldFont);

                            if (!separateColumnNo.HasValue && Equals(value, ""))
                                separateColumnNo = k2;
                        }

                        if (k2 == (separateColumnNo ?? -1))
                        {
                            cell.CellStyle = workbook.CreateCellStyle();
                            cell.CellStyle.FillForegroundColor = IndexedColors.Yellow.Index;
                            cell.CellStyle.FillPattern = FillPattern.SolidForeground;
                        }
                    }
                }

                if (separateColumnNo.HasValue)
                {
                    ((XSSFSheet)sheet).GetColumnHelper().SetColDefaultStyle(separateColumnNo.Value, separatorStyle);
                }

                var lastColumNum = sheet.GetRow(3).LastCellNum;
                for (var i = 1; i <= lastColumNum; i++)
                {
                    sheet.AutoSizeColumn(i, true);
                    GC.Collect();
                }

                sheet.CreateFreezePane(lastColumNum - 31, 4);
            }

            using (var sw = File.Create(Settings.MinuteYahooLogFolder + "test.xlsx"))
            {
                workbook.Write(sw);
            }

        }

        public static void AA(Dictionary<string, List<object[]>> context)
        {
            const int rowOffset = 1;

            IWorkbook workbook = new XSSFWorkbook();
            IDataFormat format = workbook.CreateDataFormat();

            var boldFont = workbook.CreateFont();
            boldFont.IsBold = true;

            ICellStyle separatorStyle = workbook.CreateCellStyle();
            separatorStyle.FillForegroundColor = IndexedColors.Yellow.Index;
            separatorStyle.FillPattern = FillPattern.SolidForeground;

            foreach (var kvp in context)
            {
                var sheet = workbook.CreateSheet(kvp.Key);
                int? separateColumnNo = null;
                for (var k1 = 0; k1 < kvp.Value.Count; k1++)
                {
                    var row = sheet.CreateRow(rowOffset + k1);
                    //if (k1==2)
                    // row.Height = 5000;

                    var oo = kvp.Value[k1];
                    for (var k2 = 0; k2 < oo.Length; k2++)
                    {
                        var cell = row.CreateCell(k2);
                        var value = oo[k2];
                        if (value == null)
                            cell.SetCellValue((string)null);
                        else if (value is string s)
                            cell.SetCellValue(s);
                        else if (value is int a1)
                            cell.SetCellValue(a1);
                        else if (value is float a2)
                            cell.SetCellValue(a2);
                        else if (value is double a3)
                            cell.SetCellValue(a3);
                        else if (value is DateTime dt)
                            cell.SetCellValue(dt);
                        else if (value is TimeSpan ts)
                        {
                            var day = new TimeSpan(1, 0, 0, 0);
                            var tsValue = Convert.ToDouble(ts.Ticks) / Convert.ToDouble(day.Ticks);
                            SetValueAndFormat(workbook, cell, tsValue, format.GetFormat("h:mm"));
                        }
                        else
                            throw new Exception("Add data type for Excel");

                        if (k1 == 2)
                        {
                            cell.CellStyle = workbook.CreateCellStyle();
                            cell.CellStyle.WrapText = true;
                            // cell.CellStyle.Alignment.
                            cell.CellStyle.SetFont(boldFont);

                            if (!separateColumnNo.HasValue && Equals(value, ""))
                                separateColumnNo = k2;
                        }

                        if (k2 == (separateColumnNo ?? -1))
                        {
                            cell.CellStyle = workbook.CreateCellStyle();
                            cell.CellStyle.FillForegroundColor = IndexedColors.Yellow.Index;
                            cell.CellStyle.FillPattern = FillPattern.SolidForeground;
                        }
                    }
                }

                if (separateColumnNo.HasValue)
                {
                    ((XSSFSheet)sheet).GetColumnHelper().SetColDefaultStyle(separateColumnNo.Value, separatorStyle);
                }

                var lastColumNum = sheet.GetRow(3).LastCellNum;
                for (var i = 1; i <= lastColumNum; i++)
                {
                    sheet.AutoSizeColumn(i, true);
                    GC.Collect();
                }

                sheet.CreateFreezePane(lastColumNum - 31, 4);
            }

            using (var sw = File.Create(Settings.MinuteYahooLogFolder + "test.xlsx"))
            {
                workbook.Write(sw);
            }

        }

        static void SetValueAndFormat(IWorkbook workbook, ICell cell, double value, short formatId)
        {
            //set value for the cell
            cell.SetCellValue(value);

            ICellStyle cellStyle = workbook.CreateCellStyle();
            cellStyle.DataFormat = formatId;
            cell.CellStyle = cellStyle;
        }

    }
}
