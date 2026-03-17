using DataAccessLayer.Models.FuelTyp;
using DataAccessLayer.Models.Vehicle;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public static class FuelTypeData
    {
        public static async Task<List<FuelTypeDTO>> GetAllFuelTypes()
        {
            var fuelTypes = new List<FuelTypeDTO>();

            using (SqlConnection connection = new SqlConnection(Settings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand("sp_GetAllFuelTypes", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    try
                    {
                        await connection.OpenAsync();

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                fuelTypes.Add(new FuelTypeDTO
                                (
                                    reader.GetInt32(reader.GetOrdinal("ID")),
                                    reader.GetString(reader.GetOrdinal("FuelType"))
                                ));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
            }

            return fuelTypes;
        }
        
        public static async Task<FuelTypeDTO> GetFuelType(int id)
        {
            using (SqlConnection connection = new SqlConnection(Settings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand("sp_GetFuelTypeById", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@id", id);

                    try
                    {
                        await connection.OpenAsync();

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (reader.Read())
                            {
                                return new FuelTypeDTO
                                (
                                    reader.GetInt32(reader.GetOrdinal("ID")),
                                    reader.GetString(reader.GetOrdinal("FuelType"))
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

            return null;
        }

        public static async Task<int> AddNewFuelType(string FT_Name) 
        {
            using (SqlConnection connection = new SqlConnection(Settings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand("sp_AddNewFuelType", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@fuelType", FT_Name);

                    var NewID = new SqlParameter("@newId", SqlDbType.Int)
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

        public static async Task<bool> UpdateFuelType(FuelTypeDTO FT_DTO)
        {
            using (SqlConnection connection = new SqlConnection(Settings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand("sp_UpdateFuelType", connection))
                {
                    command.CommandType = CommandType.StoredProcedure; 
                    command.Parameters.AddWithValue("@id", FT_DTO.Id);
                    command.Parameters.AddWithValue("@newFuelType", FT_DTO.FuelType);
                   

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


        public static async Task<bool> DeleteFuelType(int id)
        {
            using (SqlConnection connection = new SqlConnection(Settings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand("sp_DeleteFuelType", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@id", id);

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






    } 
}
