using ApiCommons.Result;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace ApiCommons.Tests.Result;

public class ResultExtensionsTests
{
    // ── ToActionResult — sync ─────────────────────────────────────────────────

    [Fact]
    public void ToActionResult_OnSuccess_Returns200WithValue()
    {
        Result<int> result = 42;
        var actionResult = result.ToActionResult();
        var ok = actionResult.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().Be(42);
    }

    [Fact]
    public void ToActionResult_WithCustomStatusCode_ReturnsCustomStatus()
    {
        Result<string> result = "created";
        var actionResult = result.ToActionResult(201);
        var objResult = actionResult.Should().BeOfType<ObjectResult>().Subject;
        objResult.StatusCode.Should().Be(201);
        objResult.Value.Should().Be("created");
    }

    [Fact]
    public void ToActionResult_Unit_OnSuccess_Returns204NoContent()
    {
        Result<Unit> result = Unit.Value;
        var actionResult = result.ToActionResult();
        actionResult.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public void ToActionResult_Unit_OnFailure_ReturnsProblemDetails()
    {
        Result<Unit> result = new NotFoundError("Resource");
        var actionResult = result.ToActionResult();
        var objResult = actionResult.Should().BeOfType<ObjectResult>().Subject;
        objResult.StatusCode.Should().Be(404);
    }

    // ── BindAsync ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task BindAsync_OnSuccess_InvokesBinder_ReturnsBoundResult()
    {
        Task<Result<int>> task = Task.FromResult<Result<int>>(5);
        var result = await task.BindAsync(v => Task.FromResult<Result<string>>(v.ToString()));
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("5");
    }

    [Fact]
    public async Task BindAsync_OnFailure_ShortCircuits_BinderNotCalled()
    {
        var binderCalled = false;
        Task<Result<int>> task = Task.FromResult<Result<int>>(new NotFoundError("Resource"));
        var result = await task.BindAsync(v =>
        {
            binderCalled = true;
            return Task.FromResult<Result<string>>(v.ToString());
        });
        binderCalled.Should().BeFalse();
        result.IsError.Should().BeTrue();
        result.Error.Should().BeOfType<NotFoundError>();
    }

    [Fact]
    public async Task BindAsync_WhenBinderReturnsFailure_PropagatesBinderError()
    {
        Task<Result<int>> task = Task.FromResult<Result<int>>(5);
        var result = await task.BindAsync(_ => Task.FromResult<Result<string>>(new ConflictError("conflict")));
        result.IsError.Should().BeTrue();
        result.Error.Should().BeOfType<ConflictError>();
    }

    // ── Map (Task overload) ───────────────────────────────────────────────────

    [Fact]
    public async Task Map_TaskOverload_OnSuccess_TransformsValue()
    {
        Task<Result<int>> task = Task.FromResult<Result<int>>(3);
        var result = await task.Map(v => v * 2);
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(6);
    }

    [Fact]
    public async Task Map_TaskOverload_OnFailure_PropagatesError()
    {
        Task<Result<int>> task = Task.FromResult<Result<int>>(new UnauthorizedError("Token expired"));
        var result = await task.Map(v => v * 2);
        result.IsError.Should().BeTrue();
        result.Error.Should().BeOfType<UnauthorizedError>();
    }

    // ── Tap (Task overload) ───────────────────────────────────────────────────

    [Fact]
    public async Task Tap_TaskOverload_OnSuccess_InvokesAction_ReturnsOriginal()
    {
        var tapped = 0;
        Task<Result<int>> task = Task.FromResult<Result<int>>(7);
        var result = await task.Tap(v => tapped = v);
        tapped.Should().Be(7);
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(7);
    }

    [Fact]
    public async Task Tap_TaskOverload_OnFailure_DoesNotInvokeAction()
    {
        var actionCalled = false;
        Task<Result<int>> task = Task.FromResult<Result<int>>(new ForbiddenError("Order", "Delete"));
        var result = await task.Tap(_ => actionCalled = true);
        actionCalled.Should().BeFalse();
        result.IsError.Should().BeTrue();
    }

    // ── ToActionResultAsync ───────────────────────────────────────────────────

    [Fact]
    public async Task ToActionResultAsync_OnSuccess_Returns200WithValue()
    {
        Task<Result<int>> task = Task.FromResult<Result<int>>(42);
        var actionResult = await task.ToActionResultAsync();
        var ok = actionResult.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().Be(42);
    }

    [Fact]
    public async Task ToActionResultAsync_OnFailure_ReturnsProblemDetails()
    {
        Task<Result<int>> task = Task.FromResult<Result<int>>(new NotFoundError("Resource"));
        var actionResult = await task.ToActionResultAsync();
        var objResult = actionResult.Should().BeOfType<ObjectResult>().Subject;
        objResult.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task ToActionResultAsync_WithStatusCode_OnSuccess_ReturnsCustomStatus()
    {
        Task<Result<string>> task = Task.FromResult<Result<string>>("ok");
        var actionResult = await task.ToActionResultAsync(201);
        var objResult = actionResult.Should().BeOfType<ObjectResult>().Subject;
        objResult.StatusCode.Should().Be(201);
    }

    [Fact]
    public async Task ToActionResultAsync_Unit_OnSuccess_Returns204()
    {
        Task<Result<Unit>> task = Task.FromResult<Result<Unit>>(Unit.Value);
        var actionResult = await task.ToActionResultAsync();
        actionResult.Should().BeOfType<NoContentResult>();
    }

}
