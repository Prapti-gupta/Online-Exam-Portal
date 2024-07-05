using Microsoft.AspNetCore.Mvc;
using onlineexamproject.Extensions;
using onlineexamproject.Models;
using System.Data.SqlClient;

namespace onlineexamproject.Controllers
{
    public class StudentDashboardController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult StudentDashboard()
        {
            User? sessionUser = HttpContext.Session.GetObjectFromJson<User>("sessionUser");
            if (sessionUser == null)
            {
                return RedirectToAction("Student", "Login");
            }
            else if (sessionUser.Role == "Student")
            {
                List<Course>? enrolledCourses = getEnrolledCoursesInfo(sessionUser.Id);
                List<Exam>? exams = getExamsFromCourse_Id(getEnrolledCoursesId(sessionUser.Id));
                List<Course>? notEnrolledCourses = getNotEnrolledCoursesInfo(sessionUser.Id);
                // Add other tables as needed

                var viewModel = new StudentDashboardView
                {
                    EnrolledCourses = enrolledCourses,
                    Exams = exams,
                    NotEnrolledCourses = notEnrolledCourses
                    // Add other tables as needed
                };

                return View(viewModel);
            }
            return RedirectToAction("Student", "Login");
        }

        /* helper functions */
        private List<int>? getEnrolledCoursesId(int Student_Id)
        {
            string connectionString = "Data Source=LAPTOP-VLJVC05O\\SQLEXPRESS;Initial Catalog=exams;Integrated Security=True;";
            List<int>? enrolledCourses = new List<int>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = "SELECT Course_Id FROM StudentEnrolled WHERE Student_Id=@Student_Id";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Student_Id", Student_Id);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int course_Id = reader.GetInt32(reader.GetOrdinal("Course_Id"));
                                enrolledCourses.Add(course_Id);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred while retrieving enrolled course IDs: " + ex.Message);
                    throw;
                }
            }
            return enrolledCourses;
        }


        /* main functions */

        private List<Course>? getEnrolledCoursesInfo(int Student_Id)
        {
            string connectionString = "Data Source=LAPTOP-VLJVC05O\\SQLEXPRESS;Initial Catalog=exams;Integrated Security=True;";
            List<Course>? enrolledCourses = new List<Course>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Using JOIN query
                    string query = "SELECT c.* FROM Course c INNER JOIN StudentEnrolled se ON c.Course_Id = se.Course_Id WHERE se.Student_Id = @Student_Id";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Student_Id", Student_Id);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Course tempCourse = new Course(
                                  reader.GetString(reader.GetOrdinal("Course_Description")),
                                  reader.GetString(reader.GetOrdinal("Course_Name")),
                                  reader.GetInt32(reader.GetOrdinal("Course_Id"))
                                );
                                enrolledCourses.Add(tempCourse);
                            }
                        }
                    }

                    if (!enrolledCourses.Any())
                    {
                        return null;
                    }

                    return enrolledCourses;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred while retrieving enrolled courses: " + ex.Message);
                    throw;
                }
            }
        }

        private List<Course>? getNotEnrolledCoursesInfo(int Student_Id)
        {
            /* this function returns courses that the student is NOT involved in */
            string connectionString = "Data Source=LAPTOP-VLJVC05O\\SQLEXPRESS;Initial Catalog=exams;Integrated Security=True;";
            List<Course>? enrolledCourses = new List<Course>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Using a JOIN query
                    string query = "SELECT c.* FROM Course c WHERE NOT EXISTS (SELECT 1 FROM StudentEnrolled se WHERE se.Student_Id = @Student_Id AND se.Course_Id = c.Course_Id)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Student_Id", Student_Id);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Course tempCourse = new Course(
                                  reader.GetString(reader.GetOrdinal("Course_Description")),
                                  reader.GetString(reader.GetOrdinal("Course_Name")),
                                  reader.GetInt32(reader.GetOrdinal("Course_Id"))
                                );
                                enrolledCourses.Add(tempCourse);
                            }
                        }
                    }

                    if (!enrolledCourses.Any())
                    {
                        return null;
                    }

                    return enrolledCourses;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred while retrieving enrolled courses: " + ex.Message);
                    throw;
                }
            }
        }


        private List<Exam>? getExamsFromCourse_Id(List<int>? course_IdArray)
        {
            string connectionString = "Data Source=LAPTOP-VLJVC05O\\SQLEXPRESS;Initial Catalog=exams;Integrated Security=True;";
            List<Exam>? exams = new List<Exam>();

            if (course_IdArray == null)
            {
                return null;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    for (int i = 0; i < course_IdArray.Count; i++)
                    {
                        string query = "SELECT * FROM Exam WHERE Course_Id=@course_Id";
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@course_Id", course_IdArray[i]);

                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    int examId = reader.GetInt32(reader.GetOrdinal("ExamId"));
                                    int getCourseId = reader.GetInt32(reader.GetOrdinal("Course_Id"));
                                    string examName = reader.GetString(reader.GetOrdinal("ExamName"));
                                    DateTime examdatetime = reader.GetDateTime(reader.GetOrdinal("ExamDateTime"));

                                    Exam exam = new Exam(examId, course_IdArray[i], examName,examdatetime);
                                    exams.Add(exam);
                                }
                            }
                        }

                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine("SQL Exception: " + ex.ToString());
                    return null;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception: " + ex.ToString());
                    return null;
                }
            }
            return exams;
        }
    }
}
