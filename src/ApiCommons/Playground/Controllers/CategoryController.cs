using ApiCommons.GeneralResponse;
using ApiCommons.Pagination;
using Microsoft.AspNetCore.Mvc;
using Playground.Dtos.Category;
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

        [HttpGet("List")]
        public async Task<IActionResult> List([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
        {
            var req = new PagedRequest(page, pageSize);
            var result = await _categoryService.GetCategoriesPagedAsync(req, ct);

            Response<PagedResult<CategoryListItemDto>> response = result.Match<Response<PagedResult<CategoryListItemDto>>>(
                s => s,
                e => e
            );

            return response.ToActionResult();
        }
    }
}
