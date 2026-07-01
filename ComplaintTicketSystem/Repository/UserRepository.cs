using ComplaintTicketSystem.Data;
using ComplaintTicketSystem.Models;
using Microsoft.Data.SqlClient;
using System.Collections;
using Microsoft.AspNetCore.Identity;

namespace ComplaintTicketSystem.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DBHelper _db;
        private readonly PasswordHasher<UserModel> _passwordHasher;
        public UserRepository(DBHelper db)
        {
            _db = db;
            _passwordHasher = new PasswordHasher<UserModel>();
        }

        public bool Register(RegisterModel model)
        {
            Hashtable ht = new Hashtable();
            var user = new UserModel();
            string hashedPassword = _passwordHasher.HashPassword(user, model.Password);

            ht.Add("@UserName", model.Name);
            ht.Add("@Email", model.Email);
            ht.Add("@Password", hashedPassword);
            ht.Add("@Role", model.Role);

            return _db.ExecuteQuery("USP_RegisterUser", ht) > 0;
        }

        public UserModel? Login(LoginModel model)
        {
            UserModel? user = null;
            Hashtable ht = new Hashtable();
            ht.Add("@Email", model.Email);
            using SqlDataReader dr = _db.GetData("USP_GetUserByEmail", ht);
            if (dr.Read())
            {
                user = new UserModel
                {
                    UserId = Convert.ToInt32(dr["UserId"]),
                    UserName = dr["UserName"].ToString(),
                    Email = dr["Email"].ToString(),
                    Password = dr["Password"].ToString(),
                    Role = dr["Role"].ToString()
                };

                var result =_passwordHasher.VerifyHashedPassword(
                        user,
                        user.Password!,
                        model.Password!
                    );

                if (result == PasswordVerificationResult.Success)
                {
                    return user;
                }
            }

            return null;
        }
        public UserModel? GetUserByEmail(string email)
        {
            Hashtable ht = new Hashtable();
            ht.Add("@Email", email);

            using SqlDataReader dr = _db.GetData("USP_GetUserByEmail", ht);

            if (dr.Read())
            {
                return new UserModel
                {
                    UserId = Convert.ToInt32(dr["UserId"]),
                    UserName = dr["UserName"].ToString(),
                    Email = dr["Email"].ToString(),
                    Password = dr["Password"].ToString(),
                    Role = dr["Role"].ToString()
                };
            }

            return null;
        }
        public bool IsEmailExists(string email)
        {
            Hashtable ht = new Hashtable();
            ht.Add("@Email", email);

            using SqlDataReader dr = _db.GetData("USP_GetUserByEmail", ht);

            return dr.Read();
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
            var user = new UserModel();
            string hashedPassword = _passwordHasher.HashPassword(user, model.Password!);
            ht.Add("@UserName", model.UserName);
            ht.Add("@Email", model.Email);
            ht.Add("@Password", hashedPassword);
            ht.Add("@Role", model.Role);
            return _db.ExecuteQuery("USP_RegisterUser", ht) > 0;
        }

        public bool ModifyEmployee(SupportEmployeeModel model)
        {
            Hashtable ht = new Hashtable();
            var user = new UserModel();
            string hashedPassword =_passwordHasher.HashPassword(user, model.Password!);
            ht.Add("@Email", model.Email);
            ht.Add("@Password", hashedPassword);
            ht.Add("@Role", model.Role);
            return _db.ExecuteQuery("USP_ModifyEmployee", ht) > 0;
        }
    }
}