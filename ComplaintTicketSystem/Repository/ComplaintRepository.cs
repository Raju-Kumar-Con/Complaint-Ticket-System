using ComplaintTicketSystem.Data;
using ComplaintTicketSystem.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ComplaintTicketSystem.Repositories
{
    public class ComplaintRepository : IComplaintRepository
    {
        private readonly DBHelper _db;

        public ComplaintRepository(DBHelper db)
        {
            _db = db;
        }

        // Helper method for safe string conversion
        private static string SafeString(object value)
        {
            return Convert.ToString(value) ?? string.Empty;
        }

        // Helper method for safe int conversion
        private static int SafeInt(object value)
        {
            return value == DBNull.Value ? 0 : Convert.ToInt32(value);
        }

        // Helper method for nullable int
        private static int? SafeNullableInt(object value)
        {
            return value == DBNull.Value ? null : Convert.ToInt32(value);
        }

        // Helper method for nullable DateTime
        private static DateTime? SafeNullableDate(object value)
        {
            return value == DBNull.Value ? null : Convert.ToDateTime(value);
        }

        // ---------------- GET COMPLAINTS ----------------
        public List<ComplaintModel> GetComplaints(int userId, string role)
        {
            List<ComplaintModel> list = new();
            using (SqlConnection con = _db.GetConnection())
            {
                SqlCommand cmd = new SqlCommand("USP_GetComplaints", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@Role", role);
                con.Open();
                using SqlDataReader dr = cmd.ExecuteReader();
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
            }

            return list;
        }

        // ---------------- DASHBOARD ----------------
        public DashboardModel GetDashboardData(int userId, string role)
        {
            DashboardModel model = new();

            using (SqlConnection con = _db.GetConnection())
            {
                SqlCommand cmd = new SqlCommand("USP_GetComplaintDashboard", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@Role", role);
                cmd.Parameters.AddWithValue("@UserId", userId);

                con.Open();

                using SqlDataReader dr = cmd.ExecuteReader();

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
            }

            return model;
        }
        // ---------------- GET BY ID ----------------
        public ComplaintModel? GetById(int id)
        {
            ComplaintModel? model = null;

            using SqlConnection con = _db.GetConnection();

            SqlCommand cmd = new SqlCommand("USP_GetComplaintById", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@ComplaintId", id);

            con.Open();

            using SqlDataReader dr = cmd.ExecuteReader();

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
            using SqlConnection con = _db.GetConnection();

            SqlCommand cmd = new SqlCommand("USP_InsertComplaint", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@UserId", model.UserId);
            cmd.Parameters.AddWithValue("@CategoryId", model.CategoryId);
            cmd.Parameters.AddWithValue("@Subject", model.Subject);
            cmd.Parameters.AddWithValue("@Description", model.Description);
            con.Open();
            cmd.ExecuteNonQuery();
        }

        // ---------------- UPDATE ----------------
        public void UpdateComplaint(ComplaintModel model)
        {
            using SqlConnection con = _db.GetConnection();
            SqlCommand cmd = new SqlCommand("USP_UpdateComplaint", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@ComplaintId", model.ComplaintId);
            cmd.Parameters.AddWithValue("@CategoryId", model.CategoryId);
            cmd.Parameters.AddWithValue("@Subject", model.Subject);
            cmd.Parameters.AddWithValue("@Description", model.Description);

            con.Open();
            cmd.ExecuteNonQuery();
        }

        // ---------------- DELETE ----------------
        public void DeleteComplaint(int complaintId)
        {
            using SqlConnection con = _db.GetConnection();

            SqlCommand cmd = new SqlCommand("USP_DeleteComplaint", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@ComplaintId", complaintId);
            con.Open();
            cmd.ExecuteNonQuery();
        }

        // ---------------- ASSIGN ----------------
        public void AssignComplaint(int complaintId, int assignedTo)
        {
            using SqlConnection con = _db.GetConnection();

            SqlCommand cmd = new SqlCommand("USP_AssignComplaint", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@ComplaintId", complaintId);
            cmd.Parameters.AddWithValue("@AssignedTo", assignedTo);
            con.Open();
            cmd.ExecuteNonQuery();
        }

        // ---------------- STATUS UPDATE ----------------
        public void UpdateStatus(int complaintId, string status)
        {
            using SqlConnection con = _db.GetConnection();

            SqlCommand cmd = new SqlCommand("USP_UpdateComplaintStatus", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@ComplaintId", complaintId);
            cmd.Parameters.AddWithValue("@Status", status);
            con.Open();
            cmd.ExecuteNonQuery();
        }

        // ---------------- SEARCH ----------------
        public List<ComplaintModel> SearchComplaints(string? subject, string? status, int? categoryId)
        {
            List<ComplaintModel> list = new();

            using SqlConnection con = _db.GetConnection();

            SqlCommand cmd = new SqlCommand("USP_SearchComplaints", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Subject", (object?)subject ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Status", (object?)status ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CategoryId", (object?)categoryId ?? DBNull.Value);

            con.Open();

            using SqlDataReader dr = cmd.ExecuteReader();

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

            using SqlConnection con = _db.GetConnection();

            SqlCommand cmd = new SqlCommand("USP_GetComplaintReport", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            con.Open();

            using SqlDataReader dr = cmd.ExecuteReader();

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
        public List<object> GetComplaintChartData(int userId,string role,string filterType)
        {
            List<object> list = new();

            using SqlConnection con = _db.GetConnection();

            SqlCommand cmd =
                new SqlCommand("USP_GetComplaintChartData",con);

            cmd.CommandType =CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@UserId",userId);

            cmd.Parameters.AddWithValue( "@Role",role);

            cmd.Parameters.AddWithValue("@FilterType",filterType);

            con.Open();

            using SqlDataReader dr =cmd.ExecuteReader();

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