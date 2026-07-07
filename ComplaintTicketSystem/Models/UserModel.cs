using System.ComponentModel.DataAnnotations;

namespace ComplaintTicketSystem.Models
{
    public class UserModel : IValidatableObject
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(50, MinimumLength = 3,
            ErrorMessage = "Name must be between 3 and 50 characters")]
        [RegularExpression(@"^[A-Za-z\s]+$",
            ErrorMessage = "Name can contain only letters and spaces")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? Email { get; set; }

        public string? Password { get; set; }

        public string? Role { get; set; }

        public bool IsActive { get; set; }

        public string? ProfileImage { get; set; }

        [Required(ErrorMessage = "Date of Birth is required")]
        [DataType(DataType.Date)]
        public DateTime? DOB { get; set; }

        [Required(ErrorMessage = "Mobile Number is required")]
        [RegularExpression(@"^[0-9]{10}$",
            ErrorMessage = "Enter valid 10 digit mobile number")]
        public string? MobileNo { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        public string? Gender { get; set; }

        [Required(ErrorMessage = "Marital Status is required")]
        public string? MaritalStatus { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [StringLength(500,
            ErrorMessage = "Address cannot exceed 500 characters")]
        public string? Address { get; set; }

        public string? Hobbies { get; set; }

        public IFormFile? ImageFile { get; set; }

        public IEnumerable<ValidationResult> Validate(
            ValidationContext validationContext)
        {
            if (DOB.HasValue)
            {
                int age = DateTime.Today.Year - DOB.Value.Year;

                if (DOB.Value.Date > DateTime.Today.AddYears(-age))
                {
                    age--;
                }

                if (age < 18)
                {
                    yield return new ValidationResult(
                        "User must be at least 18 years old.",
                        new[] { nameof(DOB) });
                }

                if (DOB > DateTime.Today)
                {
                    yield return new ValidationResult(
                        "DOB cannot be a future date.",
                        new[] { nameof(DOB) });
                }
            }
        }
    }
}