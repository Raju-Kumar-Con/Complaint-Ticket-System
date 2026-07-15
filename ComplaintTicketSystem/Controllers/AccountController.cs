using ComplaintTicketSystem.Models;
using ComplaintTicketSystem.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ComplaintTicketSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserRepository _userRepo;
        public AccountController(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        // REGISTER - GET
        [HttpGet]
        public IActionResult Register(bool isAdmin = false)
        {
            try
            {
                ViewBag.IsAdmin = isAdmin;
                return View();
            }
            catch (Exception)
            {
                ViewBag.Error = "Unable to load registration page.";
                return View();
            }
        }

        // REGISTER - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterModel model,string[] SelectedHobbies,bool? isAdmin)
        {
            try
            {
                model.Normalize();
                model.Hobbies = string.Join(",", SelectedHobbies ?? Array.Empty<string>());
                if (!ModelState.IsValid)
                    return View(model);
                string? fileName = null;
                if (model.ProfileImage != null)
                {
                    var extension = Path.GetExtension(model.ProfileImage.FileName).ToLower();
                    string[] allowed ={".jpg",".jpeg",".png"};
                    if (!allowed.Contains(extension))
                    {
                        ModelState.AddModelError("ProfileImage", "Only JPG, JPEG and PNG files allowed.");
                        return View(model);
                    }
                    string uploadFolder = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot/uploads/profile");
                    if (!Directory.Exists(uploadFolder))
                    {
                        Directory.CreateDirectory(uploadFolder);
                    }
                    fileName = Guid.NewGuid().ToString() + extension;
                    string filePath = Path.Combine(uploadFolder, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        model.ProfileImage.CopyTo(stream);
                    }
                }
                int result = _userRepo.Register(model, fileName);

                switch (result)
                {
                    case 1:
                        if (isAdmin == true)
                        {
                            return RedirectToAction("Users", "Admin");
                        }

                        return RedirectToAction("Login");

                    case 0:
                        ModelState.AddModelError("Email", "Email already exists.");
                        break;

                    case -2:
                        ModelState.AddModelError("DOB", "User must be at least 18 years old.");
                        break;

                    default:
                        ModelState.AddModelError("", "Registration failed.");
                        break;
                }

                return View(model);
            }
            catch
            {
                ModelState.AddModelError("","Something went wrong during registration.");
                return View(model);
            }
        }

        // LOGIN - GET
        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                return RedirectToAction("Dashboard", "Complaint");
            }

            return View();
        }

        // LOGIN - POST
        [HttpPost]
        public IActionResult Login(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = _userRepo.GetUserByEmail(model.Email!);
            if (user == null)
            {
                ModelState.AddModelError("Email", "Email does not exist");
                return View(model);
            }
            var passwordHasher = new PasswordHasher<UserModel>();
            var result = passwordHasher.VerifyHashedPassword(user,user.Password!,model.Password!);
            if (result == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError("Password", "Incorrect password");
                return View(model);
            }
            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("UserName", user.UserName ?? "");
            HttpContext.Session.SetString("Role", user.Role ?? "");
            HttpContext.Session.SetString("ProfileImage",user.ProfileImage ?? "");

            return RedirectToAction("Dashboard", "Complaint");
        }

        // ACCESS DENIED
        public IActionResult AccessDenied()
        {
            try
            {
                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("Login");
            }
        }

        // LOGOUT
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            Response.Cookies.Delete(".AspNetCore.Session");

            return RedirectToAction("Login", "Account");
        }
    }
}