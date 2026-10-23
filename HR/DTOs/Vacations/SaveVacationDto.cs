namespace HR.DTOs.Vacations
{
    public class SaveVacationDto
    {
        public long? Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Notes { get; set; }
        public long EmployeeId { get; set; }
        public long TypeId { get; set; }
    }
}
