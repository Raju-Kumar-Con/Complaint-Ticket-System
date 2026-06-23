using ComplaintTicketSystem.Data;
using System.Collections;

namespace ComplaintTicketSystem.Repositories
{
    public class ErrorRepository
    {
        private readonly DBHelper _db;

        public ErrorRepository(DBHelper db)
        {
            _db = db;
        }

        public void LogError(string message)
        {
            try
            {
                Hashtable ht = new Hashtable();

                ht.Add("@Message", message);

                _db.ExecuteQuery("USP_LogError", ht);
            }
            catch
            {
                // Logging failure ko ignore kar rahe hain
                // taaki application crash na ho.
            }
        }
    }
}