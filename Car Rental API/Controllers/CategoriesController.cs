using BusinessLayer;
using DataAccessLayer.Models;
using DataAccessLayer.Models.FuelTyp;
using DataAccessLayer.Models.Vehicle;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static DataAccessLayer.Helper;

namespace Car_Rental_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        [HttpGet("All")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult<IEnumerable<CreateCategoryDTO>>> GetCategories()
        {
            var Categories = await Category.GetAllCategories();

            if (Categories.Count() == 0)
            {
                return NotFound("No Categories found.");
            }

            return Ok(Categories);
        }
        
        [HttpGet("{categoryId}/Vehicles")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult<IEnumerable<VehicleReadDTO>>> GetVehicles(int categoryId)
        {
            var vehicles = await Category.GetAllVehicles(categoryId);

            if (!vehicles.Any())
            {
                return NotFound(new { message = "No vehicles found with this category." });
            }

            return Ok(vehicles);
        }

        [HttpGet("{id}", Name = "GetCategory")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CategoryDTO>> GetCategory(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid Id");

            var Category = await BusinessLayer.Category.Find(id);

            if (Category == null)
                return NotFound($"Category With Id {id} Not Found");

            return Ok(Category.CategoryDTO);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CategoryDTO>> AddNewCategory([FromForm]CreateCategoryDTO dto)
        {

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                               .Select(e => e.ErrorMessage);
                return BadRequest(errors);
            }

            var Category = new Category(dto.Name);

            if (!await Category.Save())
            {
                return BadRequest("Failed in add new Category.");
            }

            // handle category image
            var result = await ImageUploaderHelper.UploadImageAsync(
                dto.CategoryImage,
                "uploads/categories",
                Request.Host.Value,
                Request.Scheme,
                $"category{Category.ID}"
            );

            if (!result.success)
                return BadRequest(result.error);

            if (await Category.UpdateImage(result.url!))
                return CreatedAtRoute("GetCategory", new { id = Category.ID }, Category.CategoryDTO);
            else
                return StatusCode(StatusCodes.Status500InternalServerError, $"category added with id [{Category.ID}], but failed to upload image.");
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CategoryDTO>> UpdateCategory(int id, UpdateCategoryDTO dto)
        {
            if (id <= 0)
                return BadRequest("Invalid ID.");

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                               .Select(e => e.ErrorMessage);
                return BadRequest(errors);
            }

            var category = await Category.Find(id);

            if (category == null)
                return NotFound($"category with id {id} not found!");

            category.Name = dto.Name;

            if (!await category.Save())
                return BadRequest("Failed!");

            return Ok(category.CategoryDTO);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<bool>> DeleteCategory(int id)
        {
            if (id <= 0)
                return BadRequest(new { message = "Invalid Id" });

            var category = await Category.Find(id);

            if (category == null)
                return NotFound(new { message =  $"category with id {id} not found!" } );


            if (await category.Delete())
                return Ok(new { message = "Deleted Successfully."  });
            else
                return BadRequest(new { message = $"Failed delete category with id {id},may be there are vehicles related with it."  });
        }

        [HttpPost("Upload-Category-Image")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UploadCategoryImage(int categoryId, IFormFile imageFile)
        {
            if (categoryId <= 0)
                return BadRequest("Invalid Category ID!");

            var Category = await BusinessLayer.Category.Find(categoryId);

            if (Category == null)
                return NotFound($"Category with id {categoryId} not found!");

            // handle category image
            var result = await ImageUploaderHelper.UploadImageAsync(
                imageFile,
                "uploads/categories",
                Request.Host.Value,
                Request.Scheme,
                $"category{Category.ID}"
            );

            if (!result.success)
                return BadRequest(result.error);

            if (await Category.UpdateImage(result.url!))
                return Ok(new { result.fileName, result.url });
            else
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to update Category image.");
        }


    }
}
