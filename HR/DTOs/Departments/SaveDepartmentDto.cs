namespace HR.DTOs.Departments
{
    public class SaveDepartmentDto // عشان احفظ الداتا اللي جايه من الكلاينت قبل ما احولها لموديل
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? FloorNumber { get; set; }
        public long? TypeId { get; set; }
    }
}
