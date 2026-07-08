using ComplaintTicketSystem.Data;
using ComplaintTicketSystem.Models;
using Microsoft.Data.SqlClient;
using System.Collections;

namespace ComplaintTicketSystem.Repositories
{
    public class ComplaintRepository : IComplaintRepository
    {
        private readonly DBHelper _db;
        public ComplaintRepository(DBHelper db)
        {
            _db = db;
        }
        private static string SafeString(object value)
        {
            return Convert.ToString(value) ?? string.Empty;
        }
        private static int SafeInt(object value)
        {
            return value == DBNull.Value ? 0 : Convert.ToInt32(value);
        }
        private static int? SafeNullableInt(object value)
        {
            return value == DBNull.Value ? null : Convert.ToInt32(value);
        }
        private static DateTime? SafeNullableDate(object value)
        {
            return value == DBNull.Value ? null : Convert.ToDateTime(value);
        }
        // ---------------- GET COMPLAINTS ----------------
        public List<ComplaintModel> GetComplaints(int userId, string role)
        {
            List<ComplaintModel> list = new();

            Hashtable ht = new Hashtable();
            ht.Add("@UserId", userId);
            ht.Add("@Role", role);

            using SqlDataReader dr = _db.GetData("USP_GetComplaints", ht);

            while (dr.Read())
            {
                list.Add(new ComplaintModel
                {
                    ComplaintId = SafeInt(dr["ComplaintId"]),
                    UserName = SafeString(dr["UserName"]),
                    CategoryName = SafeString(dr["CategoryName"]),
                    Subject = SafeString(dr["Subject"]),
                    Description = SafeString(dr["Description"]),
                    Status = SafeString(dr["Status"]),
                    AssignedTo = SafeNullableInt(dr["AssignedTo"]),
                    AssignedToName = SafeString(dr["AssignedToName"]),
                    CreatedDate = Convert.ToDateTime(dr["CreatedDate"]),
                    ResolvedDate = SafeNullableDate(dr["ResolvedDate"])
                });
            }

            return list;
        }

        // ---------------- DASHBOARD ----------------
        public DashboardModel GetDashboardData(int userId, string role)
        {
            DashboardModel model = new();

            Hashtable ht = new Hashtable();

            ht.Add("@Role", role);
            ht.Add("@UserId", userId);

            using SqlDataReader dr =_db.GetData("USP_GetComplaintDashboard", ht);

            if (dr.Read())
            {
                if (role == "Admin")
                {
                    model.OpenCount = SafeInt(dr["OpenCount"]);
                    model.AssignedCount = SafeInt(dr["AssignedCount"]);
                    model.ResolvedCount = SafeInt(dr["ResolvedCount"]);
                    model.RejectedCount = SafeInt(dr["RejectedCount"]);
                }
                else if (role == "Support")
                {
                    model.TotalComplaints = SafeInt(dr["TotalComplaints"]);
                    model.OpenCount = SafeInt(dr["OpenCount"]);
                    model.AssignedCount = SafeInt(dr["AssignedCount"]);
                    model.ResolvedCount = SafeInt(dr["ResolvedCount"]);
                    model.RejectedCount = SafeInt(dr["RejectedCount"]);
                }
                else // User
                {
                    model.TotalComplaints = SafeInt(dr["TotalComplaints"]);
                    model.OpenCount = SafeInt(dr["OpenCount"]);
                    model.ResolvedCount = SafeInt(dr["ResolvedCount"]);
                    model.RejectedCount = SafeInt(dr["RejectedCount"]);
                }
            }

            return model;
        }
        // ---------------- GET BY ID ----------------
        public ComplaintModel? GetById(int id)
        {
            ComplaintModel? model = null;

            Hashtable ht = new Hashtable();
            ht.Add("@ComplaintId", id);

            using SqlDataReader dr =
                _db.GetData("USP_GetComplaintById", ht);

            if (dr.Read())
            {
                model = new ComplaintModel
                {
                    ComplaintId = SafeInt(dr["ComplaintId"]),
                    UserId = SafeInt(dr["UserId"]),
                    CategoryId = SafeInt(dr["CategoryId"]),
                    Subject = SafeString(dr["Subject"]),
                    Description = SafeString(dr["Description"]),
                    Status = SafeString(dr["Status"]),
                    CategoryName = SafeString(dr["CategoryName"]),
                    UserName = SafeString(dr["UserName"])
                };
            }

            return model;
        }

