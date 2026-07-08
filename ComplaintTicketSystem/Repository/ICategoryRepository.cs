using ComplaintTicketSystem.Models;

public interface ICategoryRepository
{
    List<ComplaintCategoryModel> GetCategories();

    List<ComplaintCategoryModel> GetAllCategories();

    ComplaintCategoryModel? GetCategoryById(int id);

    bool InsertCategory(ComplaintCategoryModel model);

    bool UpdateCategory(ComplaintCategoryModel model);

    bool ToggleCategoryStatus(int id);
}