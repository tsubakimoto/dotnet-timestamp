namespace Tsubakimoto.Tools.Timestamp.E2E.Test;

/// <summary>
/// Helper class for running the dotnet-timestamp CLI tool in E2E tests.
/// </summary>
public static class TimestampCliRunner
{
    private static readonly string RepoRoot = Path.GetFullPath(
        Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));

    private static readonly string ProjectPath = Path.Combine(
        RepoRoot, "src", "Timestamp", "Timestamp.csproj");

    /// <summary>
    /// Runs the dotnet-timestamp CLI tool with the specified arguments.
    /// </summary>
    /// <param name="arguments">The command-line arguments to pass to the tool.</param>
    /// <param name="timeout">The maximum time to wait for the command to complete.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A <see cref="ProcessRunResult"/> containing the command execution results.</returns>
    public static async Task<ProcessRunResult> RunAsync(
        string arguments,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        timeout ??= TimeSpan.FromSeconds(60);

        var dotnetArgs = $"run --project \"{ProjectPath}\" -- {arguments}";

        return await ProcessRunner.RunAsync(
            fileName: "dotnet",
            arguments: dotnetArgs,
            workingDirectory: RepoRoot,
            timeout: timeout,
            cancellationToken: cancellationToken);
    }
}
