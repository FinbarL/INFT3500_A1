using INFT3500.Models;
using INFT3500.ViewModels;
using Microsoft.AspNetCore.Mvc;

public class AccountController : Controller
{
    private readonly StoreDbContext _dbContext; 

    public AccountController(StoreDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = _dbContext.Users.SingleOrDefault(u => u.UserName == model.UserName);

            if (user != null && ValidatePassword(model.Password, user.Salt, user.HashPw))
            {
                if (user.IsAdmin != null && user.IsAdmin.Value)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid username or password.");
                return View(model);
            }
        }
        else
        {
            ModelState.AddModelError("", "ModelState is invalid.");
            return View(model);
        }
    }

    // Helper method to validate the password
    private bool ValidatePassword(string password, string salt, string hashedPassword)
    {
        //Todo: add pw validation
        return password == hashedPassword;
    }
}