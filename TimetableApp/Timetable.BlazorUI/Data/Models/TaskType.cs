namespace Timetable.BlazorUI.Data.Models
{
    public class TaskType
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public Guid CreatedById { get; set; }
    }
}
