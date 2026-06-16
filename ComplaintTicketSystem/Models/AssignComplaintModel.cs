using ComplaintTicketSystem.Models;

public class AssignComplaintModel
{
    public int ComplaintId { get; set; }
    public int AssignedTo { get; set; }

    public List<UserModel> Users { get; set; } = new();
}