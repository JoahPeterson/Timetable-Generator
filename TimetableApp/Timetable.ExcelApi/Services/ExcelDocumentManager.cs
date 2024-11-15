using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using TimetableApp.DataModels.DataAccess;
using TimetableApp.DataModels.Models;

namespace Timetable.ExcelApi.Services;

public class ExcelDocumentManager
{
    private readonly ITaskTypeData _taskTypeData;
    private readonly IWorkTaskData _workTaskData;

    public ExcelDocumentManager(ITaskTypeData taskTypeData, IWorkTaskData workTaskData)
    {
        _taskTypeData = taskTypeData;
        _workTaskData = workTaskData;
    }
    public async Task<byte[]> ProcessCourse(Course course)
    {
        var getTaskData = _workTaskData.GetUsersWorkTasksAsync(course.AuditInformation.CreatedById);
        var getTaskTypeData = _taskTypeData.GetUsersTaskTypesAsync(course.AuditInformation.CreatedById);

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Timetable");

        // Create Header
        int currentRow = 1;
        // Add title in merged cells (first row)
        worksheet.Range($"A{currentRow}:E{currentRow}").Merge();
        worksheet.Cell($"A{currentRow}").Value = "Course Timetable";
        worksheet.Range($"A{currentRow}:E{currentRow}").Style
            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
            .Font.SetBold(true)
            .Font.SetFontSize(16)
            .Font.SetFontColor(XLColor.White)
            .Fill.SetBackgroundColor(XLColor.FromHtml("#F79646"));

        // Drop down a row
        currentRow++;
        
        // Create the Course details section
        var tempColumnIndex = 1;
        worksheet.Cell(currentRow, tempColumnIndex).Value = "Start Date";
        worksheet.Cell(currentRow + 1, tempColumnIndex).Value = course.StartDate.ToString("d");

        tempColumnIndex++;
        worksheet.Cell(currentRow, tempColumnIndex).Value = "Course Duration";
        worksheet.Cell(currentRow + 1, tempColumnIndex).Value = course.Term.Duration;

        tempColumnIndex++;
        worksheet.Cell(currentRow, tempColumnIndex).Value = "Term";
        worksheet.Cell(currentRow + 1, tempColumnIndex).Value = course.Term.Name;

        worksheet.Range($"A{currentRow}:E{currentRow}").Style
            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
            .Font.SetBold(true)
            .Font.SetFontSize(10);

        // Drop down a row
        currentRow++;
        worksheet.Range($"A{currentRow}:E{currentRow}").Style
            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
            .Font.SetItalic(true)
            .Font.SetFontSize(8);

        // Add Border to the Course Details Section
        worksheet.Range($"A{currentRow - 1}:E{currentRow}").Style
            .Border.SetOutsideBorder(XLBorderStyleValues.Thin);

        // Drop down a row
        currentRow++;

        var tableHeaderRow = currentRow;

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
            worksheet.Cell(currentRow, i + 1).Value = headers[i];
            worksheet.Cell(currentRow, i + 1).Style.Font.Bold = true;
        }

        // Drop down a row
        currentRow++;

        // Run both tasks in parallel and wait for completion
        await Task.WhenAll(getTaskData, getTaskTypeData);
        var taskDataResult = await getTaskData;
        var taskTypeDataResult = await getTaskTypeData;

        foreach (WorkUnit workUnit in course.WorkUnits)
        {
            if (workUnit.Tasks == null)
                continue;

            foreach (WorkUnitTask task in workUnit.Tasks)
            {
                WorkTask? workTask = taskDataResult.FirstOrDefault(td => td.Id == task.TaskId);
                worksheet.Cell(currentRow, 1).Value = workUnit.Name;
                worksheet.Cell(currentRow, 2).Value = workTask?.Name ?? "Task Name not found";
                worksheet.Cell(currentRow, 3).Value = Convert.ToInt32(task.Duration);
                worksheet.Cell(currentRow, 4).Value = task.DueDate?.ToString("d") ?? "N/A";
                worksheet.Cell(currentRow, 5).Value =
                    workTask != null
                        ? (workTask.TypeId != null
                            ? taskTypeDataResult.FirstOrDefault(tt => tt.Id == workTask.TypeId)?.Name ?? "Task Type not found"
                            : "N/A")
                        : "Work Task not found";
                currentRow++;
            }
        }

        // Style the worksheet
        var dataRange = worksheet.Range(tableHeaderRow,1, currentRow -1, headers.Length);

        var table = dataRange.CreateTable();
        table.ShowHeaderRow = true;
        table.Theme = XLTableTheme.TableStyleMedium7;
        
        // Auto-fit columns
        worksheet.Columns().AdjustToContents();

        // Create memory stream and return byte array
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
