using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimetableApp.DataModels.Models
{
    public class Auditable
    {
        public int Id { get; set; }

        public string CreatedById { get; set; }

        public DateTime? DateCreated { get; set; }

        public bool IsArchived { get; set; }

        public DateTime? LastModified { get; set; }
    }
}
