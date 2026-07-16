using ComplaintTicketSystem.Models;
using System.Data;
namespace ComplaintTicketSystem.Repositories
{
    public interface IUserRepository
    {
        int Register(RegisterModel model, string? profileImage);

        UserModel? Login(LoginModel model);
        UserModel? GetUserByEmail(string email);
        bool IsEmailExists(string email);
        List<UserModel> GetUsersForAssignment();
        bool AddSupportEmployee(SupportEmployeeModel model);
        bool ModifyEmployee(SupportEmployeeModel model);
        UserModel? GetUserById(int userId);
        DataTable GetAllUsers();

        bool UpdateUser(UserModel model);

        bool ToggleUserStatus(int userId);

    }
}