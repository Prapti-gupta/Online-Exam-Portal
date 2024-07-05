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
using onlineexamproject.Models;

namespace onlineexamproject.Controllers
{
    public class QuestionsController : BaseController
    {
        private readonly IConfiguration _configuration;

        public QuestionsController(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        // GET: Questions
        public IActionResult Index(string searchString)
        {

            

            DataTable dtbl = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
            {
                sqlConnection.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("QuestionsAll",sqlConnection);
                sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
                sqlDa.Fill(dtbl);
            }

            return View(dtbl);
        }

        // GET: Questions/AddorEdit/
        public IActionResult AddorEdit(int? id)
        {

            Questions questions = new Questions();
            if(id > 0)
            {
                questions =FetchQuestionById(id);
            }
            return View(questions);
        }

        // POST: Questions/AddorEdit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddorEdit(int id, [Bind("Q_Id,ExamId,Question,op_1,op_2,op_3,op_4,Correct_Answer")] Questions questions)
        {
            
            if (ModelState.IsValid)
            {
                using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
                {
                    sqlConnection.Open();
                    SqlCommand sqlCmd = new SqlCommand("QuestionsAddorEdit", sqlConnection);
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.Parameters.AddWithValue("Q_Id", questions.Q_Id);
                    sqlCmd.Parameters.AddWithValue("ExamId", questions.ExamId);
                    sqlCmd.Parameters.AddWithValue("Question", questions.Question);
                    sqlCmd.Parameters.AddWithValue("op_1", questions.op_1);
                    sqlCmd.Parameters.AddWithValue("op_2", questions.op_2);
                    sqlCmd.Parameters.AddWithValue("op_3", questions.op_3);
                    sqlCmd.Parameters.AddWithValue("op_4", questions.op_4);
                    sqlCmd.Parameters.AddWithValue("Correct_Answer", questions.Correct_Answer);
                    sqlCmd.ExecuteNonQuery();
                }

                return RedirectToAction(nameof(Index));
            }
            return View(questions);
        }

        // GET: Questions/Delete/5
        public IActionResult Delete(int? id)
        {
            Questions questions = FetchQuestionById(id);

            
            return View(questions);
        }

        // POST: Questions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
            {
                sqlConnection.Open();
                SqlCommand sqlCmd = new SqlCommand("QuestionsDeleteById", sqlConnection);
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Parameters.AddWithValue("Q_Id", id);
                sqlCmd.ExecuteNonQuery();
            }

            return RedirectToAction(nameof(Index));
        }

       [NonAction]
       public  Questions FetchQuestionById(int? id)
        {
            Questions questions = new Questions();
            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
            {
                DataTable dtbl = new DataTable();
                sqlConnection.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("QuestionsById", sqlConnection);
                sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
                sqlDa.SelectCommand.Parameters.AddWithValue("Q_Id",id);
                sqlDa.Fill(dtbl);
                if(dtbl.Rows.Count == 1)
                {
                    questions.Q_Id = Convert.ToInt32(dtbl.Rows[0]["Q_Id"].ToString());
                    questions.ExamId = Convert.ToInt32(dtbl.Rows[0]["ExamId"].ToString());
                    questions.Question = dtbl.Rows[0]["Question"].ToString();
                    questions.op_1 = dtbl.Rows[0]["op_1"].ToString();
                    questions.op_2 = dtbl.Rows[0]["op_2"].ToString();
                    questions.op_3 = dtbl.Rows[0]["op_3"].ToString();
                    questions.op_4 = dtbl.Rows[0]["op_4"].ToString();
                    questions.Correct_Answer = dtbl.Rows[0]["Correct_Answer"].ToString();
                }
                return (questions);
            }
        }
    }
}
