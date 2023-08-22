namespace INFT3500.ViewModels;

using System.ComponentModel.DataAnnotations;

public class UserViewModel
{
    public string? UserName { get; set; }
    public string? Name { get; set; }
    public string? EmailAddress { get; set; }
    
    public string? Password { get; set; }
    public string? BillingEmail { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? PostCode { get; set; }
    public string? Suburb { get; set; }
    public string? State { get; set; }
    public string? CardNumber { get; set; }
    public string? CardOwner { get; set; }
    public string? CardExpiry { get; set; }
    public string? CardCVV { get; set; }
    public bool IsAdmin { get; set; }
    public bool IsStaff { get; set; }
}