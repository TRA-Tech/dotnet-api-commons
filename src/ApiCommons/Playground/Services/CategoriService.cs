using ApiCommons.Result;
using Playground.Entities;

namespace Playground.Services
{
    public interface ICategoryService
    {
        Task<Result<string, Exception>> GetDescriptionById(int id);
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
    }
}
