namespace Timetable.BlazorUI.Data.Models
{
    public class Task
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string TypeId { get; set; }

        public string CreatedById { get; set; }

        public DateTime? DateCreated { get; set; }

        public DateTime? LastModified { get; set; }

        public bool IsArchived { get; set; }
    }
}
