using ComplaintTicketSystem.Models;

namespace ComplaintTicketSystem.Repositories
{
    public interface IUserRepository
    {
        bool Register(RegisterModel model, string? profileImage);

        UserModel? Login(LoginModel model);
        UserModel? GetUserByEmail(string email);
        bool IsEmailExists(string email);
        List<UserModel> GetUsersForAssignment();
        bool AddSupportEmployee(SupportEmployeeModel model);
        bool ModifyEmployee(SupportEmployeeModel model);


        UserModel? GetUserById(int userId);
    }
}