using System.ComponentModel.DataAnnotations;

namespace INFT3500.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Please enter your username or email.")]
    public string UserName { get; set; }

    public string FirstName { get; set; }
    public string LastName { get; set; }

    [Required(ErrorMessage = "Please enter your email address.")]
    [DataType(DataType.EmailAddress)]
    public string emailAddress { get; set; }

    [Required(ErrorMessage = "Please enter your password.")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required(ErrorMessage = "Please confirm your password.")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; }
}