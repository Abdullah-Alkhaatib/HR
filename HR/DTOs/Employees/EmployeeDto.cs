namespace HR.DTOs.Employees
{
    // Data Transfer Object (DTO) class => نسخه شبيهه بالموديل اللي عندي بس بحط القيم اللي بدي استحدمها بدون ما احط اشيا زياده ومعلومات حساسه وابعثها ل الكلاينت
    public class EmployeeDto // Data Transfer Object
    {
        public long Id { get; set; }
        public string Name { get; set; }
        //public int? Age { get; set; } // Nullable // Optional parameter
        public DateTime? BirthDate { get; set; }
        //public string Position { get; set; }
        public bool IsActive { get; set; }
        public DateTime StartDate { get; set; }
        public string? Phone { get; set; }
        public long? DepartmentId { get; set; } // Foreign Key
        public long? ManagerId { get; set; } // Foreign Key
        public string? DepartmentName { get; set; } // Foreign Key
        public string? ManagerName { get; set; } // Foreign Key
        public long? PositionId { get; set; } // Foreign Key
        public string? PositionName { get; set; } // Foreign Key

        public string? ImagePath { get; set; }
    }
}
