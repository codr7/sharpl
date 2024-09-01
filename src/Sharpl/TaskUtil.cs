using System.Threading.Channels;

namespace Sharpl;

public static class TaskUtil
{
    public static async ValueTask<Channel<T>> Poll<T>(Channel<T>[] sources)
    {
        var cts = new CancellationTokenSource();
        var trs = sources.Select(c => (c.Reader.WaitToReadAsync(cts.Token).AsTask(), c)).ToDictionary();
        Task<bool> ct = await Task.WhenAny(trs.Keys);
        cts.Cancel();
        return trs[ct];
    }
}