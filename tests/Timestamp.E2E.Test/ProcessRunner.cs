using System.Diagnostics;
using System.Text;

namespace Tsubakimoto.Tools.Timestamp.E2E.Test;

/// <summary>
/// Utility class for running external processes and capturing their output.
/// </summary>
public static class ProcessRunner
{
    /// <summary>
    /// Runs an external process asynchronously and captures its output.
    /// </summary>
    /// <param name="fileName">The executable file name to run.</param>
    /// <param name="arguments">The command-line arguments to pass to the process.</param>
    /// <param name="workingDirectory">The working directory for the process.</param>
    /// <param name="environmentVariables">Optional environment variables to set for the process.</param>
    /// <param name="timeout">The maximum time to wait for the process to complete. Defaults to 30 seconds.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A <see cref="ProcessRunResult"/> containing the exit code, output, and error streams.</returns>
    public static async Task<ProcessRunResult> RunAsync(
        string fileName,
        string arguments,
        string workingDirectory,
        IReadOnlyDictionary<string, string>? environmentVariables = null,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        timeout ??= TimeSpan.FromSeconds(30);

        var psi = new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = arguments,
            WorkingDirectory = workingDirectory,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
        };

        if (environmentVariables is not null)
        {
            foreach (var (key, value) in environmentVariables)
            {
                psi.Environment[key] = value;
            }
        }

        using var process = new Process { StartInfo = psi, EnableRaisingEvents = true };

        var stdout = new StringBuilder();
        var stderr = new StringBuilder();

        process.OutputDataReceived += (_, e) =>
        {
            if (e.Data is not null)
            {
                stdout.AppendLine(e.Data);
            }
        };
        process.ErrorDataReceived += (_, e) =>
        {
            if (e.Data is not null)
            {
                stderr.AppendLine(e.Data);
            }
        };

        if (!process.Start())
        {
            throw new InvalidOperationException($"Failed to start process: {fileName} {arguments}");
        }

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        using var timeoutCts = new CancellationTokenSource(timeout.Value);
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

        try
        {
            await process.WaitForExitAsync(linkedCts.Token).ConfigureAwait(false);
            return new ProcessRunResult(
                ExitCode: process.ExitCode,
                StandardOutput: stdout.ToString(),
                StandardError: stderr.ToString(),
                TimedOut: false);
        }
        catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested)
        {
            try
            {
                if (!process.HasExited)
                {
                    process.Kill(entireProcessTree: true);
                }
            }
            catch
            {
                // Ignore kill exceptions
            }

            return new ProcessRunResult(
                ExitCode: -1,
                StandardOutput: stdout.ToString(),
                StandardError: stderr.ToString(),
                TimedOut: true);
        }
    }
}
