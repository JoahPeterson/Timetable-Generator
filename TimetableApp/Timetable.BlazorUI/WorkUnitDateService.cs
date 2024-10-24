using Mono.TextTemplating;

namespace Timetable.BlazorUI;

public class WorkUnitDateService
{
    public static async Task<Course> UpdateWorkUnitsStartAndEndDate(Course course, IWorkUnitData workUnitData)
    {
        int weekCount = 0;

        foreach (WorkUnit workUnit in course.WorkUnits)
        {
            DateTime startDate = course.StartDate.Date.AddDays(weekCount * 7).Date;
            DateTime endDate = startDate.AddDays(workUnit.Duration * 7).Date;
            
            weekCount += workUnit.Duration;

            if (startDate == workUnit.StartDate && endDate == workUnit.EndDate)
                continue;

            workUnit.StartDate = startDate.Date;
            workUnit.EndDate = endDate.Date;

            await workUnitData.UpdateAsync(workUnit);
        }

        return course;
    }

    public static WorkUnit SetWorkUnitStartAndEndDate (Course course, WorkUnit workUnit)
    {
        int weekCount = 0;

        foreach (WorkUnit wu in course.WorkUnits)
        {
            weekCount += wu.Duration;
        }

        workUnit.StartDate = course.StartDate.AddDays(weekCount * 7).Date;
        workUnit.EndDate = workUnit.StartDate?.AddDays(workUnit.Duration * 7).Date;

        return workUnit;
    }
}
