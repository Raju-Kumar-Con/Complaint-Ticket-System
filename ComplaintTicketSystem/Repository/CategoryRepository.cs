using ComplaintTicketSystem.Data;
using ComplaintTicketSystem.Models;
using Microsoft.Data.SqlClient;

namespace ComplaintTicketSystem.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly DBHelper _db;

        public CategoryRepository(DBHelper db)
        {
            _db = db;
        }

        public List<ComplaintCategoryModel> GetCategories()
        {
            List<ComplaintCategoryModel> list = new();
            using (SqlConnection con = _db.GetConnection())
            {
                SqlCommand cmd =new SqlCommand( "SELECT * FROM ComplaintCategories WHERE IsActive=1", con);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new ComplaintCategoryModel
                    {
                        CategoryId =
                        Convert.ToInt32(dr["CategoryId"]),

                        CategoryName =
                        dr["CategoryName"].ToString()
                    });
                }
            }

            return list;
        }
    }
}