namespace ComplaintTicketSystem.Models
{
    public class DashboardModel
    {
        public List<ComplaintModel> Complaints{ get; set; } = new();
        public int TotalComplaints{ get; set; }
        public int OpenCount{ get; set; }
        public int AssignedCount{ get; set; }
        public int ResolvedCount{ get; set; }
        public int RejectedCount{ get; set; }
    }
}