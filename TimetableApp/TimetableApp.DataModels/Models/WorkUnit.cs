namespace Timetable.BlazorUI.Data.Models
{
    public class WorkUnit
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Duration { get; set; }

        public int SequenceNumber { get; set; }

        public string CourseId { get; set; }

        public List<WorkUnitTask>? Tasks { get; set; }
    }
}
