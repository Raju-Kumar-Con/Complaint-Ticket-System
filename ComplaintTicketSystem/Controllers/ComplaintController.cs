using ComplaintTicketSystem.Models;
using ComplaintTicketSystem.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ComplaintTicketSystem.Filters;

namespace ComplaintTicketSystem.Controllers
{
    [RoleAuthorize("Admin", "Support", "User")]
    [ResponseCache(Duration = 0,Location = ResponseCacheLocation.None,NoStore = true)]
    public class ComplaintController : Controller
    {
        private readonly IComplaintRepository _complaintRepo;
        private readonly ICategoryRepository _categoryRepo;
        private readonly IUserRepository _userRepo;
        private readonly ErrorRepository _errorRepo;

        public ComplaintController(
            IComplaintRepository complaintRepo,
            ICategoryRepository categoryRepo,
            IUserRepository userRepo,
            ErrorRepository errorRepo)
        {
            _complaintRepo = complaintRepo;
            _categoryRepo = categoryRepo;
            _userRepo = userRepo;
            _errorRepo = errorRepo;
        }

        // DASHBOARD
        public IActionResult Dashboard()
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                var role = HttpContext.Session.GetString("Role") ?? "";

                if (userId == null || string.IsNullOrEmpty(role))
                {
                    return RedirectToAction("Login", "Account");
                }

                DashboardModel model =_complaintRepo.GetDashboardData(userId.Value, role);

                model.Complaints =_complaintRepo.GetComplaints(userId.Value, role);

                ViewBag.Role = role;

                return View(model);
            }
            catch
            {
                return RedirectToAction("Index", "Error");
            }
        }

        // CHART DATA
        [HttpGet]
        public JsonResult GetComplaintChartData(string filterType = "user")
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            string role = HttpContext.Session.GetString("Role") ?? "";

            var data = _complaintRepo.GetComplaintChartData(userId, role, filterType);

            return Json(data);
        }

        // INDEX
        [HttpGet]
        public IActionResult Index(string status = "", string category = "", string userName = "")
        {
            ViewBag.StatusFilter = status;
            ViewBag.CategoryFilter = category;
            ViewBag.UserFilter = userName;

            return View();
        }

        // GET COMPLAINTS (JSON)
        [HttpGet]
        public IActionResult GetComplaints()
        {
            try
            {
                int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
                string role = HttpContext.Session.GetString("Role") ?? "";

                var complaints = _complaintRepo.GetComplaints(userId, role);

                return Json(new { success = true, role = role, data = complaints });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message, data = new List<ComplaintModel>() });
            }
        }

        // CREATE - GET
        [HttpGet]
        public IActionResult Raise()
        {
            try
            {
                LoadCategories();

                return View(new ComplaintModel
                {
                    CreatedDate = DateTime.Now
                });
            }
            catch
            {
                return RedirectToAction("Error", "Error");
            }
        }

        // CREATE - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Raise(ComplaintModel model)
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

                TempData["Error"] = "Unable to create complaint.";
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
            catch
            {
                TempData["Error"] = "Unable to load complaint.";
                return RedirectToAction("Index");
            }
        }

        // EDIT - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
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
            catch
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
            catch
            {
                TempData["Error"] = "Unable to load complaint details.";
                return RedirectToAction("Index");
            }
        }

        // DELETE - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            try
            {
                var complaint = _complaintRepo.GetById(id);

                if (complaint == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Complaint not found"
                    });
                }

                if (complaint.Status != "Open")
                {
                    return Json(new
                    {
                        success = false,
                        message = "Only Open complaints can be deleted"
                    });
                }

                int deletedBy =
                    HttpContext.Session.GetInt32("UserId") ?? 0;

                _complaintRepo.DeleteComplaint(id, deletedBy);

                return Json(new
                {
                    success = true,
                    message = "Complaint deleted successfully"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        // SEARCH
        public IActionResult Search(string? subject, string? status, int? categoryId)
        {
            try
            {
                var result = _complaintRepo.SearchComplaints(subject, status, categoryId);

                return PartialView("_ComplaintList", result);
            }
            catch
            {
                return PartialView("_ComplaintList", new List<ComplaintModel>());
            }
        }

        // GET ALL COMPLAINTS
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