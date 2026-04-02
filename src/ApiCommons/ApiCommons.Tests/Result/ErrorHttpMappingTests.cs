using ApiCommons.Result;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace ApiCommons.Tests.Result;

/// <summary>
/// Verifies that each built-in Error type exposes the correct StatusCode, Detail, and Code,
/// and that <see cref="ResultExtensions.ToActionResult{T}"/> maps them to the expected ProblemDetails.
/// Also verifies that consumer-defined errors work without any library changes.
/// </summary>
public class ErrorHttpMappingTests
{
    private static (int status, string? detail, string? code) GetProblemDetails<T>(Result<T> result)
    {
        var actionResult = result.ToActionResult();
        var objResult = actionResult.Should().BeOfType<ObjectResult>().Subject;
        var pd = objResult.Value.Should().BeOfType<ProblemDetails>().Subject;
        var code = pd.Extensions.TryGetValue("code", out var c) ? c as string : null;
        return (pd.Status!.Value, pd.Detail, code);
    }

    [Fact]
    public void NotFoundError_MapsTo_404_WithDetailAndCode()
    {
        Result<int> result = new NotFoundError("Product");
        var (status, detail, code) = GetProblemDetails(result);
        status.Should().Be(404);
        detail.Should().Be("Product was not found.");
        code.Should().Be("PRODUCT_NOT_FOUND");
    }

    [Fact]
    public void AlreadyExistsError_MapsTo_409_WithDetailAndCode()
    {
        Result<int> result = new AlreadyExistsError("Email");
        var (status, detail, code) = GetProblemDetails(result);
        status.Should().Be(409);
        detail.Should().Be("Email already exists.");
        code.Should().Be("EMAIL_ALREADY_EXISTS");
    }

    [Fact]
    public void ConflictError_MapsTo_409_WithDetailAndCode()
    {
        Result<int> result = new ConflictError("Version mismatch");
        var (status, detail, code) = GetProblemDetails(result);
        status.Should().Be(409);
        detail.Should().Be("Version mismatch");
        code.Should().Be("CONFLICT");
    }

    [Fact]
    public void UnauthorizedError_MapsTo_401_WithDetailAndCode()
    {
        Result<int> result = new UnauthorizedError("Token expired");
        var (status, detail, code) = GetProblemDetails(result);
        status.Should().Be(401);
        detail.Should().Be("Token expired");
        code.Should().Be("UNAUTHORIZED");
    }

    [Fact]
    public void ForbiddenError_MapsTo_403_WithDetailAndCode()
    {
        Result<int> result = new ForbiddenError("Order", "Delete");
        var (status, detail, code) = GetProblemDetails(result);
        status.Should().Be(403);
        detail.Should().Be("Delete on Order is forbidden.");
        code.Should().Be("ORDER_DELETE_FORBIDDEN");
    }

    [Fact]
    public void CustomError_WithNoOverrides_MapsTo_500_WithNoDetailOrCode()
    {
        Result<int> result = new UnknownDomainError();
        var (status, detail, code) = GetProblemDetails(result);
        status.Should().Be(500);
        detail.Should().BeNull();
        code.Should().BeNull();
    }

    [Fact]
    public void CustomError_WithOverrides_MapsCorrectly()
    {
        Result<int> result = new PaymentFailedError("Insufficient funds");
        var (status, detail, code) = GetProblemDetails(result);
        status.Should().Be(402);
        detail.Should().Be("Insufficient funds");
        code.Should().Be("PAYMENT_FAILED");
    }

    [Fact]
    public void ToActionResult_OnSuccess_DoesNotProduceProblemDetails()
    {
        Result<int> result = 42;
        result.ToActionResult().Should().BeOfType<OkObjectResult>();
    }

    // Simulates a consumer error with no overrides → falls back to 500, no detail, no code.
    private sealed record UnknownDomainError : Error;

    // Simulates a consumer error with full overrides — no library changes needed.
    private sealed record PaymentFailedError(string Reason) : Error
    {
        public override int StatusCode => StatusCodes.Status402PaymentRequired;
        public override string? Detail => Reason;
        public override string? Code => "PAYMENT_FAILED";
    }
}
