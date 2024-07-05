using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace onlineexamproject.Models
{
    [Table("Questions")]
    public class Questions
    {
        [Key]
        public int Q_Id { get; set; }

        [ForeignKey("Course_Id")]
        public int ExamId { get; set; }

        [Required]
        public string Question { get; set; }

        [Required]
        public string op_1 { get; set; }

        [Required]
        public string op_2 { get; set; }

        [Required]
        public string op_3 { get; set; }

        [Required]
        public string op_4 { get; set; }

        [Required]
        public string Correct_Answer { get; set; }

        public Questions() { }

        public Questions(int questionId, int examId, string questionText, string option1, string option2, string option3, string option4, string answer)
        {
            Q_Id = questionId;
            ExamId = examId;
            Question = questionText;
            op_1 = option1;
            op_2 = option2;
            op_3 = option3;
            op_4 = option4;
            Correct_Answer = answer;
        }

    }
}
