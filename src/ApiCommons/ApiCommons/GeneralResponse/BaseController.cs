using Microsoft.AspNetCore.Mvc;

namespace ApiCommons.GeneralResponse
{
    public class BaseController : ControllerBase
    {
        [NonAction]
        public IActionResult CreateResult<T>(Response<T> response)
        {
            return new ObjectResult(response)
            {
                StatusCode = (int)response.StatusCode,
                Value = response
            };
        }
    }
}
