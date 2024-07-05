using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace onlineexamproject.Models
{
    public class Exam
    {
            [Key]
            public int ExamId { get; set; }

            [Required]
            public string? ExamName { get; set; }

        /*[Required]
          public DateTime ExamDateTime { get; set; }*/

           [Required]
           [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss.fff}", ApplyFormatInEditMode = true)]
            public DateTime? ExamDateTime { get; set; }


            [Required]
            public int? Course_Id { get; set; }

        public Exam() { }

        public Exam(int _examId, int _courseId, string _examName, DateTime _examDateTime)
        {
            ExamId = _examId;
            Course_Id = _courseId;
            ExamName = _examName;
            ExamDateTime = _examDateTime;
        }


    }
}
