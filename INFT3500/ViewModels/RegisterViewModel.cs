using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

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
    [EmailAddress(ErrorMessage = "Email address is not valid.")]
    public string EmailAddress { get; set; }

    [Required(ErrorMessage = "Please enter your password.")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required(ErrorMessage = "Please confirm your password.")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; }
    [Required(ErrorMessage = "Billing Email is required.")]
    [EmailAddress(ErrorMessage = "Billing Email is not valid.")]
    public string BillingEmail { get; set; }
    [Required]
    [Phone]
    public string PhoneNumber { get; set; }
    [Required]
    [RegularExpression(@"\d+\s+[A-Za-z]+(\s+[A-Za-z]+)*", ErrorMessage = "Address Line should contain both letters and numbers.")]
    public string Address { get; set; }
    [Required]
    [RegularExpression(@"^\d+$", ErrorMessage = "Post Code should contain numbers only.")]
    [StringLength(4, ErrorMessage = "Post Code should be 4 digits.")]
    public string PostCode { get; set; }
    [Required]
    public string Suburb { get; set; }

    [Required]
    public string State { get; set; }
    [Required]
    [CreditCard(ErrorMessage = "Card Number is not valid.")]
    public string CardNumber { get; set; }
    [Required]
    public string CardOwner { get; set; }
    [Required]
    [RegularExpression(@"^(0[1-9]|1[0-2])\/?([0-9]{2})$", ErrorMessage = "Card Expiry must be in MM/YY format.")]
    [ValidCardExpiry(ErrorMessage = "Card has expired.")]
    public string CardExpiry { get; set; }
    [Required]
    [RegularExpression(@"^\d+$", ErrorMessage = "Card CVV should contain numbers only.")]
    [StringLength(3, ErrorMessage = "Card CVV should be 3 digits.")]
    public string CardCVV { get; set; }

    public List<SelectListItem> States => new List<SelectListItem>
    {
        new SelectListItem {Value= "Australian Capital Territory", Text="ACT"},
        new SelectListItem {Value= "New South Wales", Text="NSW"},
        new SelectListItem {Value= "Northern Territory", Text="NT"},
        new SelectListItem {Value= "Queensland", Text="QLD"},
        new SelectListItem {Value= "South Australia", Text="SA"},
        new SelectListItem {Value= "Tasmania", Text="TAS"},
        new SelectListItem {Value= "Victoria", Text="VIC"},
        new SelectListItem {Value= "Western Australia", Text="WA"},
    };
}