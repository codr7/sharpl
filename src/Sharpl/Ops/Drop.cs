namespace Sharpl.Ops;

public class Drop : Op
{
    public static Op Make(int count) => new Drop(count);
    public readonly int Count;
    public Drop(int count) {  Count = count; }
    public OpCode Code => OpCode.Drop;
    public string Dump(VM vm) => $"Drop {Count}";
}