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
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        [RegularExpression(@"^[a-z0-9]+([._%+-]?[a-z0-9]+)*@[a-z0-9-]+(\.[a-z0-9-]+)*\.[a-z]{2,}$",
            ErrorMessage = "Email must be in lowercase and valid format (e.g. raju@gmail.com)")]
        public string? Email { get; set; }

        // Password is optional while editing user
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
        [StringLength(250, MinimumLength = 10,
            ErrorMessage = "Address must be between 10 and 250 characters")]
        [RegularExpression(
            @"^[A-Za-z0-9\s,./#():-]+$",
            ErrorMessage = "Address contains invalid characters")]
        public string? Address { get; set; }

        public string? Hobbies { get; set; }

        public IFormFile? ImageFile { get; set; }

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
            // DOB Validation
            if (DOB.HasValue)
            {
                if (DOB > DateTime.Today)
                {
                    yield return new ValidationResult(
                        "DOB cannot be a future date.",
                        new[] { nameof(DOB) });
                }

                int age = DateTime.Today.Year - DOB.Value.Year;

                if (DOB.Value.Date > DateTime.Today.AddYears(-age))
                    age--;

                if (age < 18)
                {
                    yield return new ValidationResult(
                        "User must be at least 18 years old.",
                        new[] { nameof(DOB) });
                }
            }

            // Name Validation
            if (string.IsNullOrWhiteSpace(UserName))
            {
                yield return new ValidationResult(
                    "Name cannot be empty or contain only spaces.",
                    new[] { nameof(UserName) });
            }

            // Address Validation
            if (string.IsNullOrWhiteSpace(Address))
            {
                yield return new ValidationResult(
                    "Address cannot be empty or contain only spaces.",
                    new[] { nameof(Address) });
            }

            // Email Validation
            if (!string.IsNullOrWhiteSpace(Email))
            {
                Email = Email.Trim().ToLower();
            }

            // Trim Values
            if (!string.IsNullOrWhiteSpace(UserName))
                UserName = UserName.Trim();

            if (!string.IsNullOrWhiteSpace(Address))
                Address = Address.Trim();

            if (!string.IsNullOrWhiteSpace(MobileNo))
                MobileNo = MobileNo.Trim();

            // Image Validation
            if (ImageFile != null)
            {
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png" };
                string extension = Path.GetExtension(ImageFile.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    yield return new ValidationResult(
                        "Only JPG, JPEG and PNG images are allowed.",
                        new[] { nameof(ImageFile) });
                }

                if (ImageFile.Length > 2 * 1024 * 1024)
                {
                    yield return new ValidationResult(
                        "Image size must not exceed 2 MB.",
                        new[] { nameof(ImageFile) });
                }
            }
        }
    }
}