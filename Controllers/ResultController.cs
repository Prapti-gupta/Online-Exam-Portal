using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
    public class ResultController : BaseController
    {
        private readonly IConfiguration _configuration;

        public ResultController(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        public IActionResult All()
        {
            User? sessionUser = HttpContext.Session.GetObjectFromJson<User>("sessionUser");
            if (sessionUser == null)
            {
                return RedirectToAction("Student", "Login");
            }

            int studentId = sessionUser.Id;

            DataTable dtbl = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
            {
                sqlConnection.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("StudentResultsById", sqlConnection);
                sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
                sqlDa.SelectCommand.Parameters.AddWithValue("@StudentId", studentId);
                sqlDa.Fill(dtbl);
            }
            return View(dtbl);
        }

        // GET: Result
        public IActionResult Index()
        {
            DataTable dtbl = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
            {
                sqlConnection.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("ResultAll", sqlConnection);
                sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
                sqlDa.Fill(dtbl);

            }
            return View(dtbl);
        }

        // GET: Result/Edit/5
        public IActionResult Edit(int? id)
        {
            Result result = new Result();
            if(id > 0)
            {
                result = FetchResultById(id);
            }
            return View(result);
        }

        // POST: Result/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("ResultId,Student_Id,ExamId,Score")] Result result)
        {
            

            if (ModelState.IsValid)
            {
                using (SqlConnection sqlConnection= new SqlConnection(_configuration.GetConnectionString("DevConnection")))
                {
                    sqlConnection.Open();
                    SqlCommand sqlCmd =  new SqlCommand("ResultEdit",sqlConnection);
                    sqlCmd.CommandType = CommandType.StoredProcedure ;
                    sqlCmd.Parameters.AddWithValue("ResultId", result.ResultId);
                    sqlCmd.Parameters.AddWithValue("Student_Id", result.Student_Id); 
                    sqlCmd.Parameters.AddWithValue("ExamId", result.ExamId);
                    sqlCmd.Parameters.AddWithValue("Score", result.Score);
                    sqlCmd.ExecuteNonQuery();
                }
                
                return RedirectToAction(nameof(Index));
            }
            return View(result);
        }

        [NonAction]
        public Result FetchResultById(int? id)
        {
            Result result = new Result();
            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
            {
                DataTable dtbl = new DataTable();
                sqlConnection.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("ResultById", sqlConnection);
                sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
                sqlDa.SelectCommand.Parameters.AddWithValue("ResultId", id);
                sqlDa.Fill(dtbl);
                if (dtbl.Rows.Count == 1)
                {
                    result.ResultId = Convert.ToInt32(dtbl.Rows[0]["ResultId"].ToString());
                    result.Student_Id = Convert.ToInt32(dtbl.Rows[0]["Student_Id"].ToString());
                    result.ExamId = Convert.ToInt32(dtbl.Rows[0]["ExamId"].ToString());
                    result.Score = (int)Convert.ToDouble(dtbl.Rows[0]["Score"].ToString());
                }
                return result;
            }
        }





        public IActionResult Result(int examId)
        {
            
            try
            {
                User? sessionUser = HttpContext.Session.GetObjectFromJson<User>("sessionUser");
                if (sessionUser == null)
                {
                    return RedirectToAction("Student", "Login");
                }

                int student_Id = sessionUser.Id;

                List<StudentChoice> studentChoices = GetStudentChoices(student_Id, examId);
                if (studentChoices == null || studentChoices.Count == 0)
                {
                    return RedirectToAction("StudentDashboard", "StudentDashboard");
                }

                List<Questions> question = GetQuestions(examId);
                float percentage = GetResult(studentChoices, question);

                var resultViewModel = new ResultView
                {
                    StudentChoices = studentChoices,
                    OriginalAnswers = question,
                    ExamId = examId,
                    Student_Id = student_Id,
                    Percentage = percentage
                };
                bool StoreResultStatus = StoreResult(student_Id, examId, percentage);

                return View(resultViewModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return View("Error");
            }
        }

        private float GetResult(List<StudentChoice> studentChoices, List<Questions> correctAns)
        {
            float totalQuestions = correctAns.Count;
            float correctAnswers = 0;

            foreach (var choice in studentChoices)
            {
                var correctAnswer = correctAns.Find(q => q.Q_Id == choice.Q_Id)?.Correct_Answer;
                if (correctAnswer == choice.OptionSelected)
                {
                    correctAnswers++;
                }
            }

            if (totalQuestions == 0) return 0;
            float result = (correctAnswers * 100) / totalQuestions;
            return result;
        }

        private List<StudentChoice> GetStudentChoices(int student_Id, int examId)
        {
            List<StudentChoice> choices = new List<StudentChoice>();
            string connectionString = "Data Source=LAPTOP-VLJVC05O\\SQLEXPRESS;Initial Catalog=exams;Integrated Security=True;";
            string query = "SELECT * FROM StudentChoice WHERE Student_Id=@Student_Id AND ExamId=@ExamId";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Student_Id", student_Id);
                        command.Parameters.AddWithValue("@ExamId", examId);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int questionId = reader.GetInt32(reader.GetOrdinal("Q_Id"));
                                string optionSelected = reader.GetString(reader.GetOrdinal("OptionSelected"));

                                StudentChoice choice = new StudentChoice
                                {
                                    Student_Id = student_Id,
                                    ExamId = examId,
                                    Q_Id = questionId,
                                    OptionSelected = optionSelected
                                };
                                choices.Add(choice);
                            }
                        }
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            return choices;
        }

        private List<Questions> GetQuestions(int examId)
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


        private bool StoreResult(int student_Id, int examId, float percentage)
        {
            string connectionString = "Data Source=LAPTOP-VLJVC05O\\SQLEXPRESS;Initial Catalog=exams;Integrated Security=True;";
            string query = "INSERT INTO Result (StudentId, ExamId, Score) VALUES (@StudentId, @ExamId, @Score)";

            bool success = false;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@StudentId", student_Id);
                        command.Parameters.AddWithValue("@ExamId", examId);
                        command.Parameters.AddWithValue("@Score", percentage);

                        int rowsAffected = command.ExecuteNonQuery();
                        success = rowsAffected == 1; // Successful insert returns 1 row affected
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            return success;
        }

    }
}
