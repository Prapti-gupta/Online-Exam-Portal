using System.Collections.Generic;
namespace onlineexamproject.Models
{
    public class ResultView
    {
        public List<StudentChoice>? StudentChoices { get; set; }
        public List<Questions>? OriginalAnswers { get; set; }
        public int Student_Id { get; set; }
        public int ExamId { get; set; }
        public float Percentage { get; set; }
    }
}
