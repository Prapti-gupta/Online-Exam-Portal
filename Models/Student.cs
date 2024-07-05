namespace onlineexamproject.Models;

// class to send objects to and from database
public class Student
{
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required int Student_Id { get; set; }
}