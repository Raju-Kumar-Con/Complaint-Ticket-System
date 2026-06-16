using ComplaintTicketSystem.Models;

namespace ComplaintTicketSystem.Repositories
{
    public interface ICategoryRepository
    {
        List<ComplaintCategoryModel> GetCategories();
    }
}