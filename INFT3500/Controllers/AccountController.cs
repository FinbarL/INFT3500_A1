using System.Net.Security;
using System.Security.Claims;
using INFT3500.Models;
using INFT3500.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class AccountController : Controller
{
    private readonly StoreDbContext _context;

    public AccountController(StoreDbContext context)
    {
        _context = context;
    }
    
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == model.UserName);

            if (user != null && ValidatePassword(model.Password, user.Salt, user.HashPw))
            {
                await Authenticate(user);
                return RedirectToAction("UserInfo", "Account");
            }
            ModelState.AddModelError("", "Invalid username or password.");
                return View(model);
        }
        ModelState.AddModelError("", "ModelState Invalid.");
        return View(model);
    }
    [Authorize]
    public IActionResult UserInfo()
    {
        string username = User.Identity.Name;
        var user = _context.Users.FirstOrDefault(u => u.UserName == username);
        return View(user);
    }
    private async Task Authenticate(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Role, (bool)user.IsAdmin ? "Admin" : "Customer"),
            new Claim(ClaimTypes.Email, user.Email ?? String.Empty),
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        HttpContext.Session.SetString("Cart", "Test");

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
    }
    
    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }
    private bool ValidatePassword(string password, string salt, string hashedPassword)
    {
        //Todo: add pw validation
        return password == hashedPassword;
    }
}