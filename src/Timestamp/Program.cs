using ConsoleAppFramework;

using Tsubakimoto.Tools.Timestamp;

var app = ConsoleApp.Create();
app.Add<TimestampCommands>();
app.Run(args);
