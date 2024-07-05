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
    public class CourseController : BaseController
    {
        private readonly IConfiguration _configuration;

        public CourseController(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        // GET: Course
        public IActionResult Index()
        {
            DataTable dtbl = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
            {
                sqlConnection.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("CourseAll", sqlConnection);
                sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
                sqlDa.Fill(dtbl);
            }
            return View(dtbl);
        }

        // GET: Course/AddorEdit/
        public IActionResult AddorEdit(int? id)
        {
            Course course = new Course();
            if(id > 0)
            {
                course = FetchCourseById(id);
            }
            return View(course);
        }

        // POST: Course/AddorEdit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddorEdit(int id, [Bind("Course_Id,Course_Name,Course_Description")] Course course)
        {

            if (ModelState.IsValid)
            {
                using(SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
                {
                    sqlConnection.Open();
                    SqlCommand sqlCmd = new SqlCommand("CourseAddorEdit", sqlConnection);
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.Parameters.AddWithValue("Course_Id", course.Course_Id);
                    sqlCmd.Parameters.AddWithValue("Course_Name", course.Course_Name);
                    sqlCmd.Parameters.AddWithValue("Course_Description", course.Course_Description);
                    sqlCmd.ExecuteNonQuery();
                }
               
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        // GET: Course/Delete/5
        public IActionResult Delete(int? id)
        {
           Course course = FetchCourseById(id);
            return View(course);
        }

        // POST: Course/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
            {
                sqlConnection.Open();
                SqlCommand sqlCmd = new SqlCommand("CourseDeleteById", sqlConnection);
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Parameters.AddWithValue("Course_Id",id);
                sqlCmd.ExecuteNonQuery();
            }

            return RedirectToAction(nameof(Index));
        }

        [NonAction]
        public Course FetchCourseById(int? id)
        {
             Course course = new Course();

            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
            {
                DataTable dtbl = new DataTable();
                sqlConnection.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("CourseById", sqlConnection);
                sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
                sqlDa.SelectCommand.Parameters.AddWithValue("Course_Id", id);
                sqlDa.Fill(dtbl);
                if (dtbl.Rows.Count == 1)
                {
                    course.Course_Id = Convert.ToInt32(dtbl.Rows[0]["Course_Id"].ToString());
                    course.Course_Name = dtbl.Rows[0]["Course_Name"].ToString();
                    course.Course_Description = dtbl.Rows[0]["Course_Description"].ToString();

                }
                return course;
            }
        }
    }
}
