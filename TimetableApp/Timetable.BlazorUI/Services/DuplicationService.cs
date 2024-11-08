using MudBlazor.Extensions;
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
        dupeWorkUnit.Tasks = HandleWorkUnitTaskDuplication(workUnitToDuplicate.Tasks, dupeWorkUnit, workUnitToDuplicate.StartDate, isCopy );

        // Update the duplicated work unit to include duplicated tasks
        await _workUnitData.UpdateAsync(dupeWorkUnit);

        return dupeWorkUnit;
    }

    private List<WorkUnitTask> HandleWorkUnitTaskDuplication(List<WorkUnitTask> tasks, WorkUnit workUnit, DateTime? originalWorkUnitStartDate, bool isCopy)
    {
        // List to hold duplicated work unit tasks
        List<WorkUnitTask> duplicatedTasks = new List<WorkUnitTask>();
 
        foreach (var originalTask in tasks)
        {
            var newTask = new WorkUnitTask
            {
                Id = Guid.NewGuid().ToString(),
                Duration = originalTask.Duration,
                TaskId = originalTask.TaskId,
                WorkUnitId = workUnit.Id,
            };

            if (isCopy)
            {
                int differenceInStartDays = (originalTask.DueDate?.Date- originalWorkUnitStartDate?.Date)?.Days ?? 0;
                newTask.DueDate = workUnit.StartDate?.AddDays(differenceInStartDays);
            }
            else
            {
                newTask.DueDate = originalTask.DueDate;
            }
            duplicatedTasks.Add(newTask);
        }

        // Return the updated tasks list
        return duplicatedTasks;
    }

    public async Task<List<WorkUnit>> CombineWorkUnits(List<WorkUnit> workUnits)
    {
        workUnits.Sort((wu1, wu2) =>
            (wu1?.StartDate ?? DateTime.MinValue).CompareTo(wu2?.StartDate ?? DateTime.MinValue));

        List<WorkUnit> combinedWorkUnits = new List<WorkUnit>();
        List<WorkUnit> workUnitsToRemove = new List<WorkUnit>();

        // Process pairs of work units
        for (int i = 0; i < workUnits.Count - 1; i += 2)
        {
            WorkUnit currentWorkUnit = workUnits[i];
            WorkUnit nextWorkUnit = workUnits[i + 1];

            currentWorkUnit.Tasks?.AddRange(
                HandleWorkUnitTaskDuplication(
                    nextWorkUnit.Tasks,
                    currentWorkUnit,
                    nextWorkUnit.StartDate,
                    true));

            await _workUnitData.UpdateAsync(currentWorkUnit);

            // Add to our lists
            combinedWorkUnits.Add(currentWorkUnit);
            workUnitsToRemove.Add(nextWorkUnit);
        }

        // Handle the last work unit if there's an odd number
        if (workUnits.Count % 2 != 0)
        {
            combinedWorkUnits.Add(workUnits.Last());
        }

        // Remove the unneeded work units
        foreach (WorkUnit workUnit in workUnitsToRemove)
        {
            await _workUnitData.DeleteAsync(workUnit.Id);
        }

        return combinedWorkUnits;
    }

}
