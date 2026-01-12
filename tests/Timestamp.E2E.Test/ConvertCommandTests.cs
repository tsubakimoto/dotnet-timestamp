namespace Tsubakimoto.Tools.Timestamp.E2E.Test;

/// <summary>
/// E2E tests for the 'convert' command.
/// </summary>
public sealed class ConvertCommandTests
{
    [Fact]
    public async Task Convert_FromUtcToJapan_ConvertsTimezone()
    {
        var result = await TimestampCliRunner.RunAsync(
            $"convert --datetime \"2026-01-12T12:00:00\" --from {TimeZoneHelper.Utc} --to \"{TimeZoneHelper.JapanStandardTime}\"");

        Assert.False(result.TimedOut, "Command should not timeout");
        Assert.Equal(0, result.ExitCode);
        Assert.False(string.IsNullOrWhiteSpace(result.StandardOutput), 
            "Output should contain converted timestamp");
        
        var output = result.StandardOutput.Trim();
        Assert.True(DateTimeOffset.TryParse(output, out var timestamp), 
            $"Output should be a valid timestamp, but was: {output}");
        
        // Japan is UTC+9, so 12:00 UTC should be 21:00 JST
        Assert.Equal(21, timestamp.Hour);
        Assert.Equal(TimeZoneHelper.JapanStandardTimeOffset, timestamp.Offset);
    }

    [Fact]
    public async Task Convert_FromJapanToUtc_ConvertsTimezone()
    {
        var result = await TimestampCliRunner.RunAsync(
            $"convert -d \"2026-01-12T00:00:00\" -f \"{TimeZoneHelper.JapanStandardTime}\" -t {TimeZoneHelper.Utc}");

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
            $"convert --datetime \"2026-01-12T12:00:00\" --from {TimeZoneHelper.Utc} --to \"{TimeZoneHelper.JapanStandardTime}\" --format yyyy-MM-dd");

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
            $"convert -d \"2026-01-12T12:00:00\" -f {TimeZoneHelper.Utc} -t \"{TimeZoneHelper.JapanStandardTime}\" -m HH:mm:ss");

        Assert.False(result.TimedOut);
        Assert.Equal(0, result.ExitCode);
        
        var output = result.StandardOutput.Trim();
        Assert.Matches(@"^\d{2}:\d{2}:\d{2}$", output);
        
        // Japan is UTC+9, so 12:00 UTC should be 21:00 JST
        Assert.Contains("21:", output);
    }

    [Fact]
    public async Task Convert_WithUtcRoundTrip_MaintainsTimestamp()
    {
        var result = await TimestampCliRunner.RunAsync(
            $"convert --datetime \"2026-01-12T23:00:00\" --from {TimeZoneHelper.Utc} --to {TimeZoneHelper.Utc}");

        Assert.False(result.TimedOut);
        Assert.Equal(0, result.ExitCode);
        
        var output = result.StandardOutput.Trim();
        Assert.True(DateTimeOffset.TryParse(output, out var timestamp), 
            $"Output should be a valid timestamp, but was: {output}");
        
        Assert.Equal(23, timestamp.Hour);
        Assert.Equal(TimeSpan.Zero, timestamp.Offset);
    }

    [Fact]
    public async Task Convert_WithIso8601Input_ConvertsCorrectly()
    {
        var result = await TimestampCliRunner.RunAsync(
            $"convert --datetime \"2026-01-12T12:00:00+09:00\" --from \"{TimeZoneHelper.JapanStandardTime}\" --to {TimeZoneHelper.Utc}");

        Assert.False(result.TimedOut);
        Assert.Equal(0, result.ExitCode);
        
        var output = result.StandardOutput.Trim();
        Assert.True(DateTimeOffset.TryParse(output, out var timestamp), 
            $"Output should be a valid timestamp, but was: {output}");
        
        Assert.Equal(3, timestamp.Hour);
        Assert.Equal(TimeSpan.Zero, timestamp.Offset);
    }
}
