using ComplaintTicketSystem.Models;
using ComplaintTicketSystem.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ComplaintTicketSystem.Controllers
{
    public class ComplaintController : Controller
    {
        private readonly IComplaintRepository _complaintRepo;
        private readonly ICategoryRepository _categoryRepo;
        private readonly IUserRepository _userRepo;
        private readonly ErrorRepository _errorRepo;

        public ComplaintController(IComplaintRepository complaintRepo,ICategoryRepository categoryRepo,IUserRepository userRepo,ErrorRepository errorRepo)
        {
            _complaintRepo = complaintRepo;
            _categoryRepo = categoryRepo;
            _userRepo = userRepo;
            _errorRepo = errorRepo;
        }

        // DASHBOARD
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult Dashboard()
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                var role = HttpContext.Session.GetString("Role");
                if (userId == null || string.IsNullOrEmpty(role))
                {
                    return RedirectToAction("Login", "Account");
                }
                if (role != "Admin" && role != "Support" && role != "User")
                {
                    return RedirectToAction("AccessDenied", "Account");
                }
                DashboardModel model = _complaintRepo.GetDashboardData(userId.Value, role);
                model.Complaints = _complaintRepo.GetComplaints(userId.Value, role);

                ViewBag.Role = role;

                return View(model);
            }
            catch
            {
                return RedirectToAction("Index", "Error");
            }
        }
        [HttpGet]
        public JsonResult GetComplaintChartData(string filterType = "user")
        {
            int userId =HttpContext.Session.GetInt32("UserId") ?? 0;
            string role =HttpContext.Session.GetString("Role") ?? "";
            var data =_complaintRepo.GetComplaintChartData(userId,role,filterType);
            return Json(data);
        }

        [HttpGet]
        public IActionResult Index(string status = "",string category = "",string userName = "")
        {
            ViewBag.StatusFilter = status;
            ViewBag.CategoryFilter = category;
            ViewBag.UserFilter = userName;

            return View();
        }
        [HttpGet]
        public IActionResult GetComplaints()
        {
            try
            {
                int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
                string role = HttpContext.Session.GetString("Role") ?? "";
                var complaints = _complaintRepo.GetComplaints(userId, role);
                return Json(new{success = true,role = role,data = complaints});
            }
            catch (Exception ex)
            {
                return Json(new{success = false,message = ex.Message,data = new List<ComplaintModel>()});
            }
        }
        // CREATE - GET
        [HttpGet]
        public IActionResult Create()
        {
            try
            {
                LoadCategories();

                var model = new ComplaintModel
                {
                    CreatedDate = DateTime.Now
                };

                return View(model);
            }
            catch (Exception)
            {
                return RedirectToAction("Error", "Error");
            }
        }

        // CREATE - POST
        [HttpPost]
        public IActionResult Create(ComplaintModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.UserId = HttpContext.Session.GetInt32("UserId") ?? 0;
                    model.CreatedDate = DateTime.Now;
                    model.Status = "Open";

                    _complaintRepo.InsertComplaint(model);

                    TempData["Success"] = "Complaint created successfully.";

                    return RedirectToAction("Index");
                }

                LoadCategories();

                return View(model);
            }
            catch (Exception ex)
            {
                _errorRepo.LogError(ex.Message);

                TempData["Error"] ="Unable to create complaint.";

                return View(model);
            }
        }

        // EDIT - GET
        [HttpGet]
        public IActionResult Edit(int id)
        {
            try
            {
                LoadCategories();

                var complaint = _complaintRepo.GetById(id);

                if (complaint == null)
                    return NotFound();

                if (complaint.Status != "Open")
                    return Content("Only Open complaints can be edited.");

                return View(complaint);
            }
            catch (Exception)
            {
                TempData["Error"] = "Unable to load complaint.";
                return RedirectToAction("Index");
            }
        }

        // EDIT - POST
        [HttpPost]
        public IActionResult Edit(ComplaintModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _complaintRepo.UpdateComplaint(model);

                    TempData["Success"] = "Complaint updated successfully.";

                    return RedirectToAction("Index");
                }

                LoadCategories();

                return View(model);
            }
            catch (Exception)
            {
                LoadCategories();

                TempData["Error"] = "Unable to update complaint.";

                return View(model);
            }
        }

        // DETAILS
        public IActionResult Details(int id)
        {
            try
            {
                var complaint = _complaintRepo.GetById(id);

                if (complaint == null)
                    return NotFound();

                return View(complaint);
            }
            catch (Exception)
            {
                TempData["Error"] = "Unable to load complaint details.";
                return RedirectToAction("Index");
            }
        }

        // DELETE - POST
        [HttpPost]
        public IActionResult Delete(int id)
        {
            try
            {
                var complaint = _complaintRepo.GetById(id);

                if (complaint == null)
                {
                    return Json(new { success = false, message = "Complaint not found" });
                }

                if (complaint.Status != "Open")
                {
                    return Json(new { success = false, message = "Only Open complaints can be deleted" });
                }

                _complaintRepo.DeleteComplaint(id);

                return Json(new { success = true, message = "Deleted successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Server error: " + ex.Message });
            }
        }

        // SEARCH
        public IActionResult Search(string? subject, string? status, int? categoryId)
        {
            try
            {
                var result = _complaintRepo.SearchComplaints(subject, status, categoryId);

                return PartialView("_ComplaintSearch", result);
            }
            catch (Exception)
            {
                return PartialView("_ComplaintSearch", new List<ComplaintModel>());
            }
        }
        [HttpGet]
        public IActionResult GetAllComplaints(string filterType)
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            string role = HttpContext.Session.GetString("Role") ?? "";

            var data = _complaintRepo.GetComplaints(userId, role);

            return Json(data);
        }
        // LOAD DROPDOWN
        private void LoadCategories()
        {
            ViewBag.CategoryList = new SelectList(
                _categoryRepo.GetCategories(),
                "CategoryId",
                "CategoryName");
        }
    }
}