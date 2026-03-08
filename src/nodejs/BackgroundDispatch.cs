using System;
using System.Threading;
using System.Threading.Tasks;

namespace nodejs;

internal static class BackgroundDispatch
{
    private static readonly TaskFactory Factory = new(
        CancellationToken.None,
        TaskCreationOptions.LongRunning,
        TaskContinuationOptions.None,
        TaskScheduler.Default);

    public static Task Run(Action action, string? name = null)
    {
        return Factory.StartNew(action);
    }

    public static Task RunAsync(Func<Task> action, string? name = null)
    {
        return Factory.StartNew(action).Unwrap();
    }
}
