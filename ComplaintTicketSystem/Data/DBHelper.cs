using Microsoft.Data.SqlClient;
using System.Collections;
using System.Data;

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
            return new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string not found."));
        }

        // SELECT
        public SqlDataReader GetData(string spName, Hashtable ht)
        {
            SqlConnection con = GetConnection();

            SqlCommand cmd = new SqlCommand(spName, con);

            cmd.CommandType = CommandType.StoredProcedure;

            foreach (DictionaryEntry item in ht)
            {
                cmd.Parameters.AddWithValue(
                    item.Key.ToString()!,
                    item.Value ?? DBNull.Value);
            }

            con.Open();

            return cmd.ExecuteReader(CommandBehavior.CloseConnection);
        }

        // INSERT UPDATE DELETE
        public int ExecuteQuery(string spName, Hashtable ht)
        {
            using SqlConnection con = GetConnection();

            using SqlCommand cmd = new SqlCommand(spName, con);

            cmd.CommandType = CommandType.StoredProcedure;

            foreach (DictionaryEntry item in ht)
            {
                cmd.Parameters.AddWithValue(
                    item.Key.ToString()!,
                    item.Value ?? DBNull.Value);
            }

            con.Open();

            return cmd.ExecuteNonQuery();
        }

        // SCALAR
        public object? ExecuteScalar(string spName, Hashtable ht)
        {
            using SqlConnection con = GetConnection();

            using SqlCommand cmd = new SqlCommand(spName, con);

            cmd.CommandType = CommandType.StoredProcedure;

            foreach (DictionaryEntry item in ht)
            {
                cmd.Parameters.AddWithValue(
                    item.Key.ToString()!,
                    item.Value ?? DBNull.Value);
            }

            con.Open();

            return cmd.ExecuteScalar();
        }

        public DataTable GetDataTable(string spName, Hashtable ht)
        {
            DataTable dt = new DataTable();

            using SqlConnection con = GetConnection();
            using SqlCommand cmd = new SqlCommand(spName, con);

            cmd.CommandType = CommandType.StoredProcedure;

            foreach (DictionaryEntry item in ht)
            {
                cmd.Parameters.AddWithValue(
                    item.Key.ToString()!,
                    item.Value ?? DBNull.Value);
            }

            using SqlDataAdapter da = new SqlDataAdapter(cmd);

            da.Fill(dt);

            return dt;
        }


    }
}