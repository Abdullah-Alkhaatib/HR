namespace HR.DTOs.Vacations
{
    public class VacationDto
    {
        public long Id { get; set; }
        public long EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public long TypeId { get; set; }
        public string TypeName { get; set; }
        public string? Notes { get; set; }
    }
}
