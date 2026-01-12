namespace Tsubakimoto.Tools.Timestamp.E2E.Test;

/// <summary>
/// E2E tests for the 'now' command.
/// </summary>
public sealed class NowCommandTests
{
    [Fact]
    public async Task Now_WithoutOptions_DisplaysCurrentTimestamp()
    {
        var result = await TimestampCliRunner.RunAsync("now");

        Assert.False(result.TimedOut, "Command should not timeout");
        Assert.Equal(0, result.ExitCode);
        Assert.False(string.IsNullOrWhiteSpace(result.StandardOutput), "Output should contain timestamp");
        
        var output = result.StandardOutput.Trim();
        Assert.True(DateTimeOffset.TryParse(output, out _), 
            $"Output should be a valid timestamp, but was: {output}");
    }

    [Fact]
    public async Task Now_WithCustomFormat_DisplaysFormattedTimestamp()
    {
        var result = await TimestampCliRunner.RunAsync("now --format yyyy-MM-dd");

        Assert.False(result.TimedOut);
        Assert.Equal(0, result.ExitCode);
        
        var output = result.StandardOutput.Trim();
        // Verify the output matches the expected format
        Assert.Matches(@"^\d{4}-\d{2}-\d{2}$", output);
    }

    [Fact]
    public async Task Now_WithShortFormatOption_DisplaysFormattedTimestamp()
    {
        var result = await TimestampCliRunner.RunAsync("now -m yyyy/MM/dd");

        Assert.False(result.TimedOut);
        Assert.Equal(0, result.ExitCode);
        
        var output = result.StandardOutput.Trim();
        Assert.Matches(@"^\d{4}/\d{2}/\d{2}$", output);
    }

    [Fact]
    public async Task Now_WithTimezoneOption_DisplaysTimestampInSpecifiedTimezone()
    {
        var result = await TimestampCliRunner.RunAsync("now --timezone \"Tokyo Standard Time\"");

        Assert.False(result.TimedOut);
        Assert.Equal(0, result.ExitCode);
        Assert.False(string.IsNullOrWhiteSpace(result.StandardOutput), "Output should contain timestamp");
        
        var output = result.StandardOutput.Trim();
        Assert.True(DateTimeOffset.TryParse(output, out var timestamp), 
            $"Output should be a valid timestamp, but was: {output}");
        
        // Verify the offset is +09:00 for Tokyo Standard Time
        Assert.Equal(TimeSpan.FromHours(9), timestamp.Offset);
    }

    [Fact]
    public async Task Now_WithShortTimezoneOption_DisplaysTimestampInSpecifiedTimezone()
    {
        var result = await TimestampCliRunner.RunAsync("now -z \"Eastern Standard Time\"");

        Assert.False(result.TimedOut);
        Assert.Equal(0, result.ExitCode);
        Assert.False(string.IsNullOrWhiteSpace(result.StandardOutput), "Output should contain timestamp");
    }

    [Fact]
    public async Task Now_WithBothFormatAndTimezone_DisplaysFormattedTimestampInTimezone()
    {
        var result = await TimestampCliRunner.RunAsync("now --format \"HH:mm\" --timezone \"UTC\"");

        Assert.False(result.TimedOut);
        Assert.Equal(0, result.ExitCode);
        
        var output = result.StandardOutput.Trim();
        // Verify the output matches the time-only format
        Assert.Matches(@"^\d{2}:\d{2}$", output);
    }
}
