using ComplaintTicketSystem.Data;
using ComplaintTicketSystem.Models;
using Microsoft.Data.SqlClient;
using System.Collections;

namespace ComplaintTicketSystem.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly DBHelper _db;

        public CategoryRepository(DBHelper db)
        {
            _db = db;
        }

        private static string SafeString(object value)
        {
            return Convert.ToString(value) ?? string.Empty;
        }

        private static int SafeInt(object value)
        {
            return value == DBNull.Value ? 0 : Convert.ToInt32(value);
        }

        // Existing Method (Complaint Dropdown)
        public List<ComplaintCategoryModel> GetCategories()
        {
            List<ComplaintCategoryModel> list = new();

            Hashtable ht = new Hashtable();

            using SqlDataReader dr = _db.GetData("USP_GetCategories", ht);

            while (dr.Read())
            {
                list.Add(new ComplaintCategoryModel
                {
                    CategoryId = SafeInt(dr["CategoryId"]),
                    CategoryName = SafeString(dr["CategoryName"])
                });
            }

            return list;
        }

        // Get All Categories (Admin)
        public List<ComplaintCategoryModel> GetAllCategories()
        {
            List<ComplaintCategoryModel> list = new();

            Hashtable ht = new Hashtable();

            using SqlDataReader dr = _db.GetData("USP_GetAllCategories", ht);

            while (dr.Read())
            {
                list.Add(new ComplaintCategoryModel
                {
                    CategoryId = SafeInt(dr["CategoryId"]),
                    CategoryName = SafeString(dr["CategoryName"]),
                    IsActive = Convert.ToBoolean(dr["IsActive"])
                });
            }

            return list;
        }

        // Get Category By Id
        public ComplaintCategoryModel? GetCategoryById(int id)
        {
            Hashtable ht = new Hashtable();

            ht.Add("@CategoryId", id);

            using SqlDataReader dr = _db.GetData("USP_GetCategoryById", ht);

            if (dr.Read())
            {
                return new ComplaintCategoryModel
                {
                    CategoryId = SafeInt(dr["CategoryId"]),
                    CategoryName = SafeString(dr["CategoryName"]),
                    IsActive = Convert.ToBoolean(dr["IsActive"])
                };
            }

            return null;
        }

        // Insert Category
        public bool InsertCategory(ComplaintCategoryModel model)
        {
            Hashtable ht = new Hashtable();

            ht.Add("@CategoryName", model.CategoryName);

            object? result = _db.ExecuteScalar("USP_InsertCategory", ht);

            return Convert.ToInt32(result) == 1;
        }

        // Update Category
        public bool UpdateCategory(ComplaintCategoryModel model)
        {
            Hashtable ht = new Hashtable();

            ht.Add("@CategoryId", model.CategoryId);
            ht.Add("@CategoryName", model.CategoryName);
            ht.Add("@IsActive", model.IsActive);

            object? result = _db.ExecuteScalar("USP_UpdateCategory", ht);

            return Convert.ToInt32(result) == 1;
        }

        // Delete Category (Soft Delete)
        public bool DeleteCategory(int id)
        {
            Hashtable ht = new Hashtable();

            ht.Add("@CategoryId", id);

            int result = _db.ExecuteQuery("USP_DeleteCategory", ht);

            return result > 0;
        }
    }
}