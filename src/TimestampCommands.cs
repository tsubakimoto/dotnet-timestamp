using ConsoleAppFramework;

namespace Tsubakimoto.Tools.Timestamp;

internal class TimestampCommands
{
    /// <summary>
    /// Root command
    /// </summary>
    /// <param name="format">-f, Timestamp format</param>
    /// <param name="timezone">-z, Timezone identifier</param>
    [Command("")]
    public void Root(string format = "o", string timezone = "UTC")
    {
        ShowCurrentDateTime(format, timezone);
    }

    /// <summary>
    /// Now command
    /// </summary>
    /// <param name="format">-f, Timestamp format</param>
    /// <param name="timezone">-z, Timezone identifier</param>
    public void Now(string format = "o", string timezone = "UTC")
    {
        ShowCurrentDateTime(format, timezone);
    }

    /// <summary>
    /// Timezone command
    /// </summary>
    /// <param name="list">-l, List available timezones</param>
    public void Timezone(bool list = false)
    {
        if (list)
        {
            // 利用可能なタイムゾーンの一覧を表示
            var timeZones = TimeZoneInfo.GetSystemTimeZones();
            foreach (var tz in timeZones)
            {
                Console.WriteLine($"{tz.Id}: {tz.DisplayName}");
            }
        }
        else
        {
            // ローカルシステムのタイムゾーンを表示
            var localTimeZone = TimeZoneInfo.Local;
            Console.WriteLine($"{localTimeZone.Id}: {localTimeZone.DisplayName}");
        }
    }

    /// <summary>
    /// 現在の日時を指定されたフォーマットとタイムゾーンで表示します。
    /// </summary>
    /// <param name="format">日時のフォーマット文字列（C# の標準的な日時フォーマット）</param>
    /// <param name="timezone">タイムゾーン識別子</param>
    static void ShowCurrentDateTime(string format, string timezone)
    {
        try
        {
            // タイムゾーンを取得
            TimeZoneInfo timeZoneInfo;
            if (timezone.Equals("UTC", StringComparison.OrdinalIgnoreCase))
            {
                timeZoneInfo = TimeZoneInfo.Utc;
            }
            else
            {
                timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timezone);
            }

            // 現在の UTC 時刻を取得し、指定されたタイムゾーンに変換
            var utcNow = DateTimeOffset.UtcNow;
            var convertedTime = TimeZoneInfo.ConvertTime(utcNow, timeZoneInfo);

            // 指定されたフォーマットで表示
            Console.WriteLine(convertedTime.ToString(format));
        }
        catch (TimeZoneNotFoundException)
        {
            Console.Error.WriteLine($"Error: タイムゾーン '{timezone}' が見つかりません。");
        }
        catch (FormatException)
        {
            Console.Error.WriteLine($"Error: フォーマット '{format}' が無効です。");
        }
    }
}