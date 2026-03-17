using BusinessLayer;
using DataAccessLayer.Models.FuelTyp;
using DataAccessLayer.Models.Vehicle;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Car_Rental_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FuelTypeAPIController : ControllerBase
    {

        [HttpGet("All")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult<IEnumerable<FuelTypeDTO>>> GetFuelTypes()
        {
            var FuelTypes = await FuelType.GetAllFuelTypes();

            if (FuelTypes.Count == 0)
            {
                return NotFound("No FuelTypes found.");
            }

            return Ok(FuelTypes);
        }

        [HttpGet("{id}",Name = "GetFuelType")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<FuelTypeDTO>> GetFuelType(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid Id");

            var FT = await FuelType.Find(id);

            if (FT == null)
                return NotFound($"FuelType With Id {id} Not Found");

            return Ok(FT.FT_DTO);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]

        public async Task<ActionResult<FuelTypeDTO>> AddNewFuelType(CreateFuelTypeDTO FT_DTO)
        {
            var fuelType = new FuelType(FT_DTO);

            if (!await fuelType.Save())
            {
                return BadRequest("Failed in add new FuelType.");
            }

            return CreatedAtRoute("GetFuelType", new { id = fuelType.Id },fuelType.FT_DTO);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VehicleUpdateDTO>> UpdateFuelType(int id, [FromBody]string TypeName)
        {
            if (id <= 0 ||string.IsNullOrEmpty(TypeName))
                return BadRequest("Invalid"); 

            var FT = await FuelType.Find(id); 

            if (FT == null)
                return NotFound($"FuelType with id {id} not found!");

            FT.Type = TypeName;


            if (!await FT.Save())
                return BadRequest("Failed!");

            return Ok(FT.FT_DTO);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<bool>> DeleteFuelType(int id)
        {
            if (id <= 0)
                return BadRequest(new { message = "Invalid Id"}); 

            var fuelType = await FuelType.Find(id);

            if (fuelType == null)
                return NotFound(new {message= $"fuelType with id {id} not found!"});


            if (await fuelType.DeleteFuelType())
                return Ok(new { message = "Deleted Successfully."});
            else
                return BadRequest(new { message = $"Failed delete fuelType with id {id}" });
        }





    }
}