        // ---------------- INSERT ----------------
        public void InsertComplaint(ComplaintModel model)
        {
            Hashtable ht = new Hashtable();

            ht.Add("@UserId", model.UserId);
            ht.Add("@CategoryId", model.CategoryId);
            ht.Add("@Subject", model.Subject);
            ht.Add("@Description", model.Description);

            _db.ExecuteQuery("USP_InsertComplaint", ht);
        }

        // ---------------- UPDATE ----------------
        public void UpdateComplaint(ComplaintModel model)
        {
            Hashtable ht = new Hashtable();

            ht.Add("@ComplaintId", model.ComplaintId);
            ht.Add("@CategoryId", model.CategoryId);
            ht.Add("@Subject", model.Subject);
            ht.Add("@Description", model.Description);

            _db.ExecuteQuery("USP_UpdateComplaint", ht);
        }

        // ---------------- DELETE ----------------
        public void DeleteComplaint(int complaintId, int deletedBy)
        {
            Hashtable ht = new Hashtable();

            ht.Add("@ComplaintId", complaintId);
            ht.Add("@DeletedBy", deletedBy);

            _db.ExecuteQuery("USP_DeleteComplaint", ht);
        }

        // ---------------- ASSIGN ----------------
        public bool AssignComplaint(int complaintId, int assignedTo)
        {
            Hashtable ht = new Hashtable();

            ht.Add("@ComplaintId", complaintId);
            ht.Add("@AssignedTo", assignedTo);

            return _db.ExecuteQuery("USP_AssignComplaint", ht) > 0;
        }

        // ---------------- STATUS UPDATE ----------------
        public void UpdateStatus(int complaintId, string status)
        {
            Hashtable ht = new Hashtable();

            ht.Add("@ComplaintId", complaintId);
            ht.Add("@Status", status);

            _db.ExecuteQuery("USP_UpdateComplaintStatus", ht);
        }

        // ---------------- SEARCH ----------------
        public List<ComplaintModel> SearchComplaints(string? subject, string? status, int? categoryId)
        {
            List<ComplaintModel> list = new();

            Hashtable ht = new Hashtable();
            ht.Add("@Subject", subject);
            ht.Add("@Status", status);
            ht.Add("@CategoryId", categoryId);

            using SqlDataReader dr =_db.GetData("USP_SearchComplaints", ht);

            while (dr.Read())
            {
                list.Add(new ComplaintModel
                {
                    ComplaintId = SafeInt(dr["ComplaintId"]),
                    Subject = SafeString(dr["Subject"]),
                    Description = SafeString(dr["Description"]),
                    Status = SafeString(dr["Status"])
                });
            }

            return list;
        }

        // ---------------- REPORT ----------------
        public List<ReportModel> GetComplaintReport()
        {
            List<ReportModel> list = new();

            Hashtable ht = new Hashtable();

            using SqlDataReader dr = _db.GetData("USP_GetComplaintReport", ht);

            while (dr.Read())
            {
                list.Add(new ReportModel
                {
                    CategoryName = SafeString(dr["CategoryName"]),
                    Status = SafeString(dr["Status"]),
                    ComplaintCount = SafeInt(dr["ComplaintCount"])
                });
            }

            return list;
        }
        // ---------------- CHART DATA ----------------
        public List<object> GetComplaintChartData(int userId, string role, string filterType)
        {
            List<object> list = new();

            Hashtable ht = new Hashtable();
            ht.Add("@UserId", userId);
            ht.Add("@Role", role);
            ht.Add("@FilterType", filterType);

            using SqlDataReader dr =_db.GetData("USP_GetComplaintChartData", ht);

            while (dr.Read())
            {
                list.Add(new
                {
                    name = dr["Name"].ToString(),
                    status = dr["Status"].ToString(),
                    count = Convert.ToInt32(dr["Count"])
                });
            }

            return list;
        }
    }
}