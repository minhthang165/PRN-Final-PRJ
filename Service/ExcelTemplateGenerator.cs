using System;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace PRN_Final_Project.Service
{
    public class ExcelTemplateGenerator
    {
        public byte[] GenerateScheduleTemplate()
        {
            using (var workbook = new XSSFWorkbook())
            {
                // Create a new sheet
                var sheet = workbook.CreateSheet("Schedule Template");

                // Create header row
                var headerRow = sheet.CreateRow(0);
                headerRow.CreateCell(0).SetCellValue("ClassId");
                headerRow.CreateCell(1).SetCellValue("SubjectId");
                headerRow.CreateCell(2).SetCellValue("LessonsPerWeek");
                headerRow.CreateCell(3).SetCellValue("MentorId");

                // Add some example data (optional)
                var exampleRow = sheet.CreateRow(1);
                exampleRow.CreateCell(0).SetCellValue(1); // Example ClassId
                exampleRow.CreateCell(1).SetCellValue(1); // Example SubjectId
                exampleRow.CreateCell(2).SetCellValue(2); // Example LessonsPerWeek
                exampleRow.CreateCell(3).SetCellValue(1); // Example MentorId

                // Auto-size columns
                for (int i = 0; i < 4; i++)
                {
                    sheet.AutoSizeColumn(i);
                }

                // Write to memory stream
                using (var ms = new MemoryStream())
                {
                    workbook.Write(ms);
                    return ms.ToArray();
                }
            }
        }
    }
}
