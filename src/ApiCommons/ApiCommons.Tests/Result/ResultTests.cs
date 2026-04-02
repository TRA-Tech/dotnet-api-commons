using ApiCommons.Result;
using FluentAssertions;
using Xunit;

namespace ApiCommons.Tests.Result;

public class ResultTests
{
    // ── Implicit conversion & basic properties ────────────────────────────────

    [Fact]
    public void Success_IsSuccess_IsTrue_IsError_IsFalse()
    {
        Result<int, string> result = 42;
        result.IsSuccess.Should().BeTrue();
        result.IsError.Should().BeFalse();
        result.Value.Should().Be(42);
    }

    [Fact]
    public void Failure_IsError_IsTrue_IsSuccess_IsFalse()
    {
        Result<int, Exception> result = new InvalidOperationException("oops");
        result.IsError.Should().BeTrue();
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().BeOfType<InvalidOperationException>();
    }

    // ── Match ─────────────────────────────────────────────────────────────────

    [Fact]
    public void Match_OnSuccess_InvokesSuccessBranch()
    {
        Result<int, string> result = 10;
        var output = result.Match(v => v * 2, _ => -1);
        output.Should().Be(20);
    }

    [Fact]
    public void Match_OnFailure_InvokesFailureBranch()
    {
        Result<int, string> result = "err";
        var output = result.Match(v => v * 2, _ => -1);
        output.Should().Be(-1);
    }

    // ── MatchAsync ────────────────────────────────────────────────────────────

    [Fact]
    public async Task MatchAsync_OnSuccess_InvokesSuccessBranch()
    {
        Result<int, string> result = 5;
        var output = await result.MatchAsync(
            v => ValueTask.FromResult(v + 1),
            _ => ValueTask.FromResult(-99));
        output.Should().Be(6);
    }

    [Fact]
    public async Task MatchAsync_OnFailure_InvokesFailureBranch()
    {
        Result<int, string> result = "fail";
        var output = await result.MatchAsync(
            v => ValueTask.FromResult(v + 1),
            _ => ValueTask.FromResult(-99));
        output.Should().Be(-99);
    }

    // ── Handle ────────────────────────────────────────────────────────────────

    [Fact]
    public void Handle_OnSuccess_InvokesSuccessAction_NotFailureAction()
    {
        Result<int, string> result = 7;
        var successCalled = false;
        var failureCalled = false;
        result.Handle(
            _ => successCalled = true,
            _ => failureCalled = true);
        successCalled.Should().BeTrue();
        failureCalled.Should().BeFalse();
    }

    [Fact]
    public void Handle_OnFailure_InvokesFailureAction_NotSuccessAction()
    {
        Result<int, string> result = "err";
        var successCalled = false;
        var failureCalled = false;
        result.Handle(
            _ => successCalled = true,
            _ => failureCalled = true);
        successCalled.Should().BeFalse();
        failureCalled.Should().BeTrue();
    }

    // ── HandleAsync ───────────────────────────────────────────────────────────

    [Fact]
    public async Task HandleAsync_OnSuccess_InvokesSuccessAction()
    {
        Result<int, string> result = 3;
        var successCalled = false;
        await result.HandleAsync(
            _ => { successCalled = true; return ValueTask.CompletedTask; },
            _ => ValueTask.CompletedTask);
        successCalled.Should().BeTrue();
    }

    [Fact]
    public async Task HandleAsync_OnFailure_InvokesFailureAction()
    {
        Result<int, string> result = "err";
        var failureCalled = false;
        await result.HandleAsync(
            _ => ValueTask.CompletedTask,
            _ => { failureCalled = true; return ValueTask.CompletedTask; });
        failureCalled.Should().BeTrue();
    }

    // ── Map ───────────────────────────────────────────────────────────────────

    [Fact]
    public void Map_OnSuccess_TransformsValue()
    {
        Result<int, string> result = 10;
        var mapped = result.Map(v => v.ToString());
        mapped.IsSuccess.Should().BeTrue();
        mapped.Value.Should().Be("10");
    }

    [Fact]
    public void Map_OnFailure_PropagatesError_MapperNotCalled()
    {
        Result<int, string> result = "err";
        var mapperCalled = false;
        var mapped = result.Map(v => { mapperCalled = true; return v.ToString(); });
        mapperCalled.Should().BeFalse();
        mapped.IsError.Should().BeTrue();
        mapped.Error.Should().Be("err");
    }

    // ── Tap ───────────────────────────────────────────────────────────────────

    [Fact]
    public void Tap_OnSuccess_InvokesAction_ReturnsOriginal()
    {
        Result<int, string> result = 99;
        var tapped = 0;
        var returned = result.Tap(v => tapped = v);
        tapped.Should().Be(99);
        returned.IsSuccess.Should().BeTrue();
        returned.Value.Should().Be(99);
    }

    [Fact]
    public void Tap_OnFailure_DoesNotInvokeAction_ReturnsOriginal()
    {
        Result<int, string> result = "err";
        var actionCalled = false;
        var returned = result.Tap(_ => actionCalled = true);
        actionCalled.Should().BeFalse();
        returned.IsError.Should().BeTrue();
        returned.Error.Should().Be("err");
    }

    // ── Bind ──────────────────────────────────────────────────────────────────

    [Fact]
    public void Bind_OnSuccess_ChainsOperation()
    {
        Result<int, string> result = 5;
        Result<double, string> Binder(int v) => (double)v * 1.5;
        var bound = result.Bind(Binder);
        bound.IsSuccess.Should().BeTrue();
        bound.Value.Should().Be(7.5);
    }

    [Fact]
    public void Bind_OnFailure_ShortCircuits_BinderNotCalled()
    {
        Result<int, string> result = "err";
        var binderCalled = false;
        Result<double, string> Binder(int v) { binderCalled = true; return (double)v; }
        var bound = result.Bind(Binder);
        binderCalled.Should().BeFalse();
        bound.IsError.Should().BeTrue();
        bound.Error.Should().Be("err");
    }

    [Fact]
    public void Bind_WhenBinderReturnsFailure_PropagatesBinderError()
    {
        Result<int, string> result = 5;
        Result<double, string> Binder(int _) => "binder error";
        var bound = result.Bind(Binder);
        bound.IsError.Should().BeTrue();
        bound.Error.Should().Be("binder error");
    }

    // ── GetValueOrDefault ─────────────────────────────────────────────────────

    [Fact]
    public void GetValueOrDefault_OnSuccess_ReturnsValue()
    {
        Result<int, string> result = 42;
        result.GetValueOrDefault(0).Should().Be(42);
    }

    [Fact]
    public void GetValueOrDefault_OnFailure_ReturnsFallback()
    {
        Result<int, string> result = "err";
        result.GetValueOrDefault(-1).Should().Be(-1);
    }

    // ── Result<TValue> shorthand ──────────────────────────────────────────────

    [Fact]
    public void ResultShorthand_Success_WidensToGenericForm()
    {
        Result<int> shorthand = 42;
        Result<int, ApiCommons.Result.Error> widened = shorthand;
        widened.IsSuccess.Should().BeTrue();
        widened.Value.Should().Be(42);
    }

    [Fact]
    public void ResultShorthand_Failure_WidensToGenericForm()
    {
        Result<int> shorthand = new NotFoundError("Resource");
        Result<int, ApiCommons.Result.Error> widened = shorthand;
        widened.IsError.Should().BeTrue();
        widened.Error.Should().BeOfType<NotFoundError>();
    }
}
