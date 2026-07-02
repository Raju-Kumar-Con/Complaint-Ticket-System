using ComplaintTicketSystem.Models;
using ComplaintTicketSystem.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ComplaintTicketSystem.Filters;
using ComplaintTicketSystem.Services;

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
        private readonly IEmailService _emailService;

        public ComplaintController(
            IComplaintRepository complaintRepo,
            ICategoryRepository categoryRepo,
            IUserRepository userRepo,
            ErrorRepository errorRepo,
            IEmailService emailService)
        {
            _complaintRepo = complaintRepo;
            _categoryRepo = categoryRepo;
            _userRepo = userRepo;
            _errorRepo = errorRepo;
            _emailService = emailService;
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
        public async Task<IActionResult> Raise(ComplaintModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.UserId = HttpContext.Session.GetInt32("UserId") ?? 0;
                    model.CreatedDate = DateTime.Now;
                    model.Status = "Open";

                    _complaintRepo.InsertComplaint(model);

                    // Get user details
                    var user = _userRepo.GetUserById(model.UserId);

                    if (user != null && !string.IsNullOrEmpty(user.Email))
                    {
                        string body = $@"
                            <!DOCTYPE html>
                            <html>
                            <head>
                            <meta charset='utf-8'>
                            </head>
                            <body style='background-color:#f8f9fa;padding:20px;font-family:Arial,sans-serif;'>

                            <div style='max-width:700px;margin:auto;background:white;
                            border-radius:10px;box-shadow:0 2px 10px rgba(0,0,0,.1);
                            overflow:hidden;'>

                                <div style='background:#0d6efd;color:white;
                                text-align:center;padding:20px;'>
                                    <h2>Complaint Registered Successfully</h2>
                                </div>

                                <div style='padding:25px;'>

                                    <p class='mb-3'>
                                        Hello <strong>{user.UserName}</strong>,
                                    </p>

                                    <p>
                                        Your complaint has been registered successfully.
                                        Our support team will review it shortly.
                                    </p>

                                    <table style='width:100%;
                                    border-collapse:collapse;
                                    margin-top:20px;'>

                                        <tr>
                                            <td style='padding:10px;background:#f8f9fa;
                                            border:1px solid #dee2e6;'>
                                                <strong>Subject</strong>
                                            </td>

                                            <td style='padding:10px;
                                            border:1px solid #dee2e6;'>
                                                {model.Subject}
                                            </td>
                                        </tr>

                                        <tr>
                                            <td style='padding:10px;background:#f8f9fa;
                                            border:1px solid #dee2e6;'>
                                                <strong>Description</strong>
                                            </td>

                                            <td style='padding:10px;
                                            border:1px solid #dee2e6;'>
                                                {model.Description}
                                            </td>
                                        </tr>

                                        <tr>
                                            <td style='padding:10px;background:#f8f9fa;
                                            border:1px solid #dee2e6;'>
                                                <strong>Status</strong>
                                            </td>

                                            <td style='padding:10px;
                                            border:1px solid #dee2e6;
                                            color:#198754;font-weight:bold;'>
                                                Open
                                            </td>
                                        </tr>

                                        <tr>
                                            <td style='padding:10px;background:#f8f9fa;
                                            border:1px solid #dee2e6;'>
                                                <strong>Created Date</strong>
                                            </td>

                                            <td style='padding:10px;
                                            border:1px solid #dee2e6;'>
                                                {DateTime.Now:dd-MMM-yyyy hh:mm tt}
                                            </td>
                                        </tr>

                                    </table>

                                    <div style='margin-top:20px;
                                    padding:15px;
                                    background:#e7f1ff;
                                    border-left:5px solid #0d6efd;'>

                                        Your complaint has been recorded and will be reviewed by our support team.

                                    </div>

                                    <p style='margin-top:20px;'>
                                        Regards,<br>
                                        <strong>Complaint Ticket System Team</strong>
                                    </p>

                                </div>

                                <div style='background:#f8f9fa;
                                text-align:center;
                                padding:12px;
                                color:#6c757d;
                                font-size:12px;'>

                                    This is an automated email. Please do not reply.

                                </div>

                            </div>

                            </body>
                            </html>";

                        await _emailService.SendEmailAsync(user.Email,"Complaint Registered Successfully",body);
                    }

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