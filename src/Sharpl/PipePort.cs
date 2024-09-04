using System.Threading.Channels;

namespace Sharpl;

public record class PipePort(ChannelReader<Value> Reader, ChannelWriter<Value> Writer) : Port
{
    public async Task<bool> Poll(CancellationToken ct) => await Reader.WaitToReadAsync(ct);
    public async Task<Value> Read(VM vm, Loc loc) => await Reader.ReadAsync();
    public async Task Write(Value value, VM vm, Loc loc) => await Writer.WriteAsync(value);
}