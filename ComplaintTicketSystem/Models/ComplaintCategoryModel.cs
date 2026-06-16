namespace ComplaintTicketSystem.Models
{
    public class ComplaintCategoryModel
    {
        public int CategoryId { get; set; }

        public string? CategoryName { get; set; }

        public bool IsActive { get; set; }
    }
}