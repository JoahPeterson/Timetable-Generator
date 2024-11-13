using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using TimetableApp.DataModels.Models;

namespace Timetable.ExcelApi.Services;

public class ExcelDocumentManager
{
    public byte[] ProcessCourse(Course course)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Timetable");

        // An example using CLOSED XML

        //// Define headers
        //var headers = new[]
        //{
        //    "Course Code",
        //    "Course Name",
        //    "Day",
        //    "Start Time",
        //    "End Time",
        //    "Room",
        //    "Lecturer"
        //};

        //// Add headers
        //for (int i = 0; i < headers.Length; i++)
        //{
        //    worksheet.Cell(1, i + 1).Value = headers[i];
        //    worksheet.Cell(1, i + 1).Style.Font.Bold = true;
        //}

        //// Add data
        //for (int row = 0; row < sessions.Count; row++)
        //{
        //    var session = sessions[row];
        //    worksheet.Cell(row + 2, 1).Value = session.CourseCode;
        //    worksheet.Cell(row + 2, 2).Value = session.CourseName;
        //    worksheet.Cell(row + 2, 3).Value = session.Day;
        //    worksheet.Cell(row + 2, 4).Value = session.StartTime;
        //    worksheet.Cell(row + 2, 5).Value = session.EndTime;
        //    worksheet.Cell(row + 2, 6).Value = session.Room;
        //    worksheet.Cell(row + 2, 7).Value = session.Lecturer;
        //}

        //// Style the worksheet
        //var dataRange = worksheet.Range(1, 1, sessions.Count + 1, headers.Length);
        //dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        //dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

        //// Auto-fit columns
        //worksheet.Columns().AdjustToContents();

        // Create memory stream and return byte array
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
