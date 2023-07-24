using System.Net.Security;
using System.Security.Claims;
using System.Security.Cryptography;
using INFT3500.Models;
using INFT3500.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
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
    
    public IActionResult Register()
    {
        return View();
    }
    [Authorize]
    [HttpGet]
    public IActionResult UpdateUser()
    {
        string username = User.Identity.Name;
        var user = _context.Users.FirstOrDefault(u => u.UserName == username);
        var updateUserViewModel = new UpdateUserViewModel
        {
            UserName = user.UserName,
            Email = user.Email,
            Name = user.Name,
            IsAdmin = user.IsAdmin ?? false,
            IsStaff = user.IsStaff ?? false
        };
        return View(updateUserViewModel);
    }
    [HttpGet]
    [Route("Account/UpdateUser/{userName}")]
    public async Task<IActionResult> UpdateUser(string userName)
    {
        var user = _context.Users.FirstOrDefault(u => u.UserName == userName);
        var updateUserViewModel = new UpdateUserViewModel
        {
            UserName = user.UserName,
            Email = user.Email,
            Name = user.Name,
            IsAdmin = user.IsAdmin ?? false,
            IsStaff = user.IsStaff ?? false
        };
        if (user != null)
        {
            _context.Attach(user);
            _context.Entry(user).Property(u => u.Email).IsModified = true;
            _context.Entry(user).Property(u => u.Name).IsModified = true;
            _context.Entry(user).Property(u => u.IsAdmin).IsModified = true;
            _context.Entry(user).Property(u => u.IsStaff).IsModified = true;
            await _context.SaveChangesAsync();
            return RedirectToAction("UserInfo", "Account");
        }

        
        return View(updateUserViewModel);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateUser(UpdateUserViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == model.UserName);
            if(user != null)
            {
                _context.Attach(user);
                _context.Entry(user).Property(u => u.Email).IsModified = true;
                _context.Entry(user).Property(u => u.Name).IsModified = true;
                _context.Entry(user).Property(u => u.IsAdmin).IsModified = true;
                _context.Entry(user).Property(u => u.IsStaff).IsModified = true;
                await _context.SaveChangesAsync();
                return RedirectToAction("UserInfo", "Account");
            }
        }
        Console.WriteLine("invalid!!!");
        return View(model);
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
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            var passwordMatches = model.Password == model.ConfirmPassword;
            var userExists = await _context.Users.AnyAsync(u => u.UserName == model.UserName);
            var emailExists = await _context.Users.AnyAsync(u => u.Email == model.emailAddress);
            if (!passwordMatches)
            {
                ModelState.AddModelError("ConfirmPassword", "Passwords do not match.");
                return View(model);
            }
            if (userExists)
            {
                ModelState.AddModelError("UserName", "Username already exists.");
                return View(model);
            }
            if (emailExists)
            {
                ModelState.AddModelError("emailAddress", "Email already in use.");
                return View(model);
            }
            var newUser = new User
            {
                UserName = model.UserName,
                Email = model.emailAddress,
                Salt = GenerateSalt(),
                HashPw = HashPassword(model.Password, GenerateSalt()),
                IsAdmin = false,
                IsStaff = false
            };
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
            await Authenticate(newUser);
            return RedirectToAction("UserInfo", "Account");
        }
        ModelState.AddModelError("ConfirmPassword", "ModelState Invalid.");
        return View(model);
    }
    
    [Authorize]
    public IActionResult UserInfo()
    {
        string username = User.Identity.Name;
        var user = _context.Users.FirstOrDefault(u => u.UserName == username);
        return View(user);
    }

    private string GenerateSalt()
    {
        byte[] salt = new byte[128 / 8];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }
        return Convert.ToBase64String(salt);
    }
    private string HashPassword(string password, string salt)
    {
        var bytes = KeyDerivation.Pbkdf2(
            password: password,
            salt: Convert.FromBase64String(salt),
            prf: KeyDerivationPrf.HMACSHA512,
            iterationCount: 10000,
            numBytesRequested: 256 / 8);

        return Convert.ToBase64String(bytes);
    }
    private async Task Authenticate(User user)
    {


        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email ?? String.Empty)
        };

        if (user.IsAdmin == true)
        {
            Console.WriteLine("Test");
            claims.Add(new Claim(ClaimTypes.Role, "Admin"));
        }
        else if (user.IsStaff == true)
        {
            Console.WriteLine("Test");
            claims.Add(new Claim(ClaimTypes.Role, "Staff"));
        }
        else
        {
            claims.Add(new Claim(ClaimTypes.Role, "Customer"));
        }

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