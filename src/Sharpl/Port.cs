namespace Sharpl;

public interface Port
{
    void Close();
    Task<bool> Poll(CancellationToken ct);
    Task<Value?> Read(VM vm, Loc loc);
    Task Write(Value value, VM vm, Loc loc);
}