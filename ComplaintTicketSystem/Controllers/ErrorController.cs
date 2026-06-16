using Microsoft.AspNetCore.Mvc;

namespace ComplaintTicketSystem.Controllers
{
    public class ErrorController : Controller
    {
        // Default Error Page
        public IActionResult Index()
        {
            return View();
        }

        // Optional: custom error action (agar future me use karna ho)
        public IActionResult Error()
        {
            return View("Index");
        }
    }
}