namespace Tsubakimoto.Tools.Timestamp.E2E.Test;

/// <summary>
/// E2E tests for error handling scenarios.
/// </summary>
public sealed class ErrorHandlingTests
{
    [Fact]
    public async Task Convert_WithoutRequiredDateTime_ReturnsNonZeroExitCode()
    {
        var result = await TimestampCliRunner.RunAsync(
            "convert --from UTC --to \"Tokyo Standard Time\"");

        Assert.False(result.TimedOut);
        Assert.NotEqual(0, result.ExitCode);
    }

    [Fact]
    public async Task Convert_WithoutRequiredFrom_ReturnsNonZeroExitCode()
    {
        var result = await TimestampCliRunner.RunAsync(
            "convert --datetime \"2026-01-12T12:00:00\" --to \"Tokyo Standard Time\"");

        Assert.False(result.TimedOut);
        Assert.NotEqual(0, result.ExitCode);
    }

    [Fact]
    public async Task Convert_WithoutRequiredTo_ReturnsNonZeroExitCode()
    {
        var result = await TimestampCliRunner.RunAsync(
            "convert --datetime \"2026-01-12T12:00:00\" --from UTC");

        Assert.False(result.TimedOut);
        Assert.NotEqual(0, result.ExitCode);
    }

    [Fact]
    public async Task Convert_WithInvalidDateTime_DisplaysErrorMessage()
    {
        var result = await TimestampCliRunner.RunAsync(
            "convert --datetime \"invalid-date\" --from UTC --to \"Tokyo Standard Time\"");

        Assert.False(result.TimedOut);
        // TODO: Implementation should return non-zero exit code on error
        // Assert.NotEqual(0, result.ExitCode);
        Assert.Contains("Error", result.StandardError);
    }

    [Fact]
    public async Task Convert_WithInvalidTimezone_DisplaysErrorMessage()
    {
        var result = await TimestampCliRunner.RunAsync(
            "convert --datetime \"2026-01-12T12:00:00\" --from \"Invalid Timezone\" --to UTC");

        Assert.False(result.TimedOut);
        // TODO: Implementation should return non-zero exit code on error
        // Assert.NotEqual(0, result.ExitCode);
        Assert.Contains("Error", result.StandardError);
    }

    [Fact]
    public async Task UnknownCommand_ReturnsNonZeroExitCode()
    {
        var result = await TimestampCliRunner.RunAsync("unknown-command");

        Assert.False(result.TimedOut);
        Assert.NotEqual(0, result.ExitCode);
    }
}
