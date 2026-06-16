using Microsoft.Data.SqlClient;

namespace ComplaintTicketSystem.Data
{
    public class DBHelper
    {
        private readonly IConfiguration _configuration;
        public DBHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public SqlConnection GetConnection()
        {
            return new SqlConnection( _configuration.GetConnectionString("DefaultConnection"));
        }
    }
}