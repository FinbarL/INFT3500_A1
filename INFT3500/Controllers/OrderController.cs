using INFT3500.Models;
using Microsoft.AspNetCore.Mvc;

namespace INFT3500.Controllers;

public class OrderController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly StoreDbContext _context;
    
    public OrderController(StoreDbContext context, ILogger<HomeController> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    public IActionResult Index()
    {
        return View();
    }
    
}