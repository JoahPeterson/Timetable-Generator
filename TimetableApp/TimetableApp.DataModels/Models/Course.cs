namespace Timetable.BlazorUI.Data.Models
{
    public class Course
    {
        public string Id { get; set; }

        public string CreatedById { get; set; }

        public DateTime? DateCreated { get; set; }

        public string? Description { get; set; }

        public DateTime EndDate { get; set; }

        public bool IsArchived { get; set; }

        public DateTime? LastModified { get; set; }

        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        public string TermId { get; set; }

        public List<WorkUnit> WorkUnits { get; set; }

    }
}
