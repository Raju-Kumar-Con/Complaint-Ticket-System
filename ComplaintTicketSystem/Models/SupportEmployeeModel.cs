using System.ComponentModel.DataAnnotations;

namespace ComplaintTicketSystem.Models
{
    public class SupportEmployeeModel
    {
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be 3-50 characters")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Password must be 6-20 characters")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,20}$",
            ErrorMessage = "Password must contain at least 1 uppercase, 1 lowercase, 1 number, and 1 special character"
        )]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and Confirm Password do not match")]
        public string? ConfirmPassword { get; set; }

        
        [Required(ErrorMessage = "Role is required")]
        [RegularExpression("^(Admin|Support)$", ErrorMessage = "Role must be Admin, Support")]
        public string? Role { get; set; }
        public void Normalize()
        {
            UserName = UserName?.Trim();
            Email = Email?.Trim().ToLower();
            Role = Role?.Trim();
        }
    }
}