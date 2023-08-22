using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace INFT3500.ViewModels;

public class UpdateUserViewModel
{
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public bool IsAdmin { get; set; }
    public bool IsStaff { get; set; }
    
    public string BillingEmail { get; set; }
    public string PhoneNumber { get; set; }
    public string Address { get; set; }
    public string PostCode { get; set; }
    public string Suburb { get; set; }
    public string State { get; set; } 
    public string CardNumber { get; set; }
    public string CardOwner { get; set; }
    public string CardExpiry { get; set; }
    public string CardCVV { get; set; }

}