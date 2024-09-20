using System.Threading.Channels;

namespace Sharpl;

public static class TaskUtil
{
    public static async ValueTask<Channel<T>> Poll<T>(Channel<T>[] sources)
    {
        var cts = new CancellationTokenSource();
        var trs = sources.Select(c => (c.Reader.WaitToReadAsync(cts.Token).AsTask(), c)).ToArray();
        return await Any(trs);
    }

    public static async ValueTask<T> Any<T>((Task<bool>, T)[] sources) =>
        await Any(sources, new CancellationTokenSource());

    public static async ValueTask<T> Any<T>((Task<bool>, T)[] sources, CancellationTokenSource cts)
    {
        var lookup = sources.ToDictionary();
        Task<bool> ct = await Task.WhenAny(lookup.Keys);
        cts.Cancel();
        return lookup[ct];
    }
}