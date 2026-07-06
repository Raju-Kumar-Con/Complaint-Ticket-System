using System.ComponentModel.DataAnnotations;

namespace ComplaintTicketSystem.Models
{
    public class UserModel
    {
        public int UserId { get; set; }

        [Required]
        public string? UserName { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        public string? Password { get; set; }

        public string? Role { get; set; }

        public bool IsActive { get; set; }
        public string? ProfileImage { get; set; }
        public DateTime? DOB { get; set; }
       

    }
}