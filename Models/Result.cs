using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient.DataClassification;

namespace onlineexamproject.Models
{
    public class Result
    {
        [Key]
        public int  ResultId { get; set; }

        [Required]
        public int Student_Id { get; set; }

        [Required]
        public int  ExamId { get; set; }

        [Required]
        [Range (0,int.MaxValue,ErrorMessage ="Score cannot be negative")]
        public int Score { get; set; }

    }
}
