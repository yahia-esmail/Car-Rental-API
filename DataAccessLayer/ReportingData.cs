using Dapper;
using DataAccessLayer.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public static class ReportingData
    {

        public static async Task<Counts> GetCounts()
        {
            using (SqlConnection conn = new SqlConnection(Settings.ConnectionString))
            {
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@users", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);
                    parameters.Add("@admins", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);
                    parameters.Add("@vehicles", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);
                    parameters.Add("@fuelTypes", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);
                    parameters.Add("@categories", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);

                    await conn.ExecuteAsync("sp_CountsReporting", parameters, commandType: System.Data.CommandType.StoredProcedure);

                    var counts = new Counts
                    {
                        UsersCount = parameters.Get<int>("@users"),
                        AdminsCount = parameters.Get<int>("@admins"),
                        VehiclesCount = parameters.Get<int>("@vehicles"),
                        FuelTypesCount = parameters.Get<int>("@fuelTypes"),
                        CategoriesCount = parameters.Get<int>("@categories")
                    };

                    return counts;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return null!;
                }
            }
        }







    }
}
