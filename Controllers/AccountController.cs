using ECommerceWebsite.Context;
using ECommerceWebsite.Models;
using ECommerceWebsite.Models.ViewModals;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ECommerceWebsite.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public AccountController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register([Bind("UserID,UserName,Email,PasswordHash,FirstName,LastName,Address,City,Country,PostalCode,PhoneNumber,UserType,AccountStatus,DateCreated")] User user, IFormFile UserPhoto)
        {
            var isUserExist = _context.Users.FirstOrDefault(u => u.Email == user.Email);
            if (isUserExist != null)
            {
                ModelState.AddModelError(string.Empty, "User with this email already exists.");
                return View();
            }

            if (UserPhoto != null && UserPhoto.Length > 0)
            {
                var webRootPath = Path.Combine(_hostEnvironment.WebRootPath, "images/usrProfileImages");
                if (!Directory.Exists(webRootPath))
                {
                    Directory.CreateDirectory(webRootPath);
                }
                var picName = Guid.NewGuid().ToString();
                var fullPath = Path.Combine(webRootPath, picName + Path.GetExtension(UserPhoto.FileName));
                using (var fileStream = new FileStream(fullPath, FileMode.Create))
                {
                    await UserPhoto.CopyToAsync(fileStream);
                }
                user.UserPhoto = Path.GetFileName(fullPath);
            }

            byte[] salt;
            user.PasswordHash = HashPassword(user.PasswordHash, out salt);

            try
            {
                user.Salt = Convert.ToBase64String(salt); // Store the salt
                //user.UserType = Constants.Roles.Admin;//adding a user as admin
                _context.Add(user);
                await _context.SaveChangesAsync();
                var userRole = user.UserType.ToString();

                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
            new Claim(ClaimTypes.Role, userRole),
               new Claim(CustomClaimTypes.UserImage, user.UserPhoto),

        };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true // Set to true if you want the cookie to persist across browser sessions
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                if (user.UserType == Constants.Roles.Admin) return RedirectToAction("Index", "Users");
                else return RedirectToAction("Index", "Home");
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while saving the user. Please try again.");
                return View();
            }
        }


        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError(string.Empty, "All fields must be filled.");
                return View();
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            if (user != null && VerifyPassword(user.PasswordHash, password, Convert.FromBase64String(user.Salt)))
            {
                if (user.AccountStatus == Constants.AccountStatus.Inactive)
                {
                    ModelState.AddModelError(string.Empty, "no such an account !");
                    return View();
                }
                var userRole = user.UserType.ToString();

                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
            new Claim(ClaimTypes.Role, userRole),
               new Claim(CustomClaimTypes.UserImage, user.UserPhoto),
        };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true // Set to true if you want the cookie to persist across browser sessions
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                if (user.UserType == Constants.Roles.Admin) return RedirectToAction("Index", "Users");
                else return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View();
        }


        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult NotAuth()
        {
            return View();
        }

        public static string HashPassword(string password, out byte[] salt)
        {
            using (var hmac = new HMACSHA512())
            {
                salt = hmac.Key; // The salt is the key for HMACSHA512
                var passwordBytes = Encoding.UTF8.GetBytes(password);
                var hashBytes = hmac.ComputeHash(passwordBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }


        public static bool VerifyPassword(string storedHash, string providedPassword, byte[] salt)
        {
            using (var hmac = new HMACSHA512(salt))
            {
                var passwordBytes = Encoding.UTF8.GetBytes(providedPassword);
                var hashBytes = hmac.ComputeHash(passwordBytes);
                var providedPasswordHash = Convert.ToBase64String(hashBytes);
                return storedHash == providedPasswordHash;
            }
        }

    }
}
