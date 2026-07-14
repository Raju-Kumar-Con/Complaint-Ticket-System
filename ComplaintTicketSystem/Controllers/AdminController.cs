using ComplaintTicketSystem.Filters;
using ComplaintTicketSystem.Models;
using ComplaintTicketSystem.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace ComplaintTicketSystem.Controllers
{
    [RoleAuthorize("Admin", "Support")]
    public class AdminController : Controller
    {
        private readonly IComplaintRepository _repo;
        private readonly IUserRepository _userRepo;
        private readonly ICategoryRepository _categoryRepo;

        public AdminController(IComplaintRepository repo,IUserRepository userRepo,ICategoryRepository categoryRepo)
        {
            _repo = repo;
            _userRepo = userRepo;
            _categoryRepo = categoryRepo;
        }

        // DASHBOARD
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

        // ASSIGN COMPLAINT - GET

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

        // ASSIGN COMPLAINT - POST
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
                bool result = _repo.AssignComplaint(model.ComplaintId, model.AssignedTo);
                if (result)
                {
                    TempData["Success"] = "Complaint Assigned Successfully.";
                }
                else
                {
                    TempData["Error"] = "Unable to assign complaint.";
                }
                return RedirectToAction("Index", "Complaint");
            }
            catch (Exception)
            {
                ViewBag.Users = _userRepo.GetUsersForAssignment();
                TempData["Error"] = "Unable to assign complaint.";

                return View(model);
            }
        }

        // UPDATE STATUS - GET
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

        // UPDATE STATUS - POST
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
        // REPORTS
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
                return Json(new{ success = false,message = ex.Message});
            }
        }
        // SUPPORT TEAM - GET
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
        // SUPPORT TEAM - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SupportTeam(SupportEmployeeModel model)
        {
            try
            {
                if (!ModelState.IsValid) { 
                    model.Normalize();
                    return View(model);
                }
                bool result = _userRepo.AddSupportEmployee(model);
                if (result)
                {
                    TempData["Success"] = "Support Employee Added Successfully";
                }
                else
                {
                    TempData["Error"] = "Employee already exists. Click Modify Employee.";
                }
                return RedirectToAction(nameof(SupportTeam));
            }
            catch
            {
                TempData["Error"] = "Something went wrong.";
                return RedirectToAction(nameof(SupportTeam));
            }
        }
        // SUPPORT TEAM - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ModifyEmployee(SupportEmployeeModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View("SupportTeam", model);

                bool result = _userRepo.ModifyEmployee(model);
                if (result)
                {
                    TempData["Success"] = "Employee Updated Successfully";
                    return RedirectToAction(nameof(SupportTeam));
                }
                TempData["Error"] = "Employee not found.";
                return View("SupportTeam", model);
            }
            catch
            {
                TempData["Error"] = "Something went wrong.";
                return View("SupportTeam", model);
            }
        }

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
        // ADD CATEGORY - GET
        [HttpGet]
        public IActionResult AddCategory()
        {
            return View(new ComplaintCategoryModel());
        }
        // ADD CATEGORY - POST

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
                TempData["Error"] = "Unable to add category  It's already Exists";
                return View(model);
            }
            catch
            {
                TempData["Error"] = "Something went wrong.";
                return View(model);
            }
        }
        // EDIT CATEGORY - GET
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

        // EDIT CATEGORY - POST
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
                {
                    TempData["Success"] = "Category Updated Successfully.";
                    return RedirectToAction(nameof(Category));
                }

                ViewBag.Error = "Category already exists.";
                return View(model);
            }
            catch
            {
                ViewBag.Error = "Something went wrong.";
                return View(model);
            }
        }

        // CHANGE CATEGORY STATUS
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleCategoryStatus(int id)
        {
            try
            {
                bool result = _categoryRepo.ToggleCategoryStatus(id);

                if (result)
                    TempData["Success"] = "Category status updated successfully.";
                else
                    TempData["Error"] = "Unable to update category status.";
            }
            catch
            {
                TempData["Error"] = "Something went wrong.";
            }

            return RedirectToAction(nameof(Category));
        }

        //--------------------------------------------USER SECTION--------------------------------------
        [HttpGet]
        public IActionResult AddUser()
        {
            return RedirectToAction("Register","Account",new { isAdmin = true });
        }

        [HttpGet]
        public IActionResult Users()
        {
            try
            {
                DataTable users = _userRepo.GetAllUsers();
                return View(users);
            }
            catch
            {
                TempData["Error"] = "Unable to load users.";
                return RedirectToAction(nameof(Dashboard));
            }
        }
        [HttpGet]
        public IActionResult EditUser(int id)
        {
            try
            {
                var user = _userRepo.GetUserById(id);
                if (user == null)
                {
                    TempData["Error"] = "User not found.";
                    return RedirectToAction(nameof(Users));
                }

                if (!user.IsActive)
                {
                    TempData["Error"] = "Inactive user cannot be edited.";
                    return RedirectToAction(nameof(Users));
                }
                return View(user);
            }
            catch
            {
                TempData["Error"] = "Unable to load user.";
                return RedirectToAction(nameof(Users));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditUser(UserModel model,string[]? SelectedHobbies)
        {
            try
            {
                var existingUser = _userRepo.GetUserById(model.UserId);
                if (existingUser == null)
                {
                    TempData["Error"] = "User not found.";
                    return RedirectToAction(nameof(Users));
                }
                if (!existingUser.IsActive)
                {
                    TempData["Error"] = "Inactive user cannot be updated.";
                    return RedirectToAction(nameof(Users));
                }
                if (SelectedHobbies != null && SelectedHobbies.Length > 0)
                {
                    model.Hobbies = string.Join(",", SelectedHobbies);
                }
                else
                {
                    ModelState.AddModelError("Hobbies","Please select at least one hobby.");
                }
                model.ProfileImage = existingUser.ProfileImage;
                if (model.DOB.HasValue)
                {
                    int age = DateTime.Today.Year - model.DOB.Value.Year;
                    if (model.DOB.Value.Date > DateTime.Today.AddYears(-age))
                    {
                        age--;
                    }

                    if (age < 18)
                    {
                        ModelState.AddModelError( "DOB", "User must be at least 18 years old.");
                    }
                }
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    string extension =Path.GetExtension(model.ImageFile.FileName).ToLower();
                    string[] allowedExtensions ={".jpg", ".jpeg",".png"};
                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("ImageFile","Only JPG, JPEG and PNG files allowed.");
                        return View(model);
                    }
                    if (model.ImageFile.Length > 2 * 1024 * 1024)
                    {
                        ModelState.AddModelError("ImageFile","Image size cannot exceed 2 MB.");
                        return View(model);
                    }
                    string uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/profile");
                    if (!Directory.Exists(uploadFolder))
                    {
                        Directory.CreateDirectory(uploadFolder);
                    }
                    string newFileName =Guid.NewGuid().ToString() + extension;
                    string filePath =Path.Combine(uploadFolder, newFileName);
                    using (FileStream stream =new FileStream(filePath, FileMode.Create))
                    {
                        model.ImageFile.CopyTo(stream);
                    }
                    if (!string.IsNullOrEmpty(existingUser.ProfileImage))
                    {
                        string oldImagePath =Path.Combine(uploadFolder,existingUser.ProfileImage);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    model.ProfileImage = newFileName;
                }
                bool result = _userRepo.UpdateUser(model);
                if (result)
                {
                    TempData["Success"] ="User updated successfully.";
                    return RedirectToAction(nameof(Users));
                }
                TempData["Error"] ="Unable to update user.";
                return View(model);
            }
            catch (Exception)
            {
                TempData["Error"] ="Something went wrong.";
                return View(model);
            }
        }
        [HttpGet]
        public IActionResult ToggleUserStatus(int id)
        {
            try
            {
                bool result = _userRepo.ToggleUserStatus(id);

                if (result)
                    TempData["Success"] = "User status updated successfully.";
                else
                    TempData["Error"] = "Unable to update user status.";
            }
            catch
            {
                TempData["Error"] = "Something went wrong.";
            }

            return RedirectToAction(nameof(Users));
        }

    }
}