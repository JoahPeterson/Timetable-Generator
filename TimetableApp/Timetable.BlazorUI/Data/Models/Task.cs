namespace Timetable.BlazorUI.Data.Models
{
    public class Task
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public Guid TypeId { get; set; }

        public Guid CreatedById { get; set; }

        public DateTime? DateCreated { get; set; }

        public DateTime? LastModified { get; set; }

        public bool IsArchived { get; set; }
    }
}
