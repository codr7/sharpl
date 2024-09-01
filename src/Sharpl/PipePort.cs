using System.Threading.Channels;

namespace Sharpl;

public record class PipePort(ChannelReader<Value> Reader, ChannelWriter<Value> Writer) : Port
{
    public Value Read() => Task.Run(async () => await Reader.ReadAsync()).Result;
    public void Write(Value value) => Task.Run(async () => await Writer.WriteAsync(value));
}