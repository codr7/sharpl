namespace Sharpl;

public interface Port {
    public Task<bool> Poll(CancellationToken ct);
    public Task<Value> Read(VM vm, Loc loc);
    public Task Write(Value value, VM vm, Loc loc);
}