using ComplaintTicketSystem.Data;
using ComplaintTicketSystem.Models;
using Microsoft.Data.SqlClient;
using System.Data;
namespace ComplaintTicketSystem.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DBHelper _db;
        public UserRepository(DBHelper db)
        {
            _db = db;
        }
        public bool Register(RegisterModel model)
        {
            using (SqlConnection con = _db.GetConnection())
            {
                SqlCommand cmd = new SqlCommand("USP_RegisterUser",con);
                cmd.CommandType =CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserName",model.Name);
                cmd.Parameters.AddWithValue("@Email",model.Email);
                cmd.Parameters.AddWithValue("@Password",model.Password);
                cmd.Parameters.AddWithValue( "@Role",model.Role);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
        public UserModel? Login(LoginModel model)
        {
            UserModel? user = null;
            using (SqlConnection con = _db.GetConnection())
            {
                SqlCommand cmd = new SqlCommand( "USP_LoginUser",con);
                cmd.CommandType =CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Email",model.Email);
                cmd.Parameters.AddWithValue("@Password",model.Password);
                con.Open();
                SqlDataReader dr =cmd.ExecuteReader();
                if (dr.Read())
                {
                    user = new UserModel
                    {
                        UserId = Convert.ToInt32(dr["UserId"]),
                        UserName = dr["UserName"].ToString(),
                        Email = dr["Email"].ToString(),
                        Role = dr["Role"].ToString()
                    };
                }
            }
            return user;
        }
        public List<UserModel> GetUsersForAssignment()
        {
            List<UserModel> users = new();
            using (SqlConnection con = _db.GetConnection())
            {
                SqlCommand cmd = new SqlCommand("USP_GetUsersForAssignment", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    users.Add(new UserModel
                    {
                        UserId = Convert.ToInt32(dr["UserId"]),
                        UserName = dr["UserName"].ToString(),
                        Role = dr["Role"].ToString()
                    });
                }
            }
            return users;
        }
        public bool AddSupportEmployee(SupportEmployeeModel model)
        {
            using (SqlConnection con = _db.GetConnection())
            {
                SqlCommand cmd = new SqlCommand("USP_RegisterUser",con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserName", model.UserName);
                cmd.Parameters.AddWithValue("@Email", model.Email);
                cmd.Parameters.AddWithValue("@Password", model.Password);
                cmd.Parameters.AddWithValue("@Role", model.Role);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}