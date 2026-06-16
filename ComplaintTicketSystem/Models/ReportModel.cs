namespace ComplaintTicketSystem.Models
{
    public class ReportModel
    {
        public string CategoryName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int ComplaintCount { get; set; }
    }
}