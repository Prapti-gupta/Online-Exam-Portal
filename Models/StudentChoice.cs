namespace onlineexamproject.Models
{
    // used to send the student choices to the database

    public class StudentChoice
    {
        public int? Student_Id { get; set; }
        public int ExamId { get; set; }
        public int Q_Id { get; set; }
        public string? OptionSelected { get; set; } // can be null when autosubmitting
    }
}

