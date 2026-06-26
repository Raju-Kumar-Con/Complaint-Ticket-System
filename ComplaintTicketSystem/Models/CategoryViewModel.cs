using System.Collections.Generic;

namespace ComplaintTicketSystem.Models
{
    public class CategoryViewModel
    {
        public ComplaintCategoryModel Category { get; set; } = new ComplaintCategoryModel();

        public List<ComplaintCategoryModel> Categories { get; set; } = new();
    }
}