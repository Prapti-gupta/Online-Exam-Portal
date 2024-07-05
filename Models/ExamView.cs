using System.Collections.Generic;

namespace onlineexamproject.Models
{
    public class ExamView
    {
        public List<Questions>? Questions { get; set; }
        public List<StudentChoice>? StudentChoices { get; set; }
        public int Student_Id { get; set; }
        public int ExamId { get; set; }
    }
}
