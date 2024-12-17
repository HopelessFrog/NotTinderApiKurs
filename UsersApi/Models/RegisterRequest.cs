namespace UsersApi.Models.Entitys;

public class RegisterRequest
{
    //[Required(ErrorMessage = "Login is required.")]
    //[MinLength(5, ErrorMessage = "Login must be at least 5 characters long.")]
    //[MaxLength(20, ErrorMessage = "Login cannot be longer than 20 characters.")]
    public string Login { get; set; }

    //[Required(ErrorMessage = "Password is required.")]
    //[MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
    public string Password { get; set; }

    //[Required(ErrorMessage = "Email is required.")]
    //[EmailAddress(ErrorMessage = "Invalid email address format.")]
    public string Email { get; set; }
}