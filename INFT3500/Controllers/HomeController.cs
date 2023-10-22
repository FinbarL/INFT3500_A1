using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using INFT3500.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace INFT3500.Controllers;
[Route("[controller]")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly StoreDbContext _context;
    
    public HomeController(StoreDbContext context, ILogger<HomeController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [Authorize(Policy = "RequireAdminRole")]
    [HttpGet("[action]")]
    public async Task<IActionResult> AdminPage()
    {
        var userList = await GetUsers(null);
        return View(userList);
    }
    
    [Authorize(Policy = "RequireAdminRole")]
    [HttpPost("[action]")]
    public async Task<IActionResult> AdminPage(string searchString)
    {
        var userList = await GetUsers(searchString);
        return View(userList);
    }
    [Authorize(Roles = "Customer")]
    [HttpGet("[action]")]
    public IActionResult UserPage()
    {
        return View();
    }
    [Authorize]
    [Route("")]
    [HttpGet]
    public IActionResult Index()
    {
        return RedirectToAction("Index", "Product");
    }
    [Authorize]
    [HttpGet("[action]")]
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [HttpGet("[action]")]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    private async Task<List<User>> GetUsers(string? searchString)
    {
        if (searchString == null)
        {
            return await _context.Users.ToListAsync();
        }
        return await _context.Users.Where(u => u.UserName.Contains(searchString) || u.Name.Contains(searchString) || u.Email.Contains(searchString)).ToListAsync();
    }

}