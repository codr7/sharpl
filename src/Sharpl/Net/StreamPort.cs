using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace Sharpl.Net;

public record class StreamPort(NetworkStream stream) : Port
{
    public async Task<byte[]> Read(int size) {
        var buffer = new byte[size];
        await stream.ReadExactlyAsync(buffer);
        return buffer;
    }

    public Task<bool> Poll(CancellationToken ct) => Task.Run(() => stream.Socket.Poll(0, SelectMode.SelectRead));
    
    public async Task<Value> Read(VM vm, Loc loc) {
        var sizeBytes = await Read(sizeof(ushort));
        var size = IPAddress.NetworkToHostOrder(ushort.Parse(Encoding.ASCII.GetString(sizeBytes)));
        var data = Encoding.UTF8.GetString(await Read(size));
        var jsLoc = new Loc("json");
        if (Json.ReadValue(vm, new StringReader(data), ref jsLoc) is Value v) { return v; }
        throw new EvalError(loc, "Failed to parse JSON value");
    }

    public async Task Write(Value value, VM vm, Loc loc) {
        var js = value.ToJson(loc);
        var bs = Encoding.UTF8.GetBytes(js);
        var sbs = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((ushort)bs.Length));
        await stream.WriteAsync(sbs);
        await stream.WriteAsync(bs);
    }
}