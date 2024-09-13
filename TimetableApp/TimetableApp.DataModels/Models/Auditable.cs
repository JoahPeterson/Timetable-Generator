using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimetableApp.DataModels.Models
{
    public class Auditable
    {
        public string CreatedById { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.Now;

        public bool IsArchived { get; set; } = false; 

        public DateTime LastModified { get; set; } = DateTime.Now;
    }
}
