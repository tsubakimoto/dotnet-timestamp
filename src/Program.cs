using ConsoleAppFramework;

var app = ConsoleApp.Create();
app.Add("", () =>
{
    var unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    Console.WriteLine(unixTimestamp);
});
app.Run(args);
