using ApiCommons.GeneralResponse;
using ApiCommons.Result;
using Microsoft.AspNetCore.Mvc;
using Playground.Services;

namespace Playground.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet("GetDescriptionById")]
        public async Task<IActionResult> GetDescriptionById(int id)
        {
            var result = await _categoryService.GetDescriptionById(id);
            Result<string?, Exception> result2 = "selam";

            Response<string> response = result;

            Response<string?> response2 = result2;

            return response.ToActionResult();
        }
    }
}
