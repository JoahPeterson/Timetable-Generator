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

        // Create top row with Course details

        // Add title in merged cells (first row)
        worksheet.Range("A1:E1").Merge();
        worksheet.Cell("A1").Value = "Course Timetable";
        worksheet.Range("A1:E1").Style
            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
            .Font.SetBold(true)
            .Font.SetFontSize(16)
            .Fill.SetBackgroundColor(XLColor.LightGray);

        worksheet.Cell(2, 2).Value = "Start Date";
        worksheet.Cell(2, 3).Value = course.StartDate;

        worksheet.Range("A3:E3").Merge();
        worksheet.Range("A3:E3").Style
            .Fill.SetBackgroundColor(XLColor.AntiqueBrass)
            .Font.SetFontSize(6);

        // Define headers
        var headers = new[]
        {
            "Work Unit",
            "Task Name",
            "Duration Min",
            "Due Date",
            "Category"
        };

        //// Add headers
        for (int i = 0; i < headers.Length; i++)
        {
            worksheet.Cell(4, i + 1).Value = headers[i];
            worksheet.Cell(4, i + 1).Style.Font.Bold = true;
        }

        List<WorkUnitTask> workUnitTasks = TaskListFlattened(course);

        //// Add data
        for (int row = 0; row < workUnitTasks.Count; row++)
        {
            var task = workUnitTasks[row];
            worksheet.Cell(row + 5, 1).Value = task.WorkUnitId;
            worksheet.Cell(row + 5, 2).Value = task.TaskId;
            worksheet.Cell(row + 5, 3).Value = task.Duration;
            worksheet.Cell(row + 5, 4).Value = task.DueDate;
            worksheet.Cell(row + 5, 5).Value = "NEEDS HELP";
        }

        //// Style the worksheet
        //var dataRange = worksheet.Range(1, 1, sessions.Count + 1, headers.Length);
        //dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        //dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

        // Auto-fit columns
        worksheet.Columns().AdjustToContents();

        // Create memory stream and return byte array
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }


    private List<WorkUnitTask> TaskListFlattened(Course course)
    {
        List<WorkUnitTask> flattenedList = new();

        foreach(WorkUnit workUnit in course.WorkUnits)
        {
            if (workUnit.Tasks != null && workUnit.Tasks.Any())
                flattenedList.AddRange(workUnit.Tasks);
        }

        return flattenedList;
    }
}
