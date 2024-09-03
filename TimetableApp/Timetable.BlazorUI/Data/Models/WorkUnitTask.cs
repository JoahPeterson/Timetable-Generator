namespace Timetable.BlazorUI.Data.Models
{
    public class WorkUnitTask
    {
        public Guid Id { get; set; }

        public string Duration { get; set; }

        public int SequenceNumber { get; set; }

        public Guid WorkUnitId { get; set; }

        public Guid TaskId { get; set; }

    }
}
