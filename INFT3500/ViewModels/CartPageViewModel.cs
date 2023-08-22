using INFT3500.Models;
namespace INFT3500.ViewModels;

public class CartPageViewModel
{
    public List<CartViewModel> Products {get; set; }
    public UserViewModel User { get; set; }
    
}