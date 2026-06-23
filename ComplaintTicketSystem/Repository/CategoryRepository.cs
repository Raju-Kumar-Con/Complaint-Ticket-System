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
    }
}