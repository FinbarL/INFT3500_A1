using INFT3500.Models;

namespace INFT3500.ViewModels;

public class OrderViewModel
{
    public Order Order { get; set; }
    public List<ProductsInOrder> ProductsInOrder { get; set; }
    
    public List<ProductViewModel> Products { get; set; }
}