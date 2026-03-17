using DataAccessLayer;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class Cart
    {
        public enum eMode { AddNew = 0, Update = 1 }
        public eMode Mode;
        public Cart(int userId, int vehicleId)
        {
            this.UserId = userId;
            this.VehicleId = vehicleId;

            this.Mode = eMode.AddNew;
        }
        
        public Cart(CartDTO dto)
        {
            this.UserId = dto.UserId;
            this.VehicleId = dto.VehicleId;
            this.Quantity = dto.Quantity;

            this.Mode = eMode.Update;
        }

        private CartDTO CartDTO 
        { 
            get
            {
                return new CartDTO()
                {
                    UserId = this.UserId,
                    VehicleId = this.VehicleId,
                    Quantity = this.Quantity
                };
            }
        }

        

        public int UserId { get; set; }
        public int VehicleId { get; set; }
        public int VehicleImage { get; set; } 
        public int Quantity { get; set; }
        public DateTime AddedAt { get; set; }

        private async Task<bool> _AddNew()
        {
            return await CartData.AddItemToCart(this.UserId, this.VehicleId);
        }
        private async Task<bool> _UpdateQuantity()
        {
            return await CartData.UpdateQuantity(this.CartDTO);
        }

        public static async Task<Cart?> FindItemInCart(int userId, int vehicleId) 
        {
            var DTO = await CartData.GetItem(userId,vehicleId);

            return DTO != null ? 
                new Cart(DTO) : null;
        }

        public async Task<CartDTO?> ItemDetails()
        {
            return await CartData.GetItem(this.UserId, this.VehicleId);
        }

        public async Task<bool> Save()
        {
            switch (Mode)
            {
                case eMode.AddNew:
                    if (await _AddNew())
                    {
                        this.Mode = eMode.Update;
                        return true;
                    }
                    else
                        return false;

                case eMode.Update:
                    return await _UpdateQuantity();
            }

            return false;
        }

        public static async Task<bool> DeleteItem(int userId, int vehicleId)
        {
            return await CartData.DeleteItem( userId, vehicleId);
        }

        public async Task<bool> Delete()
        {
            return await DeleteItem(this.UserId,this.VehicleId);
        }

        public async Task<IEnumerable<CartDTO>> ItemsInCart()
        {
            return await ItemsInCart(this.UserId);
        } 
        
        public static async Task<IEnumerable<CartDTO>> ItemsInCart(int userId)
        {
            return await CartData.GetAllInCart(userId);
        }



    }
}
