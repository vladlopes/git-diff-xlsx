﻿using System;
using System.IO;
using System.Linq;
using OfficeOpenXml;

namespace git_diff_xlsx
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.Error.WriteLine("Usage: git-diff-xlsx.exe infile.xlsx");
                return -1;
            }

            var inputFilePath = args[0];

            try
            {
                Parse(inputFilePath, Console.Out);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
            
            return 0;
        }

        static void Parse(string inputFilePath, TextWriter output)
        {
            using (var inputStream = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var package = new ExcelPackage(inputStream))
            {
                PrintNames(package.Workbook, output);

                PrintLastEditedBy(package.Workbook, output);

                foreach (var sheet in package.Workbook.Worksheets.OrderBy(x => x.Index))
                {
                    PrintSheetContent(sheet, output);
                }
            }
        }

        static void PrintNames(ExcelWorkbook workbook, TextWriter output)
        {
            output.WriteLine(string.Join(",", workbook.Worksheets.OrderBy(x => x.Index).Select(x => x.Name)));
        }

        static void PrintLastEditedBy(ExcelWorkbook workbook, TextWriter output)
        {
            output.WriteLine("File last edited by " + workbook.Properties.LastModifiedBy);
        }

        static void PrintSheetContent(ExcelWorksheet sheet, TextWriter output)
        {
            output.WriteLine("=================================");
            output.WriteLine("Sheet: " + sheet.Name + "[ " + sheet.RowCount() + " , " + sheet.ColumnCount() + " ]");
            output.WriteLine("=================================");

            for (int row = 1; row < sheet.RowCount() + 1; row++)
            {
                for (int column = 1; column < sheet.ColumnCount() + 1; column++)
                {
                    var cell = sheet.Cells[row, column];
                    if (!string.IsNullOrEmpty(cell.Text))
                    {
                        output.WriteLine("    " + cell.Address + ": " + cell.Text);
                    }
                }
            }
            output.WriteLine();
        }
    }
}
