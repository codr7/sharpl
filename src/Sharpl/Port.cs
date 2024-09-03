namespace Sharpl;

public interface Port {
    public Task<bool> Poll(CancellationToken ct);
    public Task<Value> Read();
    public Task Write(Value value);
}