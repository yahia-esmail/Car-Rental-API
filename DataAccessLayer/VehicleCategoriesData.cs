using Dapper;
using DataAccessLayer.Models;
using DataAccessLayer.Models.Vehicle;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public static class VehicleCategoriesData
    {
        public static async Task<IEnumerable<CategoryDTO>> GetAllCategories() 
        {
            using(SqlConnection conn = new SqlConnection(Settings.ConnectionString))
            {
                try
                {
                    return await conn.QueryAsync<CategoryDTO>("SELECT * FROM VehicleCategories");
                   
                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.Message.ToString());
                    return Enumerable.Empty<CategoryDTO>();
                }
            }
        }


        public static async Task<bool> DeleteCategory(int id)
        {
            using (SqlConnection conn = new SqlConnection(Settings.ConnectionString))
            {
                try
                {
                    var rowsAffected =  await conn.ExecuteAsync("DELETE FROM VehicleCategories WHERE ID = @ID", new { ID = id});
                    return rowsAffected > 0;
                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.Message.ToString());
                    return false;
                }
            }
        }
        public static async Task<CategoryDTO?> GetCategoryById(int id)
        {
            using (SqlConnection conn = new SqlConnection(Settings.ConnectionString))
            {
                try
                {
                    return  await conn.QuerySingleOrDefaultAsync<CategoryDTO>("SELECT * FROM VehicleCategories WHERE ID = @ID", new { ID = id });

                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.Message.ToString());
                    return null;
                }
            }
        } 
        public static async Task<bool> UpdateCategory(CategoryDTO CDTO)
        {
            using (SqlConnection conn = new SqlConnection(Settings.ConnectionString))
            {
                try
                {
                    int rowsAffected =  
                        await conn.ExecuteAsync("UPDATE VehicleCategories SET CategoryName = @Name WHERE ID = @ID", 
                        new { ID = CDTO.ID, Name = CDTO.Name });

                    return rowsAffected > 0;

                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.Message.ToString());
                    return false;
                }
            }
        }
        public static async Task<int> AddNewCategory(CategoryDTO DTO)
        {
            using (SqlConnection conn = new SqlConnection(Settings.ConnectionString))
            {
                try
                {
                    int newId =
                        await conn.ExecuteScalarAsync<int>("INSERT INTO VehicleCategories VALUES(@Name,@ImagePath);SELECT SCOPE_IDENTITY()",
                        new { Name= DTO.Name, DTO.ImagePath });

                    return newId;

                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.Message.ToString());
                    return -1;
                }
            }
        }

        public static async Task<bool> UpdateImage(int id, string imageUrl)
        {
            using (SqlConnection conn = new SqlConnection(Settings.ConnectionString))
            {
                try
                {
                    int rowsAffected = await conn.ExecuteAsync("UPDATE VehicleCategories SET ImagePath = @imageUrl WHERE ID = @id",
                        new { id = id, imageUrl = imageUrl });

                    return rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }
            }
        }


        public static async Task<IEnumerable<VehicleReadDTO>> GetVehicles(int categoryId)
        {
            using (SqlConnection conn = new SqlConnection(Settings.ConnectionString))
            {
                try
                {
                    var vehicles = await conn.QueryAsync<VehicleReadDTO>("sp_GetVehiclesByCategoryId", 
                        new { CategoryId = categoryId },
                        commandType: System.Data.CommandType.StoredProcedure);

                    return vehicles;
                }
                catch (SqlException ex) 
                {
                    Console.WriteLine(ex.Message.ToString());
                    return Enumerable.Empty<VehicleReadDTO>();
                }
            }
        }







    }
}
