using ComplaintTicketSystem.Models;
using ComplaintTicketSystem.Repositories;
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
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                if (model.Password != model.ConfirmPassword)
                {
                    ViewBag.Error = "Password and Confirm Password do not match";
                    return View(model);
                }

                bool result = _userRepo.Register(model);

                if (result)
                {
                    TempData["Success"] = "Registration Successful. Please Login.";
                    return RedirectToAction("Login");
                }

                ViewBag.Error = "Registration Failed";
                return View(model);
            }
            catch (Exception)
            {
                ViewBag.Error = "Something went wrong during registration.";
                return View(model);
            }
        }

        // LOGIN - GET
        [HttpGet]
        public IActionResult Login()
        {
            try
            {
                return View();
            }
            catch (Exception)
            {
                ViewBag.Error = "Unable to load login page.";
                return View();
            }
        }

        // LOGIN - POST
        [HttpPost]
        public IActionResult Login(LoginModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var user = _userRepo.Login(model);

                if (user != null)
                {
                    // SESSION SET
                    HttpContext.Session.SetInt32("UserId", user.UserId);
                    HttpContext.Session.SetString("UserName", user.UserName ?? "");
                    HttpContext.Session.SetString("Role", user.Role ?? "");

                    // SESSION TEST (DEBUG)
                    var testUser = HttpContext.Session.GetString("UserName");

                    System.Diagnostics.Debug.WriteLine("Session User = " + testUser);
                    Console.WriteLine("Session User = " + testUser);

                    return RedirectToAction("Dashboard", "Complaint");
                }

                ViewBag.Error = "Invalid Email or Password";
                return View(model);
            }
            catch (Exception)
            {
                ViewBag.Error = "Unable to login. Please try again.";
                return View(model);
            }
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
            try
            {
                HttpContext.Session.Clear();

                TempData["Success"] = "Logged out successfully.";

                return RedirectToAction("Login", "Account");
            }
            catch (Exception)
            {
                return RedirectToAction("Login", "Account");
            }
        }
    }
}