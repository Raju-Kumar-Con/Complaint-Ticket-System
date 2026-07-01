using ComplaintTicketSystem.Models;

namespace ComplaintTicketSystem.Repositories
{
    public interface IComplaintRepository
    {
        List<ComplaintModel> GetComplaints(int userId, string role);

        ComplaintModel? GetById(int id);

        List<ComplaintModel> SearchComplaints(string? subject,string? status,int? categoryId);

        void InsertComplaint(ComplaintModel model);

        void UpdateComplaint(ComplaintModel model);

        void DeleteComplaint(int complaintId, int deletedBy);
        void AssignComplaint(int complaintId, int assignedTo);

        void UpdateStatus(int complaintId, string status);

        DashboardModel GetDashboardData(int userId, string role);

        List<ReportModel> GetComplaintReport();

        List<object> GetComplaintChartData(int userId,string role,string filterType);
    }
}