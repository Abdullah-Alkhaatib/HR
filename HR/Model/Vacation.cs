using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Model
{
    public class Vacation
    {
        public long Id { get; set; }


        [ForeignKey("Employee")]
        public long EmployeeId { get; set; } // Foreign Key
        public Employee Employee { get; set; } // Navigation Property
        public DateTime CreationDate { get; set; } = DateTime.Now; // Default Value
        public DateTime StartDate { get; set; } // Vacation Start Date
        public DateTime EndDate { get; set; } // Vacation End Date


        [ForeignKey("Type")]
        public long TypeId { get; set; } // Foreign Key
        public Lookup Type { get; set; } // Navigation Property

        public string? Notes { get; set; }
    }
}
