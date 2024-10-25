using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Timetable.XUnitTests
{
    public class Auditable
    {
        public string CreatedById { get; set; }
        public bool IsArchived { get; set; }
    }
}
