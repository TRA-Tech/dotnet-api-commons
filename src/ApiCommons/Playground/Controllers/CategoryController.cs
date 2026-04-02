using System.Security.Claims;
using ApiCommons.Extensions;
using ApiCommons.Pagination;
using ApiCommons.Result;
using Microsoft.AspNetCore.Mvc;
using Playground.Dtos.Category;
using Playground.Services;

namespace Playground.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoryController(ICategoryService categoryService) : ControllerBase
{
    // Demonstrates: SortedPagedRequest + ToPagedAsync + ToActionResultAsync
    // TanStack Table sends: ?pageIndex=0&pageSize=20&sorting[0][id]=categoryName&sorting[0][desc]=false
    [HttpPost("list")]
    public Task<IActionResult> List([FromBody] SortedPagedRequest req, CancellationToken ct = default) =>
        categoryService.GetCategoriesPagedAsync(req, ct).ToActionResultAsync();

    // Demonstrates: Result<T> → automatic 404 mapping via ToActionResult
    [HttpGet("{id:int}")]
    public Task<IActionResult> GetById(int id, CancellationToken ct = default) =>
        categoryService.GetByIdAsync(id, ct).ToActionResultAsync();

    // Demonstrates: fluent async pipeline — BindAsync is not needed here since
    // GetByIdAsync already includes ProductCount; Map is used to project to a
    // trimmed summary shape, showing the pipeline without a second DB call.
    [HttpGet("{id:int}/summary")]
    public Task<IActionResult> GetSummary(int id, CancellationToken ct = default) =>
        categoryService.GetByIdAsync(id, ct)
            .Map(dto => new
            {
                dto.CategoryId,
                dto.CategoryName,
                dto.ProductCount,
                dto.HasPicture
            })
            .ToActionResultAsync();
}
