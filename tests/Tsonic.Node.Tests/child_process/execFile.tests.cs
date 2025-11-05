using Xunit;
using System;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;

namespace Tsonic.Node.Tests;

public class ChildProcessExecFileTests
{
    private static bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

    [Fact]
    public void execFile_CallsCallback()
    {
        var file = IsWindows ? "cmd.exe" : "/bin/echo";
        var args = IsWindows ? new[] { "/c", "echo", "Hello" } : new[] { "Hello" };
        var callbackCalled = false;
        var stdout = "";

        child_process.execFile(file, args, null, (error, stdoutStr, stderr) =>
        {
            callbackCalled = true;
            stdout = stdoutStr;
        });

        // Wait for async execution
        Thread.Sleep(500);

        Assert.True(callbackCalled);
        Assert.Contains("Hello", stdout);
    }
}
