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

            Response<string> response = result.Match(
                s => s,
                e => string.Empty
            );

            await result.MatchAsync(
                s => ValueTask.FromResult(s),
                e => ValueTask.FromResult(string.Empty));

            return response.ToActionResult();
        }
    }
}
