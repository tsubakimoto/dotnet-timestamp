namespace Tsubakimoto.Tools.Timestamp.E2E.Test;

/// <summary>
/// E2E tests for the 'convert' command.
/// </summary>
public sealed class ConvertCommandTests
{
    [Fact]
    public async Task Convert_WithRequiredOptions_ConvertsTimezone()
    {
        var result = await TimestampCliRunner.RunAsync(
            "convert --datetime \"2026-01-12T12:00:00\" --from UTC --to \"Tokyo Standard Time\"");

        Assert.False(result.TimedOut, "Command should not timeout");
        Assert.Equal(0, result.ExitCode);
        Assert.False(string.IsNullOrWhiteSpace(result.StandardOutput), 
            "Output should contain converted timestamp");
        
        var output = result.StandardOutput.Trim();
        Assert.True(DateTimeOffset.TryParse(output, out var timestamp), 
            $"Output should be a valid timestamp, but was: {output}");
        
        // Tokyo is UTC+9, so 12:00 UTC should be 21:00 JST
        Assert.Equal(21, timestamp.Hour);
        Assert.Equal(TimeSpan.FromHours(9), timestamp.Offset);
    }

    [Fact]
    public async Task Convert_WithShortOptions_ConvertsTimezone()
    {
        var result = await TimestampCliRunner.RunAsync(
            "convert -d \"2026-01-12T00:00:00\" -f \"Tokyo Standard Time\" -t UTC");

        Assert.False(result.TimedOut);
        Assert.Equal(0, result.ExitCode);
        
        var output = result.StandardOutput.Trim();
        Assert.True(DateTimeOffset.TryParse(output, out var timestamp), 
            $"Output should be a valid timestamp, but was: {output}");
        
        // Midnight JST (UTC+9) should be 15:00 previous day UTC
        Assert.Equal(TimeSpan.Zero, timestamp.Offset);
    }

    [Fact]
    public async Task Convert_WithCustomFormat_ConvertsAndFormatsTimestamp()
    {
        var result = await TimestampCliRunner.RunAsync(
            "convert --datetime \"2026-01-12T12:00:00\" --from UTC --to \"Eastern Standard Time\" --format yyyy-MM-dd");

        Assert.False(result.TimedOut);
        Assert.Equal(0, result.ExitCode);
        
        var output = result.StandardOutput.Trim();
        // Verify the output matches the expected format
        Assert.Matches(@"^\d{4}-\d{2}-\d{2}$", output);
    }

    [Fact]
    public async Task Convert_WithShortFormatOption_ConvertsAndFormatsTimestamp()
    {
        var result = await TimestampCliRunner.RunAsync(
            "convert -d \"2026-01-12T12:00:00\" -f UTC -t \"Pacific Standard Time\" -m HH:mm:ss");

        Assert.False(result.TimedOut);
        Assert.Equal(0, result.ExitCode);
        
        var output = result.StandardOutput.Trim();
        Assert.Matches(@"^\d{2}:\d{2}:\d{2}$", output);
    }

    [Fact]
    public async Task Convert_AcrossDateLine_HandlesCorrectly()
    {
        var result = await TimestampCliRunner.RunAsync(
            "convert --datetime \"2026-01-12T23:00:00\" --from UTC --to \"Pacific Standard Time\"");

        Assert.False(result.TimedOut);
        Assert.Equal(0, result.ExitCode);
        
        var output = result.StandardOutput.Trim();
        Assert.True(DateTimeOffset.TryParse(output, out var timestamp), 
            $"Output should be a valid timestamp, but was: {output}");
        
        // 23:00 UTC should be on the previous day in PST (UTC-8)
        Assert.Equal(12, timestamp.Day);
        Assert.Equal(15, timestamp.Hour);
    }

    [Fact]
    public async Task Convert_WithIso8601Input_ConvertsCorrectly()
    {
        var result = await TimestampCliRunner.RunAsync(
            "convert --datetime \"2026-01-12T12:00:00+09:00\" --from \"Tokyo Standard Time\" --to UTC");

        Assert.False(result.TimedOut);
        Assert.Equal(0, result.ExitCode);
        
        var output = result.StandardOutput.Trim();
        Assert.True(DateTimeOffset.TryParse(output, out var timestamp), 
            $"Output should be a valid timestamp, but was: {output}");
        
        Assert.Equal(3, timestamp.Hour);
        Assert.Equal(TimeSpan.Zero, timestamp.Offset);
    }
}
