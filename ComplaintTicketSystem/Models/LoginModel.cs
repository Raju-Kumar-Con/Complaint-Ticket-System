using System.ComponentModel.DataAnnotations;

namespace ComplaintTicketSystem.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        [RegularExpression(
            @"^[a-z0-9]+([._%+-]?[a-z0-9]+)*@[a-z0-9-]+(\.[a-z0-9-]+)*\.[a-z]{2,}$",
            ErrorMessage = "Email must be in lowercase and valid format (e.g. raju@gmail.com)"
        )]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [RegularExpression(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,20}$",
            ErrorMessage = "Password must be 8-20 characters with uppercase, lowercase, number, and special character"
        )]
        public string? Password { get; set; }
    }
}