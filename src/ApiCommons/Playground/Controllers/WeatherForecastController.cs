using ApiCommons.Attributes;
using ApiCommons.GeneralResponse;
using Microsoft.AspNetCore.Mvc;
using Playground.Entities;
using System.Net;

namespace Playground.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : BaseController
    {
        readonly NorthwindDbContext _northwindDbContext;

        public WeatherForecastController(NorthwindDbContext northwindDbContext)
        {
            _northwindDbContext = northwindDbContext;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        [DbTransaction(typeof(NorthwindDbContext))]
        public async Task<IActionResult> Get()
        {
            _northwindDbContext.Categories.Add(new Category
            {
                CategoryName = "Test2"
            });

            await _northwindDbContext.SaveChangesAsync();

            throw new Exception("You like to dance close to the fire don't you?");

            var response = Response<object>.Success(
                HttpStatusCode.OK,
                _northwindDbContext.Categories.OrderByDescending(o => o.CategoryId).ToList(),
                "Success!"
            );

            return CreateResult(response);
        }
    }
}