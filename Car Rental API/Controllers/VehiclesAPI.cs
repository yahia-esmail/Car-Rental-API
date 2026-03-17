using DataAccessLayer.Models.Vehicle;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BusinessLayer;
using static DataAccessLayer.Helper;

namespace Car_Rental_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleController : ControllerBase
    {
        [HttpGet("All")] 
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<IEnumerable<VehicleReadDTO>>> GetVehicles()
        {
            var vehicles = await Vehicle.GetAllVehicles();

            if(vehicles.Count == 0)
            {
                return NotFound("No vehicles found.");
            }

            return Ok(vehicles);
        }
        
        [HttpGet("Count")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<int>> CountVehicles()
        {
            try
            {
                return Ok(await Vehicle.CountVehicles());
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message.ToString());
            }
        }
        
        [HttpGet("{id}",Name = "GetVehicleByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<ActionResult<VehicleReadDTO>> GetVehicleByID(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid ID!");

            var vehicle =await Vehicle.Find(id);

            if(vehicle == null)
            {
                return NotFound("No Vehicle Founded!");
            }

            return Ok(await vehicle.ToReadDTO());
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<VehicleUpdateDTO>> AddNewVehicle([FromForm]VehicleCreateFromUserDTO VDTO)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            if (VDTO.VehicleImage == null || VDTO.VehicleImage.Length == 0)
                return BadRequest("Vehicle image is required.");

            var vehicle = new Vehicle(VDTO);
            
            if(!await vehicle.Save())
            {
                return BadRequest("Failed in add new vehicle.");
            }

            // handle vehicle image
            var result = await ImageUploaderHelper.UploadImageAsync(
                VDTO.VehicleImage,
                "uploads/vehicles",
                Request.Host.Value,
                Request.Scheme,
                $"vehicle{vehicle.VehicleID}"
            );

            if (!result.success)
                return BadRequest(result.error);

            if (await vehicle.UpdateImage(result.url!))
                return CreatedAtRoute("GetVehicleByID", new { id = vehicle.VehicleID }, await vehicle.ToReadDTO());
            else
                return StatusCode(StatusCodes.Status500InternalServerError, $"Vehicle added with id [{vehicle.VehicleID}], but failed to upload image.");
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VehicleReadDTO>> UpdateVehicle(int id ,VehicleUpdateDTO VDTO)
        {
            if(VDTO == null || VDTO.FuelTypeID<= 0 ||VDTO.VehicleCategoryID <= 0)
                return BadRequest("Invalid");

            if(id <= 0)
                return BadRequest(new {message = $"id [{id}] is not valid."});

            var vehicle = await Vehicle.Find(id);

            if (vehicle == null)
                return NotFound($"Vehicle with id {id} not found!");

            VDTO.Id = id;
            VDTO.ImagePath = vehicle.ImagePath;
            vehicle = new Vehicle(VDTO);
            
            if (!await vehicle.Save())
                return BadRequest("Failed!");

            return Ok(await vehicle.ToReadDTO());
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteVehicle(int id)
        {
            if (id <= 0)
                return BadRequest(new { message = "Invalid Id" });

            var vehicle = await Vehicle.Find(id);

            if (vehicle == null)
                return NotFound(new { message = $"Vehicle with id {id} not found!" });

            if (await vehicle.DeleteVehicle())
                return Ok(new { message = "Deleted Successfully." });
            else
                return BadRequest(new { message = $"Failed to delete vehicle with id {id}" });
        }
        

        [HttpPost("Upload-Vehicle-Image")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UploadVehicleImage(int vehicleId, IFormFile imageFile)
        {
            if (vehicleId <= 0) 
                return BadRequest("Invalid Vehicle ID!");

            var vehicle = await BusinessLayer.Vehicle.Find(vehicleId);
            if (vehicle == null)
                return NotFound($"Vehicle with id {vehicleId} not found!");

            var result = await ImageUploaderHelper.UploadImageAsync(
                imageFile,
                "uploads/vehicles",
                Request.Host.Value,
                Request.Scheme,
                $"vehicle{vehicleId}"
            );

            if (!result.success)
                return BadRequest(result.error);

            if (await vehicle.UpdateImage(result.url!))
                return Ok(new { result.fileName, result.url });
            else
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to update vehicle image.");
        }

    }
}
