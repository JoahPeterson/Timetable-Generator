namespace Timetable.BlazorUI;

public class WorkUnitDateService
{
    IWorkUnitData _workUnitData;

    public WorkUnitDateService(IWorkUnitData workUnitData)
    {
        _workUnitData = workUnitData;
    }

    /// <summary>
    /// Set the Start and End date of a WorkUnit
    /// </summary>
    /// <param name="course">Course that the workunit belongs to.</param>
    /// <param name="workUnit">New workunit that need the dates set.</param>
    /// <returns>The work unit with the dates set.</returns>
    public WorkUnit SetWorkUnitStartAndEndDate(Course course, WorkUnit workUnit)
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

    /// <summary>
    /// Updated a the start and end dates in a course's work units.
    /// </summary>
    /// <param name="course">Course which need to be updated</param>
    /// <returns>The updated course.</returns>
    public async Task<Course> UpdateWorkUnitsStartAndEndDate(Course course)
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

            await _workUnitData.UpdateAsync(workUnit);
        }
        return course;
    }
}
