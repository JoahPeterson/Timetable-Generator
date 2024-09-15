using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimetableApp.DataModels.Models;
public class TermDuration
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public int Length { get; set; }

    public string ToolTip { get; } =  "Number of weeks in the term.";
}
