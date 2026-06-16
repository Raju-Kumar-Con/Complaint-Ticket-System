using ComplaintTicketSystem.Data;
using Microsoft.Data.SqlClient;

namespace ComplaintTicketSystem.Repositories
{
    public class ErrorRepository
    {
        private readonly DBHelper _db;

        public ErrorRepository
        (DBHelper db)
        {
            _db = db;
        }

        public void LogError
        (string message)
        {
            using (SqlConnection con =
            _db.GetConnection())
            {
                string query = @"
                INSERT INTO ErrorLog
                (
                    ErrorMessage
                )
                VALUES
                (
                    @Message
                )";

                SqlCommand cmd =
                new SqlCommand(query, con);

                cmd.Parameters.AddWithValue
                ("@Message", message);

                con.Open();

                cmd.ExecuteNonQuery();
            }
        }
    }
}