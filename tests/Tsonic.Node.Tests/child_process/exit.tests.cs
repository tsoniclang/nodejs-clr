using Xunit;
using System;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;

namespace Tsonic.Node.Tests;

public class ChildProcessExitTests
{
    private static bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

    [Fact]
    public void exit_ExitEvent_ContainsExitCode()
    {
        var command = IsWindows ? "cmd" : "sh";
        var args = IsWindows ? new[] { "/c", "exit 0" } : new[] { "-c", "exit 0" };
        int? capturedExitCode = null;
        var resetEvent = new ManualResetEventSlim(false);

        var child = child_process.spawn(command, args);
        child.on("exit", (int? code, string? signal) =>
        {
            capturedExitCode = code;
            resetEvent.Set();
        });

        var signaled = resetEvent.Wait(2000);

        Assert.True(signaled);
        Assert.NotNull(capturedExitCode);
        Assert.Equal(0, capturedExitCode.Value);
    }

    [Fact]
    public void exit_ExitEvent_NonZeroExitCode()
    {
        var command = IsWindows ? "cmd" : "sh";
        var args = IsWindows ? new[] { "/c", "exit 42" } : new[] { "-c", "exit 42" };
        int? capturedExitCode = null;
        var resetEvent = new ManualResetEventSlim(false);

        var child = child_process.spawn(command, args);
        child.on("exit", (int? code, string? signal) =>
        {
            capturedExitCode = code;
            resetEvent.Set();
        });

        var signaled = resetEvent.Wait(2000);

        Assert.True(signaled);
        Assert.Equal(42, capturedExitCode);
    }

    [Fact]
    public void exit_ExitCodeProperty_SetAfterExit()
    {
        var command = IsWindows ? "cmd" : "echo";
        var args = IsWindows ? new[] { "/c", "echo", "test" } : new[] { "test" };
        var resetEvent = new ManualResetEventSlim(false);

        var child = child_process.spawn(command, args);
        child.on("exit", (int? code, string? signal) =>
        {
            resetEvent.Set();
        });

        resetEvent.Wait(2000);
        Thread.Sleep(100); // Give time for property to be set

        Assert.NotNull(child.exitCode);
    }
}
