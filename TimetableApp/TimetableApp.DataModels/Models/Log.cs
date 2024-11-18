using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace TimetableApp.DataModels.Models
{
    public class Log
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string? CallStack { get; set; }

        public string? LoggedInUserId {  get; set; }

        public string LogLevel { get; set; }

        public string? Message { get; set; }

        public DateTime TimeStamp { get; set; } = DateTime.Now;

    }
}
