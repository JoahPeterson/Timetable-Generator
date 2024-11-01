using System.Runtime.CompilerServices;
using TimetableApp.DataModels.DataAccess;
using TimetableApp.DataModels.Models;
using static System.Formats.Asn1.AsnWriter;

namespace Timetable.BlazorUI.Services;

public class DuplicationService
{
    private readonly IWorkUnitData _workUnitData;
    private readonly WorkUnitDateService _workUnitDateService;
    private readonly ICourseData _courseData;

    public DuplicationService(IWorkUnitData workUnitData, ICourseData courseData, WorkUnitDateService workUnitDateService)
    {
        _courseData = courseData;
        _workUnitData = workUnitData;
        _workUnitDateService = workUnitDateService;
    }

    public async Task<Course> DuplicateCourseAsync (Course courseToDuplicate)
    {
        Course newCourse = new Course()
        {
            CourseTypeId = courseToDuplicate.CourseTypeId,
            Description = courseToDuplicate.Description,
            EndDate = courseToDuplicate.EndDate,
            Name = courseToDuplicate.Name.Contains(" -COPY") ? courseToDuplicate.Name : courseToDuplicate.Name + " -COPY",
            StartDate = courseToDuplicate.StartDate,
            Term = courseToDuplicate.Term,
        };

        newCourse.AuditInformation.CreatedById = courseToDuplicate.AuditInformation.CreatedById;
        newCourse = await _courseData.CreateCourseAsync(newCourse);

        foreach (var workUnit in courseToDuplicate.WorkUnits)
        {
            WorkUnit duplicatedWorkUnit = await DuplicateWorkUnitAsync(newCourse, workUnit, isCopy: false);
            newCourse.WorkUnits.Add(duplicatedWorkUnit);
        }
        return await _courseData.UpdateCourseAsync(newCourse);
    }

    public async Task<WorkUnit> DuplicateWorkUnitAsync(Course course, WorkUnit workUnitToDuplicate, bool isCopy = true)
    {
        // The new duplicated task
        WorkUnit dupeWorkUnit = new WorkUnit
        {
            Name = isCopy == false || workUnitToDuplicate.Name.Contains(" -COPY")? workUnitToDuplicate.Name : workUnitToDuplicate.Name + " -COPY",
            Duration = workUnitToDuplicate.Duration,
            CourseId = course.Id,
            SequenceNumber = course.WorkUnits.Count + 1,
        };

        // Set Audit info
        dupeWorkUnit.AuditInformation.CreatedById = workUnitToDuplicate.AuditInformation.CreatedById;

        // Set Dates
        dupeWorkUnit = _workUnitDateService.SetWorkUnitStartAndEndDate(course, dupeWorkUnit);

        // Create Work unit in database
        await _workUnitData.CreateAsync(dupeWorkUnit);

        // Assign tasks to duplicated work unit
        dupeWorkUnit.Tasks = HandleWorkUnitTaskDuplication(workUnitToDuplicate.Tasks, dupeWorkUnit, isCopy );

        // Update the duplicated work unit to include duplicated tasks
        await _workUnitData.UpdateAsync(dupeWorkUnit);

        return dupeWorkUnit;
    }

    private List<WorkUnitTask> HandleWorkUnitTaskDuplication(List<WorkUnitTask> tasks, WorkUnit workUnit, bool isCopy)
    {
        // List to hold duplicated work unit tasks
        List<WorkUnitTask> duplicatedTasks = new List<WorkUnitTask>();

        // Loop through the tasks and update them acoordingly 
        foreach (var originalTask in tasks)
        {
            var newTask = new WorkUnitTask
            {
                Id = Guid.NewGuid().ToString(),
                Duration = originalTask.Duration,
                TaskId = originalTask.TaskId,
                WorkUnitId = workUnit.Id,
            };

            newTask.DueDate = isCopy ? workUnit.EndDate : originalTask.DueDate;
            duplicatedTasks.Add(newTask);
        }

        // Return the updated tasks list
        return duplicatedTasks;
    }
}
