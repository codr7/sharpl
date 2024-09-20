using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Sharpl.Net;

public record class StreamPort(NetworkStream stream) : Port
{
    public void Close() => stream.Close();

    public async Task<byte[]> Read(int size)
    {
        var buffer = new byte[size];
        await stream.ReadExactlyAsync(buffer);
        return buffer;
    }

    public Task<bool> Poll(CancellationToken ct) => Task.Run(() => stream.Socket.Poll(0, SelectMode.SelectRead));

    public async Task<ushort?> ReadSize()
    {
        var buffer = new byte[sizeof(ushort)];
        var i = 0;
        var ct = new CancellationTokenSource().Token;

        while (i < buffer.Length)
        {
            int n = await stream.ReadAsync(buffer, i, buffer.Length - i, ct);
            if (n == 0) { return null; }
            i += n;
        }

        return (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(buffer));
    }

    public async Task<Value?> Read(VM vm, Loc loc)
    {
        var size = await ReadSize();
        if (size is null) { return null; }
        var data = Encoding.UTF8.GetString(await Read((ushort)size));
        var jsLoc = new Loc("json");
        if (Json.ReadValue(vm, new StringReader(data), ref jsLoc) is Value v) { return v; }
        throw new EvalError("Failed to parse JSON value", loc);
    }

    public async Task Write(Value value, VM vm, Loc loc)
    {
        var js = value.ToJson(loc);
        var bs = Encoding.UTF8.GetBytes(js);
        var sbs = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)bs.Length));
        await stream.WriteAsync(sbs);
        await stream.WriteAsync(bs);
    }
}