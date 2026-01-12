namespace Tsubakimoto.Tools.Timestamp.E2E.Test;

/// <summary>
/// Helper class for cross-platform timezone identifiers.
/// Provides timezone IDs that work on both Windows and Linux.
/// </summary>
public static class TimeZoneHelper
{
    /// <summary>
    /// Gets the UTC timezone identifier.
    /// This is consistent across all platforms.
    /// </summary>
    public static string Utc => "UTC";

    /// <summary>
    /// Gets the Japan Standard Time (JST) timezone identifier.
    /// Returns the appropriate timezone ID based on the current platform.
    /// </summary>
    public static string JapanStandardTime
    {
        get
        {
            // Windows uses "Tokyo Standard Time"
            // Linux/macOS uses "Asia/Tokyo"
            return OperatingSystem.IsWindows() ? "Tokyo Standard Time" : "Asia/Tokyo";
        }
    }

    /// <summary>
    /// Gets the expected UTC offset for Japan Standard Time.
    /// JST is always UTC+9.
    /// </summary>
    public static TimeSpan JapanStandardTimeOffset => TimeSpan.FromHours(9);
}
