using BusinessLayer;
using DataAccessLayer.Models;
using DataAccessLayer.Models.FuelTyp;
using DataAccessLayer.Models.Vehicle;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Eventing.Reader;

namespace Car_Rental_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        [HttpGet("{userId}/Items")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult<object>> GetItemsInCart(int userId)
        {
            if (userId <= 0)
                return BadRequest(new { message = "Invalid User ID" });

            if (await BusinessLayer.User.Find(userId) == null)
                return NotFound(new { message = $"No User With ID {userId}" });

            var Items = await Cart.ItemsInCart(userId);

            if (!Items.Any())
            {
                return NotFound(new { message = "No items found in the user's cart." });
            }

            var result = new
            {
                UserId = userId,
                Count = Items.Count(),
                Data = Items
            };

            return Ok(result);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CartDTO>> UpdateQuantity([FromBody]CartUpdateDTO dto)
        {
            if (!ModelState.IsValid) 
                return BadRequest(ModelState);

            if (await BusinessLayer.User.Find(dto.UserId) == null)
                return NotFound(new { message = $"No User With ID {dto.UserId}" });

            var cart = await Cart.FindItemInCart(dto.UserId,dto.VehicleId);

            if (cart == null)
                return NotFound(new { message = "This Item Not Found in this cart." });

            cart.Quantity = dto.Quantity;

            if (cart.Quantity == 0)
            {
                if (await cart.Delete())
                    return Ok(new { message = "Item Deleted from Cart." });
                else
                    return BadRequest(new { message = "Failed to delete the item from the cart." });
            }


            if (!await cart.Save())
                return BadRequest(new { message = "Failed to update quantity." });

            return Ok(await cart.ItemDetails());
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<bool>> DeleteItem(int userId, int vehicleId)
        {
            if (userId <= 0 || vehicleId <= 0)
                return BadRequest(new { message = "Invalid Id" });

            if (await BusinessLayer.User.Find(userId) == null)
                return NotFound(new { message = $"No User With ID {userId}" });

            var cart = await Cart.FindItemInCart(userId, vehicleId);

            if (cart == null)
                return NotFound(new { message = $"This Item Not Found in the cart." });


            if (await cart.Delete())
                return Ok(new { message = "Item Deleted Successfully." });
            else
                return BadRequest(new { message = $"Failed to delete this item." });
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]

        public async Task<ActionResult<CartDTO>> AddItemInCart(int userId, int vehicleId)
        {
            if(userId <= 0 || vehicleId  <=0)
                return BadRequest(new {message = "Invalid Id"});

            if (await BusinessLayer.User.Find(userId) == null)
                return NotFound(new { message = $"No User With ID {userId}" });


            if (await Cart.FindItemInCart(userId, vehicleId) != null)
                return NotFound(new { message = "This Item Is Already existed in the cart." });

            var cart = new Cart(userId, vehicleId);

            if (!await cart.Save())
            {
                return BadRequest(new { message = "Failed in add Item to cart." });
            }

            return Ok(await cart.ItemDetails());
        }


    }
}
