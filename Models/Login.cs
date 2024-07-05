namespace onlineexamproject.Models;
public class Login
{
    public required string Username { get; set; }
    public required string Password { get; set; }
    public bool RememberMe { get; set; }
}
