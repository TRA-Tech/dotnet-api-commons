using ApiCommons.Extensions;
using ApiCommons.Pagination;
using ApiCommons.Result;
using Microsoft.EntityFrameworkCore;
using Playground.Dtos.Category;
using Playground.Entities;

namespace Playground.Services
{
    public interface ICategoryService
    {
        Task<Result<string, Exception>> GetDescriptionById(int id);
        Task<Result<PagedResult<CategoryListItemDto>, Exception>> GetCategoriesPagedAsync(PagedRequest request, CancellationToken ct = default);
    }

    public class CategoryService : ICategoryService
    {
        private readonly NorthwindDbContext _northwindDbContext;

        public CategoryService(NorthwindDbContext northwindDbContext)
        {
            _northwindDbContext = northwindDbContext;
        }

        public async Task<Result<string, Exception>> GetDescriptionById(int id)
        {
            if (id == 0) return new ArgumentException("can not be zero", nameof(id));
            var category = await _northwindDbContext.Categories.FindAsync(id);
            if (category == null) return new Exception($"there is no category with id '{id}'");
            return category.Description ?? string.Empty;
        }

        public async Task<Result<PagedResult<CategoryListItemDto>, Exception>> GetCategoriesPagedAsync(PagedRequest request, CancellationToken ct = default)
        {
            var query = _northwindDbContext.Categories
                .AsNoTracking()
                .OrderBy(c => c.CategoryId)
                .Select(s => new CategoryListItemDto()
                {
                    CategoryName = s.CategoryName,
                    Description = s.Description,
                });

            var page = await query.ToPagedAsync(request, ct);

            return page;
        }
    }
}
