using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using onlineexamproject.Models;
using System.Text.RegularExpressions;
//using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Data.SqlClient;

namespace onlineexamproject.Controllers;

public class SignupController : BaseController
{
    private readonly ILogger<SignupController> _logger;

    public SignupController(ILogger<SignupController> logger)
    {
        _logger = logger;
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

    public bool validPassword(string pwd)
    {
        const int MinimumLength = 8;
        const int MaximumLength = 16;
        const string UpperCasePattern = @"[A-Z]";
        const string LowerCasePattern = @"[a-z]";
        const string DigitPattern = @"\d";
        const string SpecialCharacterPattern = @"[!@#$%^&*(),.?:{}|<>]";
        if (string.IsNullOrWhiteSpace(pwd))
        {
            return false;
        }

        if (pwd.Length < MinimumLength || pwd.Length > MaximumLength)
        {
            return false;
        }

        if (!Regex.IsMatch(pwd, UpperCasePattern))
        {
            return false;
        }

        if (!Regex.IsMatch(pwd, LowerCasePattern))
        {
            return false;
        }

        if (!Regex.IsMatch(pwd, DigitPattern))
        {
            return false;
        }

        if (!Regex.IsMatch(pwd, SpecialCharacterPattern))
        {
            return false;
        }
        return true;
    }

    public bool uniqueStudentUsername(string username)
    {
        string connectionString = "Data Source=LAPTOP-VLJVC05O\\SQLEXPRESS;Initial Catalog=exams;Integrated Security=True;";
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            try
            {
                connection.Open();

                string query = "SELECT Username FROM Student WHERE username=@username";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@username", username);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
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

    public bool AddStudentEntry(string username, string password)
    {
        string connectionString = "Data Source=LAPTOP-VLJVC05O\\SQLEXPRESS;Initial Catalog=exams;Integrated Security=True";
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            try
            {
                connection.Open();
                string query = "INSERT INTO Student (Username, Password) values (@username, @password)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", password);
                command.ExecuteNonQuery();
                return true;
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }
    }

    [HttpPost]
    public IActionResult Student(Signup signupModel)
    {
        if (ModelState.IsValid)
        {
            if (signupModel.ConfirmPassword == signupModel.Password
                && uniqueStudentUsername(signupModel.Username)
                && validPassword(signupModel.Password))
            {
                AddStudentEntry(signupModel.Username, signupModel.Password);
                return View("~/Views/Login/Student.cshtml");
            }
            else
            {
                if (uniqueStudentUsername(signupModel.Username) != true)
                {
                    ModelState.AddModelError("", "Username already taken.");
                }
                else if (validPassword(signupModel.Password) != true)
                {
                    ModelState.AddModelError("", "Password is not strong enough!");
                }
                else
                {
                    ModelState.AddModelError("", "Passwords do not match.");
                }
            }
        }
        return View("Student");
    }

    public bool AddTeacherEntry(string username, string password)
    {
        string connectionString = "Data Source=LAPTOP-VLJVC05O\\SQLEXPRESS;Initial Catalog=exams;Integrated Security=True";
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            try
            {
                connection.Open();
                string query = "INSERT INTO Teacher (Username, Password) values (@username, @password)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", password);
                command.ExecuteNonQuery();
                return true;
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }
    }

    public bool uniqueTeacherUsername(string username)
    {
        string connectionString = "Data Source=LAPTOP-VLJVC05O\\SQLEXPRESS;Initial Catalog=exams;Integrated Security=True";
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            try
            {
                connection.Open();

                string query = "SELECT Username FROM Teacher WHERE username=@username";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@username", username);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
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
    public IActionResult Teacher(Signup signupModel)
    {
        if (ModelState.IsValid)
        {
            if (signupModel.ConfirmPassword == signupModel.Password
                && uniqueStudentUsername(signupModel.Username)
                && validPassword(signupModel.Password))
            {
                AddTeacherEntry(signupModel.Username, signupModel.Password);
                return View("~/Views/Login/Teacher.cshtml");
            }
            else
            {
                if (uniqueTeacherUsername(signupModel.Username) != true)
                {
                    ModelState.AddModelError("", "Username already taken.");
                }
                else if (validPassword(signupModel.Password) != true)
                {
                    ModelState.AddModelError("", "Password is not strong enough!");
                }
                else
                {
                    ModelState.AddModelError("", "Passwords do not match.");
                }
            }
        }
        return View("Teacher");
    }
}