using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using INFT3500.Models;
using Microsoft.AspNetCore.Authorization;

namespace INFT3500.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }
    [Authorize(Roles = "Admin")]
    public IActionResult AdminPage()
    {
        return View();
    }

    [Authorize(Roles = "User")]
    public IActionResult UserPage()
    {
        return View();
    }
    public IActionResult Index()
    {
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Login", "Account");
        }
        return RedirectToAction("UserPage", "Home");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}