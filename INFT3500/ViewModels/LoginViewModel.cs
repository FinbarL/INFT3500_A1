using System.ComponentModel.DataAnnotations;

namespace INFT3500.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Please enter your username or email.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Please enter your password.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}