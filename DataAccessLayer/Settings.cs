using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public static class Settings
    {
        public static string ConnectionString 
        { 
            get 
            {
                return "Server=tcp:car-rental-db-server.database.windows.net,1433;Initial Catalog=Car Rental DataBase;Persist Security Info=False;User ID=admin_user;Password=V3h!cl3R3nt@l2025#;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            } 
        }
    }
}
