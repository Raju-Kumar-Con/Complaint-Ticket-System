using System.ComponentModel.DataAnnotations;

namespace ComplaintTicketSystem.Models
{
    public class RegisterModel : IValidatableObject
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Name must be 3-50 characters")]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Name can contain only letters and spaces")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        [RegularExpression(
            @"^[a-z0-9]+([._%+-]?[a-z0-9]+)*@[a-z0-9-]+(\.[a-z0-9-]+)*\.[a-z]{2,}$",
            ErrorMessage = "Email must be in lowercase and valid format (e.g. raju@gmail.com)"
        )]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [RegularExpression(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,20}$",
            ErrorMessage = "Password must be 8-20 characters with uppercase, lowercase, number, and special character"
        )]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm Password is required")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and Confirm Password do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public string Role { get; set; } = "User";

        [Required(ErrorMessage ="Image Must be added")]
        public IFormFile? ProfileImage { get; set; }

        [Required(ErrorMessage = "Date of Birth is required")]
        [DataType(DataType.Date)]
        public DateTime? DOB { get; set; }

        public int Age
        {
            get
            {
                if (!DOB.HasValue)
                    return 0;

                int age = DateTime.Today.Year - DOB.Value.Year;

                if (DOB.Value.Date > DateTime.Today.AddYears(-age))
                    age--;

                return age;
            }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (DOB.HasValue)
            {
                int age = DateTime.Today.Year - DOB.Value.Year;

                if (DOB.Value.Date > DateTime.Today.AddYears(-age))
                    age--;

                if (age < 18)
                {
                    yield return new ValidationResult("User must be at least 18 years old.", new[] { nameof(DOB) });
                }
            }
        }
        public void Normalize()
        {
            Email = Email.Trim().ToLower();
        }
    }
}