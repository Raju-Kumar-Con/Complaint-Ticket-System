using ComplaintTicketSystem.Data;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ComplaintTicketSystem.Repositories
{
    public class ErrorRepository
    {
        private readonly DBHelper _db;
        public ErrorRepository(DBHelper db)
        {
            _db = db;
        }
        public void LogError(string message)
        {
            using (SqlConnection con = _db.GetConnection())
            {
                SqlCommand cmd = new SqlCommand("USP_LogError", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Message", message);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}