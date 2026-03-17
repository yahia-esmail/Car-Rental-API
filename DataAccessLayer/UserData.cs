using Dapper;
using DataAccessLayer.Models.FuelTyp;
using DataAccessLayer.Models.User;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public static class UserData
    {
        public static async Task<int> AddNewUser(NewUserDataDTO User_DTO)
        {
            using (SqlConnection connection = new SqlConnection(Settings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand("sp_AddNewUser", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@roleId", User_DTO.RoleId);
                    command.Parameters.AddWithValue("@firstName", User_DTO.FirstName);
                    command.Parameters.AddWithValue("@lastName", User_DTO.LastName);
                    command.Parameters.AddWithValue("@email", User_DTO.Email);
                    command.Parameters.AddWithValue("@passwordHash", User_DTO.PasswordHash);
                    command.Parameters.AddWithValue("@phoneNumber", User_DTO.PhoneNumber);
                    command.Parameters.AddWithValue("@isActive", User_DTO.IsActive);

                    command.Parameters.AddWithValue("@imagePath",
                        string.IsNullOrWhiteSpace(User_DTO.ImagePath) ?
                        DBNull.Value : User_DTO.ImagePath.Trim());

                    command.Parameters.AddWithValue("@driverLicenseNumber",
                        string.IsNullOrWhiteSpace(User_DTO.DriverLicenseNumber) ?
                        DBNull.Value : User_DTO.DriverLicenseNumber.Trim());

                    var NewID = new SqlParameter("@userId", SqlDbType.Int)
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

        public static async Task<UserInfoDTO> GetUser(int id)
        {
            using (SqlConnection connection = new SqlConnection(Settings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand("sp_GetUserById", connection))
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
                                return new UserInfoDTO
                                (
                                    reader.GetInt32(reader.GetOrdinal("UserId")),
                                    reader.GetInt32(reader.GetOrdinal("RoleId")),
                                    reader.GetString(reader.GetOrdinal("RoleName")),
                                    reader.GetString(reader.GetOrdinal("FirstName")),
                                    reader.GetString(reader.GetOrdinal("LastName")),
                                    reader.GetString(reader.GetOrdinal("Email")),
                                    reader.GetString(reader.GetOrdinal("PhoneNumber")),
                                    reader.GetBoolean(reader.GetOrdinal("IsActive")),
                                    reader.GetDateTime(reader.GetOrdinal("CreatedAt")),

                                    !reader.IsDBNull(reader.GetOrdinal("ImagePath")) ?
                                    reader.GetString(reader.GetOrdinal("ImagePath")) : string.Empty,

                                    !reader.IsDBNull(reader.GetOrdinal("DriverLicenseNumber")) ?
                                    reader.GetString(reader.GetOrdinal("DriverLicenseNumber")) : string.Empty

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
        
        public static async Task<UserInfoDTO> GetUser(string email)
        {
            using (SqlConnection connection = new SqlConnection(Settings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand("sp_GetUserByEmail", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@email", email);

                    try
                    {
                        await connection.OpenAsync();

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (reader.Read())
                            {
                                return new UserInfoDTO
                                (
                                    reader.GetInt32(reader.GetOrdinal("UserId")),
                                    reader.GetInt32(reader.GetOrdinal("RoleId")),
                                    reader.GetString(reader.GetOrdinal("RoleName")),
                                    reader.GetString(reader.GetOrdinal("FirstName")),
                                    reader.GetString(reader.GetOrdinal("LastName")),
                                    reader.GetString(reader.GetOrdinal("Email")),
                                    reader.GetString(reader.GetOrdinal("PhoneNumber")),
                                    reader.GetBoolean(reader.GetOrdinal("IsActive")),
                                    reader.GetDateTime(reader.GetOrdinal("CreatedAt")),

                                    !reader.IsDBNull(reader.GetOrdinal("ImagePath")) ?
                                    reader.GetString(reader.GetOrdinal("ImagePath")) : string.Empty,

                                    !reader.IsDBNull(reader.GetOrdinal("DriverLicenseNumber")) ?
                                    reader.GetString(reader.GetOrdinal("DriverLicenseNumber")) : string.Empty
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

        public static async Task<List<UserInfoDTO>> GetAllUsers()
        {
            var users = new List<UserInfoDTO>();

            using (SqlConnection connection = new SqlConnection(Settings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand("sp_GetAllUsers", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    try
                    {
                        await connection.OpenAsync();

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                users.Add(new UserInfoDTO
                                (
                                    reader.GetInt32(reader.GetOrdinal("UserId")),
                                    reader.GetInt32(reader.GetOrdinal("RoleId")),
                                    reader.GetString(reader.GetOrdinal("RoleName")),
                                    reader.GetString(reader.GetOrdinal("FirstName")),
                                    reader.GetString(reader.GetOrdinal("LastName")),
                                    reader.GetString(reader.GetOrdinal("Email")),
                                    reader.GetString(reader.GetOrdinal("PhoneNumber")),
                                    reader.GetBoolean(reader.GetOrdinal("IsActive")),
                                    reader.GetDateTime(reader.GetOrdinal("CreatedAt")),

                                    !reader.IsDBNull(reader.GetOrdinal("ImagePath")) ?
                                    reader.GetString(reader.GetOrdinal("ImagePath")) : string.Empty,

                                    !reader.IsDBNull(reader.GetOrdinal("DriverLicenseNumber")) ?
                                    reader.GetString(reader.GetOrdinal("DriverLicenseNumber")) : string.Empty)
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

            return users;
        }

        public static async Task<bool> UpdateUser(int id,UpdateUserDTO User_DTO) 
        {
            using (SqlConnection connection = new SqlConnection(Settings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand("sp_UpdateUser", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@userId", id);
                    command.Parameters.AddWithValue("@roleId", User_DTO.RoleId);
                    command.Parameters.AddWithValue("@firstName", User_DTO.FirstName);
                    command.Parameters.AddWithValue("@lastName", User_DTO.LastName);
                    command.Parameters.AddWithValue("@email", User_DTO.Email);
                    command.Parameters.AddWithValue("@phoneNumber", User_DTO.PhoneNumber);
                    command.Parameters.AddWithValue("@isActive", User_DTO.IsActive);

                    command.Parameters.AddWithValue("@imagePath",
                        string.IsNullOrWhiteSpace(User_DTO.ImagePath) ?
                        DBNull.Value : User_DTO.ImagePath.Trim());

                    command.Parameters.AddWithValue("@driverLicenseNumber",
                        string.IsNullOrWhiteSpace(User_DTO.DriverLicenseNumber) ?
                        DBNull.Value : User_DTO.DriverLicenseNumber.Trim());


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

        public static async Task<bool> DeleteUser(int id) 
        {
            using (SqlConnection connection = new SqlConnection(Settings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand("sp_DeleteUser", connection))
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
        
        public static async Task<bool> CheckPassword(int id,string password)  
        {
            using (SqlConnection connection = new SqlConnection(Settings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand("sp_CheckPassword", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@id", id);
                    command.Parameters.AddWithValue("@password", password);

                    try
                    {
                        await connection.OpenAsync();

                        var isCorrect =await  command.ExecuteScalarAsync();
                        return isCorrect != null && (int)isCorrect == 1;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message.ToString());
                    }
                }
            }

            return false;
        }
                                 
        public static async Task<bool> ChangePassword(int id ,string newPassword)
        {
            using(SqlConnection conn = new SqlConnection(Settings.ConnectionString))
            {
                try
                {
                    int rowsAffected = await conn.ExecuteAsync("UPDATE Users SET PasswordHash = @newPassword WHERE UserId = @id",
                        new { id = id, newPassword = newPassword });

                    return rowsAffected > 0;
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }
            }
        }

        public static async Task<bool> UpdateImage(int id, string imageUrl)
        {
            using (SqlConnection conn = new SqlConnection(Settings.ConnectionString))
            {
                try
                {
                    int rowsAffected = await conn.ExecuteAsync("UPDATE Users SET ImagePath = @imageUrl WHERE UserId = @id",
                        new { id = id, imageUrl = imageUrl });

                    return rowsAffected > 0;
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }
            }
        }




    }
}
