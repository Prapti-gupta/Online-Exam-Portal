namespace onlineexamproject.Models;
public class Signup
{
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string ConfirmPassword { get; set; }
    public bool RememberMe { get; set; }
}
