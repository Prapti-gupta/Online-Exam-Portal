using Microsoft.AspNetCore.Mvc;
using onlineexamproject.Extensions;
using onlineexamproject.Models;
using System.Data;
using System.Data.SqlClient;


namespace onlineexamproject.Controllers
{
    public class TeacherDashboardController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult TeacherDashboard()
        {

            User? sessionUser = HttpContext.Session.GetObjectFromJson<User>("sessionUser");
            if (sessionUser == null)
            {
                return RedirectToAction("Teacher", "Login");
            }
            else if (sessionUser.Role == "Teacher")
            {
                List<Course> courses = new List<Course>();
                return View(courses);
            }
            return RedirectToAction("Teacher", "Login");



        }


       /* public List<Course> ViewCourseList()
        {
            string connectionString = "Data Source=LAPTOP-VLJVC05O\\SQLEXPRESSInitial Catalog=exams;Integrated Security=True;";
            List<Course> courses = new List<Course>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = "SELECT * FROM Course";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Course course = new Course
                                {
                                    Course_Id = reader.GetInt32("Course_Id"),
                                    Course_Name = reader.GetString("Course_Name"),
                                    Course_Description = reader.GetString("Course_Description")
                                };
                                courses.Add(course);
                            }
                        }
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            return courses;
        }*/

    }

}
