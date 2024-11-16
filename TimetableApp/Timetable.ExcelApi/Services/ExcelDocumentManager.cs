using Castle.Core.Logging;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using TimetableApp.DataModels.DataAccess;
using TimetableApp.DataModels.Models;

namespace Timetable.ExcelApi.Services;

public class ExcelDocumentManager
{
    private readonly ITaskTypeData _taskTypeData;
    private readonly IUserData _userData;
    private readonly IWorkTaskData _workTaskData;

    private readonly int _courseHeaderRow = 2;
    private readonly string[] _courseHeaders = { "Start Date", "Course Duration", "Term", "Name", "Description" };
    private readonly int _workUnitHeaderRow = 4;
    private readonly string[] _workUnitHeaders = { "Work Unit", "Task Name", "Duration Min", "Due Date", "Category" };

    public ExcelDocumentManager(ITaskTypeData taskTypeData, IUserData userData, IWorkTaskData workTaskData)
    {
        _taskTypeData = taskTypeData;
        _userData = userData;
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
        worksheet.Cell(currentRow, tempColumnIndex).Value = _courseHeaders[0];
        worksheet.Cell(currentRow + 1, tempColumnIndex).Value = course.StartDate.ToString("d");

        tempColumnIndex++;
        worksheet.Cell(currentRow, tempColumnIndex).Value = _courseHeaders[1];
        worksheet.Cell(currentRow + 1, tempColumnIndex).Value = course.Term.Duration;

        tempColumnIndex++;
        worksheet.Cell(currentRow, tempColumnIndex).Value = _courseHeaders[2];
        worksheet.Cell(currentRow + 1, tempColumnIndex).Value = course.Term.Name;

        tempColumnIndex++;
        worksheet.Cell(currentRow, tempColumnIndex).Value = _courseHeaders[3];
        worksheet.Cell(currentRow + 1, tempColumnIndex).Value = course.Name;

        tempColumnIndex++;
        worksheet.Cell(currentRow, tempColumnIndex).Value = _courseHeaders[4];
        worksheet.Cell(currentRow + 1, tempColumnIndex).Value = course.Description;

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

        //// Add headers
        for (int i = 0; i < _workUnitHeaders.Length; i++)
        {
            worksheet.Cell(currentRow, i + 1).Value = _workUnitHeaders[i];
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
        var dataRange = worksheet.Range(_workUnitHeaderRow, 1, currentRow - 1, _workUnitHeaders.Length);

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

    public async Task<Course> CreateCourseFromExcel(Stream excelStream, string userId)
    {
        using var workbook = new XLWorkbook(excelStream);
        var worksheet = workbook.Worksheets.FirstOrDefault(w => w.Name == "Timetable");

        if (worksheet == null)
        {
            throw new Exception("Worksheet 'Timetable' not found.");
        }

        if (!AreCourseHeadersValid(worksheet))
        {
            throw new Exception("Course headers are not valid.");
        }

        if (!AreTaskHeadersValid(worksheet))
        {
            throw new Exception("Task headers are not valid.");
        }

        if (string.IsNullOrEmpty(userId))
        {
            throw new Exception("User ID is not valid.");
        }

        User user = await _userData.GetByIdAsync(userId);

        if (user == null)
        {
            throw new Exception("User not found.");
        }

        List<TaskType> taskTypes = await _taskTypeData.GetUsersTaskTypesAsync(userId);
        List<WorkTask> workTasks = await _workTaskData.GetUsersWorkTasksAsync(userId);

        // Extract Course details
        var course = new Course
        {
            Description = worksheet.Cell(_courseHeaderRow + 1, Array.IndexOf(_courseHeaders, "Description")).GetString(),
            Name = worksheet.Cell(_courseHeaderRow + 1, Array.IndexOf(_courseHeaders, "Name")).GetString(),
            StartDate = DateTime.Parse(worksheet.Cell(_courseHeaderRow + 1, Array.IndexOf(_courseHeaders, "Start Date")).GetString()),
            Term = new Term
            {
                Duration = int.Parse(worksheet.Cell(_courseHeaderRow + 1, Array.IndexOf(_courseHeaders, "Course Duration")).GetString()),
                Name = worksheet.Cell(3, 3).GetString() // Assuming term name at (3, 3)
            },
            WorkUnits = new List<WorkUnit>()
        };



        // Parse work units and tasks from the worksheet, starting from row 6
        int currentRow = 6;
        while (!worksheet.Cell(currentRow, 1).IsEmpty())
        {
            var workUnitName = worksheet.Cell(currentRow, 1).GetString();
            var workUnit = course.WorkUnits.FirstOrDefault(w => w.Name == workUnitName) ?? new WorkUnit
            {
                Name = workUnitName,
                Tasks = new List<WorkUnitTask>()
            };

            var taskName = worksheet.Cell(currentRow, 2).GetString();
            var duration = int.Parse(worksheet.Cell(currentRow, 3).GetString());
            var dueDate = DateTime.TryParse(worksheet.Cell(currentRow, 4).GetString(), out var parsedDate)
                ? parsedDate
                : (DateTime?)null;
            var category = worksheet.Cell(currentRow, 5).GetString();

            //workUnit.Tasks.Add(new WorkUnitTask
            //{
            //    TaskId = Guid.NewGuid().ToString(), // Generate a new ID if not available
            //    Name = taskName,
            //    Duration = duration,
            //    DueDate = dueDate,
            //    Category = category
            //});

            //// Ensure the work unit is added only once
            //if (!course.WorkUnits.Contains(workUnit))
            //{
            //    course.WorkUnits.Add(workUnit);
            //}

            //currentRow++;
        }

        return course;
    }

    private void VerifyTaskCategories(IEnumerable<string> taskCategories)
    {
        if (taskCategories == null || !taskCategories.Any())
        {
            throw new Exception("Task categories are not valid.");
        }
    }

    private bool AreCourseHeadersValid(IXLWorksheet worksheet)
    {
        for (int i = 0; i < _courseHeaders.Length; i++)
        {
            if (!worksheet.Cell(_courseHeaderRow, i + 1).GetString().Equals(_courseHeaders[i], StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
        }

        return true;
    }

    private bool AreTaskHeadersValid(IXLWorksheet worksheet)
    {
        for (int i = 0; i < _workUnitHeaders.Length; i++)
        {
            if (!worksheet.Cell(_workUnitHeaderRow, i + 1).GetString().Equals(_workUnitHeaders[i], StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
        }

        return true;
    }
}