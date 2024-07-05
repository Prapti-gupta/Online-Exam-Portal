using System.Data.SqlClient;

namespace onlineexamproject.Models;

// used to keep login sessions across websites.
public class User
{
    public string Username { get; set; }
    public string Role { get; set; }
    public int Id { get; set; }

    public User(string username, string role, int id)
    {
        Username = username;
        Role = role;
        Id = id;
    }

    public int getId(string uname)
    {
        int tempId = -1;
        Console.WriteLine(Role);

        string connectionString = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=exams;Integrated Security=True;";
        string query = Role == "Student"
            ? "SELECT Student_Id FROM Student WHERE Username=@username"
            : "SELECT Teacher_Id FROM Teacher WHERE Username=@username";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            try
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@username", uname);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read(); // Advance to the first row
                            tempId = reader.GetInt32(0); // Get the value of the first column
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.ToString());
                return -1;
            }
        }

        return tempId;
    }
}
