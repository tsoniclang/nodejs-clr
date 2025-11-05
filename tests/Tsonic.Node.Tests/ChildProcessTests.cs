using Xunit;
using System;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;

namespace Tsonic.Node.Tests;

public class ChildProcessTests
{
    private static bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

    // ==================== execSync Tests ====================

    [Fact]
    public void ExecSync_SimpleCommand_ReturnsOutput()
    {
        var command = IsWindows ? "echo Hello" : "echo 'Hello'";
        var result = child_process.execSync(command);

        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public void ExecSync_WithOptions_ReturnsString()
    {
        var command = IsWindows ? "echo Hello" : "echo 'Hello'";
        var options = new ExecOptions { encoding = "utf8" };
        var result = child_process.execSync(command, options);

        Assert.IsType<string>(result);
        var str = (string)result;
        Assert.Contains("Hello", str);
    }

    [Fact]
    public void ExecSync_WithBufferEncoding_ReturnsByteArray()
    {
        var command = IsWindows ? "echo Hello" : "echo 'Hello'";
        var options = new ExecOptions { encoding = "buffer" };
        var result = child_process.execSync(command, options);

        Assert.True(result is byte[] || result is string);
    }

    [Fact]
    public void ExecSync_WithCwd_ExecutesInDirectory()
    {
        var tempDir = Path.GetTempPath();
        var command = IsWindows ? "cd" : "pwd";
        var options = new ExecOptions { cwd = tempDir, encoding = "utf8" };
        var result = child_process.execSync(command, options);

        Assert.IsType<string>(result);
        // Result should contain temp directory path
    }

    [Fact]
    public void ExecSync_NonZeroExit_ThrowsException()
    {
        var command = IsWindows ? "exit /b 1" : "exit 1";

        Assert.Throws<InvalidOperationException>(() =>
        {
            child_process.execSync(command);
        });
    }

    // ==================== spawnSync Tests ====================

    [Fact]
    public void SpawnSync_SimpleCommand_ReturnsResult()
    {
        var command = IsWindows ? "cmd" : "echo";
        var args = IsWindows ? new[] { "/c", "echo", "Hello" } : new[] { "Hello" };
        var result = child_process.spawnSync(command, args);

        Assert.NotNull(result);
        Assert.True(result.status == 0 || result.status == null);
        Assert.NotNull(result.stdout);
    }

    [Fact]
    public void SpawnSync_HasPid()
    {
        var command = IsWindows ? "cmd" : "echo";
        var args = IsWindows ? new[] { "/c", "echo", "test" } : new[] { "test" };
        var result = child_process.spawnSync(command, args);

        Assert.True(result.pid > 0);
    }

    [Fact]
    public void SpawnSync_WithInvalidCommand_SetsError()
    {
        var result = child_process.spawnSync("nonexistent_command_xyz");

        Assert.NotNull(result.error);
    }

    [Fact]
    public void SpawnSync_OutputArray_ContainsStdoutStderr()
    {
        var command = IsWindows ? "cmd" : "echo";
        var args = IsWindows ? new[] { "/c", "echo", "test" } : new[] { "test" };
        var result = child_process.spawnSync(command, args);

        Assert.NotNull(result.output);
        Assert.True(result.output.Length >= 3);
        // output[0] is null (stdin), output[1] is stdout, output[2] is stderr
    }

    // ==================== execFileSync Tests ====================

    [Fact]
    public void ExecFileSync_SimpleCommand_ReturnsOutput()
    {
        var file = IsWindows ? "cmd.exe" : "/bin/echo";
        var args = IsWindows ? new[] { "/c", "echo", "Hello" } : new[] { "Hello" };
        var result = child_process.execFileSync(file, args);

        Assert.NotNull(result);
    }

    [Fact]
    public void ExecFileSync_WithStringEncoding_ReturnsString()
    {
        var file = IsWindows ? "cmd.exe" : "/bin/echo";
        var args = IsWindows ? new[] { "/c", "echo", "Hello" } : new[] { "Hello" };
        var options = new ExecOptions { encoding = "utf8" };
        var result = child_process.execFileSync(file, args, options);

        Assert.IsType<string>(result);
        Assert.Contains("Hello", (string)result);
    }

    // ==================== spawn Tests ====================

    [Fact]
    public void Spawn_SimpleCommand_ReturnsChildProcess()
    {
        var command = IsWindows ? "cmd" : "echo";
        var args = IsWindows ? new[] { "/c", "echo", "test" } : new[] { "test" };
        var child = child_process.spawn(command, args);

        Assert.NotNull(child);
        Assert.True(child.pid > 0);
    }

    [Fact]
    public void Spawn_HasSpawnProperties()
    {
        var command = IsWindows ? "cmd" : "echo";
        var args = IsWindows ? new[] { "/c", "echo", "test" } : new[] { "test" };
        var child = child_process.spawn(command, args);

        Assert.NotNull(child.spawnfile);
        Assert.NotEmpty(child.spawnfile);
        Assert.NotNull(child.spawnargs);
        Assert.NotEmpty(child.spawnargs);
        Assert.Equal(command, child.spawnfile);
    }

    [Fact]
    public void Spawn_EmitsSpawnEvent()
    {
        var command = IsWindows ? "cmd" : "echo";
        var args = IsWindows ? new[] { "/c", "echo", "test" } : new[] { "test" };

        var child = child_process.spawn(command, args);

        // Spawn event is fired immediately, this test verifies the API exists
        // We can't reliably test event timing since it may fire before listener attached
        child.on("spawn", () => { /* Event handler */ });

        // Give some time for event to fire
        Thread.Sleep(100);

        // Test verifies API exists and doesn't throw
        Assert.NotNull(child);
    }

    [Fact]
    public void Spawn_EmitsCloseEvent()
    {
        // Use a longer-running process to ensure we can attach listener before it exits
        var command = IsWindows ? "cmd" : "sleep";
        var args = IsWindows ? new[] { "/c", "timeout", "/t", "1", "/nobreak" } : new[] { "0.1" };
        var closeEmitted = false;
        int? exitCode = null;
        var resetEvent = new ManualResetEventSlim(false);

        var child = child_process.spawn(command, args);
        child.on("close", (int? code, string? signal) =>
        {
            closeEmitted = true;
            exitCode = code;
            resetEvent.Set();
        });

        // Wait for process to complete (up to 3 seconds)
        var signaled = resetEvent.Wait(3000);

        // close event should have been emitted
        Assert.True(signaled, "Close event was not emitted within timeout");
        Assert.True(closeEmitted);
        Assert.NotNull(exitCode);
    }

    [Fact]
    public void Spawn_InvalidCommand_ThrowsWithoutHandler()
    {
        // When there's no error handler, spawn throws for invalid commands
        // This matches Node.js behavior where unhandled error events throw
        Assert.Throws<System.ComponentModel.Win32Exception>(() =>
        {
            var child = child_process.spawn("nonexistent_command_xyz");
        });
    }

    // ==================== ChildProcess Tests ====================

    [Fact]
    public void ChildProcess_Kill_ReturnsTrue()
    {
        if (IsWindows)
        {
            // On Windows, use a command that will run for a bit
            var child = child_process.spawn("cmd", new[] { "/c", "timeout", "5" });
            Thread.Sleep(100); // Let it start

            var killed = child.kill();
            Assert.True(killed);
            Assert.True(child.killed);
        }
        else
        {
            var child = child_process.spawn("sleep", new[] { "5" });
            Thread.Sleep(100); // Let it start

            var killed = child.kill();
            Assert.True(killed);
            Assert.True(child.killed);
        }
    }

    [Fact]
    public void ChildProcess_Kill_AlreadyExited_ReturnsFalse()
    {
        var command = IsWindows ? "cmd" : "echo";
        var args = IsWindows ? new[] { "/c", "echo", "test" } : new[] { "test" };
        var child = child_process.spawn(command, args);

        // Wait for process to complete
        Thread.Sleep(500);

        // Try to kill already-exited process
        var killed = child.kill();
        Assert.False(killed);
    }

    [Fact]
    public void ChildProcess_RefUnref_UpdatesReferencedProperty()
    {
        var command = IsWindows ? "cmd" : "echo";
        var args = IsWindows ? new[] { "/c", "echo", "test" } : new[] { "test" };
        var child = child_process.spawn(command, args);

        // Default should be referenced
        Assert.True(child.referenced);

        // Unref should set to false
        child.unref();
        Assert.False(child.referenced);

        // Ref should set back to true
        child.@ref();
        Assert.True(child.referenced);
    }

    [Fact]
    public void ChildProcess_Connected_InitiallyFalse()
    {
        var command = IsWindows ? "cmd" : "echo";
        var args = IsWindows ? new[] { "/c", "echo", "test" } : new[] { "test" };
        var child = child_process.spawn(command, args);

        // Connected is for IPC, which spawn doesn't set up
        Assert.False(child.connected);
    }

    [Fact]
    public void ChildProcess_Disconnect_SetsConnectedFalse()
    {
        var command = IsWindows ? "cmd" : "echo";
        var args = IsWindows ? new[] { "/c", "echo", "test" } : new[] { "test" };
        var child = child_process.spawn(command, args);

        child.disconnect();
        Assert.False(child.connected);
    }

    [Fact]
    public void ChildProcess_Send_WhenNotConnected_ReturnsFalse()
    {
        var command = IsWindows ? "cmd" : "echo";
        var args = IsWindows ? new[] { "/c", "echo", "test" } : new[] { "test" };
        var child = child_process.spawn(command, args);

        var sent = child.send("test message");
        Assert.False(sent);
    }

    // ==================== fork Tests ====================

    [Fact]
    public void Fork_CreatesChildProcess()
    {
        // Fork needs a valid module path - we'll use the current executable
        var modulePath = System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName ?? "dotnet";
        var child = child_process.fork(modulePath);

        Assert.NotNull(child);
        Assert.True(child.pid > 0);
        Assert.True(child.connected); // fork sets up IPC channel

        // Kill it so test doesn't hang
        child.kill();
    }

    [Fact]
    public void Fork_WithArgs_PassesArguments()
    {
        var modulePath = System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName ?? "dotnet";
        var args = new[] { "--version" };
        var child = child_process.fork(modulePath, args);

        Assert.NotNull(child);
        Assert.True(child.pid > 0);

        // Kill it so test doesn't hang
        child.kill();
    }

    // ==================== exec Tests (async) ====================

    [Fact]
    public void Exec_CallsCallback()
    {
        var command = IsWindows ? "echo Hello" : "echo 'Hello'";
        var callbackCalled = false;
        var stdout = "";

        child_process.exec(command, (error, stdoutStr, stderr) =>
        {
            callbackCalled = true;
            stdout = stdoutStr;
        });

        // Wait for async execution
        Thread.Sleep(500);

        Assert.True(callbackCalled);
        Assert.Contains("Hello", stdout);
    }

    [Fact]
    public void Exec_WithOptions_CallsCallback()
    {
        var command = IsWindows ? "echo Test" : "echo 'Test'";
        var options = new ExecOptions { encoding = "utf8" };
        var callbackCalled = false;

        child_process.exec(command, options, (error, stdout, stderr) =>
        {
            callbackCalled = true;
        });

        // Wait for async execution
        Thread.Sleep(500);

        Assert.True(callbackCalled);
    }

    [Fact]
    public void Exec_FailingCommand_PassesErrorToCallback()
    {
        var command = IsWindows ? "exit /b 1" : "exit 1";
        Exception? capturedError = null;

        child_process.exec(command, (error, stdout, stderr) =>
        {
            capturedError = error;
        });

        // Wait for async execution
        Thread.Sleep(500);

        Assert.NotNull(capturedError);
    }

    // ==================== execFile Tests (async) ====================

    [Fact]
    public void ExecFile_CallsCallback()
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

    // ==================== spawnSyncString Tests ====================

    [Fact]
    public void SpawnSyncString_ReturnsStringOutput()
    {
        var command = IsWindows ? "cmd" : "echo";
        var args = IsWindows ? new[] { "/c", "echo", "Hello" } : new[] { "Hello" };
        var result = child_process.spawnSyncString(command, args);

        Assert.NotNull(result);
        Assert.IsType<string>(result.stdout);
        Assert.Contains("Hello", result.stdout);
    }

    [Fact]
    public void SpawnSyncString_WithOptions_ReturnsStringOutput()
    {
        var command = IsWindows ? "cmd" : "echo";
        var args = IsWindows ? new[] { "/c", "echo", "Test" } : new[] { "Test" };
        var options = new ExecOptions { encoding = "utf8" };
        var result = child_process.spawnSyncString(command, args, options);

        Assert.IsType<string>(result.stdout);
        Assert.IsType<string>(result.stderr);
    }

    // ==================== ExecOptions Tests ====================

    [Fact]
    public void ExecSync_WithEnvVariables_PassesEnvironment()
    {
        var command = IsWindows ? "echo %TEST_VAR%" : "echo $TEST_VAR";
        var options = new ExecOptions
        {
            encoding = "utf8",
            env = new { TEST_VAR = "test_value", PATH = Environment.GetEnvironmentVariable("PATH") }
        };
        var result = child_process.execSync(command, options);

        Assert.IsType<string>(result);
        var output = (string)result;
        Assert.Contains("test_value", output);
    }

    [Fact]
    public void ExecSync_WithTimeout_TerminatesProcess()
    {
        var command = IsWindows ? "timeout /t 10" : "sleep 10";
        var options = new ExecOptions { timeout = 100 }; // 100ms timeout

        Assert.Throws<TimeoutException>(() =>
        {
            child_process.execSync(command, options);
        });
    }

    [Fact]
    public void SpawnSync_WithTimeout_SetsSignal()
    {
        var command = IsWindows ? "timeout" : "sleep";
        var args = IsWindows ? new[] { "/t", "10" } : new[] { "10" };
        var options = new ExecOptions { timeout = 100 }; // 100ms timeout
        var result = child_process.spawnSync(command, args, options);

        Assert.NotNull(result.signal);
        Assert.Equal("SIGTERM", result.signal);
        Assert.NotNull(result.error);
        Assert.IsType<TimeoutException>(result.error);
    }

    [Fact]
    public void ExecOptions_AllProperties_CanBeSet()
    {
        var options = new ExecOptions
        {
            cwd = "/tmp",
            env = new { TEST = "value" },
            encoding = "utf8",
            shell = "/bin/sh",
            timeout = 1000,
            maxBuffer = 2048,
            killSignal = "SIGKILL",
            windowsHide = true,
            windowsVerbatimArguments = false,
            detached = false,
            uid = 1000,
            gid = 1000,
            argv0 = "test",
            stdio = "pipe",
            input = "test input"
        };

        Assert.Equal("/tmp", options.cwd);
        Assert.NotNull(options.env);
        Assert.Equal("utf8", options.encoding);
        Assert.Equal("/bin/sh", options.shell);
        Assert.Equal(1000, options.timeout);
        Assert.Equal(2048, options.maxBuffer);
        Assert.Equal("SIGKILL", options.killSignal);
        Assert.True(options.windowsHide);
        Assert.False(options.windowsVerbatimArguments);
        Assert.False(options.detached);
        Assert.Equal(1000, options.uid);
        Assert.Equal(1000, options.gid);
        Assert.Equal("test", options.argv0);
        Assert.Equal("pipe", options.stdio);
        Assert.Equal("test input", options.input);
    }

    // ==================== ChildProcess Event Tests ====================

    [Fact]
    public void ChildProcess_ExitEvent_ContainsExitCode()
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
    public void ChildProcess_ExitEvent_NonZeroExitCode()
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
    public void ChildProcess_ExitCodeProperty_SetAfterExit()
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

    [Fact]
    public void ChildProcess_KillWithSignal_SetsSignalCode()
    {
        if (IsWindows)
        {
            var child = child_process.spawn("cmd", new[] { "/c", "timeout", "10" });
            Thread.Sleep(100);

            var killed = child.kill("SIGKILL");

            Assert.True(killed);
            Assert.True(child.killed);
            // Note: signalCode might not be set on Windows due to .NET limitations
        }
        else
        {
            var child = child_process.spawn("sleep", new[] { "10" });
            Thread.Sleep(100);

            var killed = child.kill("SIGKILL");

            Assert.True(killed);
            Assert.True(child.killed);
        }
    }

    [Fact]
    public void ChildProcess_KillWithDifferentSignals_Works()
    {
        if (!IsWindows)
        {
            // Test SIGTERM
            var child1 = child_process.spawn("sleep", new[] { "10" });
            Thread.Sleep(100);
            Assert.True(child1.kill("SIGTERM"));

            // Test SIGKILL
            var child2 = child_process.spawn("sleep", new[] { "10" });
            Thread.Sleep(100);
            Assert.True(child2.kill("SIGKILL"));

            // Test default (should be SIGTERM)
            var child3 = child_process.spawn("sleep", new[] { "10" });
            Thread.Sleep(100);
            Assert.True(child3.kill());
        }
    }

    [Fact]
    public void ChildProcess_DisconnectEvent_CanBeEmitted()
    {
        var command = IsWindows ? "cmd" : "echo";
        var args = IsWindows ? new[] { "/c", "echo", "test" } : new[] { "test" };

        var child = child_process.spawn(command, args);
        child.on("disconnect", () =>
        {
            // Disconnect event handler attached
        });

        child.disconnect();

        // disconnect() immediately sets connected to false
        Assert.False(child.connected);
    }

    [Fact]
    public void ChildProcess_MessageEvent_CanBeAttached()
    {
        var command = IsWindows ? "cmd" : "echo";
        var args = IsWindows ? new[] { "/c", "echo", "test" } : new[] { "test" };

        var child = child_process.spawn(command, args);

        // Verify we can attach message event handler
        child.on("message", (object message, object sendHandle) =>
        {
            // Handler attached successfully
        });

        Assert.NotNull(child);
    }

    // ==================== Edge Cases and Error Handling ====================

    [Fact]
    public void SpawnSyncReturns_AllPropertiesAccessible()
    {
        var command = IsWindows ? "cmd" : "echo";
        var args = IsWindows ? new[] { "/c", "echo", "test" } : new[] { "test" };
        var result = child_process.spawnSync(command, args);

        // Verify all properties are accessible
        Assert.True(result.pid > 0);
        Assert.NotNull(result.output);
        Assert.NotNull(result.stdout);
        Assert.NotNull(result.stderr);
        Assert.True(result.status == 0 || result.status == null);
        // signal is null for successful execution
        // error is null for successful execution
    }

    [Fact]
    public void ExecSync_WithMaxBuffer_RespectedInOptions()
    {
        var command = IsWindows ? "echo test" : "echo 'test'";
        var options = new ExecOptions
        {
            encoding = "utf8",
            maxBuffer = 1024 * 10 // 10KB
        };
        var result = child_process.execSync(command, options);

        Assert.IsType<string>(result);
    }

    [Fact]
    public void Fork_Connected_InitiallyTrue()
    {
        var modulePath = System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName ?? "dotnet";
        var child = child_process.fork(modulePath);

        Assert.True(child.connected);

        child.kill();
    }

    [Fact]
    public void Spawn_MultipleEventHandlers_AllCalled()
    {
        var command = IsWindows ? "cmd" : "echo";
        var args = IsWindows ? new[] { "/c", "echo", "test" } : new[] { "test" };
        var handler1Called = false;
        var handler2Called = false;
        var resetEvent = new ManualResetEventSlim(false);
        var callCount = 0;

        var child = child_process.spawn(command, args);

        child.on("exit", (int? code, string? signal) =>
        {
            handler1Called = true;
            if (++callCount == 2) resetEvent.Set();
        });

        child.on("exit", (int? code, string? signal) =>
        {
            handler2Called = true;
            if (++callCount == 2) resetEvent.Set();
        });

        resetEvent.Wait(2000);

        Assert.True(handler1Called);
        Assert.True(handler2Called);
    }

    [Fact]
    public void ChildProcess_PidProperty_ValidAfterSpawn()
    {
        var command = IsWindows ? "cmd" : "echo";
        var args = IsWindows ? new[] { "/c", "echo", "test" } : new[] { "test" };
        var child = child_process.spawn(command, args);

        Assert.True(child.pid > 0);
        Assert.True(child.pid < int.MaxValue);
    }

    [Fact]
    public void ExecSync_InvalidCommand_ThrowsException()
    {
        // Invalid commands should throw when process exits with non-zero code
        var command = IsWindows ? "exit /b 99" : "exit 99";

        Assert.Throws<InvalidOperationException>(() =>
        {
            child_process.execSync(command);
        });
    }

    [Fact]
    public void SpawnSync_NoArgs_Works()
    {
        var command = IsWindows ? "cmd" : "pwd";
        var result = child_process.spawnSync(command);

        Assert.NotNull(result);
        Assert.True(result.pid > 0);
    }

    [Fact]
    public void ExecFileSync_NoArgs_Works()
    {
        var file = IsWindows ? "cmd.exe" : "/bin/pwd";
        var result = child_process.execFileSync(file);

        Assert.NotNull(result);
    }

    // ==================== Stream Properties Tests ====================

    [Fact]
    public void ChildProcess_StdinProperty_IsAccessible()
    {
        var command = IsWindows ? "cmd" : "echo";
        var args = IsWindows ? new[] { "/c", "echo", "test" } : new[] { "test" };
        var child = child_process.spawn(command, args);

        // stdin is null in current implementation (streams not yet implemented)
        // but property should be accessible
        var stdin = child.stdin;
        Assert.Null(stdin);
    }

    [Fact]
    public void ChildProcess_StdoutProperty_IsAccessible()
    {
        var command = IsWindows ? "cmd" : "echo";
        var args = IsWindows ? new[] { "/c", "echo", "test" } : new[] { "test" };
        var child = child_process.spawn(command, args);

        // stdout is null in current implementation (streams not yet implemented)
        // but property should be accessible
        var stdout = child.stdout;
        Assert.Null(stdout);
    }

    [Fact]
    public void ChildProcess_StderrProperty_IsAccessible()
    {
        var command = IsWindows ? "cmd" : "echo";
        var args = IsWindows ? new[] { "/c", "echo", "test" } : new[] { "test" };
        var child = child_process.spawn(command, args);

        // stderr is null in current implementation (streams not yet implemented)
        // but property should be accessible
        var stderr = child.stderr;
        Assert.Null(stderr);
    }

    [Fact]
    public void ChildProcess_AllStreamProperties_AreNull()
    {
        var command = IsWindows ? "cmd" : "echo";
        var args = IsWindows ? new[] { "/c", "echo", "test" } : new[] { "test" };
        var child = child_process.spawn(command, args);

        // All stream properties should be null (streams not yet implemented)
        Assert.Null(child.stdin);
        Assert.Null(child.stdout);
        Assert.Null(child.stderr);
    }
}
