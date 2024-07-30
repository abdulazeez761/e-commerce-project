using ECommerceWebsite.Context;
using ECommerceWebsite.Models;
using ECommerceWebsite.Models.ViewModals;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ECommerceWebsite.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public UsersController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;

        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.UserID == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserID,UserName,Email,PasswordHash,FirstName,LastName,Address,City,Country,PostalCode,PhoneNumber,UserType,AccountStatus,DateCreated")] User user, IFormFile UserPhoto)
        {
            var webRootPath = Path.Combine(_hostEnvironment.WebRootPath, "images/usrProfileImages");
            if (!Directory.Exists(webRootPath))
            {
                Directory.CreateDirectory(webRootPath);
            }
            Guid picName = Guid.NewGuid();
            string fullPath = Path.Combine(webRootPath, picName + Path.GetExtension(UserPhoto.FileName));
            using (var fileStream = new FileStream(fullPath, FileMode.Create))
            {
                UserPhoto.CopyTo(fileStream);
            }
            user.UserPhoto = Path.GetFileName(fullPath);
            try
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) { }

            return View(user);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserID,UserName,Email,PasswordHash,FirstName,LastName,Address,City,Country,PostalCode,PhoneNumber,UserType,AccountStatus,DateCreated")] User user)
        {
            if (id != user.UserID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingUser = await _context.Users.FindAsync(user.UserID);
                    if (existingUser == null)
                    {
                        return NotFound();
                    }

                    existingUser.PhoneNumber = user.PhoneNumber;
                    existingUser.FirstName = user.FirstName;
                    existingUser.LastName = user.LastName;
                    existingUser.Email = user.Email;
                    existingUser.UserName = user.UserName;
                    if (user.UserPhoto != null) existingUser.UserPhoto = user.UserPhoto;

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        public async Task<IActionResult> EditUser(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(int id, [Bind("UserID,UserName,Email,PasswordHash,FirstName,LastName,Address,City,Country,PostalCode,PhoneNumber,UserType,AccountStatus,DateCreated")] User user)
        {
            if (id != user.UserID)
            {
                return NotFound();
            }

            try
            {
                var existingUser = await _context.Users.FindAsync(user.UserID);
                if (existingUser == null)
                {
                    return NotFound();
                }

                existingUser.PhoneNumber = user.PhoneNumber;
                existingUser.FirstName = user.FirstName;
                existingUser.LastName = user.LastName;
                existingUser.Email = user.Email;
                existingUser.UserName = user.UserName;
                if (user.UserPhoto != null) existingUser.UserPhoto = user.UserPhoto;

                _context.Users.Update(existingUser);
                await _context.SaveChangesAsync();

                var claimsIdentity = User.Identity as ClaimsIdentity;
                if (claimsIdentity != null)
                {
                    var nameClaim = claimsIdentity.FindFirst(ClaimTypes.Name);
                    if (nameClaim != null)
                    {
                        claimsIdentity.RemoveClaim(nameClaim);
                        claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
                    }

                    var updatedUserClaimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, updatedUserClaimsPrincipal);
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(user.UserID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {

                user.AccountStatus = Constants.AccountStatus.Inactive;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Active(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {

                user.AccountStatus = Constants.AccountStatus.Active;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        //should add a logic to delete the prev image
        public async Task<IActionResult> EditImage(IFormFile UserPhoto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "User not found.");
                return Redirect(Request.Headers["Referer"].ToString());
            }

            if (UserPhoto != null && UserPhoto.Length > 0)
            {
                var webRootPath = Path.Combine(_hostEnvironment.WebRootPath, "images/usrProfileImages");

                if (!Directory.Exists(webRootPath))
                {
                    Directory.CreateDirectory(webRootPath);
                }

                var picName = Guid.NewGuid().ToString();
                var fileExtension = Path.GetExtension(UserPhoto.FileName);
                var fullPath = Path.Combine(webRootPath, picName + fileExtension);

                using (var fileStream = new FileStream(fullPath, FileMode.Create))
                {
                    await UserPhoto.CopyToAsync(fileStream);
                }


                if (!string.IsNullOrEmpty(user.UserPhoto))
                {
                    var oldFilePath = Path.Combine(webRootPath, user.UserPhoto);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                user.UserPhoto = Path.GetFileName(fullPath);

                try
                {
                    // Update user claims with the new image path
                    var claimsIdentity = User.Identity as ClaimsIdentity;
                    if (claimsIdentity != null)
                    {
                        var imageClaim = claimsIdentity.FindFirst(CustomClaimTypes.UserImage);
                        if (imageClaim != null)
                        {
                            claimsIdentity.RemoveClaim(imageClaim);
                            claimsIdentity.AddClaim(new Claim(CustomClaimTypes.UserImage, user.UserPhoto));
                        }

                        var updatedUserClaimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, updatedUserClaimsPrincipal);
                    }

                    // Save changes to the database
                    await _context.SaveChangesAsync();

                    // Redirect back to the previous page
                    return Redirect(Request.Headers["Referer"].ToString());
                }
                catch (Exception ex)
                {
                    // Log the exception and show an error message
                    ModelState.AddModelError(string.Empty, "An error occurred while saving the user. Please try again.");
                    return Redirect(Request.Headers["Referer"].ToString());
                }
            }

            // Redirect if no photo was uploaded
            return Redirect(Request.Headers["Referer"].ToString());
        }


        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserID == id);
        }
    }
}
