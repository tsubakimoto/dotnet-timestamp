namespace Tsubakimoto.Tools.Timestamp.E2E.Test;

/// <summary>
/// E2E tests for the 'convert unix' command.
/// </summary>
public sealed class ConvertUnixCommandTests
{
    [Fact]
    public async Task ConvertUnix_WithZero_ReturnsUnixEpoch()
    {
        var result = await TimestampCliRunner.RunAsync("convert unix 0");

        Assert.False(result.TimedOut, "Command should not timeout");
        Assert.Equal(0, result.ExitCode);

        var output = result.StandardOutput.Trim();
        Assert.True(DateTimeOffset.TryParse(output, out var timestamp),
            $"Output should be a valid timestamp, but was: {output}");

        // Unix epoch is 1970-01-01T00:00:00Z
        Assert.Equal(1970, timestamp.Year);
        Assert.Equal(1, timestamp.Month);
        Assert.Equal(1, timestamp.Day);
        Assert.Equal(0, timestamp.Hour);
        Assert.Equal(0, timestamp.Minute);
        Assert.Equal(0, timestamp.Second);
    }

    [Fact]
    public async Task ConvertUnix_WithKnownTimestamp_ReturnsCorrectDatetime()
    {
        // 946684800000 milliseconds = 2000-01-01T00:00:00Z
        var result = await TimestampCliRunner.RunAsync("convert unix 946684800000");

        Assert.False(result.TimedOut);
        Assert.Equal(0, result.ExitCode);

        var output = result.StandardOutput.Trim();
        Assert.True(DateTimeOffset.TryParse(output, out var timestamp),
            $"Output should be a valid timestamp, but was: {output}");

        Assert.Equal(2000, timestamp.Year);
        Assert.Equal(1, timestamp.Month);
        Assert.Equal(1, timestamp.Day);
        Assert.Equal(TimeSpan.Zero, timestamp.Offset);
    }

    [Fact]
    public async Task ConvertUnix_WithCustomFormat_FormatsOutput()
    {
        // 946684800000 milliseconds = 2000-01-01T00:00:00Z
        var result = await TimestampCliRunner.RunAsync("convert unix 946684800000 --format yyyy-MM-dd");

        Assert.False(result.TimedOut);
        Assert.Equal(0, result.ExitCode);

        var output = result.StandardOutput.Trim();
        Assert.Equal("2000-01-01", output);
    }

    [Fact]
    public async Task ConvertUnix_WithShortFormatOption_FormatsOutput()
    {
        // 946684800000 milliseconds = 2000-01-01T00:00:00Z
        var result = await TimestampCliRunner.RunAsync("convert unix 946684800000 -m HH:mm:ss");

        Assert.False(result.TimedOut);
        Assert.Equal(0, result.ExitCode);

        var output = result.StandardOutput.Trim();
        Assert.Equal("00:00:00", output);
    }

    [Fact]
    public async Task ConvertUnix_WithNegativeTimestamp_ReturnsDateBeforeEpoch()
    {
        // -86400000 milliseconds = 1969-12-31T00:00:00Z (one day before epoch)
        var result = await TimestampCliRunner.RunAsync("convert unix -86400000");

        Assert.False(result.TimedOut);
        Assert.Equal(0, result.ExitCode);

        var output = result.StandardOutput.Trim();
        Assert.True(DateTimeOffset.TryParse(output, out var timestamp),
            $"Output should be a valid timestamp, but was: {output}");

        Assert.Equal(1969, timestamp.Year);
        Assert.Equal(12, timestamp.Month);
        Assert.Equal(31, timestamp.Day);
    }

    [Fact]
    public async Task ConvertUnix_RoundTrip_ReturnsOriginalTimestamp()
    {
        // First, convert a known datetime to unix timestamp
        var knownDatetime = new DateTimeOffset(2024, 6, 15, 12, 30, 45, TimeSpan.Zero);
        var unixTimestamp = knownDatetime.ToUnixTimeMilliseconds();

        // Then convert back using convert unix
        var result = await TimestampCliRunner.RunAsync($"convert unix {unixTimestamp}");

        Assert.False(result.TimedOut);
        Assert.Equal(0, result.ExitCode);

        var output = result.StandardOutput.Trim();
        Assert.True(DateTimeOffset.TryParse(output, out var timestamp),
            $"Output should be a valid timestamp, but was: {output}");

        Assert.Equal(knownDatetime.Year, timestamp.Year);
        Assert.Equal(knownDatetime.Month, timestamp.Month);
        Assert.Equal(knownDatetime.Day, timestamp.Day);
        Assert.Equal(knownDatetime.Hour, timestamp.Hour);
        Assert.Equal(knownDatetime.Minute, timestamp.Minute);
        Assert.Equal(knownDatetime.Second, timestamp.Second);
    }

    [Fact]
    public async Task ConvertUnix_WithInvalidFormat_DisplaysError()
    {
        var result = await TimestampCliRunner.RunAsync("convert unix 0 --format \"%%\"");

        Assert.False(result.TimedOut);
        Assert.Contains("Error:", result.StandardError);
    }
}
