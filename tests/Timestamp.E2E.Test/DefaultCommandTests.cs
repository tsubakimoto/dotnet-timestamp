namespace Tsubakimoto.Tools.Timestamp.E2E.Test;

/// <summary>
/// E2E tests for the default command (equivalent to 'now' command).
/// </summary>
public sealed class DefaultCommandTests
{
    [Fact]
    public async Task NoCommand_DisplaysCurrentTimestamp_InDefaultFormat()
    {
        var result = await TimestampCliRunner.RunAsync("");

        Assert.False(result.TimedOut, "Command should not timeout");
        Assert.Equal(0, result.ExitCode);
        Assert.False(string.IsNullOrWhiteSpace(result.StandardOutput), "Output should contain timestamp");
        
        // Verify the output is a valid ISO 8601 format timestamp (default format is 'o')
        var output = result.StandardOutput.Trim();
        Assert.True(DateTimeOffset.TryParse(output, out _), 
            $"Output should be a valid timestamp, but was: {output}");
    }

    [Fact]
    public async Task NoCommand_WithFormatOption_DisplaysFormattedTimestamp()
    {
        var result = await TimestampCliRunner.RunAsync("--format yyyy-MM-dd");

        Assert.False(result.TimedOut);
        Assert.Equal(0, result.ExitCode);
        
        var output = result.StandardOutput.Trim();
        // Verify the output matches the expected format (e.g., "2026-01-12")
        Assert.Matches(@"^\d{4}-\d{2}-\d{2}$", output);
    }
}
