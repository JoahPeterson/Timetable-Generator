using Microsoft.AspNetCore.Mvc;
using ClosedXML.Excel;
using System.Text.Json;
using System.Dynamic;
using System.IO;
using TimetableApp.DataModels.Models;
using Timetable.ExcelApi.Services;
using Microsoft.AspNetCore.Http;

namespace Timetable.ExcelApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ExcelExportController : ControllerBase
{
    private readonly ExcelDocumentManager _excelManager;
    public ExcelExportController(ExcelDocumentManager excelManager)
    {
        _excelManager = excelManager;
    }

    [HttpPost("export")]
    public async Task<IActionResult> Export([FromBody] JsonDocument jsonData)
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var data = JsonSerializer.Deserialize<Course>(jsonData, options);

            if (data == null)
            {
                return BadRequest("Invalid or empty JSON data");
            }

            var fileContents = await _excelManager.ProcessCourse(data);

            return File(
                fileContents,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"TimeTable_{DateTime.Now:yyyyMMddHHmmss}.xlsx"
            );
        }
        catch (Exception ex)
        {
            return BadRequest($"Error processing request: {ex.Message}");
        }
    }

    [HttpPost("import")]
    public async Task<IActionResult> Import([FromHeader(Name = "X-User-Id")] string userId, IFormFile file)
    {
        if (string.IsNullOrEmpty(userId))
            return BadRequest("User ID is missing in the header.");

        if (file == null || file.Length == 0)
            return BadRequest("No file was uploaded or the file is empty.");

        if (!file.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase) ||
                file.ContentType != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
        {
            return BadRequest("The uploaded file must be an Excel (.xlsx) file.");
        }

        try
        {
            using var stream = file.OpenReadStream();

            // Assuming _excelManager has a method to read and create a Course object from the Excel file
            var course = await _excelManager.CreateCourseFromExcel(stream, userId);

            if (course == null)
            {
                return BadRequest("Excel formatting error, please see help page for guidance.");
            }

            // You can return a result based on your needs, such as a confirmation message or the parsed data
            return Ok(course); // Or return relevant response based on your use case
        }
        catch (Exception ex)
        {
            return BadRequest($"Error processing file: {ex.Message}");
        }
    }
}