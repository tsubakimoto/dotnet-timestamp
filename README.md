# dotnet-timestamp

`dotnet-timestamp` is a .NET global tool that prints timestamps and converts them across time zones from the command line. The installed command name is `dtstamp`.

## Install

```bash
dotnet tool install -g dotnet-timestamp
```

## Usage

- Show the current time (defaults to UTC and round-trip format):
	```bash
	dtstamp now
	```
- Use a custom format and time zone (C# standard format strings, `DateTimeOffset` time zone IDs):
	```bash
	dtstamp now --format "yyyy-MM-dd HH:mm:ss" --timezone Asia/Tokyo
	```
- List available time zones on the system:
	```bash
	dtstamp timezone --list
	```
- Convert a timestamp between time zones:
	```bash
	dtstamp convert --datetime "2024-09-01T12:00:00" --from UTC --to America/New_York --format "yyyy-MM-dd HH:mm:ss zzz"
	```
- Convert a datetime to Unix timestamp (milli seconds since 1970-01-01 00:00:00 UTC):
	```bash
	dtstamp unix --datetime "2024-09-01T12:00:00Z"
	```
- Get the current Unix timestamp:
	```bash
	dtstamp unix
	```

The default command runs `now`.