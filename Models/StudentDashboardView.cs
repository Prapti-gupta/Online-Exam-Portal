namespace onlineexamproject.Models
{
    public class StudentDashboardView
    {
        public IEnumerable<Course>? EnrolledCourses { get; set; }

        public IEnumerable<Exam>? Exams { get; set; }

        public IEnumerable<Course>? NotEnrolledCourses { get; set; }
    }
}
