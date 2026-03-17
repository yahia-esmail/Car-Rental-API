using DataAccessLayer.Models.Vehicle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.IdentityModel.Tokens;
using Dapper;

namespace DataAccessLayer
{
    public static class VehicleData
    {
        
        public static async Task<List<VehicleReadDTO>> GetAllVehicles()
        {
            var vehicles = new List<VehicleReadDTO>();

            using (SqlConnection connection = new SqlConnection(Settings.ConnectionString))
            {
                using(SqlCommand command = new SqlCommand("view_GetAllVehicles",connection))
                {
                    command.CommandType  = CommandType.Text;
                    command.CommandText = "SELECT * FROM view_GetAllVehicles";
                    try
                    {
                        await connection.OpenAsync();

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                vehicles.Add(new VehicleReadDTO
                                (
                                    reader.GetInt32(reader.GetOrdinal("VehicleID")),
                                    reader.GetString(reader.GetOrdinal("Make")),
                                    reader.GetString(reader.GetOrdinal("Model")),
                                    reader.GetInt32(reader.GetOrdinal("Year")),
                                    reader.GetInt32(reader.GetOrdinal("Mileage")),
                                    reader.GetString(reader.GetOrdinal("FuelType")),
                                    reader.GetString(reader.GetOrdinal("PlateNumber")),
                                    reader.GetString(reader.GetOrdinal("CategoryName")),
                                    reader.GetDecimal(reader.GetOrdinal("RentalPricePerDay")),
                                    reader.GetBoolean(reader.GetOrdinal("IsAvailableForRent")),

                                    !reader.IsDBNull(reader.GetOrdinal("ImagePath")) ?
                                    reader.GetString(reader.GetOrdinal("ImagePath")) : string.Empty,

                                    !reader.IsDBNull(reader.GetOrdinal("Features")) ?
                                    reader.GetString(reader.GetOrdinal("Features")) : string.Empty
                                ));
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.Message.ToString());
                    }
                }
            }

            return vehicles;
        }

        public static async Task<int> CountVehicles()
        {
            using (SqlConnection connection = new SqlConnection(Settings.ConnectionString))
            {
                return await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Vehicle");
            }
        }

