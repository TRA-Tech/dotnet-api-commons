using ApiCommons.Extensions;
using ApiCommons.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Playground.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UtilityController(IConfiguration configuration) : ControllerBase
{
    // Demonstrates: BrotliHelper.Compress + Decompress round-trip smoke test
    [HttpGet("compress/roundtrip")]
    public IActionResult CompressRoundTrip([FromQuery] string value = "Hello from ApiCommons v2!")
    {
        var compressed = BrotliHelper.Compress(value);
        var decompressed = BrotliHelper.Decompress(compressed);
        var matches = decompressed == value;
        return Ok(new { Original = value, Compressed = compressed, Decompressed = decompressed, Matches = matches });
    }

    // Demonstrates: ConfigurationExtensions.GetRequired
    // Returns 500 ProblemDetails (via exception handler) if "Demo:RequiredKey" is missing.
    // Add the following to appsettings.json to see the success path:
    //   "Demo": { "RequiredKey": "my-value" }
    [HttpGet("config-check")]
    public IActionResult ConfigCheck(string? key)
    {
        key = key ?? "Demo:RequiredKey";
        var value = configuration.GetRequired(key);
        return Ok(new { Key = key, Value = value });
    }


    // Demonstrates: ClaimsPrincipalExtensions.GetId<int>()
    // In a real app this route would be [Authorize]; the UserId will be 0 (default)
    // when no NameIdentifier claim is present.
    [HttpGet("me")]
    public IActionResult GetCurrentUser()
    {
        var userId = User.GetRequiredId<int>();
        var email = User.GetEmail();
        return Ok(new { UserId = userId, Email = email });
    }
}

