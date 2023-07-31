using System.ComponentModel.DataAnnotations;

namespace INFT3500.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Please enter your username or email.")]
    public string UserName { get; set; }
    [Required(ErrorMessage = "Firstname is required.")]
    public string FirstName { get; set; }
    [Required(ErrorMessage = "Lastname is required.")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "Please enter your email address.")]
    [DataType(DataType.EmailAddress)]
    public string EmailAddress { get; set; }

    [Required(ErrorMessage = "Please enter your password.")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required(ErrorMessage = "Please confirm your password.")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; }
    [Required]
    public string BillingEmail { get; set; }
    [Required]
    public string PhoneNumber { get; set; }
    [Required]
    public string Address { get; set; }
    [Required]
    [StringLength(4)]
    public string PostCode { get; set; }
    [Required]
    public string Suburb { get; set; }
    [Required]
    public string State { get; set; }
    [Required]
    public string CardNumber { get; set; }
    [Required]
    public string CardOwner { get; set; }
    [Required]
    public string CardExpiry { get; set; }
    [Required]
    public string CardCVV { get; set; }
}