        public static async Task<VehicleUpdateDTO> GetVehicleById(int id)
        {
            using (SqlConnection connection = new SqlConnection(Settings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand("sp_GetVehicleByID", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@VehicleID", id);

                    try
                    {
                        await connection.OpenAsync();

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if(reader.Read())
                            {
                                return new VehicleUpdateDTO
                                (
                                    reader.GetInt32(reader.GetOrdinal("VehicleID")),
                                    reader.GetString(reader.GetOrdinal("Make")),
                                    reader.GetString(reader.GetOrdinal("Model")),
                                    reader.GetInt32(reader.GetOrdinal("Year")),
                                    reader.GetInt32(reader.GetOrdinal("Mileage")),
                                    reader.GetInt32(reader.GetOrdinal("FuelTypeID")),
                                    reader.GetString(reader.GetOrdinal("PlateNumber")),
                                    reader.GetInt32(reader.GetOrdinal("VehicleCategoryID")),
                                    reader.GetDecimal(reader.GetOrdinal("RentalPricePerDay")),
                                    reader.GetBoolean(reader.GetOrdinal("IsAvailableForRent")),

                                    !reader.IsDBNull(reader.GetOrdinal("ImagePath")) ?
                                    reader.GetString(reader.GetOrdinal("ImagePath")) : string.Empty,

                                    !reader.IsDBNull(reader.GetOrdinal("Features")) ?
                                    reader.GetString(reader.GetOrdinal("Features")) : string.Empty
                                );
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
            }

            return null!;
        }


        public static async Task<int> AddNewVehicle(VehicleCreateDTO vehicleDto)
        {
            using (SqlConnection connection = new SqlConnection(Settings.ConnectionString))
            {
                using(SqlCommand command = new SqlCommand("sp_AddNewVehicle",connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@make", vehicleDto.Make);
                    command.Parameters.AddWithValue("@model", vehicleDto.Model);
                    command.Parameters.AddWithValue("@year", vehicleDto.Year);
                    command.Parameters.AddWithValue("@fuelTypeID", vehicleDto.FuelTypeID);
                    command.Parameters.AddWithValue("@mileage", vehicleDto.Mileage);
                    command.Parameters.AddWithValue("@plateNumber", vehicleDto.PlateNumber);
                    command.Parameters.AddWithValue("@vehicleCategoryID", vehicleDto.VehicleCategoryID);
                    command.Parameters.AddWithValue("@rentalPricePerDay", vehicleDto.RentalPricePerDay);
                    command.Parameters.AddWithValue("@isAvailableForRent",vehicleDto.IsAvailableForRent?1:0);

                    command.Parameters.AddWithValue("@imagePath",
                        string.IsNullOrWhiteSpace(vehicleDto.ImagePath) ? 
                        DBNull.Value:vehicleDto.ImagePath.Trim());
                    
                    command.Parameters.AddWithValue("@features",
                        string.IsNullOrWhiteSpace(vehicleDto.Features) ? 
                        DBNull.Value:vehicleDto.Features.Trim());

                    

                    var NewID = new SqlParameter("@NewID", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output,
                    };

                    command.Parameters.Add(NewID);

                    try
                    {
                        await connection.OpenAsync();

                        await command.ExecuteNonQueryAsync();
                        return NewID.Value != DBNull.Value ? 
                            Convert.ToInt32(NewID.Value) : -1;
                    } 
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message.ToString());
                    }
                }
            }

            return -1;
        }

        public static async Task<bool> UpdateVehicle(VehicleUpdateDTO vehicleDto)
        {
            using (SqlConnection connection = new SqlConnection(Settings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand("sp_UpdateVehicle", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@VehicleID", vehicleDto.Id);
                    command.Parameters.AddWithValue("@make", vehicleDto.Make);
                    command.Parameters.AddWithValue("@model", vehicleDto.Model);
                    command.Parameters.AddWithValue("@year", vehicleDto.Year);
                    command.Parameters.AddWithValue("@fuelTypeID", vehicleDto.FuelTypeID);
                    command.Parameters.AddWithValue("@mileage", vehicleDto.Mileage);
                    command.Parameters.AddWithValue("@plateNumber", vehicleDto.PlateNumber);
                    command.Parameters.AddWithValue("@vehicleCategoryID", vehicleDto.VehicleCategoryID);
                    command.Parameters.AddWithValue("@rentalPricePerDay", vehicleDto.RentalPricePerDay);
                    command.Parameters.AddWithValue("@isAvailableForRent", vehicleDto.IsAvailableForRent ? 1 : 0);

                    command.Parameters.AddWithValue("@imagePath",
                        string.IsNullOrWhiteSpace(vehicleDto.ImagePath) ?
                        DBNull.Value : vehicleDto.ImagePath.Trim());

                    command.Parameters.AddWithValue("@features",
                        string.IsNullOrWhiteSpace(vehicleDto.Features) ?
                        DBNull.Value : vehicleDto.Features.Trim());

                    var returnValue = new SqlParameter()
                    {
                        Direction = ParameterDirection.ReturnValue,
                        SqlDbType = SqlDbType.Int
                    };
                    
                    command.Parameters.Add(returnValue);


                    try
                    {
                        await connection.OpenAsync();

                        await command.ExecuteNonQueryAsync();
                        return (int)returnValue.Value == 1;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message.ToString());
                    }
                }
            }

            return false;
        }

        public static async Task<bool> DeleteVehicle(int id) 
        {
            using (SqlConnection connection = new SqlConnection(Settings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand("sp_DeleteVehicle", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@vehicleID", id);

                    var returnValue = new SqlParameter()
                    {
                        Direction = ParameterDirection.ReturnValue,
                        SqlDbType = SqlDbType.Int
                    };

                    command.Parameters.Add(returnValue);


                    try
                    {
                        await connection.OpenAsync();

                        await command.ExecuteNonQueryAsync();
                        return (int)returnValue.Value == 1;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message.ToString());
                    }
                }
            }

            return false;
        }


        public static async Task<bool> UpdateImage(int id, string imageUrl)
        {
            using (SqlConnection conn = new SqlConnection(Settings.ConnectionString))
            {
                try
                {
                    int rowsAffected = await conn.ExecuteAsync("UPDATE Vehicle SET ImagePath = @imageUrl WHERE VehicleID = @id",
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
















    }
}
