using ComplaintTicketSystem.Filters;
using ComplaintTicketSystem.Models;
using ComplaintTicketSystem.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ComplaintTicketSystem.Controllers
{
    [RoleAuthorize("Admin", "Support")]
    public class AdminController : Controller
    {
        private readonly IComplaintRepository _repo;
        private readonly IUserRepository _userRepo;
        private readonly ICategoryRepository _categoryRepo;

        public AdminController(
            IComplaintRepository repo,
            IUserRepository userRepo,
            ICategoryRepository categoryRepo)
        {
            _repo = repo;
            _userRepo = userRepo;
            _categoryRepo = categoryRepo;
        }

        // =========================
        // DASHBOARD
        // =========================

        public IActionResult Dashboard()
        {
            try
            {
                return View();
            }
            catch (Exception)
            {
                TempData["Error"] = "Unable to load dashboard.";
                return RedirectToAction("Index", "Complaint");
            }
        }

        // =========================
        // ASSIGN COMPLAINT - GET
        // =========================

        [HttpGet]
        [Route("Admin/Assign/{id}")]
        public IActionResult Assign(int id)
        {
            try
            {
                var model = new AssignComplaintModel
                {
                    ComplaintId = id
                };

                ViewBag.Users = _userRepo.GetUsersForAssignment();

                return View(model);
            }
            catch (Exception)
            {
                TempData["Error"] = "Unable to load assignment page.";
                return RedirectToAction("Index", "Complaint");
            }
        }

        // =========================
        // ASSIGN COMPLAINT - POST
        // =========================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Assign(AssignComplaintModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.Users = _userRepo.GetUsersForAssignment();
                    return View(model);
                }

                _repo.AssignComplaint(model.ComplaintId, model.AssignedTo);

                TempData["Success"] = "Complaint Assigned Successfully.";

                return RedirectToAction("Index", "Complaint");
            }
            catch (Exception)
            {
                ViewBag.Users = _userRepo.GetUsersForAssignment();

                TempData["Error"] = "Unable to assign complaint.";

                return View(model);
            }
        }

        // =========================
        // UPDATE STATUS - GET
        // =========================

        [HttpGet]
        [Route("Admin/Status/{id}")]
        public IActionResult Status(int id)
        {
            try
            {
                var model = new ComplaintStatusModel
                {
                    ComplaintId = id
                };

                return View(model);
            }
            catch (Exception)
            {
                TempData["Error"] = "Unable to load status page.";
                return RedirectToAction("Index", "Complaint");
            }
        }

        // =========================
        // UPDATE STATUS - POST
        // =========================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Status(ComplaintStatusModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                _repo.UpdateStatus(model.ComplaintId, model.Status);

                TempData["Success"] = "Complaint Status Updated Successfully.";

                return RedirectToAction("Index", "Complaint");
            }
            catch (Exception)
            {
                TempData["Error"] = "Unable to update complaint status.";

                return View(model);
            }
        }

        // =========================
        // REPORTS
        // =========================

        [HttpGet("Admin/Reports")]
        public IActionResult Reports()
        {
            try
            {
                var reportData = _repo.GetComplaintReport();
                return View(reportData);
            }
            catch (Exception)
            {
                TempData["Error"] = "Unable to load reports.";
                return RedirectToAction("Dashboard");
            }
        }

        [HttpGet]
        public IActionResult GetReportsData()
        {
            try
            {
                var reportData = _repo.GetComplaintReport();
                return Json(reportData);
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

        // =========================
        // SUPPORT TEAM - GET
        // =========================

        [HttpGet]
        public IActionResult SupportTeam()
        {
            try
            {
                return View();
            }
            catch (Exception)
            {
                TempData["Error"] = "Unable to load support team page.";
                return RedirectToAction("Dashboard");
            }
        }

        // =========================
        // SUPPORT TEAM - POST
        // =========================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SupportTeam(SupportEmployeeModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool result = _userRepo.AddSupportEmployee(model);

                    if (result)
                    {
                        TempData["Success"] = "Support Employee Added Successfully";
                        return RedirectToAction("Dashboard", "Complaint");
                    }

                    TempData["Error"] = "Unable to add support employee.";
                }

                return View(model);
            }
            catch (Exception)
            {
                TempData["Error"] = "An unexpected error occurred.";
                return View(model);
            }
        }

        // =========================
        // CATEGORY LIST
        // =========================

        [HttpGet]
        // =========================
        // CATEGORY LIST
        // =========================

        // CATEGORY LIST
        [HttpGet]
        public IActionResult Category()
        {
            return View();
        }

        // AG GRID DATA
        [HttpGet]
        public IActionResult GetCategoryData()
        {
            try
            {
                var data = _categoryRepo.GetAllCategories();
                return Json(data);
            }
            catch
            {
                return Json(new List<ComplaintCategoryModel>());
            }
        }
        // =========================
        // ADD CATEGORY - GET
        // =========================

        [HttpGet]
        public IActionResult AddCategory()
        {
            return View(new ComplaintCategoryModel());
        }

        // =========================
        // ADD CATEGORY - POST
        // =========================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddCategory(ComplaintCategoryModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                bool result = _categoryRepo.InsertCategory(model);

                if (result)
                {
                    TempData["Success"] = "Category Added Successfully.";
                    return RedirectToAction(nameof(Category));
                }

                TempData["Error"] = "Unable to add category.";
                return View(model);
            }
            catch
            {
                TempData["Error"] = "Something went wrong.";
                return View(model);
            }
        }

        // =========================
        // EDIT CATEGORY - GET
        // =========================

        [HttpGet]
        public IActionResult EditCategory(int id)
        {
            try
            {
                var category = _categoryRepo.GetCategoryById(id);

                if (category == null)
                {
                    TempData["Error"] = "Category not found.";
                    return RedirectToAction(nameof(Category));
                }

                return View(category);
            }
            catch
            {
                TempData["Error"] = "Unable to load category.";
                return RedirectToAction(nameof(Category));
            }
        }

        // =========================
        // EDIT CATEGORY - POST
        // =========================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditCategory(ComplaintCategoryModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(model);

                bool result = _categoryRepo.UpdateCategory(model);

                if (result)
                    TempData["Success"] = "Category Updated Successfully.";
                else
                    TempData["Error"] = "Unable to update category.";

                return RedirectToAction(nameof(Category));
            }
            catch
            {
                TempData["Error"] = "Something went wrong.";
                return View(model);
            }
        }

        // =========================
        // DELETE CATEGORY
        // =========================

        [HttpGet]
        public IActionResult DeleteCategory(int id)
        {
            try
            {
                bool result = _categoryRepo.DeleteCategory(id);

                if (result)
                    TempData["Success"] = "Category Deleted Successfully.";
                else
                    TempData["Error"] = "Unable to delete category.";
            }
            catch
            {
                TempData["Error"] = "Something went wrong.";
            }

            return RedirectToAction(nameof(Category));
        }

        // SUPPORT TEAM - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ModifyEmployee(SupportEmployeeModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool result = _userRepo.AddSupportEmployee(model);

                    if (result)
                    {
                        TempData["Success"] = "Support Employee Added Successfully";

                        return RedirectToAction("Dashboard", "Complaint");
                    }

                    TempData["Error"] = "Unable to add support employee.";
                }

                return View(model);
            }
            catch (Exception)
            {
                TempData["Error"] = "An unexpected error occurred.";

                return View(model);
            }
        }
    }
}