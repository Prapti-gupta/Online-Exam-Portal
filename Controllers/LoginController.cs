using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using onlineexamproject.Models;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using onlineexamproject.Extensions;

namespace onlineexamproject.Controllers
{
    public class LoginController : BaseController
    {
        private readonly ILogger<LoginController> _logger;
        public LoginController(ILogger<LoginController> logger)
        {
            _logger = logger;
        }

        public IActionResult StudentDashboard()
        {
            return View();
        }

        public IActionResult TeacherDashboard()
        {
            return View();
        }

        public IActionResult Student()
        {
            return View();
        }

        public IActionResult Teacher()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public bool dbAuthStudent(string Username, string Password)
        {
            string connectionString = "Data Source=LAPTOP-VLJVC05O\\SQLEXPRESS;Initial Catalog=exams;Integrated Security=True;";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = "SELECT * FROM Student WHERE username=@username AND password=@password";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@username", Username);
                        command.Parameters.AddWithValue("@password", Password);

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

        public bool dbAuthTeacher(string Username, string Password)
        {
            string connectionString = "Data Source=LAPTOP-VLJVC05O\\SQLEXPRESS;Initial Catalog=exams;Integrated Security=True;";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = "SELECT  * FROM Teacher WHERE username=@username AND password=@password";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@username", Username);
                        command.Parameters.AddWithValue("@password", Password);

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

        [HttpPost]
        public IActionResult Student(Login loginModel)
        {
            if (ModelState.IsValid)
            {
                if (dbAuthStudent(loginModel.Username, loginModel.Password))
                {

                    string uname = loginModel.Username;
                    User sessionUser = new User(uname, "Student", 0); // Set Id to a placeholder (0)
                    int Student_Id = sessionUser.getId(loginModel.Username); // Use loginModel.Username after creation
                    sessionUser.Id = Student_Id; // Update Id after retrieval
                    HttpContext.Session.SetObjectAsJson("sessionUser",sessionUser);

                    ViewBag.Username = sessionUser.Username;
                    ViewBag.Id = sessionUser.Id;

                    return RedirectToAction("StudentDashboard", "StudentDashboard");
                    //return Redirect("StudentSuccesss");

                    //return Redirect("StudentDashboard");
                }
                else
                {
                    ModelState.AddModelError("", "Wrong username or password.");
                }
            }
            return View(loginModel);
        }

        [HttpPost]
        public IActionResult Teacher(Login loginModel)
        {
            if (ModelState.IsValid)
            {
                if (dbAuthTeacher(loginModel.Username, loginModel.Password))
                {

                    string uname = loginModel.Username;
                    User sessionUser = new User(uname, "Teacher", 0); // Set Id to a placeholder (0)
                    int TeacherId = sessionUser.getId(loginModel.Username); // Use loginModel.Username after creation
                    sessionUser.Id = TeacherId; // Update Id after retrieval
                    HttpContext.Session.SetObjectAsJson("sessionUser", sessionUser);

                    ViewData["Username"] = sessionUser.Username;
                    ViewData["Id"] = sessionUser.Id;

                    return View("~/Views/TeacherDashboard/TeacherDashboard.cshtml");
                    //return View("TeacherDashboard");


                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                }
            }
            return View(loginModel);
        }

    }
}