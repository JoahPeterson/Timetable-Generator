using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimetableApp.DataModels.Models;
public class TermDuration
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public Auditable AuditInformation { get; set; } = new();

    [Required(ErrorMessage = "The length is required.")]
    [Range(1, 22, ErrorMessage = "The length must be between 1 and 22.")]
    public int Length { get; set; }

    public string ToolTip { get; } =  "Number of weeks in the term.";
}
