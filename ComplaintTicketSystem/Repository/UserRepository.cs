using ComplaintTicketSystem.Data;
using ComplaintTicketSystem.Models;
using Microsoft.Data.SqlClient;
using System.Collections;

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
            Hashtable ht = new Hashtable();

            ht.Add("@UserName", model.Name);
            ht.Add("@Email", model.Email);
            ht.Add("@Password", model.Password);
            ht.Add("@Role", model.Role);

            return _db.ExecuteQuery("USP_RegisterUser", ht) > 0;
        }

        public UserModel? Login(LoginModel model)
        {
            UserModel? user = null;

            Hashtable ht = new Hashtable();

            ht.Add("@Email", model.Email);
            ht.Add("@Password", model.Password);

            using SqlDataReader dr =
                _db.GetData("USP_LoginUser", ht);

            if (dr.Read())
            {
                user = new UserModel
                {
                    UserId = Convert.ToInt32(dr["UserId"]),
                    UserName = dr["UserName"].ToString() ?? "",
                    Email = dr["Email"].ToString() ?? "",
                    Role = dr["Role"].ToString() ?? ""
                };
            }

            return user;
        }

        public List<UserModel> GetUsersForAssignment()
        {
            List<UserModel> users = new();

            Hashtable ht = new Hashtable();

            using SqlDataReader dr =
                _db.GetData("USP_GetUsersForAssignment", ht);

            while (dr.Read())
            {
                users.Add(new UserModel
                {
                    UserId = Convert.ToInt32(dr["UserId"]),
                    UserName = dr["UserName"].ToString() ?? "",
                    Role = dr["Role"].ToString() ?? ""
                });
            }

            return users;
        }

        public bool AddSupportEmployee(SupportEmployeeModel model)
        {
            Hashtable ht = new Hashtable();

            ht.Add("@UserName", model.UserName);
            ht.Add("@Email", model.Email);
            ht.Add("@Password", model.Password);
            ht.Add("@Role", model.Role);

            return _db.ExecuteQuery("USP_RegisterUser", ht) > 0;
        }

        public bool ModifyEmployee(SupportEmployeeModel model)
        {
            Hashtable ht = new Hashtable();

            ht.Add("@Email", model.Email);
            ht.Add("@Password", model.Password);
            ht.Add("@Role", model.Role);

            return _db.ExecuteQuery("USP_ModifyEmployee", ht) > 0;
        }
    }
}