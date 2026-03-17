using DataAccessLayer.Models.Vehicle;
using DataAccessLayer.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace DataAccessLayer
{
    public static class CartData
    {
        public static async Task<bool> DeleteItem( int userId,  int vehicleId)
        {
            using (SqlConnection conn = new SqlConnection(Settings.ConnectionString))
            {
                try
                {
                    var rowsAffected = await conn.ExecuteAsync("DELETE FROM Cart WHERE UserID = @userId AND VehicleId = @vehicleId"
                        , new { userId = userId, vehicleId = vehicleId });

                    return rowsAffected > 0;
                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.Message.ToString());
                    return false;
                }
            }
        }

        public static async Task<IEnumerable<CartDTO>> GetAllInCart(int userId)
        {
            using (SqlConnection conn = new SqlConnection(Settings.ConnectionString))
            {
                try
                {
                    var sql = @"
                SELECT 
                    C.UserId, C.VehicleId, C.Quantity, C.AddedAt,
                    V.VehicleID, V.Make, V.Model, V.Year, V.ImagePath
                FROM Cart C
                INNER JOIN Vehicle V ON C.VehicleId = V.VehicleID
                WHERE C.UserId = @userId";

                    var result = await conn.QueryAsync<CartDTO, VehicleSimpleDTO, CartDTO>(
                        sql,
                        (cart, vehicle) =>
                        {
                            cart.VehicleData = vehicle;
                            return cart;
                        },
                        new { userId },
                        splitOn: "VehicleID"
                    );

                    return result;
                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.Message);
                    return Enumerable.Empty<CartDTO>();
                }
            }
        }


        public static async Task<CartDTO?> GetItem(int userId, int vehicleId)
        {
            using (SqlConnection conn = new SqlConnection(Settings.ConnectionString))
            {
                try
                {
                    var sql = @"
                SELECT 
                    C.UserId, C.VehicleId, C.Quantity, C.AddedAt,
                    V.VehicleID, V.Make, V.Model, V.Year, V.ImagePath
                FROM Cart C
                INNER JOIN Vehicle V ON C.VehicleId = V.VehicleID
                WHERE C.UserId = @userId AND C.VehicleId = @vehicleId";

                    var result = await conn.QueryAsync<CartDTO, VehicleSimpleDTO, CartDTO>(
                        sql,
                        (cart, vehicle) =>
                        {
                            cart.VehicleData = vehicle;
                            return cart;
                        },
                        new { userId, vehicleId },
                        splitOn: "VehicleID"
                    );

                    return result.SingleOrDefault();
                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.ToString());
                    return null;
                }
            }
        }



        //public static async Task<bool> DeleteCategory(int id)
        //{
        //    using (SqlConnection conn = new SqlConnection(Settings.ConnectionString))
        //    {
        //        try
        //        {
        //            var rowsAffected = await conn.ExecuteAsync("DELETE FROM VehicleCategories WHERE ID = @ID", new { ID = id });
        //            return rowsAffected > 0;
        //        }
        //        catch (SqlException ex)
        //        {
        //            Console.WriteLine(ex.Message.ToString());
        //            return false;
        //        }
        //    }
        //}
        //public static async Task<CategoryDTO?> GetCategoryById(int id)
        //{
        //    using (SqlConnection conn = new SqlConnection(Settings.ConnectionString))
        //    {
        //        try
        //        {
        //            return await conn.QuerySingleOrDefaultAsync<CategoryDTO>("SELECT * FROM VehicleCategories WHERE ID = @ID", new { ID = id });

        //        }
        //        catch (SqlException ex)
        //        {
        //            Console.WriteLine(ex.Message.ToString());
        //            return null;
        //        }
        //    }
        //}
        public static async Task<bool> UpdateQuantity(CartDTO dto)
        {
            using (SqlConnection conn = new SqlConnection(Settings.ConnectionString))
            {
                try
                {
                    var sql = @"UPDATE Cart SET Quantity = @quantity WHERE UserId = @userId AND VehicleId = @vehicleId";
                    int rowsAffected =
                        await conn.ExecuteAsync(sql,
                        new {quantity = dto.Quantity, userId = dto.UserId, vehicleId = dto.VehicleId});

                    return rowsAffected > 0;

                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.Message.ToString());
                    return false;
                }
            }
        }


        public static async Task<bool> AddItemToCart(int userId , int vehicleId)
        {
            using (SqlConnection conn = new SqlConnection(Settings.ConnectionString))
            {
                try
                {
                    int newId =
                        await conn.ExecuteScalarAsync<int>("INSERT INTO Cart(UserId, VehicleId) VALUES(@userId,@vehicleId);",
                        new { userId = userId, vehicleId = vehicleId });

                    return true;

                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.Message.ToString());
                    return false;
                }
            }
        }






    }
}
