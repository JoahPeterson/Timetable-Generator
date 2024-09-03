namespace Timetable.BlazorUI.Data.Models
{
    public class WorkUnit
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Duration { get; set; }

        public int SequenceNumber { get; set; }

        public Guid CourseId { get; set; }

        public IEnumerable<WorkUnitTask>? Tasks { get; set; }
    }
}
