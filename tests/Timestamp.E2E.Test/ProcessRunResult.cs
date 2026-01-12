namespace Tsubakimoto.Tools.Timestamp.E2E.Test;

/// <summary>
/// Represents the result of running an external process.
/// </summary>
/// <param name="ExitCode">The exit code returned by the process.</param>
/// <param name="StandardOutput">The standard output from the process.</param>
/// <param name="StandardError">The standard error from the process.</param>
/// <param name="TimedOut">Indicates whether the process timed out.</param>
public sealed record ProcessRunResult(
    int ExitCode,
    string StandardOutput,
    string StandardError,
    bool TimedOut);
