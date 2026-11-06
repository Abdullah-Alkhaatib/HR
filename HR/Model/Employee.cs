using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Model
{
    public class Employee // Model class
    {
        public long Id { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }
        //public int? Age { get; set; } // Nullable // Optional parameter
        public DateTime? BirthDate { get; set; }

        //[MaxLength(50)]
        //public string Position { get; set; }

        [MaxLength(50)]
        public string? Phone { get; set; }
        public bool IsActive { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        // Foreign Key
        [ForeignKey("DepartmentRow")]
        public long? DepartmentId { get; set; } // Foreign Key
        public Department? DepartmentRow { get; set; } // Navigation Property = > باشر على التيبل اللي بدي اعمل عليها foreign key

        [ForeignKey("Manager")]
        public long? ManagerId { get; set; } // Foreign Key
        public Employee? Manager { get; set; }

        [ForeignKey("Lookup")]
        public long? PositionId { get; set; } // Foreign Key
        public Lookup? Lookup { get; set; } // Navigation Property


        [ForeignKey("User")]
        public long? UserId { get; set; } // Foreign Key
        public User? User { get; set; } // Navigation Property

        public string? ImagePath { get; set; }
    }
}
