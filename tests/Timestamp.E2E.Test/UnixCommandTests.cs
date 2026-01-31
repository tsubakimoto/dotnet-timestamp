namespace Tsubakimoto.Tools.Timestamp.E2E.Test;

/// <summary>
/// E2E tests for the 'unix' command.
/// </summary>
public sealed class UnixCommandTests
{
    [Fact]
    public async Task Unix_WithoutOptions_DisplaysCurrentUnixTimestamp()
    {
        var beforeRun = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var result = await TimestampCliRunner.RunAsync("unix");
        var afterRun = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        Assert.False(result.TimedOut, "Command should not timeout");
        Assert.Equal(0, result.ExitCode);
        Assert.False(string.IsNullOrWhiteSpace(result.StandardOutput), "Output should contain timestamp");

        var output = result.StandardOutput.Trim();
        Assert.True(long.TryParse(output, out var unixTimestamp),
            $"Output should be a valid Unix timestamp, but was: {output}");

        Assert.InRange(unixTimestamp, beforeRun, afterRun);
    }

    [Fact]
    public async Task Unix_WithDatetimeOption_DisplaysUnixTimestamp()
    {
        var result = await TimestampCliRunner.RunAsync("unix --datetime 1970-01-01T00:00:00Z");

        Assert.False(result.TimedOut);
        Assert.Equal(0, result.ExitCode);

        var output = result.StandardOutput.Trim();
        Assert.Equal("0", output);
    }

    [Fact]
    public async Task Unix_WithShortDatetimeOption_DisplaysUnixTimestamp()
    {
        var result = await TimestampCliRunner.RunAsync("unix -t 2000-01-01T00:00:00Z");

        Assert.False(result.TimedOut);
        Assert.Equal(0, result.ExitCode);

        var output = result.StandardOutput.Trim();
        Assert.Equal("946684800", output);
    }

    [Fact]
    public async Task Unix_WithInvalidDatetime_DisplaysError()
    {
        var result = await TimestampCliRunner.RunAsync("unix --datetime invalid-datetime");

        Assert.False(result.TimedOut);
        Assert.Contains("Error:", result.StandardError);
    }

    [Fact]
    public async Task Unix_WithDatetimeWithOffset_DisplaysCorrectUnixTimestamp()
    {
        // 2024-01-01T00:00:00+09:00 (JST) is 2023-12-31T15:00:00Z (UTC)
        var result = await TimestampCliRunner.RunAsync("unix --datetime 2024-01-01T00:00:00+09:00");

        Assert.False(result.TimedOut);
        Assert.Equal(0, result.ExitCode);

        var output = result.StandardOutput.Trim();
        var expected = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.FromHours(9)).ToUnixTimeSeconds();
        Assert.Equal(expected.ToString(), output);
    }
}
