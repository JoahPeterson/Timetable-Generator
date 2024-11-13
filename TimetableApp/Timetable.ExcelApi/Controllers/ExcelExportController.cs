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
    public IActionResult Export([FromBody] JsonDocument jsonData)
    {
        try
        {
            var data = JsonSerializer.Deserialize<Course>(jsonData);

            if (data == null)
            {
                return BadRequest("Invalid or empty JSON data");
            }

            var fileContents = _excelManager.ProcessCourse(data);

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
}