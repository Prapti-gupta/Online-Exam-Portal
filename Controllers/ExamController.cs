using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using onlineexamproject.Data;
using onlineexamproject.Extensions;
using onlineexamproject.Models;

namespace onlineexamproject.Controllers
{
    public class ExamController : BaseController
    {

        private readonly ILogger<ExamController> _logger;
        private readonly IConfiguration _configuration;

        public ExamController(ILogger<ExamController> logger, IConfiguration configuration)
        {
            _logger = logger;
            this._configuration = configuration;
        } 


        // GET: Exam
        public IActionResult Index()
        {
            DataTable dtbl = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
            {
                sqlConnection.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("ExamAll", sqlConnection);
                sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
                sqlDa.Fill(dtbl);

            }
            return View(dtbl);
        }

       

        // GET: Exam/AddorEdit/
        public IActionResult AddorEdit(int? id)
        {
           Exam exam = new Exam();
            if(id > 0)
            {
                exam = FetchBookById(id);
            }
            return View(exam);
        }

        // POST: Exam/AddorEdit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddorEdit(int id, [Bind("ExamId,ExamName,ExamDateTime,Course_Id")] Exam exam)
        {
            if (ModelState.IsValid)
            {
                
                using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
                {
                    sqlConnection.Open();
                    SqlCommand sqlCmd = new SqlCommand("ExamAddorEdit", sqlConnection);
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.Parameters.AddWithValue("ExamId", exam.ExamId);
                    sqlCmd.Parameters.AddWithValue("ExamName", exam.ExamName);
                    sqlCmd.Parameters.AddWithValue("ExamDateTime", exam.ExamDateTime);
                    sqlCmd.Parameters.AddWithValue("Course_Id", exam.Course_Id);
                    sqlCmd.ExecuteNonQuery();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(exam);
        }

        // GET: Exam/Delete/5
        public IActionResult Delete(int? id)
        {


            Exam exam = FetchBookById(id);

            return View(exam);
        }

        // POST: Exam/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
            {
                sqlConnection.Open();
                SqlCommand sqlCmd = new SqlCommand("ExamDeleteById", sqlConnection);
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Parameters.AddWithValue("ExamId", id);
                sqlCmd.ExecuteNonQuery();
            }

            return RedirectToAction(nameof(Index));
        }

        [NonAction]
        public Exam FetchBookById(int? id)
        {
            Exam exam= new Exam();
            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
            {
                DataTable dtbl = new DataTable();
                sqlConnection.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("ExamById", sqlConnection);
                sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
                sqlDa.SelectCommand.Parameters.AddWithValue("ExamId", id);
                sqlDa.Fill(dtbl);
                if (dtbl.Rows.Count == 1) 
                {
                    exam.ExamId = Convert.ToInt32(dtbl.Rows[0]["ExamId"].ToString());
                    exam.ExamName = dtbl.Rows[0]["ExamName"].ToString();
                    exam.ExamDateTime = Convert.ToDateTime(dtbl.Rows[0]["ExamDateTime"]);
                    exam.Course_Id = Convert.ToInt32(dtbl.Rows[0]["Course_Id"].ToString());
                }

                return exam;

            }
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult Attempt(int examId)
        {
            User? sessionUser = HttpContext.Session.GetObjectFromJson<User>("sessionUser");
            if (sessionUser == null)
            {
                return RedirectToAction("Student", "Login");
            }

            int student_Id = sessionUser.Id;
            return RedirectToRoute("exam", new { examId = examId });
        }

        public IActionResult Exam(int examId)
        {
            List<Questions> questionList = GetQuestions(examId);

            User? sessionUser = HttpContext.Session.GetObjectFromJson<User>("sessionUser");
            if (sessionUser == null)
            {
                return RedirectToAction("Student", "Login");
            }

            
            int userId = sessionUser.Id;

            ExamView examViewModel = new ExamView
            {
                Questions = questionList,
                StudentChoices = new List<StudentChoice>(),
                Student_Id = userId,
                ExamId = examId
            };

            return View(examViewModel);
        }

        public bool DeleteExistingAttempt(int student_Id, int examId)
        {
            string connectionString = "Data Source=LAPTOP-VLJVC05O\\SQLEXPRESS;Initial Catalog=exams;Integrated Security=True;";
            string query = "DELETE FROM StudentChoice WHERE Student_Id=@Student_Id AND ExamId=@ExamId;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Student_Id", student_Id);
                        command.Parameters.AddWithValue("@ExamId", examId);

                        int rowsAffected = command.ExecuteNonQuery();
                        Console.WriteLine(rowsAffected);
                        return rowsAffected > 0; // Return true if rows were deleted
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }
            }
        }

        public List<Questions> GetQuestions(int examId)
        {
            List<Questions> questions = new List<Questions>();

            string connectionString = "Data Source=LAPTOP-VLJVC05O\\SQLEXPRESS;Initial Catalog=exams;Integrated Security=True;";
            string query = "SELECT * FROM Questions WHERE ExamId=@ExamId";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ExamId", examId);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int Q_Id = reader.GetInt32(reader.GetOrdinal("Q_Id"));
                                string Question = reader.GetString(reader.GetOrdinal("Question"));
                                string op_1 = reader.GetString(reader.GetOrdinal("op_1"));
                                string op_2 = reader.GetString(reader.GetOrdinal("op_2"));
                                string op_3 = reader.GetString(reader.GetOrdinal("op_3"));
                                string op_4 = reader.GetString(reader.GetOrdinal("op_4"));
                                string Correct_Answer = reader.GetString(reader.GetOrdinal("Correct_Answer"));

                                Questions question = new Questions(Q_Id, examId, Question, op_1, op_2, op_3, op_4, Correct_Answer);
                                questions.Add(question);
                            }
                        }
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            return questions;
        }

        public bool StoreStudentChoice(StudentChoice choice)
        {
            string connectionString = "Data Source=LAPTOP-VLJVC05O\\SQLEXPRESS;Initial Catalog=exams;Integrated Security=True;";
            string query = @"
                INSERT INTO StudentChoice (Student_Id, ExamId, Q_Id, OptionSelected) 
                VALUES (@Student_Id, @ExamId, @Q_Id, @OptionSelected)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Student_Id", choice.Student_Id);
                        command.Parameters.AddWithValue("@ExamId", choice.ExamId);
                        command.Parameters.AddWithValue("@Q_Id", choice.Q_Id);
                        command.Parameters.AddWithValue("@OptionSelected", choice.OptionSelected);

                        command.ExecuteNonQuery();
                        return true;
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
        public IActionResult SubmitChoices(ExamView examViewModel)
        {
            if (examViewModel.StudentChoices == null)
            {
                return View(Error());
            }

            bool delStatus = DeleteExistingAttempt(examViewModel.Student_Id, examViewModel.ExamId);

            foreach (var choice in examViewModel.StudentChoices)
            {
                choice.Student_Id = examViewModel.Student_Id;
                choice.ExamId = examViewModel.ExamId;
                StoreStudentChoice(choice);
            }

            return RedirectToRoute("result", new { ExamId = examViewModel.ExamId });
        }
    }
}
