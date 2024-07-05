using System.ComponentModel.DataAnnotations;

namespace onlineexamproject.Models
{
    public class Course
    {
        [Key]
        public int Course_Id { get; set; }
        
        public string? Course_Name { get; set; }
        
        public string? Course_Description { get; set; }

        public Course() { } 

        public Course(string _courseDescription, string _courseName, int _courseId)
        {
            Course_Description = _courseDescription;
            Course_Name = _courseName;
            Course_Id = _courseId;
        }
    }
}
