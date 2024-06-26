using INFT3500.Models;
using INFT3500.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

using Newtonsoft.Json;

namespace INFT3500.Controllers;
[Route("[controller]")]
public class OrderController : Controller
{
    private readonly ILogger<OrderController> _logger;
    private readonly StoreDbContext _context;
    private ProductController _productController;

    public OrderController(StoreDbContext context, ILogger<OrderController> logger)
    {
        _context = context;
        _logger = logger;
        _productController = new ProductController(_context);
    }
    [HttpGet("[action]")]
    public async Task<IActionResult> Index()
    {
        var username = User.Identity.Name;
        var userTo = _context.Tos.Where(t => t.UserName == username).Include(t => t.Orders).FirstOrDefault();
        var orders = userTo.Orders.ToList();
        var orderHistoryViewModel = new OrderHistoryViewModel()
        {
            Orders = new List<OrderViewModel>()
        };
        foreach (var order in orders)
        {
            var orderViewModel = new OrderViewModel()
            {
                Order = order,
                ProductsInOrder = _context.ProductsInOrders
                    .Where(pio => pio.OrderId == order.OrderId)
                    .Include(pio => pio.Produkt)
                    .ThenInclude(s => s.Product).ToList(),
                Products = new List<ProductViewModel>()
            };
            orderHistoryViewModel.Orders.Add(orderViewModel);
        }
        foreach(var order in orderHistoryViewModel.Orders)
        {
            foreach(var productInOrder in order.ProductsInOrder)
            {
                Console.WriteLine("CALLED!");
                var productId = productInOrder.Produkt.ProductId;
                if (productId != null)
                {
                    var productViewModel = await _productController.GetProductViewModelById((int)productId);
                    Console.WriteLine(JsonConvert.SerializeObject(productViewModel));
                    productViewModel.Quantity = (int)productInOrder.Quantity!;
                    order.Products.Add(productViewModel);
                }
            }
        }
        return View(orderHistoryViewModel);
    }

}
