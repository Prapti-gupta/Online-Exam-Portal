using Microsoft.AspNetCore.Mvc;
using onlineexamproject.Extensions;
using onlineexamproject.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;


namespace onlineexamproject.Controllers
{
    public class ActionController : BaseController
    {
        private readonly ILogger<ActionController> _logger;

        public ActionController(ILogger<ActionController> logger)
        {
            _logger = logger;
        }

        public IActionResult Student() { return View(); }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public bool isStudentEnrolled(int Student_Id, int Course_Id)
        {
            
            string connectionString = "Data Source=LAPTOP-VLJVC05O\\SQLEXPRESS;Initial Catalog=exams;Integrated Security=True;";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = "SELECT * FROM StudentEnrolled WHERE Course_Id=@course_Id and Student_Id=@Student_Id";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Course_Id", Course_Id);
                        command.Parameters.AddWithValue("@Student_Id", Student_Id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }
            }
        }

        public IActionResult Enroll(int Student_Id, int Course_Id)
        {
            User? sessionUser = HttpContext.Session.GetObjectFromJson<User>("sessionUser");
            if (sessionUser == null)
            {
                return RedirectToAction("Student", "Login");
            }
            if (isStudentEnrolled(sessionUser.Id, Course_Id) == true)
            {
                Console.WriteLine("User is already enrolled");
                return RedirectToAction("StudentDashboard", "StudentDashboard");
            }
            string connectionString = "Data Source=LAPTOP-VLJVC05O\\SQLEXPRESS;Initial Catalog=exams;Integrated Security=True;";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = "INSERT INTO StudentEnrolled (Student_Id, Course_Id) VALUES (@Student_Id, @Course_Id)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Student_Id", sessionUser.Id);
                        command.Parameters.AddWithValue("@Course_Id", Course_Id);
                        int rowsAffected = command.ExecuteNonQuery();
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.ToString());
                    return View("Error");
                }
            }
            return RedirectToAction("StudentDashboard", "StudentDashboard");
        }


        public IActionResult Drop(int Student_Id, int Course_Id)
        {
            User? sessionUser = HttpContext.Session.GetObjectFromJson<User>("sessionUser");
            if (sessionUser == null)
            {
                return RedirectToAction("Student", "Login");
            }
            if (isStudentEnrolled(sessionUser.Id, Course_Id) == false)
            {
                Console.WriteLine("User is not enrolled");
                return RedirectToAction("StudentDashboard", "StudentDashboard");
            }
            string connectionString = "Data Source=LAPTOP-VLJVC05O\\SQLEXPRESS;Initial Catalog=exams;Integrated Security=True;";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = "DELETE FROM StudentEnrolled WHERE Student_Id = @Student_Id AND Course_Id = @Course_Id";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Student_Id", sessionUser.Id);
                        command.Parameters.AddWithValue("@Course_Id", Course_Id);
                        int rowsAffected = command.ExecuteNonQuery();
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.ToString());
                    return View("Error");
                }
            }
            return RedirectToAction("StudentDashboard", "StudentDashboard");
        }

    }
}
