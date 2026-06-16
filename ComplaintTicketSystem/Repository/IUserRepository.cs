using ComplaintTicketSystem.Models;

namespace ComplaintTicketSystem.Repositories
{
    public interface IUserRepository
    {
        bool Register(RegisterModel model);

        UserModel? Login(LoginModel model);

        List<UserModel> GetUsersForAssignment();
        bool AddSupportEmployee(SupportEmployeeModel model);
    }
}