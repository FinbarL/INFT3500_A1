using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace INFT3500.ViewModels;

[CreditCardDetailsValidation]
public class UpdateUserViewModel
{
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public bool IsAdmin { get; set; }
    public bool IsStaff { get; set; }

    [Required]
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
    public string PostCode { get; set; }

    [Required] public string Suburb { get; set; }
    [Required] public string State { get; set; }

    [CreditCard(ErrorMessage = "Card Number is not valid.")]
    public string? CardNumber { get; set; }

    public string? CardOwner { get; set; }


    [RegularExpression(@"^(0[1-9]|1[0-2])\/?([0-9]{2})$", ErrorMessage = "Card Expiry must be in MM/YY format.")]
    [ValidCardExpiry(ErrorMessage = "Card has expired.")]
    public string? CardExpiry { get; set; }

    [RegularExpression(@"^\d+$", ErrorMessage = "Card CVV should contain numbers only.")]
    [MinLength(3)]
    public string? CardCVV { get; set; }

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
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class CreditCardDetailsValidation : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var updateUserViewModel = validationContext.ObjectInstance as UpdateUserViewModel;
        if (updateUserViewModel == null)
        {
            return new ValidationResult("Invalid context.");
        }

        bool cardNumberEmpty = string.IsNullOrWhiteSpace(updateUserViewModel.CardNumber);
        bool cardOwnerEmpty = string.IsNullOrWhiteSpace(updateUserViewModel.CardOwner);
        bool cardExpiryEmpty = string.IsNullOrWhiteSpace(updateUserViewModel.CardExpiry);

        // If one is filled, all should be filled
        if (cardNumberEmpty != cardOwnerEmpty || cardNumberEmpty != cardExpiryEmpty)
        {
            return new ValidationResult("Card details should be either all filled or all left empty.");
        }

        return ValidationResult.Success;
    }
}
public class ValidCardExpiry : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var updateUserViewModel = validationContext.ObjectInstance as UpdateUserViewModel;
        if (updateUserViewModel == null)
        {
            return new ValidationResult("Invalid context.");
        }

        // If CardExpiry is provided, validate it
        if (!string.IsNullOrWhiteSpace(updateUserViewModel.CardExpiry))
        {
            var expiry = value as string;

            var dateParts = expiry.Split('/');
            if (dateParts.Length != 2)
            {
                return new ValidationResult("Card Expiry must be in MM/YY format.");
            }

            var currentDate = DateTime.Now;
            var currentMonth = currentDate.Month;
            var currentYear = currentDate.Year % 100;

            if (int.Parse(dateParts[1]) < currentYear || (int.Parse(dateParts[1]) == currentYear && int.Parse(dateParts[0]) < currentMonth))
            {
                return new ValidationResult("Card has expired.");
            }
        }

        return ValidationResult.Success;
    }
}