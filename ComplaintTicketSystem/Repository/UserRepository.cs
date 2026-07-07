using ComplaintTicketSystem.Data;
using ComplaintTicketSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using System.Collections;
using System.Data;

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

        // REGISTER
        public bool Register(RegisterModel model, string? profileImage)
        {
            Hashtable ht = new Hashtable();

            var user = new UserModel();

            string hashedPassword =_passwordHasher.HashPassword(user, model.Password);

            ht.Add("@UserName", model.Name);
            ht.Add("@Email", model.Email);
            ht.Add("@Password", hashedPassword);
            ht.Add("@Role", model.Role);
            ht.Add("@ProfileImage", profileImage);
            ht.Add("@DOB", model.DOB);
            ht.Add("@MobileNo", model.MobileNo);
            ht.Add("@Gender", model.Gender);
            ht.Add("@MaritalStatus", model.MaritalStatus);
            ht.Add("@Address", model.Address);
            ht.Add("@Hobbies", model.Hobbies);

            return _db.ExecuteQuery("USP_RegisterUser", ht) > 0;
        }

        // LOGIN (uses DB + password check)
        public UserModel? Login(LoginModel model)
        {
            Hashtable ht = new Hashtable();
            ht.Add("@Email", model.Email);

            using SqlDataReader dr = _db.GetData("USP_GetUserByEmail", ht);

            if (dr.Read())
            {
                var user = new UserModel
                {
                    UserId = Convert.ToInt32(dr["UserId"]),
                    UserName = dr["UserName"].ToString(),
                    Email = dr["Email"].ToString(),
                    Password = dr["Password"].ToString(),
                    Role = dr["Role"].ToString(),
                    ProfileImage = dr["ProfileImage"]?.ToString(),
                    MobileNo = dr["MobileNo"]?.ToString(),
                    Gender = dr["Gender"]?.ToString(),
                    MaritalStatus = dr["MaritalStatus"]?.ToString(),
                    Address = dr["Address"]?.ToString(),
                    Hobbies = dr["Hobbies"]?.ToString()
                };

                var result = _passwordHasher.VerifyHashedPassword(
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

        // GET USER BY EMAIL
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
                    Role = dr["Role"].ToString(),
                    ProfileImage = dr["ProfileImage"]?.ToString(),
                    DOB = dr["DOB"] == DBNull.Value
                        ? null
                        : Convert.ToDateTime(dr["DOB"]),
                        MobileNo = dr["MobileNo"]?.ToString(),
                    Gender = dr["Gender"]?.ToString(),
                    MaritalStatus = dr["MaritalStatus"]?.ToString(),
                    Address = dr["Address"]?.ToString(),
                    Hobbies = dr["Hobbies"]?.ToString()
                };
            }

            return null;
        }

        // CHECK EMAIL EXISTS
        public bool IsEmailExists(string email)
        {
            Hashtable ht = new Hashtable();
            ht.Add("@Email", email);

            using SqlDataReader dr = _db.GetData("USP_GetUserByEmail", ht);

            return dr.Read();
        }

        // GET USERS FOR ASSIGNMENT
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

        // ADD SUPPORT EMPLOYEE
        public bool AddSupportEmployee(SupportEmployeeModel model)
        {
            Hashtable ht = new Hashtable();

            var user = new UserModel();
            string hashedPassword =
                _passwordHasher.HashPassword(user, model.Password!);

            ht.Add("@UserName", model.UserName);
            ht.Add("@Email", model.Email);
            ht.Add("@Password", hashedPassword);
            ht.Add("@Role", model.Role);

            return _db.ExecuteQuery("USP_RegisterUser", ht) > 0;
        }

        // MODIFY EMPLOYEE
        public bool ModifyEmployee(SupportEmployeeModel model)
        {
            Hashtable ht = new Hashtable();

            var user = new UserModel();
            string hashedPassword =
                _passwordHasher.HashPassword(user, model.Password!);

            ht.Add("@Email", model.Email);
            ht.Add("@Password", hashedPassword);
            ht.Add("@Role", model.Role);

            return _db.ExecuteQuery("USP_ModifyEmployee", ht) > 0;
        }

        // GET USER BY ID
        public UserModel? GetUserById(int userId)
        {
            Hashtable ht = new();
            ht.Add("@UserId", userId);

            using SqlDataReader dr =
                _db.GetData("USP_GetUserById", ht);

            if (dr.Read())
            {
                return new UserModel
                {
                    UserId = Convert.ToInt32(dr["UserId"]),
                    UserName = dr["UserName"]?.ToString(),
                    Email = dr["Email"]?.ToString(),
                    Role = dr["Role"]?.ToString(),
                    ProfileImage = dr["ProfileImage"]?.ToString(),
                    DOB = dr["DOB"] == DBNull.Value
                        ? null
                        : Convert.ToDateTime(dr["DOB"]),
                    MobileNo = dr["MobileNo"]?.ToString(),
                    Gender = dr["Gender"]?.ToString(),
                    MaritalStatus = dr["MaritalStatus"]?.ToString(),
                    Address = dr["Address"]?.ToString(),
                    Hobbies = dr["Hobbies"]?.ToString(),
                    IsActive = Convert.ToBoolean(dr["IsActive"])
                };
            }

            return null;
        }
        public DataTable GetAllUsers()
        {
            Hashtable ht = new Hashtable();

            return _db.GetDataTable( "USP_GetAllUsers",ht);
        }
        public bool UpdateUser(UserModel model)
        {
            Hashtable ht = new();

            ht.Add("@UserId", model.UserId);
            ht.Add("@UserName", model.UserName);
            ht.Add("@MobileNo", model.MobileNo);
            ht.Add("@Gender", model.Gender);
            ht.Add("@MaritalStatus", model.MaritalStatus);
            ht.Add("@Address", model.Address);
            ht.Add("@Hobbies", model.Hobbies);
            ht.Add("@DOB", model.DOB ?? (object)DBNull.Value);
            ht.Add("@ProfileImage",string.IsNullOrEmpty(model.ProfileImage)
                ? DBNull.Value : model.ProfileImage);

            return _db.ExecuteQuery("USP_UpdateUser", ht) > 0;
        }
        public bool ToggleUserStatus(int userId)
        {
            Hashtable ht = new();

            ht.Add("@UserId", userId);

            return _db.ExecuteQuery("USP_ToggleUserStatus", ht) > 0;
        }
    }
}