using DataAccessLayer;
using DataAccessLayer.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace BusinessLayer
{
    public class User
    {

        public enum eMode { AddNew = 0 , Update = 1 }
        public eMode Mode = eMode.AddNew;

        public User(UserInfoDTO User_DTO)
        {
            UserId = User_DTO.UserId;
            RoleId = User_DTO.RoleId;
            Role = User_DTO.Role;
            FirstName = User_DTO.FirstName;
            LastName = User_DTO.LastName;
            Email = User_DTO.Email;
            PhoneNumber = User_DTO.PhoneNumber;
            IsActive = User_DTO.IsActive;
            CreatedAt = User_DTO.CreatedAt;
            ImagePath = User_DTO.ImagePath;
            DriverLicenseNumber = User_DTO.DriverLicenseNumber;

            this.Mode = eMode.Update;
        }
        public User(int id,UpdateUserDTO User_DTO)
        {
            UserId = id;
            RoleId = User_DTO.RoleId;
            FirstName = User_DTO.FirstName;
            LastName = User_DTO.LastName;
            Email = User_DTO.Email;
            PhoneNumber = User_DTO.PhoneNumber;
            IsActive = User_DTO.IsActive;
            ImagePath = User_DTO.ImagePath;
            DriverLicenseNumber = User_DTO.DriverLicenseNumber;

            this.Mode = eMode.Update;
        }
        public UserInfoDTO UserInfoDTO 
        { 
            get 
            {
                return new UserInfoDTO(this.UserId, this.RoleId, this.Role, this.FirstName, this.LastName,
                    this.Email, this.PhoneNumber, this.IsActive, this.CreatedAt, this.ImagePath,this.DriverLicenseNumber);
            } 
        }
        public User(NewUserDTO User_DTO)
        {
            RoleId = User_DTO.RoleId;
            FirstName = User_DTO.FirstName;
            LastName = User_DTO.LastName;
            Email = User_DTO.Email;
            Password = User_DTO.Password;
            PhoneNumber = User_DTO.PhoneNumber;
            IsActive = User_DTO.IsActive;
            CreatedAt = DateTime.Now;
            ImagePath = User_DTO.ImagePath;
            DriverLicenseNumber = User_DTO.DriverLicenseNumber;

            this.Mode = eMode.AddNew;
        }
        
        public User(NewUserFromUserDTO User_DTO)
        {
            RoleId = User_DTO.RoleId;
            FirstName = User_DTO.FirstName;
            LastName = User_DTO.LastName;
            Email = User_DTO.Email;
            Password = User_DTO.Password;
            PhoneNumber = User_DTO.PhoneNumber;
            IsActive = User_DTO.IsActive;
            CreatedAt = DateTime.Now;
            DriverLicenseNumber = User_DTO.DriverLicenseNumber;

            this.Mode = eMode.AddNew;
        }
        private NewUserDataDTO _NewUserDataDTO 
        {
            get
            {
                return new NewUserDataDTO(this.NewUserDTO);
            }
        }

        public NewUserDTO NewUserDTO  
        {
            get
            {
                return new NewUserDTO(this.RoleId, this.FirstName, this.LastName, this.Email, this.Password,
                    this.PhoneNumber, this.IsActive, this.ImagePath,this.DriverLicenseNumber);
            }
                
        }
        
        public UpdateUserDTO UpdateUserDTO
        {
            get
            {
                return new UpdateUserDTO(this.RoleId, this.FirstName, this.LastName, this.Email,
                    this.PhoneNumber!, this.IsActive, this.ImagePath!,this.DriverLicenseNumber!);
            }
                
        }



        public int UserId { get; set; }
        public int RoleId { get; set; }
        public string Role { get; set; }    
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        private string Password { get; set; } 
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? ImagePath { get; set; }
        public string? DriverLicenseNumber { get; set; }


        private async Task<bool> _AddNewUser()
        {
            this.UserId = await UserData.AddNewUser(this._NewUserDataDTO);

            return this.UserId != -1;
        }

        private async Task<bool> _UpdateUser()
        {
            return await UserData.UpdateUser(this.UserId,this.UpdateUserDTO);// without updating password and CreatedAt.
        }
        public async Task<bool> Save()
        {
            switch(this.Mode)
            {
                case eMode.AddNew:
                    if (await _AddNewUser())
                    {
                        this.Mode = eMode.Update;
                        var userInfo = await Find(this.UserId);
                        if (userInfo != null)
                        {
                            this.CopyFrom(userInfo);
                            return true;
                        }
                        return false;
                    }
                    else
                        return false;

                case eMode.Update:
                    if( await _UpdateUser())
                    {
                        var userInfo = await Find(this.UserId);
                        if (userInfo != null)
                        {
                            this.CopyFrom(userInfo);
                            return true;
                        }
                        return false;
                    }
                    else
                        return false;
            }

            return false;
        }


        public static async Task<User?> Find(int id)
        {
            var User_DTO = await UserData.GetUser(id);

            if (User_DTO != null)
                return new User(User_DTO);
            else
                return null;

        }

        public static async Task<User?> Find(string email)
        {
            var User_DTO = await UserData.GetUser(email);
            return User_DTO != null ? new User(User_DTO) : null;
        }
        public static async Task<List<UserInfoDTO>> GetAllUsers()
        {
            return await UserData.GetAllUsers();
        }
        public void CopyFrom(User other)
        {
            this.UserId = other.UserId;
            this.FirstName = other.FirstName;
            this.LastName = other.LastName;
            this.Email = other.Email;
            this.Password = other.Password;
            this.PhoneNumber = other.PhoneNumber;
            this.ImagePath = other.ImagePath;
            this.IsActive = other.IsActive;
            this.Role = other.Role;
            this.RoleId = other.RoleId;
            this.CreatedAt = other.CreatedAt;
            this.DriverLicenseNumber = other.DriverLicenseNumber;
        }
        
        public void CopyFrom(UpdateUserDTO other)
        {
            this.RoleId = other.RoleId;
            this.FirstName = other.FirstName;
            this.LastName = other.LastName;
            this.Email = other.Email;
            this.PhoneNumber = other.PhoneNumber;
            this.IsActive = other.IsActive;
            this.DriverLicenseNumber = other.DriverLicenseNumber;
        }

        public async Task<bool> DeleteUser()
        {
            return await UserData.DeleteUser(this.UserId);
        }

        public static async Task<bool> CheckPassword(int id, string password)
        {
            return await UserData.CheckPassword(id,Helper.ComputeHash(password));
        }

        public async Task<bool> CheckPassword(string password)
        {
            return await CheckPassword(this.UserId, password);
        }

        public static async Task<bool> ChangePassword(int id, string newPassword)
        {
            return await UserData.ChangePassword(id, Helper.ComputeHash(newPassword));
        }
        
        public async Task<bool> ChangePassword(string newPassword)
        {
            return await ChangePassword(this.UserId, newPassword);
        }

        public async Task<bool> UpdateImage(string imageUrl)
        {
            if( await UserData.UpdateImage(this.UserId, imageUrl))
            {
                this.ImagePath = imageUrl;
                return true;
            }
            else
                return false;
        }




    }










}
