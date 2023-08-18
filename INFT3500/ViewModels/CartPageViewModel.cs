using INFT3500.Models;
namespace INFT3500.ViewModels;

public class CartPageViewModel
{
    public List<CartViewModel> Products {get; set; }
    public double CartTotal => Products.Sum(p => p.Product.Price * p.Quantity);
    
}