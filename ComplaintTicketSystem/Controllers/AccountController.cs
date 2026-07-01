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
        public IActionResult Register()
        {
            try
            {
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
        public IActionResult Register(RegisterModel model)
        {
            try
            {
                model.Normalize();
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                if (_userRepo.IsEmailExists(model.Email!))
                {
                    ModelState.AddModelError("Email", "Email already exists");
                    return View(model);
                }
                if (model.Password != model.ConfirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword","Password and Confirm Password do not match");
                    return View(model);
                }
                bool result = _userRepo.Register(model);
                if (result)
                {
                    TempData["Success"] = "Registration Successful. Please Login.";
                    return RedirectToAction("Login");
                }
                ModelState.AddModelError("", "Registration Failed");
                return View(model);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Something went wrong during registration.");
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

            var result = passwordHasher.VerifyHashedPassword(
                user,
                user.Password!,
                model.Password!
            );

            if (result == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError("Password", "Incorrect password");
                return View(model);
            }

            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("UserName", user.UserName ?? "");
            HttpContext.Session.SetString("Role", user.Role ?? "");

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