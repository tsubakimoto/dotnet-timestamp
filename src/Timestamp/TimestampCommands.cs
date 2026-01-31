using ConsoleAppFramework;

namespace Tsubakimoto.Tools.Timestamp;

internal class TimestampCommands
{
    /// <summary>
    /// Root command
    /// </summary>
    /// <param name="format">-m, Timestamp format</param>
    /// <param name="timezone">-z, Timezone identifier</param>
    [Command("")]
    public void Root(string format = "o", string timezone = "UTC")
    {
        ShowCurrentDateTime(format, timezone);
    }

    /// <summary>
    /// Now command
    /// </summary>
    /// <param name="format">-m, Timestamp format</param>
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
    /// Convert command - Converts a datetime from one timezone to another
    /// </summary>
    /// <param name="datetime">-d, Datetime to convert (parseable by DateTimeOffset)</param>
    /// <param name="from">-f, Source timezone identifier</param>
    /// <param name="to">-t, Target timezone identifier</param>
    /// <param name="format">-m, Output timestamp format (default: 'o')</param>
    public void Convert(string datetime, string from, string to, string format = "o")
    {
        ConvertDateTime(datetime, from, to, format);
    }

    /// <summary>
    /// 指定された日時を別のタイムゾーンに変換して表示します。
    /// </summary>
    /// <param name="datetime">変換する日時（DateTimeOffset で解析可能な形式）</param>
    /// <param name="from">変換元のタイムゾーン識別子</param>
    /// <param name="to">変換先のタイムゾーン識別子</param>
    /// <param name="format">出力する日時のフォーマット文字列（C# の標準的な日時フォーマット）</param>
    static void ConvertDateTime(string datetime, string from, string to, string format)
    {
        try
        {
            // 日時文字列を解析
            if (!DateTimeOffset.TryParse(datetime, out DateTimeOffset parsedDateTime))
            {
                Console.Error.WriteLine($"Error: 日時 '{datetime}' の解析に失敗しました。");
                return;
            }

            // 変換元のタイムゾーンを取得
            TimeZoneInfo? fromTimeZone = GetTimeZoneInfo(from);
            if (fromTimeZone is null)
            {
                Console.Error.WriteLine($"Error: タイムゾーン '{from}' が見つかりません。");
                return;
            }

            // 変換先のタイムゾーンを取得
            TimeZoneInfo? toTimeZone = GetTimeZoneInfo(to);
            if (toTimeZone is null)
            {
                Console.Error.WriteLine($"Error: タイムゾーン '{to}' が見つかりません。");
                return;
            }

            // 変換元のタイムゾーンに基づいて UTC に変換
            var utcDateTime = TimeZoneInfo.ConvertTimeToUtc(parsedDateTime.DateTime, fromTimeZone);
            var utcDateTimeOffset = new DateTimeOffset(utcDateTime, TimeSpan.Zero);

            // 変換先のタイムゾーンに変換
            var convertedTime = TimeZoneInfo.ConvertTime(utcDateTimeOffset, toTimeZone);

            // 指定されたフォーマットで表示
            Console.WriteLine(convertedTime.ToString(format));
        }
        catch (FormatException)
        {
            Console.Error.WriteLine($"Error: フォーマット '{format}' が無効です。");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error: 予期しないエラーが発生しました: {ex.Message}");
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
            TimeZoneInfo? timeZoneInfo = GetTimeZoneInfo(timezone);
            if (timeZoneInfo is null)
            {
                Console.Error.WriteLine($"Error: タイムゾーン '{timezone}' が見つかりません。");
                return;
            }

            // 現在の UTC 時刻を取得し、指定されたタイムゾーンに変換
            var utcNow = DateTimeOffset.UtcNow;
            var convertedTime = TimeZoneInfo.ConvertTime(utcNow, timeZoneInfo);

            // 指定されたフォーマットで表示
            Console.WriteLine(convertedTime.ToString(format));
        }
        catch (FormatException)
        {
            Console.Error.WriteLine($"Error: フォーマット '{format}' が無効です。");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error: 予期しないエラーが発生しました: {ex.Message}");
        }
    }

    /// <summary>
    /// Unix command - Converts a datetime to Unix timestamp
    /// </summary>
    /// <param name="datetime">-d, Datetime to convert (parseable by DateTimeOffset). If not specified, uses current datetime.</param>
    public void Unix(string? datetime = null)
    {
        DateTimeOffset targetDateTime;

        if (string.IsNullOrWhiteSpace(datetime))
        {
            targetDateTime = DateTimeOffset.UtcNow;
        }
        else
        {
            if (!DateTimeOffset.TryParse(datetime, out targetDateTime))
            {
                Console.Error.WriteLine($"Error: 日時 '{datetime}' の解析に失敗しました。");
                return;
            }
        }

        Console.WriteLine(targetDateTime.ToUnixTimeMilliseconds());
    }

    /// <summary>
    /// タイムゾーン識別子から TimeZoneInfo を取得します。
    /// </summary>
    /// <param name="timezoneId">タイムゾーン識別子</param>
    /// <returns>TimeZoneInfo オブジェクト。見つからない場合は null</returns>
    static TimeZoneInfo? GetTimeZoneInfo(string timezoneId)
    {
        try
        {
            if (timezoneId.Equals("UTC", StringComparison.OrdinalIgnoreCase))
            {
                return TimeZoneInfo.Utc;
            }

            return TimeZoneInfo.FindSystemTimeZoneById(timezoneId);
        }
        catch (TimeZoneNotFoundException)
        {
            return null;
        }
    }
}