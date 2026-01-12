namespace Tsubakimoto.Tools.Timestamp.E2E.Test;

/// <summary>
/// E2E tests for the 'timezone' command.
/// </summary>
public sealed class TimezoneCommandTests
{
    [Fact]
    public async Task Timezone_WithoutOptions_DisplaysLocalTimezone()
    {
        var result = await TimestampCliRunner.RunAsync("timezone");

        Assert.False(result.TimedOut, "Command should not timeout");
        Assert.Equal(0, result.ExitCode);
        Assert.False(string.IsNullOrWhiteSpace(result.StandardOutput), 
            "Output should contain local timezone information");
        
        var output = result.StandardOutput.Trim();
        // Verify the output contains a timezone identifier
        Assert.NotEmpty(output);
    }

    [Fact]
    public async Task Timezone_WithListOption_DisplaysAvailableTimezones()
    {
        var result = await TimestampCliRunner.RunAsync("timezone --list");

        Assert.False(result.TimedOut);
        Assert.Equal(0, result.ExitCode);
        Assert.False(string.IsNullOrWhiteSpace(result.StandardOutput), 
            "Output should contain list of timezones");
        
        var output = result.StandardOutput;
        // Verify the output contains well-known timezone identifiers
        Assert.Contains("UTC", output);
        Assert.Contains("Standard Time", output);
    }

    [Fact]
    public async Task Timezone_WithShortListOption_DisplaysAvailableTimezones()
    {
        var result = await TimestampCliRunner.RunAsync("timezone -l");

        Assert.False(result.TimedOut);
        Assert.Equal(0, result.ExitCode);
        Assert.False(string.IsNullOrWhiteSpace(result.StandardOutput), 
            "Output should contain list of timezones");
        
        var output = result.StandardOutput;
        Assert.Contains("UTC", output);
    }

    [Fact]
    public async Task Timezone_List_ContainsMultipleTimezones()
    {
        var result = await TimestampCliRunner.RunAsync("timezone --list");

        Assert.False(result.TimedOut);
        Assert.Equal(0, result.ExitCode);
        
        var lines = result.StandardOutput.Split(
            ["\r\n", "\r", "\n"], 
            StringSplitOptions.RemoveEmptyEntries);
        
        // Verify we have multiple timezone entries
        Assert.True(lines.Length > 10, 
            $"Expected more than 10 timezone entries, but got {lines.Length}");
    }
}
