using System.ComponentModel.DataAnnotations;
namespace INFT3500.ViewModels
{
    public class RecoverPasswordViewModel
    {
        [Required(ErrorMessage = "Please enter your email.")]
        public string Email { get; set; }
        
        public string? TempPassword { get; set; }
    }
}