using ApiCommons.Extensions;
using ApiCommons.Pagination;
using ApiCommons.Result;
using Microsoft.EntityFrameworkCore;
using Playground.Dtos.Category;
using Playground.Entities;

namespace Playground.Services;

public interface ICategoryService
{
    Task<Result<PagedResult<CategoryListItemDto>>> GetCategoriesPagedAsync(
        SortedPagedRequest request, CancellationToken ct = default);

    Task<Result<CategoryDetailDto>> GetByIdAsync(int id, CancellationToken ct = default);
}

public class CategoryService(NorthwindDbContext db) : ICategoryService
{
    public async Task<Result<PagedResult<CategoryListItemDto>>> GetCategoriesPagedAsync(
        SortedPagedRequest request, CancellationToken ct = default)
    {
        var query = db.Categories
            .AsNoTracking()
            .Select(c => new CategoryListItemDto
            {
                CategoryName = c.CategoryName,
                Description  = c.Description
            });

        return await query.ToPagedAsync(request, ct);
    }

    public async Task<Result<CategoryDetailDto>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var category = await db.Categories
            .AsNoTracking()
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.CategoryId == id, ct);

        if (category is null)
            return new NotFoundError("Category");

        return new CategoryDetailDto
        {
            CategoryId   = category.CategoryId,
            CategoryName = category.CategoryName,
            Description  = category.Description,
            HasPicture   = category.Picture is { Length: > 0 },
            ProductCount = category.Products.Count
        };
    }
}
