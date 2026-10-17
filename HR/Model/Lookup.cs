namespace HR.Model
{
    // Lecture 11 => 21 minutes
    public class Lookup
    {
        public long Id { get; set; }
        public int MajorCode { get; set; } // يعني  رقم الكود الرئيسي اللي بدي اعمل عليه lookup
        public int MinorCode { get; set; } // يعني  رقم الكود الفرعي اللي بدي اعمل عليه lookup
        public string Name { get; set; }
    }
}
