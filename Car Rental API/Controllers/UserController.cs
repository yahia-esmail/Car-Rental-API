using BusinessLayer;
using DataAccessLayer.Models.User;
using DataAccessLayer.Models.Vehicle;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using static DataAccessLayer.Helper;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Car_Rental_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        [HttpGet("{id}", Name = "GetUserByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<ActionResult<UserInfoDTO>> GetUserByID(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid ID!");

            var user = await BusinessLayer.User.Find(id);

            if (user == null)
            {
                return NotFound("Not Found!");
            }

            return Ok(user.UserInfoDTO);
        }

        [HttpPost("reg")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Consumes("multipart/form-data")]

        public async Task<ActionResult<UserInfoDTO>> RegisterNewUser([FromForm] NewUserFromUserDTO User_DTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new User(User_DTO);

            if (!await user.Save())
            {
                return BadRequest("Failed.\nmay be email is already existed.");
            }

            // handle user image
            var result = await ImageUploaderHelper.UploadImageAsync(
                User_DTO.UserImage,
                "uploads/users",
                Request.Host.Value,
                Request.Scheme,
                $"user{user.UserId}"
            );

            if (!result.success)
                return BadRequest(result.error);

            if (await user.UpdateImage(result.url!))
                return CreatedAtRoute("GetUserByID", new { id = user.UserId }, user.UserInfoDTO);
            else
                return StatusCode(StatusCodes.Status500InternalServerError, $"User added with id [{user.UserId}], but failed to upload image.");
        }

        [HttpPost("auth/login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<LoginResponseDTO>> Login(LoginDTO User_DTO)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                               .Select(e => e.ErrorMessage);
                return BadRequest(errors);
            }

            var user = await BusinessLayer.User.Find(User_DTO.Email);

            if (user == null)
            {
                return NotFound($"No User With This Email: {User_DTO.Email}.");
            }

            if (!await user.CheckPassword(User_DTO.Password))
            {
                return Unauthorized("Incorrect password.");
            }

            return Ok(new LoginResponseDTO("Token, SOON!", user.UserInfoDTO));
        }

        [HttpPut("auth/Reset-Password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<bool>> ResetPassword(ResetPasswordDTO UserDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await BusinessLayer.User.Find(UserDTO.Email);

            if (user == null)
            {
                return NotFound($"No User With This Email: {UserDTO.Email}.");
            }

            if (!await user.CheckPassword(UserDTO.CurrentPassword))
            {
                return Unauthorized("Current Password is Incorrect.");
            }

            bool result = await user.ChangePassword(UserDTO.NewPassword);

            return result ? Ok(new { Success = true, Message = "Password updated successfully." })
                : BadRequest(new { Success = false, Message = "An error occurred while changing the password.!" });
        }

        [HttpGet("All")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<VehicleReadDTO>>> GetAllUsers()
        {
            var users = await BusinessLayer.User.GetAllUsers();

            if (users.Count == 0)
            {
                return NotFound("No Users found.");
            }

            return Ok(users);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserInfoDTO>> UpdateUser(int id, UpdateUserDTO User_DTO)
        {
            if(id <= 0)
                return BadRequest("Invalid ID!");   

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await BusinessLayer.User.Find(id);

            if (user == null)
                return NotFound($"User with id {id} not found!");

            user.CopyFrom(User_DTO);

            if (!await user.Save())
                return BadRequest("Failed!");

            return Ok(user.UserInfoDTO);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<bool>> DeleteUser(int id)
        {
            if (id <= 0)
                return BadRequest(new { message = "Invalid Id" });

            var user = await BusinessLayer.User.Find(id);

            if (user == null)
                return NotFound(new { message = $"User with id {id} not found!" });


            if (await user.DeleteUser())
                return Ok(new { message = $"Deleted Successfully." });
            else
                return BadRequest(new { message = $"Failed." });
        }


        [HttpPost("Upload-User-Image")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UploadUserImage(int userId, IFormFile imageFile)
        {
            if(userId <= 0)
                return BadRequest("Invalid User ID!");

            var user = await BusinessLayer.User.Find(userId);
            if (user == null)
                return NotFound($"User with id {userId} not found!");

            var result = await ImageUploaderHelper.UploadImageAsync(
                imageFile,
                "uploads/users",
                Request.Host.Value,
                Request.Scheme,
                $"user{userId}"
            );

            if (!result.success)
                return BadRequest(result.error);

            if (await user.UpdateImage(result.url!))
                return Ok(new { result.fileName, result.url });
            else
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to update user image.");
        }





    }

}
