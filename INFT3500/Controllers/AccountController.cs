using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using INFT3500.Models;
using INFT3500.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace INFT3500.Controllers;
[Route("[controller]")]
public class AccountController : Controller
{
    private readonly StoreDbContext _context;
    public AccountController(StoreDbContext context)
    {
        _context = context;
    }
    [HttpGet("[action]")]
    public IActionResult Login()
    {
        return View();
    }
    [HttpPost("[action]")]
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

            ModelState.AddModelError("UserName", "Invalid username or password.");
            return View(model);
        }

        ModelState.AddModelError("", "ModelState Invalid.");
        return View(model);
    }

    [HttpGet("[action]")]
    public IActionResult RecoverAccount()
    {
        var recoverPasswordViewModel = new RecoverPasswordViewModel
        {
            Email = "",
            TempPassword = null,
        };
        return View(recoverPasswordViewModel);
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> RecoverAccount(RecoverPasswordViewModel recoverPasswordViewModel)
    {
        var emailAddress = recoverPasswordViewModel.Email;
        var user = _context.Users.FirstOrDefault(u => u.UserName == emailAddress || u.Email == emailAddress);
        //Generate temp password
        if (user != null)
        {
            var tempPassword = GenerateRandomPassword();
            Console.WriteLine("Temp Password: " + tempPassword);
            await ChangePassword(user.UserName, tempPassword);
            Console.WriteLine(recoverPasswordViewModel.Email);
            var viewModelWithTempPassword = new RecoverPasswordViewModel
            {
                Email = recoverPasswordViewModel.Email,
                TempPassword = tempPassword
            };
            return View(viewModelWithTempPassword);
        }

        ModelState.AddModelError("Email", "User not found");
        return View(recoverPasswordViewModel);
    }
    [HttpGet("[action]")]
    public IActionResult Register()
    {
        return View();
    }

    [Authorize]
    [HttpGet("[action]")]
    public IActionResult UpdateUser()
    {
        string username = User.Identity.Name;
        var updateUserViewModel = GetUpdateUserViewModel(username);
        return View(updateUserViewModel);
    }

    [HttpGet("UpdateUserByUserName/{userName}")]
    public async Task<IActionResult> UpdateUser(string userName)
    {
        var updateUserViewModel = GetUpdateUserViewModel(userName);
        return View(updateUserViewModel);
    }
    [NonAction]
    private UpdateUserViewModel GetUpdateUserViewModel(string userName)
    {
        var user = _context.Users.FirstOrDefault(u => u.UserName == userName);
        var userTo = _context.Tos.FirstOrDefault(u => u.UserName == userName);
        if (user == null && userTo == null)
        {
            return null;
        }

        var updateUserViewModel = new UpdateUserViewModel
        {
            UserName = user.UserName,
            Email = user.Email,
            Name = user.Name,
            IsAdmin = user.IsAdmin ?? false,
            IsStaff = user.IsStaff ?? false,
            BillingEmail = "",
            PhoneNumber = "",
            Address = "",
            PostCode = "",
            Suburb = "",
            State = "",
            CardNumber = "",
            CardOwner = "",
            CardExpiry = "",
            CardCVV = ""
        };
        if (userTo != null)
        {
            updateUserViewModel.BillingEmail = userTo.Email ?? "";
            updateUserViewModel.PhoneNumber = userTo.PhoneNumber ?? "";
            updateUserViewModel.Address = userTo.StreetAddress ?? "";
            updateUserViewModel.PostCode = userTo.PostCode.ToString() ?? "";
            updateUserViewModel.Suburb = userTo.Suburb ?? "";
            updateUserViewModel.State = userTo.State ?? "";
            updateUserViewModel.CardNumber = userTo.CardNumber ?? "";
            updateUserViewModel.CardOwner = userTo.CardOwner ?? "";
            updateUserViewModel.CardExpiry = userTo.Expiry ?? "";
            updateUserViewModel.CardCVV = userTo.Cvv.ToString() ?? "";
        }

        return updateUserViewModel;
    }

    [HttpPost("[action]")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateUser(UpdateUserViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == model.UserName);
            var userTo = await _context.Tos.FirstOrDefaultAsync(u => u.UserName == model.UserName);
            if (user != null)
            {
                user.Email = model.Email;
                user.Name = model.Name;
                user.IsAdmin = model.IsAdmin;
                user.IsStaff = model.IsStaff;
                await _context.SaveChangesAsync();
            }

            if (userTo == null)
            {
                var newUserDetails = new To
                {
                    UserName = user.UserName,
                    Email = model.BillingEmail,
                    PhoneNumber = model.PhoneNumber,
                    StreetAddress = model.Address,
                    PostCode = Convert.ToInt32(model.PostCode),
                    Suburb = model.Suburb,
                    State = model.State,
                    CardNumber = model.CardNumber,
                    CardOwner = model.CardOwner,
                    Expiry = model.CardExpiry,
                    Cvv = Convert.ToInt32(model.CardCVV),
                };
                _context.Tos.Add(newUserDetails);
                await _context.SaveChangesAsync();
            }
            else
            {
                userTo.Email = model.BillingEmail;
                userTo.PhoneNumber = model.PhoneNumber;
                userTo.StreetAddress = model.Address;
                userTo.PostCode = Convert.ToInt32(model.PostCode);
                userTo.Suburb = model.Suburb;
                userTo.State = model.State;
                userTo.CardNumber = model.CardNumber;
                userTo.CardOwner = model.CardOwner;
                userTo.Expiry = model.CardExpiry;
                userTo.Cvv = Convert.ToInt32(model.CardCVV);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("UserInfo", "Account", new { userName = model.UserName });
        }

        Console.WriteLine("invalid!!!");
        return View(model);
    }
    [NonAction]
    public UserViewModel GetUserViewModel(string userName)
    {
        var user = _context.Users.FirstOrDefault(u => u.UserName == userName);
        var userTo = _context.Tos.FirstOrDefault(u => u.UserName == userName);
        if (user == null && userTo == null)
        {
            return null;
        }

        var userViewModel = new UserViewModel
        {
            UserName = user.UserName,
            EmailAddress = user.Email,
            Name = user.Name,
            IsAdmin = user.IsAdmin ?? false,
            IsStaff = user.IsStaff ?? false,
        };
        if (userTo != null)
        {
            userViewModel.BillingEmail = userTo.Email ?? "";
            userViewModel.PhoneNumber = userTo.PhoneNumber ?? "";
            userViewModel.Address = userTo.StreetAddress ?? "";
            userViewModel.PostCode = userTo.PostCode.ToString() ?? "";
            userViewModel.Suburb = userTo.Suburb ?? "";
            userViewModel.State = userTo.State ?? "";
            userViewModel.CardNumber = userTo.CardNumber ?? "";
            userViewModel.CardOwner = userTo.CardOwner ?? "";
            userViewModel.CardExpiry = userTo.Expiry ?? "";
            userViewModel.CardCVV = userTo.Cvv.ToString() ?? "";
        }
        else
        {
            userViewModel.BillingEmail = "";
            userViewModel.PhoneNumber = "";
            userViewModel.Address = "";
            userViewModel.PostCode = "";
            userViewModel.Suburb = "";
            userViewModel.State = "";
            userViewModel.CardNumber = "";
            userViewModel.CardOwner = "";
            userViewModel.CardExpiry = "";
            userViewModel.CardCVV = "";
        }

        return userViewModel;
    }

    [Authorize(Policy = "RequireAdminRole")]
    [HttpPost("[action]")]
    public async Task<IActionResult> RemoveUser(string userName)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
        else
        {
            Console.WriteLine("ERROR: User: " + userName + " not found");
        }

        return RedirectToAction("AdminPage", "Home");
    }

    //UPDATE PASSWORD FOR USER
    [Authorize]
    [HttpGet]
    public IActionResult UpdatePassword()
    {
        var userName = User.Identity.Name;
        var updatePasswordViewModel = new UpdatePasswordViewModel
        {
            UserName = userName,
        };
        return View(updatePasswordViewModel);
    }

    [HttpGet]
    [Route("Account/UpdatePassword/{userName}")]
    public async Task<IActionResult> UpdatePassword(string userName)
    {
        var user = _context.Users.FirstOrDefault(u => u.UserName == userName);
        var updatePasswordViewModel = new UpdatePasswordViewModel
        {
            UserName = user.UserName,
        };
        return View(updatePasswordViewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdatePassword(UpdatePasswordViewModel model)
    {
        if (ModelState.IsValid)
        {
            if (model.Password == model.ConfirmPassword)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == model.UserName);
                if (user != null)
                {
                    var isChanged = await ChangePassword(model.UserName, model.ConfirmPassword);
                    Console.WriteLine("Password Changed: " + isChanged);
                    return RedirectToAction("UserInfo", "Account", new { userName = model.UserName });
                }
            }
            else
            {
                ModelState.AddModelError("ConfirmPassword", "Passwords do not match.");
            }
        }

        Console.WriteLine("invalid!!!");
        return View(model);
    }


    [HttpPost("[action]")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        //Need to make this a transaction
        if (ModelState.IsValid)
        {
            var passwordMatches = model.Password == model.ConfirmPassword;
            var userExists = await _context.Users.AnyAsync(u => u.UserName == model.UserName);
            var emailExists = await _context.Users.AnyAsync(u => u.Email == model.EmailAddress);
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

            var salt = GenerateSalt();
            var newUser = new User
            {
                UserName = model.UserName,
                Email = model.EmailAddress,
                Salt = salt,
                HashPw = HashPassword(model.Password, salt),
                IsAdmin = false,
                IsStaff = false
            };
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
            Console.WriteLine("New User: " + newUser.UserId + " Created");

            var newUserDetails = new To
            {
                UserName = newUser.UserName,
                Email = model.BillingEmail,
                PhoneNumber = model.PhoneNumber,
                StreetAddress = model.Address,
                PostCode = Convert.ToInt32(model.PostCode),
                Suburb = model.Suburb,
                State = model.State,
                CardNumber = model.CardNumber,
                CardOwner = model.CardOwner,
                Expiry = model.CardExpiry,
                Cvv = Convert.ToInt32(model.CardCVV),
            };
            Console.WriteLine(newUserDetails);
            _context.Tos.Add(newUserDetails);
            await _context.SaveChangesAsync();
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                return RedirectToAction("UserInfo", "Account", new { userName = model.UserName });
            }

            await Authenticate(newUser);
            return RedirectToAction("UserInfo", "Account");
        }

        /*
    ModelState.AddModelError("ConfirmPassword", "ModelState Invalid.");
    */
        var errors = ModelState
            .Where(x => x.Value.Errors.Count > 0)
            .Select(x => new { x.Key, x.Value.Errors })
            .ToArray();
        foreach (var item in errors)
        {
            Console.WriteLine(item.ToString());
        }

        return View(model);
    }

    [Authorize]
    [HttpGet("[action]")]
    public IActionResult UserInfo()
    {
        string username = User.Identity.Name;
        var user = GetUserViewModel(username);
        return View(user);
    }

    [HttpGet("Account/UserInfo/{userName}")]
    public IActionResult UserInfo(string userName)
    {
        var user = GetUserViewModel(userName);
        return View(user);
    }

    private string GenerateRandomPassword()
    {
        const int PW_LENGTH = 16;
        StringBuilder stringBuilder = new StringBuilder();
        Random random = new Random();

        for (var i = 0; i < PW_LENGTH; i++)
        {
            var flt = random.NextDouble();
            var shift = Convert.ToInt32(Math.Floor(81 * flt));
            var letter = Convert.ToChar(shift + 41);
            stringBuilder.Append(letter);
        }

        return stringBuilder.ToString();
    }

    private string GenerateSalt()
    {
        //I stole this from https://stackoverflow.com/questions/45220359/encrypting-and-verifying-a-hashed-password-with-salt-using-pbkdf2-encryption

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

    private bool ValidatePassword(string password, string salt, string hashedPassword)
    {
        string hashedPw = HashPassword(password, salt);
        return hashedPw.Equals(hashedPassword);
    }

    private async Task<bool> ChangePassword(string userName, string password)
    {
        Console.WriteLine("Changing Password for: " + userName);
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName || u.Email == userName);
        if (user == null)
        {
            Console.WriteLine("User not found");
            return false;
        }

        var salt = GenerateSalt();
        var hashedPassword = HashPassword(password, salt);
        Console.WriteLine("Updating Password..");
        user.Salt = salt;
        user.HashPw = hashedPassword;
        Console.WriteLine("Saving Changes..");
        await _context.SaveChangesAsync();
        Console.WriteLine("Password Changed");
        return true;
        //Update user account to new password
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

    [HttpGet("[action]")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }
}